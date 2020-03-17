using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;

namespace WindsmoonRP.Shadow
{
    public class ShadowRenderer
    {
        #region constants
        private const string bufferName = "Shadows";
        private const int maxDirectionalShadowCount = 4;
        private const int maxCascadeCount = 4;
        #endregion
        
        #region fields
        private CommandBuffer commandBuffer = new CommandBuffer { name = bufferName };
        private ScriptableRenderContext renderContext;
        private CullingResults cullingResults;
        private ShadowSettings shadowSettings;
        private DirectionalShadow[] directionalShadows = new DirectionalShadow[maxDirectionalShadowCount];
        private int currentDirectionalLightShadowCount;
        private Matrix4x4[] directionalShadowMatrices = new Matrix4x4[maxDirectionalShadowCount * maxCascadeCount];
        private static int cascadeCountPropertyID = Shader.PropertyToID("_CascadeCount");
        private static int cascadeCullingSpheresPropertyID = Shader.PropertyToID("_CascadeCullingSpheres");
//        private static int maxShadowDistancePropertyID = Shader.PropertyToID("_MaxShadowDistance");
        private static int shadowDistanceFadePropertyID = Shader.PropertyToID("_ShadowDistanceFade");
        private static Vector4[] cascadeCullingSpheres = new Vector4[maxCascadeCount];
        #endregion

        #region methods
        public void Setup(ScriptableRenderContext renderContext, CullingResults cullingResults, ShadowSettings shadowSettings)
        {
            this.renderContext = renderContext;
            this.cullingResults = cullingResults;
            this.shadowSettings = shadowSettings;
            currentDirectionalLightShadowCount = 0;
        }
        
        public Vector2 ReserveDirectionalShadows(Light light, int visibleLightIndex)
        {
            // GetShadowCasterBounds  return true if the light affects at least one shadow casting object in the Scene
            if (currentDirectionalLightShadowCount >= maxDirectionalShadowCount || light.shadows == LightShadows.None || light.shadowStrength <= 0f 
                || cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds bouds) == false)
            {
                return Vector2.zero;
            }

            directionalShadows[currentDirectionalLightShadowCount] = new DirectionalShadow(){visibleLightIndex = visibleLightIndex};
            return new Vector2(light.shadowStrength, shadowSettings.DirectionalShadowSetting.CascadeCount * currentDirectionalLightShadowCount++);
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

            int tileCount = currentDirectionalLightShadowCount * shadowSettings.DirectionalShadowSetting.CascadeCount;
            // todo : squared tile will waste texture space
            int splitCount = tileCount <= 1 ? 1 : tileCount <= 4 ? 2 : 4; // max tile count is 4 x 4 = 16, now the tile is squared
            int tileSize = shadowMapSize / splitCount;

            // ?? : An alternative approach is to apply a slope-scale bias, which is done by using a nonzero value for the second argument of SetGlobalDepthBias.
            // This value is used to scale the highest of the absolute clip-space depth derivative along the X and Y dimensions.
            // So it is zero for surfaces that are lit head-on, it's 1 when the light hits at a 45° angle in at least one of the two dimensions, and approaches infinity when the dot product of the surface normal and light direction reaches zero.
            // So the bias increases automatically when more is needed, but there's no upper bound. 
            commandBuffer.SetGlobalDepthBias(0, 1f);
            
            for (int i = 0; i < currentDirectionalLightShadowCount; ++i)
            {
                RenderDirectionalShadow(i, splitCount, tileSize); // this methods also set global shader properties
            }
            
            commandBuffer.SetGlobalDepthBias(0f, 0f);
            commandBuffer.SetGlobalInt(cascadeCountPropertyID, shadowSettings.DirectionalShadowSetting.CascadeCount);
            commandBuffer.SetGlobalVectorArray(cascadeCullingSpheresPropertyID, cascadeCullingSpheres);
            commandBuffer.SetGlobalMatrixArray(ShaderPropertyID.DirectionalShadowMatrices, directionalShadowMatrices);
//            commandBuffer.SetGlobalFloat(maxShadowDistancePropertyID, shadowSettings.MaxDistance);
            float cascadefade = 1 - shadowSettings.DirectionalShadowSetting.CascadeFade;
            commandBuffer.SetGlobalVector(shadowDistanceFadePropertyID, new Vector4(1 / shadowSettings.MaxDistance, 1 / shadowSettings.DistanceFade, 1f / (1f - cascadefade * cascadefade)));
            commandBuffer.EndSample(bufferName);
            ExecuteBuffer();
        }

