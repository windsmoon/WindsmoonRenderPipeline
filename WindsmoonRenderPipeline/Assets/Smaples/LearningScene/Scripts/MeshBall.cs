using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WindsmoonRP.Samples.LearningScene
{
    public class MeshBall : MonoBehaviour
    {
        #region fields
        private static int baseColorPropertyID = Shader.PropertyToID("_BaseColor");
        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private Material material;
        [SerializeField, Range(1, 1023)] 
        private int meshCount = 1023;
        [SerializeField] 
        private int radius = 10;
        private Matrix4x4[] matrices;
        private Vector4[] baseColors;
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
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, meshCount, materialPropertyBlock);
        }
        #endregion
        
        #region fields
        private void Init()
        {
            matrices = new Matrix4x4[meshCount];
            baseColors = new Vector4[meshCount];
            
            for (int i = 0; i < matrices.Length; i++) 
            {
                matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 10f, Quaternion.identity, Vector3.one);
                baseColors[i] = new Vector4(Random.value, Random.value, Random.value, 1f);
            }
            
            materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetVectorArray(baseColorPropertyID, baseColors);
        }
        #endregion
    }
}
