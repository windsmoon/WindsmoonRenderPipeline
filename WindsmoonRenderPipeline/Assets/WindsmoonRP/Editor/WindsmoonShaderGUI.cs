using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WindsmoonRP.Editor
{
    public class WindsmoonShaderGUI : ShaderGUI
    {
        #region fields
        private MaterialEditor editor;
        private Object[] materials;
        private MaterialProperty[] properties;
        #endregion
        
        #region properties
        private bool ClippingAlpha 
        {
            set { SetToggleKeyword("_AlphaClipping", "_CLIPPING", value); }
        }

        private bool PremultiplyAlpha 
        {
            set { SetToggleKeyword("_PremultiplyAlpha", "_PREMULTIPLY_ALPHA", value); }
        }

        private BlendMode SrcBlend 
        {
            set { SetProperty("_SrcBlend", (float)value); }
        }

        private BlendMode DstBlend 
        {
            set { SetProperty("_DstBlend", (float)value); }
        }

        private bool ZWrite
        {
            set { SetProperty("_ZWrite", value ? 1f : 0f); }
        }

        private ShadowMode ShadowMode
        {
            set
            {
                if (SetProperty("_Shadow_Mode", (float)value))
                {
                    SetKeyword("_SHADOW_MODE_CLIP", value == ShadowMode.Clip);
                    SetKeyword("_SHADOW_MODE_DITHER", value == ShadowMode.Dither);
                }
            }
        }
        
        private RenderQueue RenderQueue 
        {
            set 
            {
                foreach (Material m in materials) 
                {
                    m.renderQueue = (int)value;
                }
            }
        }
        #endregion
        
        #region methods
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);
            editor = materialEditor;
            materials = materialEditor.targets;
            this.properties = properties;
            SetOpaque();
            SetClippingAlpha();
            SetFade();
            SetTransparent();
        }

        private bool SetProperty(string name, float value)
        {
            MaterialProperty property = FindProperty(name, properties);

            if (property == null)
            {
                return false;
            }

            property.floatValue = value;
            return true;
        }

        private void SetToggleKeyword(string propertyName, string keywordName, bool value)
        {
            if (SetProperty(propertyName, value ? 1f : 0f))
            {
                SetKeyword(keywordName, value);
            }
        }
        
        private void SetKeyword(string name, bool isEnable)
        {
            if (isEnable)
            {
                foreach (Material material in materials)
                {
                    material.EnableKeyword(name);
                }
            }

            else
            {
                foreach (Material material in materials)
                {
                    material.DisableKeyword(name);
                }
            }
        }

        private bool HasProperty(string name)
        {
            return FindProperty(name, properties, false) != null;
        }
        
        private void SetOpaque()
        {
            if (DrawButton("Opaque")) 
            {
                ClippingAlpha = false;
                PremultiplyAlpha = false;
                SrcBlend = BlendMode.One;
                DstBlend = BlendMode.Zero;
                ZWrite = true;
                RenderQueue = RenderQueue.Geometry;
                ShadowMode = ShadowMode.On;
            }
        }
        
        private void SetClippingAlpha()
        {
            if (DrawButton("Clipping Alpha")) 
            {
                ClippingAlpha = true;
                PremultiplyAlpha = false;
                SrcBlend = BlendMode.One;
                DstBlend = BlendMode.Zero;
                ZWrite = true;
                RenderQueue = RenderQueue.AlphaTest;
                ShadowMode = ShadowMode.Clip;
            }
        }
        
        private void SetFade()
        {
            if (DrawButton("Fade")) 
            {
                ClippingAlpha = false;
                PremultiplyAlpha = false;
                SrcBlend = BlendMode.SrcAlpha;
                DstBlend = BlendMode.OneMinusSrcAlpha;
                ZWrite = false;
                RenderQueue = RenderQueue.Transparent;
                ShadowMode = ShadowMode.Dither;
            }
        }
        
        private void SetTransparent()
        {
            if (HasProperty("_PremultiplyAlpha") && DrawButton("Transparent")) 
            {
                ClippingAlpha = false;
                PremultiplyAlpha = true;
                SrcBlend = BlendMode.One;
                DstBlend = BlendMode.OneMinusSrcAlpha;
                ZWrite = false;
                RenderQueue = RenderQueue.Geometry;
                ShadowMode = ShadowMode.Dither;
            }
        }
        
        private bool DrawButton(string name) 
        {
            if (GUILayout.Button(name)) 
            {
                editor.RegisterPropertyChangeUndo(name);
                return true;
            }
            
            return false;
        }

        #endregion
    }
}