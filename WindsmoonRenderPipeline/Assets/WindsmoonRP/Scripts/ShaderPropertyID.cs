using UnityEngine;

namespace WindsmoonRP
{
    public static class ShaderPropertyID
    {
        #region fields
        public static int DirectionalShadowMap = Shader.PropertyToID("_DirectionalShadowMap");
        public static int DirectionalShadowMatrices = Shader.PropertyToID("_DirectionalShadowMatrices");
        public static int OtherShadowMap = Shader.PropertyToID("_OtherShadowMap");
        public static int OtherShadowMatrices = Shader.PropertyToID("_OtherShadowMatrices");
        #endregion
    }
}