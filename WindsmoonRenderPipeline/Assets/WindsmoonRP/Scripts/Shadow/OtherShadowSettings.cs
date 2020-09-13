using System;

namespace WindsmoonRP.Shadow
{
    [Serializable]
    public struct OtherShadowSettings
    {
        #region fields
        public TextureSize ShadowMapSize;
        public PCFMode PCFMode;
        #endregion
    }
}