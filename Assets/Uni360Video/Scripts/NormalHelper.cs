/*
 * Based on ReverseNormals.cs from Joachim Ante here : http://wiki.unity3d.com/index.php/ReverseNormals
 */

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshFilterHelper {
    public static void InvertNormals(MeshFilter filter) {
        if (filter != null) {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

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