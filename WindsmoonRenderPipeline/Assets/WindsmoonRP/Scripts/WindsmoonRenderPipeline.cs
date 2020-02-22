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
        private ShadowSetting shadowSetting;
        #endregion
        
        #region constructors
        public WindsmoonRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSetting shadowSetting)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
            this.shadowSetting = shadowSetting;
        }
        #endregion
        
        #region methods
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                cameraRenderer.Render(context, camera, useDynamicBatching, useGPUInstancing, shadowSetting);
            }
        }
        #endregion
    }
}