using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    [CreateAssetMenu(menuName = "Windsmoon/Windsmoon Render Pipeline/Create Windsmoon Render Pipeline")]
    public class WindsmoonRenderPipelineAsset : RenderPipelineAsset
    {
        #region methods
        protected override RenderPipeline CreatePipeline()
        {
            return new WindsmoonRenderPipeline();
        }
        #endregion
    }
}
