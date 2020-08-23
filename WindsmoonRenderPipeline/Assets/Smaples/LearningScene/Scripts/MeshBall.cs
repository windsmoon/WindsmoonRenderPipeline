using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace WindsmoonRP.Samples.LearningScene
{
    public class MeshBall : MonoBehaviour
    {
        #region fields
        private static int baseColorPropertyID = Shader.PropertyToID("_BaseColor");
        private static int metallicPropertyID = Shader.PropertyToID("_Metallic");
        private static int smoothnessPropertyID = Shader.PropertyToID("_Smoothness");
        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private Material material;
        [SerializeField, Range(1, 1023)] 
        private int meshCount = 1023;
        [SerializeField] 
        private int radius = 10;
        [SerializeField]
        private LightProbeProxyVolume llpv = null;
        private Matrix4x4[] matrices;
        private Vector4[] baseColors;
        private float[] metallics;
        private float[] smoothnesses;
        private MaterialPropertyBlock materialPropertyBlock;
        #endregion
        
        #region unity methods
        private void Awake()
        {
            Init();
        }

        private void OnValidate()
        {
            Init();
        }
        
        private void Update()
        {
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, meshCount, materialPropertyBlock,
                ShadowCastingMode.On, true, 0, null, llpv ? LightProbeUsage.UseProxyVolume : LightProbeUsage.CustomProvided, llpv);
            // Graphics.DrawMeshInstanced(mesh, 0, material, matrices, meshCount, materialPropertyBlock);
        }
        #endregion
        
        #region fields
        private void Init()
        {
            matrices = new Matrix4x4[meshCount];
            baseColors = new Vector4[meshCount];
            metallics = new float[meshCount];
            smoothnesses = new float[meshCount];
            
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * radius,
                    Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f),
                    Vector3.one * Random.Range(0.5f, 1.5f));
                baseColors[i] = new Vector4(Random.value, Random.value, Random.value, Random.Range(0.5f, 1f));
                metallics[i] = Random.value < 0.25f ? 1f : 0f;
                smoothnesses[i] = Random.Range(0.05f, 0.95f);
            }
            
            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetVectorArray(baseColorPropertyID, baseColors);
            materialPropertyBlock.SetFloatArray(metallicPropertyID, metallics);
            materialPropertyBlock.SetFloatArray(smoothnessPropertyID, smoothnesses);

            if (!llpv)
            {
                Vector3[] positions = new Vector3[meshCount];
            
                for (int i = 0; i < matrices.Length; i++)
                {
                    positions[i] = matrices[i].GetColumn(3);
                }
            
                SphericalHarmonicsL2[] lightProbes = new SphericalHarmonicsL2[meshCount];
                Vector4[] occlusionProbes = new Vector4[meshCount];
                LightProbes.CalculateInterpolatedLightAndOcclusionProbes(positions, lightProbes, occlusionProbes); // consider the environment light
                materialPropertyBlock.CopySHCoefficientArraysFrom(lightProbes);
                materialPropertyBlock.CopyProbeOcclusionArrayFrom(occlusionProbes);
            }
        }
        #endregion
    }
}
