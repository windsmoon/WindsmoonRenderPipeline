using System;
using UnityEngine;

namespace WindsmoonRP.PostProcessing
{
    [Serializable]
    public struct ShadowsMidtonesHighLightsSettings
    {
        #region fields
        [ColorUsage(false, true)]
        public Color Shadows;
        [ColorUsage(false, true)]
        public Color Midtones;
        [ColorUsage(false, true)]
        public Color Highlights;
        [Range(0f, 2f)]
        public float ShadowsStart;
        [Range(0f, 2f)]
        public float ShadowsEnd;
        [Range(0f, 2f)]
        public float HighLightsStart;
        [Range(0f, 2f)]
        public float HighLightsEnd;
        #endregion
    }
}