using System;
using UnityEngine;
using Object = System.Object;

namespace WindsmoonRP
{
    [ExecuteInEditMode]
    public class CelShading : MonoBehaviour
    {
        #region fields
        private MeshRenderer meshRenderer;
        private Material material;
        private bool hasEvent;
        #endregion

        #region unity methods
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.sharedMaterial;
        }

        private void OnEnable()
        {
            if (hasEvent == false)
            {
                CameraRenderer.PostRenderOpaqueEvent += OnPostRenderOpaque;
                hasEvent = true;
            }
        }

        private void OnDisable()
        {
            if (hasEvent)
            {
                CameraRenderer.PostRenderOpaqueEvent -= OnPostRenderOpaque;
                hasEvent = false;
            }
        }
        #endregion

        #region methods
        private void OnPostRenderOpaque(Object sender, RenderPointEventArgs args)
        {
            args.CommandBuffer.DrawRenderer(meshRenderer, material, 0, 1);    
        }
        #endregion
    }
}