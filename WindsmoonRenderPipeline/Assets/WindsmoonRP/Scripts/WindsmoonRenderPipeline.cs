using UnityEngine;
using UnityEngine.Rendering;
using WindsmoonRP.Shadow;
using Unity.Collections;
using UnityEngine.Experimental.GlobalIllumination;

namespace WindsmoonRP
{
    public class WindsmoonRenderPipeline : RenderPipeline
    {
        #region fields
        private CameraRenderer cameraRenderer = new CameraRenderer();
        private bool useDynamicBatching;
        private bool useGPUInstancing;
        private ShadowSettings shadowSettings;

#if UNITY_EDITOR
        private static Lightmapping.RequestLightsDelegate requestLightDelegate =
            (Light[] lights, NativeArray<LightDataGI> output) =>
            {
                
                for (int i = 0; i < lights.Length; ++i)
                {
                    Light light = lights[i];
                    LightDataGI lightDataGI = new LightDataGI();
                    
                    switch (light.type)
                    {
                        case UnityEngine.LightType.Directional:
                        {
                            var directionalLight = new DirectionalLight();
                            LightmapperUtils.Extract(light, ref directionalLight);
                            lightDataGI.Init(ref directionalLight);
                            break;
                        }

                        case UnityEngine.LightType.Point:
                        {
                            var pointLight = new PointLight();
                            LightmapperUtils.Extract(light, ref pointLight);
                            lightDataGI.Init(ref pointLight);
                            break;
                        }

                        case UnityEngine.LightType.Spot:
                        {
                            var spotLight = new SpotLight();
                            LightmapperUtils.Extract(light, ref spotLight);
                            spotLight.innerConeAngle = light.innerSpotAngle * Mathf.Deg2Rad;
                            spotLight.angularFalloff = AngularFalloffType.AnalyticAndInnerAngle;
                            lightDataGI.Init(ref spotLight);
                            break;
                        }

                        case UnityEngine.LightType.Area:
                        {
                            var rectangleLight = new RectangleLight();
                            rectangleLight.mode = LightMode.Baked;
                            LightmapperUtils.Extract(light, ref rectangleLight);
                            lightDataGI.Init(ref rectangleLight);
                            break;
                        }
                        
                        default:
                        {
                            lightDataGI.InitNoBake(light.GetInstanceID());
                            break;
                        }
                    }
                    
                    lightDataGI.falloff = FalloffType.InverseSquared;
                    output[i] = lightDataGI;
                }
            };
#endif
        #endregion
        
        #region constructors
        public WindsmoonRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shadowSettings)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
            this.shadowSettings = shadowSettings;
            
#if UNITY_EDITOR
            Lightmapping.SetDelegate(requestLightDelegate);            
#endif
        }
        #endregion
        
        #region methods
        protected override void Render(ScriptableRenderContext renderContex, Camera[] cameras)
        {
            foreach (Camera camera in cameras)
            {
                cameraRenderer.Render(renderContex, camera, useDynamicBatching, useGPUInstancing, shadowSettings);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Lightmapping.ResetDelegate();
        }

        #endregion
    }
}