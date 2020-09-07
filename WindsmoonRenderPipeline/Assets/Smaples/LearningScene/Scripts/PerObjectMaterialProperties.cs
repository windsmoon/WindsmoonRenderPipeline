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
        private static readonly int emissionColorPropertyID = Shader.PropertyToID("_EmissionColor");
        private static readonly int cutoffPropertyID = Shader.PropertyToID("_Cutoff");
        private static readonly int metallicPropertyID = Shader.PropertyToID("_Metallic");
        private static readonly int smoothnessPropertyID = Shader.PropertyToID("_Smoothness");
        [SerializeField]
        private Color baseColor = Color.white;
        [SerializeField, ColorUsage(false, true)]
        private Color emissionColor = Color.black;
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
            materialPripertyBlock.SetColor(emissionColorPropertyID, emissionColor);
            materialPripertyBlock.SetFloat(cutoffPropertyID, cutoff);
            materialPripertyBlock.SetFloat(metallicPropertyID, metallic);
            materialPripertyBlock.SetFloat(smoothnessPropertyID, smoothness);
            GetComponent<Renderer>().SetPropertyBlock(materialPripertyBlock);
        }
        #endregion

        #region methods
        [ContextMenu("Random")]
        public void Random()
        {
            baseColor = UnityEngine.Random.ColorHSV() + new Color(0.3f, 0.3f, 0.3f, 0.3f);
            emissionColor = UnityEngine.Random.ColorHSV() + new Color(0.6f, 0.6f, 0.6f, 1f);
            cutoff = UnityEngine.Random.Range(0f, 1f);
            metallic = UnityEngine.Random.Range(0f, 1f);
            smoothness = UnityEngine.Random.Range(0f, 1f);
            OnValidate();
        }

        [ContextMenu("Reset")]
        private void Reset()
        {
            baseColor = Color.white;
            emissionColor = Color.black;
            cutoff = 0.5f;
            metallic = 0f;
            smoothness = 0.5f;
            GetComponent<Renderer>().SetPropertyBlock(null);
        }
        #endregion
    }
}
