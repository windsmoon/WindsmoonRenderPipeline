using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    public class WindsmoonRenderPipeline : RenderPipeline
    {
        #region fields
        private CameraRenderer cameraRenderer = new CameraRenderer();
        #endregion
        
        #region constructors
        public WindsmoonRenderPipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
        }
        #endregion
        
        #region methods
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                cameraRenderer.Render(context, camera);
            }
        }
        #endregion
    }
}