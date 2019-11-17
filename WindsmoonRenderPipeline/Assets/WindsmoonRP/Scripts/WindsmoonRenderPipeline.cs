using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    public class WindsmoonRenderPipeline : RenderPipeline
    {
        #region fields
        private CameraRenderer cameraRenderer = new CameraRenderer();
        private bool useDynamicBatching;
        private bool useGPUInstancing;
        #endregion
        
        #region constructors
        public WindsmoonRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        }
        #endregion
        
        #region methods
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                cameraRenderer.Render(context, camera, useDynamicBatching, useGPUInstancing);
            }
        }
        #endregion
    }
}