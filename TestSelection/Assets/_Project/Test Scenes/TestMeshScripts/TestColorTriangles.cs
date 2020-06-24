using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColorTriangles : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        //Create new colors array where the colors will be created.
        Color32[] colors = new Color32[vertices.Length];

        for (int i = 0; i < vertices.Length; i += 3)
        {
            colors[i] = new Color32(255, 0, 0, 255);
            colors[i + 1] = new Color32(0, 255, 0, 255);
            colors[i + 2] = new Color32(0, 0, 255, 255);
        }

        //assign the array of colors to the Mesh.
        mesh.colors32 = colors;
    }
}
