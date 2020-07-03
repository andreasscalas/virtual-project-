using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeVertsColor : MonoBehaviour
{
    // Update is called once per frame
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Color.blue;
        }
        colors[10] = Color.red; colors[7] = Color.red; colors[11] = Color.red;
        colors[10] = Color.yellow; colors[6] = Color.yellow; colors[7] = Color.yellow;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        for (int i = 0; i < 12; i++)
        {
            Debug.Log("triangles " + " " + i + "    vertex " + triangles[3*i]+"     vertex position: "+ vertices[triangles[3 * i]]);
            Debug.Log("triangles " + " " + i + "    vertex " + triangles[3*i+1]+"     vertex position: "+ vertices[triangles[3 * i+1]]);
            Debug.Log("triangles " + " " + i + "    vertex " + triangles[3*i+2]+"     vertex position: "+ vertices[triangles[3 * i+2]]);
        }
    }
}
