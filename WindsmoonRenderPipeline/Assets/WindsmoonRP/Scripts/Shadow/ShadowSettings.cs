using UnityEngine;
using System;
using System.Reflection;

namespace WindsmoonRP.Shadow
{
    [Serializable]
    public class ShadowSettings
    {
        #region fields
        [Min(0f), SerializeField]
        private float maxDistance = 100f;
        [SerializeField]
        private DirectionalShadowSetting directionalShadowSetting = new DirectionalShadowSetting() {ShadowMapSize = TextureSize._2048, CascadeCount = 4,
            CascadeRatio1 = 0.1f, CascadeRatio2 = 0.25f, CascadeRatio3 = 0.5f};
        #endregion

        #region properties
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        public DirectionalShadowSetting DirectionalShadowSetting
        {
            get { return directionalShadowSetting; }
            set { directionalShadowSetting = value; }
        }
        #endregion
    }
}
