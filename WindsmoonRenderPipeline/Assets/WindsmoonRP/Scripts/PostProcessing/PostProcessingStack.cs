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
            this.postProcessingAsset = postProcessingAsset;
        }

        public void Render(int sourceID)
        {
            commandBuffer.Blit(sourceID, BuiltinRenderTextureType.CameraTarget);
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }
        #endregion
    }
}