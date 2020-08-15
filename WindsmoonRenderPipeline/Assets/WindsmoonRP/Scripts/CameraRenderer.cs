using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using WindsmoonRP.Shadow;

namespace WindsmoonRP
{
    public class CameraRenderer
    {
        #region constants
        private const string defaultCommandBufferName = "Camera Renderer";
        #endregion

        #region fields
        private ScriptableRenderContext renderContext;
        private Camera camera;
        private CommandBuffer commandBuffer = new CommandBuffer();
        private CullingResults cullingResults;
        private static ShaderTagId unlitShaderTagID = new ShaderTagId("SRPDefaultUnlit");
        private static ShaderTagId litShaderTagID = new ShaderTagId("WindsmoonLit");
        private string commandBufferName;
        private Lighting lighting = new Lighting();

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
        public void Render(ScriptableRenderContext renderContext, Camera camera, bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
        {
            this.renderContext = renderContext;
            this.camera = camera;
            
            #if UNITY_EDITOR || DEBUG
            SetCommandBufferName();
            DrawSceneView(); // draw it before culling or it may be culled
            #endif
            
            if (Cull(shadowSettings.MaxDistance) == false)
            {
                return;
            }
            
            commandBuffer.BeginSample(commandBufferName);
            ExecuteCommandBuffer(); // ?? why do this ? maybe begin sample must be execute before next sample
            lighting.Setup(renderContext, cullingResults, shadowSettings);
            commandBuffer.EndSample(commandBufferName);
            Setup(shadowSettings);
            DrawVisibleObjects(useDynamicBatching, useGPUInstancing);

            #if UNITY_EDITOR || DEBUG
            DrawUnsupportedShaderObjects();
            DrawGizmos();
            #endif
            
            lighting.Cleanup();
            Submit();
        }

        private bool Cull(float maxShadowDistance)
        {
            ScriptableCullingParameters scriptableCullingParameters;
            
            if (camera.TryGetCullingParameters(out scriptableCullingParameters)) // note: this method check if camera setting is invalid, return false 
            {
                scriptableCullingParameters.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
                cullingResults = renderContext.Cull(ref scriptableCullingParameters);
                return true;
            }

            return false;
        }
        
        private void Setup(ShadowSettings shadowSettings)
        {
            renderContext.SetupCameraProperties(camera); // ?? this method must be called before excute commandbuffer, or clear command will call GL.Draw to clear
            CameraClearFlags cameraClearFlags = camera.clearFlags;
            commandBuffer.ClearRenderTarget(cameraClearFlags <= CameraClearFlags.Depth, cameraClearFlags == CameraClearFlags.Color, cameraClearFlags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear); // ?? tbdr resolve
            commandBuffer.BeginSample(commandBufferName);
            ExecuteCommandBuffer();
        }
        
        private void DrawVisibleObjects(bool useDynamicBatching, bool useGPUInstancing)
        {
            SortingSettings sortingSettings = new SortingSettings(camera) {criteria = SortingCriteria.CommonOpaque};
            // Lightmaps enable LIGHTMAP_ON, ShadowMask enable shadow mask texture, OcclusionProbe enable unity_ProbesOcclusion
            DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagID, sortingSettings)
                {enableDynamicBatching = useDynamicBatching, enableInstancing = useGPUInstancing, 
                    perObjectData = PerObjectData.Lightmaps | PerObjectData.ShadowMask | PerObjectData.OcclusionProbe | PerObjectData.LightProbe | PerObjectData.LightProbeProxyVolume};
            drawingSettings.SetShaderPassName(1, litShaderTagID);
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
            ExecuteCommandBuffer();
            renderContext.Submit();
        }

        private void ExecuteCommandBuffer()
        {
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }
        
        private void SetCommandBufferName()
        {
            #if UNITY_EDITOR || DEBUG
            Profiler.BeginSample("Editor Only");
            commandBufferName = camera.name;
            Profiler.EndSample();
            #else
            commandBufferName = defaultCommandBufferName;
            #endif

            commandBuffer.name = commandBufferName;
        }
        #endregion
    }
}
