using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindsmoonRP.Samples.LearningScene
{
    [DisallowMultipleComponent]
    public class BaseColor : MonoBehaviour
    {
        #region fields
        private static readonly int baseColorPropertyID = Shader.PropertyToID("_BaseColor");
        [SerializeField]
        Color baseColor = Color.white;
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
            GetComponent<Renderer>().SetPropertyBlock(materialPripertyBlock);
        }
        #endregion
    }
}
