using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    public class Lighting
    {
        #region contants
        private const string bufferName = "Lighting";
        private const int maxDirectionalLightCount = 4;
        #endregion
        
        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer();
        private int directionalLightColorsPropertyID = Shader.PropertyToID("_DirectionalLightColors");
        private int directionalLightDirectionsPropertyID = Shader.PropertyToID("_DirectionalLightDirections");
        private int directionalLightCountPropertyID = Shader.PropertyToID("_directionalLightCount");
        private CullingResults cullingResults;
        private static Vector4[] directionalLightColors = new Vector4[maxDirectionalLightCount]; 
        private static Vector4[] directionalLightDirections = new Vector4[maxDirectionalLightCount];
        #endregion
        
        #region methods
        public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
        {
            this.cullingResults = cullingResults;
            commandBuffer.BeginSample(bufferName);
            SetupLights();
            commandBuffer.EndSample(bufferName);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }
        
        private void SetupLights()
        {
            NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
            int directionalCount = 0;

            for (int i = 0; i < visibleLights.Length; ++i)
            {
                VisibleLight visibleLight = visibleLights[i];

                if (visibleLight.lightType != LightType.Directional)
                {
                    continue;
                }
                
                if (directionalCount >= maxDirectionalLightCount)
                {
                    break;
                }
                
                SetupDirectionalLight(i, ref visibleLight);
                ++directionalCount;
            }
            // Light light = RenderSettings.sun;
            // commandBuffer.SetGlobalVector(directionalLightColorPropertyID, light.color.linear * light.intensity);
            // commandBuffer.SetGlobalVector(directionalLightDirectionPropertyID, -light.transform.forward);
            commandBuffer.SetGlobalVectorArray(directionalLightColorsPropertyID, directionalLightColors);
            commandBuffer.SetGlobalVectorArray(directionalLightDirectionsPropertyID, directionalLightDirections);
            commandBuffer.SetGlobalInt(directionalLightCountPropertyID, directionalCount);
        }

        private void SetupDirectionalLight(int index, ref VisibleLight visiblelight)
        {
            directionalLightColors[index] = visiblelight.finalColor;
            directionalLightDirections[index] = -visiblelight.localToWorldMatrix.GetColumn(2);
        }
        #endregion
    }
}