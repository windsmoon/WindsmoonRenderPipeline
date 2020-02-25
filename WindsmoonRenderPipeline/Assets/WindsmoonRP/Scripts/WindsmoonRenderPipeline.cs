using UnityEngine;
using UnityEngine.Rendering;
using WindsmoonRP.Shadow;

namespace WindsmoonRP
{
    public class WindsmoonRenderPipeline : RenderPipeline
    {
        #region fields
        private CameraRenderer cameraRenderer = new CameraRenderer();
        private bool useDynamicBatching;
        private bool useGPUInstancing;
        private ShadowSettings shadowSettings;
        #endregion
        
        #region constructors
        public WindsmoonRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shadowSettings)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
            this.shadowSettings = shadowSettings;
        }
        #endregion
        
        #region methods
        protected override void Render(ScriptableRenderContext renderContex, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                cameraRenderer.Render(renderContex, camera, useDynamicBatching, useGPUInstancing, shadowSettings);
            }
        }
        #endregion
    }
}