using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP.Shadow
{
    public class ShadowRenderer
    {
        #region constants
        private const string bufferName = "Shadows";
        private const int maxDirectionalLightShadowCount = 1;
        #endregion
        
        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer { name = bufferName };
        private ScriptableRenderContext renderContext;
        private CullingResults cullingResults;
        private ShadowSettings shadowSettings;
        private DirectionalShadow[] directionalShadows = new DirectionalShadow[maxDirectionalLightShadowCount];
        private int currentDirectionalLightShadowCount;
        #endregion

        #region methods
        public void Setup(ScriptableRenderContext renderContext, CullingResults cullingResults, ShadowSettings shadowSettings)
        {
            this.renderContext = renderContext;
            this.cullingResults = cullingResults;
            this.shadowSettings = shadowSettings;
            currentDirectionalLightShadowCount = 0;
        }
        
        public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
        {
            // GetShadowCasterBounds  return true if the light affects at least one shadow casting object in the Scene
            if (currentDirectionalLightShadowCount >= maxDirectionalLightShadowCount || light.shadows == LightShadows.None || light.shadowStrength <= 0f 
                || cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds bouds) == false)
            {
                return;
            }

            directionalShadows[currentDirectionalLightShadowCount] = new DirectionalShadow(){visibleLightIndex = visibleLightIndex};
            ++currentDirectionalLightShadowCount;
        }

        public void Render()
        {
            if (currentDirectionalLightShadowCount > 0)
            {
                RenderDirectionalShadow();
            }
        }

        public void Cleanup()
        {
            if (currentDirectionalLightShadowCount > 0)
            {
                commandBuffer.ReleaseTemporaryRT(ShaderPropertyID.DirectionalShadowMap);
                ExecuteBuffer();    
            }
        }

        private void RenderDirectionalShadow()
        {
            int shadowMapSize = (int)shadowSettings.DirectionalShadowSetting.ShadowMapSize;
            commandBuffer.GetTemporaryRT(ShaderPropertyID.DirectionalShadowMap, shadowMapSize, shadowMapSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
            commandBuffer.SetRenderTarget(ShaderPropertyID.DirectionalShadowMap, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            commandBuffer.ClearRenderTarget(true, false, Color.clear);
            commandBuffer.BeginSample(bufferName);
            ExecuteBuffer();

            for (int i = 0; i < currentDirectionalLightShadowCount; ++i)
            {
                RenderDirectionalShadow(i, shadowMapSize);
            }
            
            commandBuffer.EndSample(bufferName);
            ExecuteBuffer();
        }

        private void RenderDirectionalShadow(int index, int tileSize)
        {
            DirectionalShadow directionalShadow = directionalShadows[index];
            ShadowDrawingSettings shadowDrawingSettings = new ShadowDrawingSettings(cullingResults, directionalShadow.visibleLightIndex);
            cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(directionalShadow.visibleLightIndex, 0, 1,
                Vector3.zero, tileSize, 0f, out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData shadowSplitData);
            shadowDrawingSettings.splitData = shadowSplitData;
            commandBuffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            ExecuteBuffer();
            renderContext.DrawShadows(ref shadowDrawingSettings);
        }
        
        private void ExecuteBuffer() 
        {
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }
        #endregion

        #region structs
        private struct DirectionalShadow
        {
            public int visibleLightIndex;
        }
        #endregion
    }
}