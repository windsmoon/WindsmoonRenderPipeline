using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP.PostProcessing
{
    public class PostProcessingStack
    {
        #region constants
        private const string commandBufferName = "Windsmoon Post Processing";
        private const int maxBloomIterationCount = 16;
        #endregion

        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer() {name = commandBufferName};
        private ScriptableRenderContext renderContext;
        private Camera camera;
        private PostProcessingAsset postProcessingAsset;

        // bloom
        private int bloomIteration1PropertyID;
        #endregion

        #region properties
        public bool IsActive // todo : add cache
        {
            get => postProcessingAsset != null;
        }
        #endregion

        #region constructors
        public PostProcessingStack()
        {
            bloomIteration1PropertyID = Shader.PropertyToID("_BloomIteration1");

            for (int i = 2; i <= maxBloomIterationCount; ++i)
            {
                // todo : do not use the api's side effect
                Shader.PropertyToID("_BloomIteration" + i); // when firstly request a shader property, the id will be simply increase one 
            }
        }
        #endregion
        
        #region methods
        public void Setup(ScriptableRenderContext renderContext, Camera camera, PostProcessingAsset postProcessingAsset)
        {
            this.renderContext = renderContext;
            this.camera = camera;

#if UNITY_EDITOR
            // game and scene (scene has a switch)
            this.postProcessingAsset = camera.cameraType <= CameraType.SceneView ? postProcessingAsset : null;

            if (camera.cameraType == CameraType.SceneView && UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.showImageEffects == false)
            {
                this.postProcessingAsset = null;
            }
#else
            this.postProcessingAsset = postProcessingAsset;
#endif
        }

        public void Render(int sourceID)
        {
            // Draw(sourceID, BuiltinRenderTextureType.CameraTarget, PostProcessingPassEnum.Copy);
            DoBloom(sourceID);
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }

        private void Draw(RenderTargetIdentifier source, RenderTargetIdentifier dest, PostProcessingPassEnum passEnum)
        {
            commandBuffer.SetGlobalTexture(ShaderPropertyID.PostProcessingSource, source);
            commandBuffer.SetRenderTarget(dest, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            commandBuffer.DrawProcedural(Matrix4x4.identity, postProcessingAsset.Material, (int)passEnum, MeshTopology.Triangles, 3);
        }

        private void DoBloom(int sourceID)
        {
            BloomSettings bloomSettings = postProcessingAsset.BloomSettings;
            commandBuffer.BeginSample("Bloom");
            int width = camera.pixelWidth / 2;
            int height = camera.pixelHeight / 2;
            RenderTextureFormat rtFormat = RenderTextureFormat.Default;
            int fromID = sourceID;
            int toID = bloomIteration1PropertyID;
            int i;

            for (i = 1; i <= bloomSettings.MaxBloomIterationCount; ++i)
            {
                if (width < bloomSettings.MinResolution || height < bloomSettings.MinResolution)
                {
                    break;
                }
                
                commandBuffer.GetTemporaryRT(toID, width, height, 0, FilterMode.Bilinear, rtFormat);
                Draw(fromID, toID, PostProcessingPassEnum.Copy);
                fromID = toID;
                ++toID;
                width /= 2;
                height /= 2;
            }
            
            // this time the fromID is the last rt be written in above for loop
            // the BuiltinRenderTextureType.CameraTarget
            Draw(fromID, BuiltinRenderTextureType.CameraTarget, PostProcessingPassEnum.Copy);

            for (i -= 1; i >= 1; --i)
            {
                commandBuffer.ReleaseTemporaryRT(bloomIteration1PropertyID + i - 1);
            }
            
            commandBuffer.EndSample("Bloom");
        }
        #endregion
    }
}