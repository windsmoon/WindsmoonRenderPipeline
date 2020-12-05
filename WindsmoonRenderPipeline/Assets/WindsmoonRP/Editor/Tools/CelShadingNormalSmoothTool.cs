using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
    
namespace WindsmoonRP.Editor.Tools
{
    public class CelShadingNormalSmoothTool
    {
        [MenuItem("WindsmoonRP/Cel Shading/Smooth Normal")]
        public static void WirteAverageNormalToTangentToos()
        {
            string[] guids = Selection.assetGUIDs;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith(".prefab"))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    
                    MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
            
                    foreach (var meshFilter in meshFilters)
                    {
                        Mesh mesh = meshFilter.sharedMesh;
                        WirteAverageNormalToTangent(mesh);
                    }

                    SkinnedMeshRenderer[] skinMeshRenders = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            
                    foreach (var skinMeshRender in skinMeshRenders)
                    {
                        Mesh mesh = skinMeshRender.sharedMesh;
                        WirteAverageNormalToTangent(mesh);
                    }
                }

                else if (path.EndsWith(".mesh"))
                {
                    Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    WirteAverageNormalToTangent(mesh);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Smooth Nromal Finish");
        }

        private static void WirteAverageNormalToTangent(Mesh mesh)
        {
            var averageNormalDict = new Dictionary<Vector3, Vector3>();
            Vector3[] vertices = mesh.vertices;
            
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 vertex = vertices[i];
                
                if (averageNormalDict.ContainsKey(vertex) == false)
                {
                    averageNormalDict.Add(vertex, mesh.normals[i]);
                }
                
                else
                {
                    averageNormalDict[vertex] = (averageNormalDict[vertex] + mesh.normals[i]).normalized;
                }
            }

            var averageNormals = new Vector3[mesh.vertexCount];
            
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                averageNormals[i] = averageNormalDict[mesh.vertices[i]];
            }

            Vector2[] uv2s = new Vector2[mesh.vertexCount];
            Vector4[] tangents = new Vector4[mesh.vertexCount];
            
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 averageNormal = averageNormals[i];
                uv2s[i] = new Vector2(averageNormal.x, averageNormal.y);
                Vector4 meshTangent = mesh.tangents[i];
                tangents[i] = new Vector4(meshTangent.x, meshTangent.y, meshTangent.z, averageNormal.z);
            }
            
            mesh.uv2 = uv2s;
            mesh.tangents = tangents;
        }
    }
}