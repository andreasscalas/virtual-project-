using UnityEngine;
using System.Collections;


public class VertsColor : MonoBehaviour
{


    void Start()
    {
        GameObject gameobject;
        VertColor(gameObject);
    }

    public void VertColor(GameObject gameObject)
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] verticesModified = new Vector3[triangles.Length];
        int[] trianglesModified = new int[triangles.Length];
        Color32 currentColor = new Color32();
        Color32[] colors = new Color32[triangles.Length];
        for (int i = 0; i < trianglesModified.Length; i++)
        {
            // Makes every vertex unique
            verticesModified[i] = vertices[triangles[i]];
            trianglesModified[i] = i;
            // Every third vertex randomly chooses new color
            colors[triangles[i]] = currentColor;
            if (triangles[i] % 3 == 0)
                currentColor = new Color(
                                Random.Range(0.0f, 1.0f),
                                Random.Range(0.0f, 1.0f),
                                Random.Range(0.0f, 1.0f),
                                1.0f);
        }
        // Applyes changes to mesh
        mesh.vertices = verticesModified;
        mesh.triangles = trianglesModified;
        mesh.colors32 = colors;
    }


}