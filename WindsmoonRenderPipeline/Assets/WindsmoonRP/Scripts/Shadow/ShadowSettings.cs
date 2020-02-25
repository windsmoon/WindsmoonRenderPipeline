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
        private DirectionalShadowSetting directionalShadowSetting = new DirectionalShadowSetting() {ShadowMapSize = TextureSize._2048};
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
