using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindsmoonRP.Samples.LearningScene
{
    [DisallowMultipleComponent]
    public class PerObjectMaterialProperties : MonoBehaviour
    {
        #region fields
        private static readonly int baseColorPropertyID = Shader.PropertyToID("_BaseColor");
        private static readonly int cutoffPropertyID = Shader.PropertyToID("_Cutoff");
        private static readonly int metallicPropertyID = Shader.PropertyToID("_Metallic");
        private static readonly int smoothnessPropertyID = Shader.PropertyToID("_Smoothness");
        [SerializeField]
        private Color baseColor = Color.white;
        [SerializeField, Range(0f, 1f)]
        private float cutoff = 0.5f;
        [SerializeField, Range(0f, 1f)]
        private float metallic = 0f;
        [SerializeField, Range(0f, 1f)]
        private float smoothness = 0.5f;
        private static MaterialPropertyBlock materialPripertyBlock;
        #endregion

        #region unity methods
        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (materialPripertyBlock == null) 
            {
                materialPripertyBlock = new MaterialPropertyBlock();
            }
            
            materialPripertyBlock.SetColor(baseColorPropertyID, baseColor);
            materialPripertyBlock.SetFloat(cutoffPropertyID, cutoff);
            materialPripertyBlock.SetFloat(metallicPropertyID, metallic);
            materialPripertyBlock.SetFloat(smoothnessPropertyID, smoothness);
            GetComponent<Renderer>().SetPropertyBlock(materialPripertyBlock);
        }
        #endregion

        #region methods
        [ContextMenu("Random")]
        private void Random()
        {
            baseColor = UnityEngine.Random.ColorHSV();
            cutoff = UnityEngine.Random.Range(0f, 1f);
            metallic = UnityEngine.Random.Range(0f, 1f);
            smoothness = UnityEngine.Random.Range(0f, 1f);
            OnValidate();
        }
        #endregion
    }
}
