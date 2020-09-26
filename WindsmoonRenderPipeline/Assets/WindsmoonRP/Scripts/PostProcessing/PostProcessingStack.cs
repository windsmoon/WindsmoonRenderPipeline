using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP.PostProcessing
{
    public class PostProcessingStack
    {
        #region constants
        private const string commandBufferName = "Windsmoon Post Processing";
        #endregion

        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer() {name = commandBufferName};
        private ScriptableRenderContext renderContext;
        private Camera camera;
        private PostProcessingAsset postProcessingAsset;
        #endregion

        #region properties
        public bool IsActive // todo : add cache
        {
            get => postProcessingAsset != null;
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
            Draw(sourceID, BuiltinRenderTextureType.CameraTarget, PostProcessingPassEnum.Copy);
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }

        private void Draw(RenderTargetIdentifier source, RenderTargetIdentifier dest, PostProcessingPassEnum passEnum)
        {
            commandBuffer.SetGlobalTexture(ShaderPropertyID.PostProcessingSource, source);
            commandBuffer.SetRenderTarget(dest, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            commandBuffer.DrawProcedural(Matrix4x4.identity, postProcessingAsset.Material, (int)passEnum, MeshTopology.Triangles, 3);
        }
        
        #endregion
    }
}