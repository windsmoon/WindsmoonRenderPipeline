using System;
using UnityEditor;
using UnityEngine;

namespace WindsmoonRP.Editor.Tools
{
    public class MeshTool
    {
        #region fileds
        [MenuItem("WindsmoonRP/Asset/Extract Mesh To Asset")]
        public static void ExtractMeshToAsset()
        {
            GameObject gameObject = Selection.activeGameObject;
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                Debug.LogError("no mesh filter");
                return;
            }

            AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh), "Assets/Temp/Mesh_" + DateTime.Now.Millisecond + ".mesh");            
        }
        
        [MenuItem("WindsmoonRP/Asset/Create Cube")]
        public static void CreatePrimitiveCube()
        {
            SavePrimitiveMeshToAsset(PrimitiveType.Cube);
        }
        
        [MenuItem("WindsmoonRP/Asset/Create Sphere")]
        public static void CreatePrimitiveSphere()
        {
            SavePrimitiveMeshToAsset(PrimitiveType.Sphere);
        }

        private static void SavePrimitiveMeshToAsset(PrimitiveType primitiveType)
        {
            string path = EditorUtility.SaveFilePanelInProject("select save path", "mesh_", "mesh", "select the save path");
            // string path = EditorUtility.SaveFilePanel("select path", "Assets", "mesh", "mesh");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SavePrimitiveMeshToAsset(primitiveType, path);
        }
            
        private static void SavePrimitiveMeshToAsset(PrimitiveType primitiveType, string path)
        {
            GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            mesh = UnityEngine.Object.Instantiate<Mesh>(mesh);
            UnityEngine.Object.DestroyImmediate(gameObject);
            AssetDatabase.CreateAsset(mesh, path);
        }
        #endregion
    }
}