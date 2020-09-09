using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using WindsmoonRP.Shadow;


namespace WindsmoonRP
{
    public class Lighting // todo : rename to LightRenderer
    {
        #region contants
        private const string bufferName = "Lighting";
        private const int maxDirectionalLightCount = 4;
        private const int maxOtherLightCount = 64;
        #endregion
        
        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer();
        private static int directionalLightColorsPropertyID = Shader.PropertyToID("_DirectionalLightColors");
        private static int directionalLightDirectionsPropertyID = Shader.PropertyToID("_DirectionalLightDirections");
        private static int directionalLightCountPropertyID = Shader.PropertyToID("_DirectionalLightCount");
        private static int directionalShadowInfosPropertyID = Shader.PropertyToID("_DirectionalShadowInfos");

        private static Vector4[] directionalLightColors = new Vector4[maxDirectionalLightCount]; 
        private static Vector4[] directionalLightDirections = new Vector4[maxDirectionalLightCount];
        private static Vector4[] _DirectionalShadowInfos = new Vector4[maxDirectionalLightCount];
        
        private static int otherLightColorsPropertyID = Shader.PropertyToID("_OtherLightColors");
        private static int otherLightPositionsProoertyID = Shader.PropertyToID("_OtherLightPositions");
        private static int otherLightCountPropertyID = Shader.PropertyToID("_OtherLightCount");
        private static int otherLightDirectionsPropertyID = Shader.PropertyToID("_OtherLightDirections");
        private static int otherLightSpotAnglesPropertyID = Shader.PropertyToID("_OtherLightSpotAngles");
            
        private static Vector4[] otherLightColors = new Vector4[maxOtherLightCount];
        private static Vector4[] otherLightPositions = new Vector4[maxOtherLightCount];
        private static Vector4[] otherLightDirections = new Vector4[maxOtherLightCount];
        private static Vector4[] otherLightSpotAngles = new Vector4[maxOtherLightCount];

        private CullingResults cullingResults;
        private ShadowRenderer shadowRenderer = new ShadowRenderer();
        #endregion
        
        #region methods
        public void Setup(ScriptableRenderContext renderContext, CullingResults cullingResults, ShadowSettings shadowSettings)
        {
            this.cullingResults = cullingResults;
            commandBuffer.BeginSample(bufferName);
            shadowRenderer.Setup(renderContext, cullingResults, shadowSettings);
            SetupLights();
            shadowRenderer.Render();
            commandBuffer.EndSample(bufferName);
            renderContext.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
        }

        public void Cleanup()
        {
            shadowRenderer.Cleanup();
        }
        
        private void SetupLights()
        {
            NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
            int directionalLightCount = 0;
            int otherLightCount = 0;

            for (int i = 0; i < visibleLights.Length; ++i)
            {
                VisibleLight visibleLight = visibleLights[i];

                switch (visibleLight.lightType)
                {
                    case LightType.Directional:
                    {
                        if (directionalLightCount < maxDirectionalLightCount)
                        {
                            SetupDirectionalLight(directionalLightCount++, ref visibleLight);
                        }
                        
                        break;
                    }

                    case LightType.Point:
                    {
                        if (otherLightCount < maxOtherLightCount)
                        {
                            SetupPointLight(otherLightCount++, ref visibleLight);
                        }
                        
                        break;
                    }

                    case LightType.Spot:
                    {
                        if (otherLightCount < maxOtherLightCount)
                        {
                            SetupSpotLight(otherLightCount++, ref visibleLight);
                        }
                        
                        break;
                    }
                }
            }
            // Light light = RenderSettings.sun;
            // commandBuffer.SetGlobalVector(directionalLightColorPropertyID, light.color.linear * light.intensity);
            // commandBuffer.SetGlobalVector(directionalLightDirectionPropertyID, -light.transform.forward);

            commandBuffer.SetGlobalInt(directionalLightCountPropertyID, directionalLightCount);

            if (directionalLightCount > 0)
            {
                commandBuffer.SetGlobalVectorArray(directionalLightColorsPropertyID, directionalLightColors);
                commandBuffer.SetGlobalVectorArray(directionalLightDirectionsPropertyID, directionalLightDirections);
                commandBuffer.SetGlobalVectorArray(directionalShadowInfosPropertyID, _DirectionalShadowInfos);
            }
            
            commandBuffer.SetGlobalInt(otherLightCountPropertyID, otherLightCount);

            if (otherLightCount > 0)
            {
                commandBuffer.SetGlobalVectorArray(otherLightColorsPropertyID, otherLightColors);
                commandBuffer.SetGlobalVectorArray(otherLightPositionsProoertyID, otherLightPositions);
                commandBuffer.SetGlobalVectorArray(otherLightDirectionsPropertyID, otherLightDirections);
                commandBuffer.SetGlobalVectorArray(otherLightSpotAnglesPropertyID, otherLightSpotAngles);
            }
        }

        private void SetupDirectionalLight(int index, ref VisibleLight visiblelight)
        {
            directionalLightColors[index] = visiblelight.finalColor; // final color already usedthe light's intensity
            directionalLightDirections[index] = -visiblelight.localToWorldMatrix.GetColumn(2); // ?? remeber to revise
            _DirectionalShadowInfos[index] = shadowRenderer.ReserveDirectionalShadows(visiblelight.light, index);
        }
        
        private void SetupPointLight (int index, ref VisibleLight visibleLight) 
        {
            otherLightColors[index] = visibleLight.finalColor;
            Vector4 position = visibleLight.localToWorldMatrix.GetColumn(3);
            position.w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);
            otherLightPositions[index] = position;
            otherLightDirections[index] = Vector4.zero;
            otherLightSpotAngles[index] = new Vector4(0f, 1f);
        }
        
        private void SetupSpotLight(int index, ref VisibleLight visibleLight) 
        {
            otherLightColors[index] = visibleLight.finalColor;
            Vector4 position = visibleLight.localToWorldMatrix.GetColumn(3);
            position.w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);
            otherLightPositions[index] = position;
            otherLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2); // ?? remeber to revise
            
            Light light = visibleLight.light;
            float innerCos = Mathf.Cos(Mathf.Deg2Rad * 0.5f * light.innerSpotAngle);
            float outerCos = Mathf.Cos(Mathf.Deg2Rad * 0.5f * visibleLight.spotAngle);
            float angleRangeInv = 1f / Mathf.Max(innerCos - outerCos, 0.001f);
            otherLightSpotAngles[index] = new Vector4(angleRangeInv, -outerCos * angleRangeInv);
        }
        #endregion
    }
}