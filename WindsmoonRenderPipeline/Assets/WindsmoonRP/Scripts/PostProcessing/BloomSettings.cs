using System;
using UnityEngine;

namespace WindsmoonRP.PostProcessing
{
    [Serializable]
    public struct BloomSettings
    {
        #region fields
        [Range(0, 16)]
        public int MaxBloomIterationCount;
        [Min(1)]
        public int MinResolution;
        [SerializeField]
        public bool UseBicubicUpsampling;
        #endregion

        #region methods
        public void Init()
        {
            MaxBloomIterationCount = 16;
            MinResolution = 1;
            UseBicubicUpsampling = true;
        }
        #endregion
    }
}