using UnityEngine;

namespace Adrenak.UniVRMedia {
    public static class Extensions {
        // Based on ReverseNormals.cs from Joachim Ante here : http://wiki.unity3d.com/index.php/ReverseNormals
        public static void InvertNormals(this Mesh mesh) {
            // Calculate normals
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            // Create triangles
            for (int m = 0; m < mesh.subMeshCount; m++) {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3) {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }
}