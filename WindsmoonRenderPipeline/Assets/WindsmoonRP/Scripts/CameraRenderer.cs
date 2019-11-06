using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    public class CameraRenderer
    {
        #region fields
        private const string defaultCommandBufferName = "Camera Renderer";
        #endregion

        #region fields
        private ScriptableRenderContext renderContext;
        private Camera camera;
        private CommandBuffer commandBuffer = new CommandBuffer();
        private CullingResults cullingResults;
        private static ShaderTagId unlitShaderTagID = new ShaderTagId("SRPDefaultUnlit");
        private string commandBufferName;
        
        #if UNITY_EDITOR || DEBUG
        private static ShaderTagId[] legacyShaderTagIDs = 
        {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };

        private static Material errorMaterial;
        #endif
        #endregion
        
        #region methods
        public void Render(ScriptableRenderContext renderContext, Camera camera)
        {
            this.renderContext = renderContext;
            this.camera = camera;
            
            #if UNITY_EDITOR || DEBUG
            SetCommandBufferName();
            DrawSceneView();
            #endif
            
            if (Cull() == false)
            {
                return;
            }
            
            Setup();
            DrawVisibleObjects();
            
            #if UNITY_EDITOR || DEBUG
            DrawUnsupportedShaderObjects();
            DrawGizmos();
            #endif
            
            Submit();
        }

        private bool Cull()
        {
            ScriptableCullingParameters scriptableCullingParameters;
            
            if (camera.TryGetCullingParameters(out scriptableCullingParameters))
            {
                cullingResults = renderContext.Cull(ref scriptableCullingParameters);
                return true;
            }

            return false;
        }
        
        private void Setup()
        {
            renderContext.SetupCameraProperties(camera); // ?? this method must be called before excute commandbuffer, or clear command will call GL.Draw to clear
            CameraClearFlags cameraClearFlags = camera.clearFlags;
            commandBuffer.ClearRenderTarget(cameraClearFlags <= CameraClearFlags.Depth, cameraClearFlags == CameraClearFlags.Color, cameraClearFlags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear); // ?? tbdr resolve
            commandBuffer.BeginSample(commandBufferName);
            ExcuteCommandBuffer();
        }
        
        private void DrawVisibleObjects()
        {
            SortingSettings sortingSettings = new SortingSettings(camera) {criteria = SortingCriteria.CommonOpaque};
            DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagID, sortingSettings);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            renderContext.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
            renderContext.DrawSkybox(camera);
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            renderContext.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        #if UNITY_EDITOR || DEBUG
        private void DrawSceneView()
        {
            if (camera.cameraType != CameraType.SceneView)
            {
                return;
            }
            
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
        
        private void DrawUnsupportedShaderObjects()
        {
            if (errorMaterial == null)
            {
                errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            }

            DrawingSettings drawingSettings = new DrawingSettings();
            drawingSettings.sortingSettings = new SortingSettings(camera);
            drawingSettings.overrideMaterial = errorMaterial;
            
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;

            for (int i = 0; i < legacyShaderTagIDs.Length; ++i)
            {
                drawingSettings.SetShaderPassName(i, legacyShaderTagIDs[i]);
            }
            
            renderContext.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        private void DrawGizmos() // ?? nothing happened, do not call this methods also has gizmos
        {
            if (Handles.ShouldRenderGizmos())
            {
                renderContext.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                renderContext.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
        }
        #endif

        private void Submit()
        {
            commandBuffer.EndSample(commandBufferName);
            ExcuteCommandBuffer();
            renderContext.Submit();
        }

        private void ExcuteCommandBuffer()
        {
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }
        
        private void SetCommandBufferName()
        {
            #if UNITY_EDITOR || DEBUG
            commandBufferName = camera.name;
            #else
            commandBufferName = defaultCommandBufferName;
            #endif

            commandBuffer.name = commandBufferName;
        }
        #endregion
    }
}
