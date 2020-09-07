using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WindsmoonRP.Samples.LearningScene
{
    public class Settings : MonoBehaviour
    {
        #region fields
        [SerializeField]
        private float crossFadeTime = 0.5f;
        #endregion

        #region unity methods
        private void OnValidate()
        {
            LODGroup.crossFadeAnimationDuration = crossFadeTime;
        }
        #endregion

        #region methods
        [MenuItem("WindsmoonRP/Generate Objects")]
        private static void GenerateObjects()
        {
            GameObject root = new GameObject("Root");
            int rowCount = 7;
            int colCount = 7;
            int radius = 2;
            float rowOffset = ((float)rowCount / 2 - 0.5f) * radius;
            float colOffset = ((float)colCount / 2 - 0.5f) * radius;
            Material material = new Material(Shader.Find("Windsmoon RP/Windsmoon Lit"));
        
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    Vector3 pos = new Vector3(col * radius - colOffset, 0, -row * radius + rowOffset);
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.GetComponent<MeshRenderer>().sharedMaterial = material;
                    sphere.AddComponent<PerObjectMaterialProperties>().Random();
                    sphere.transform.position = pos;
                    sphere.transform.SetParent(root.transform, true);
                }
            }
        }
        #endregion
    }
}