        private void RenderDirectionalShadow(int index, int splitCount, int tileSize)
        {
            DirectionalShadow directionalShadow = directionalShadows[index];
            ShadowDrawingSettings shadowDrawingSettings = new ShadowDrawingSettings(cullingResults, directionalShadow.visibleLightIndex);

            int cascadeCount = shadowSettings.DirectionalShadowSetting.CascadeCount;
            int tileOffset = index * cascadeCount;
            Vector3 cascadeRatios = shadowSettings.DirectionalShadowSetting.CascadeRatios;

            for (int i = 0; i < cascadeCount; ++i)
            {
                // note : the split data contains information about how shadow caster objects should be culled
                cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(directionalShadow.visibleLightIndex, i, cascadeCount,
                    cascadeRatios, tileSize, 0f, out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData shadowSplitData);
                shadowDrawingSettings.splitData = shadowSplitData;

                if (index == 0) // set culling spheres, all directional light use only one group of culling spheres
                {
                    // note :  as the shadow projections are orthographic and square they end up closely fitting their culling sphere, but also cover some space around them
                    // that's why some shadows can be seen outside the culling regions
                    // also the light direction doesn't matter to the sphere, so all directional lights end up using the same culling spheres
                    // the camera is not at the sphere's center, but the surface of the sphere, all spheres will intersect at this point
                    Vector4 cullingSphere = shadowSplitData.cullingSphere; // w means sphere's radius
                    cullingSphere.w *= cullingSphere.w;
                    cascadeCullingSpheres[i] = cullingSphere;
                }

                int tileIndex = tileOffset + i;
                SetViewPort(tileIndex, splitCount, tileSize, out Vector2 offset);
                // Matrix4x4 vpMatrix = projectionMatrix * viewMatrix
                directionalShadowMatrices[tileIndex] = ConvertClipSpaceToTileSpace(projectionMatrix * viewMatrix, offset, splitCount);
                // directionalShadowMatrices[index] = projectionMatrix * viewMatrix;
                commandBuffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
                ExecuteBuffer();
                renderContext.DrawShadows(ref shadowDrawingSettings);
            }
        }

        private void SetViewPort(int index, int split, float tileSize, out Vector2 offset)
        {
            // left to right then up to down / down to up
            offset = new Vector2(index % split, index / split);
            commandBuffer.SetViewport(new Rect(offset.x * tileSize, offset.y * tileSize, tileSize, tileSize));
        }

        // ??
        private Matrix4x4 ConvertClipSpaceToTileSpace(Matrix4x4 matrix, Vector2 offset, int splitCount)
        {
            if (SystemInfo.usesReversedZBuffer)
            {
                matrix.m20 = -matrix.m20;
                matrix.m21 = -matrix.m21;
                matrix.m22 = -matrix.m22;
                matrix.m23 = -matrix.m23;
            }
            
            float scale = 1f / splitCount;
            matrix.m00 = (0.5f * (matrix.m00 + matrix.m30) + offset.x * matrix.m30) * scale;
            matrix.m01 = (0.5f * (matrix.m01 + matrix.m31) + offset.x * matrix.m31) * scale;
            matrix.m02 = (0.5f * (matrix.m02 + matrix.m32) + offset.x * matrix.m32) * scale;
            matrix.m03 = (0.5f * (matrix.m03 + matrix.m33) + offset.x * matrix.m33) * scale;
            matrix.m10 = (0.5f * (matrix.m10 + matrix.m30) + offset.y * matrix.m30) * scale;
            matrix.m11 = (0.5f * (matrix.m11 + matrix.m31) + offset.y * matrix.m31) * scale;
            matrix.m12 = (0.5f * (matrix.m12 + matrix.m32) + offset.y * matrix.m32) * scale;
            matrix.m13 = (0.5f * (matrix.m13 + matrix.m33) + offset.y * matrix.m33) * scale;
            matrix.m20 = 0.5f * (matrix.m20 + matrix.m30);
            matrix.m21 = 0.5f * (matrix.m21 + matrix.m31);
            matrix.m22 = 0.5f * (matrix.m22 + matrix.m32);
            matrix.m23 = 0.5f * (matrix.m23 + matrix.m33);
            return matrix;
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