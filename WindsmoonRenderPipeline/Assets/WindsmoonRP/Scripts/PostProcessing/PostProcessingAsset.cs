using UnityEngine;

namespace WindsmoonRP.PostProcessing
{
    [CreateAssetMenu(menuName = "WindsmoonRP/Create Post Processing Asset")]
    public class PostProcessingAsset : ScriptableObject
    {
        #region fields
        [SerializeField]
        private Shader shader;
        private Material material;
        #endregion

        #region properties
        public Material Material
        {
            get
            {
                if (material == null && shader != null)
                {
                    material = new Material(shader);
                    material.hideFlags = HideFlags.HideAndDontSave;
                }

                return material;
            }
        }
        #endregion
    }
}