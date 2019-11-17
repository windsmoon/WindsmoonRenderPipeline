using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    [CreateAssetMenu(menuName = "Windsmoon/Windsmoon Render Pipeline/Create Windsmoon Render Pipeline")]
    public class WindsmoonRenderPipelineAsset : RenderPipelineAsset
    {
        #region fields
        [SerializeField]
        private bool useDynamicBatching = true;
        [SerializeField]
        private bool useGPUInstancing = true;
        [SerializeField]
        private bool useSPRBatcher = true;
        #endregion

        #region methods
        protected override RenderPipeline CreatePipeline()
        {
            return new WindsmoonRenderPipeline(useDynamicBatching, useGPUInstancing, useSPRBatcher);
        }
        #endregion
    }
}
