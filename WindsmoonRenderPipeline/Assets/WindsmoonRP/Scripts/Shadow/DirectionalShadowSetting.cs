using System;
using UnityEngine;

namespace WindsmoonRP.Shadow
{
    [Serializable]
    public struct DirectionalShadowSetting
    {
        #region fields
        public TextureSize ShadowMapSize;
        [Range(1f, 4f)]
        public int CascadeCount;
        [Range(0f, 1f)]
        public float CascadeRatio1;
        [Range(0f, 1f)]
        public float CascadeRatio2;
        [Range(0f, 1f)]
        public float CascadeRatio3;
        #endregion

        #region properties
        public Vector3 CascadeRatios
        {
            get => new Vector3(CascadeRatio1, CascadeRatio2, CascadeRatio3);
        }
        #endregion
    }
}