using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    [CreateAssetMenu(menuName = "Windsmoon/Windsmoon Render Pipeline/Create Windsmoon Render Pipeline")]
    public class WindsmoonRenderPipeline : RenderPipeline
    {
        #region fields
        private CameraRenderer cameraRenderer = new CameraRenderer();
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