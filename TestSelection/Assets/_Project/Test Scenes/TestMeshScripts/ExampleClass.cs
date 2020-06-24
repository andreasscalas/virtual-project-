// Sets the vertex color to be red at the y=0 and green at y=1.
// (Note that most built-in Shaders don't display vertex colors. Use one that does, such as a Particle Shader, to see vertex colors)

using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    void Update()
    {
        Debug.Log("enter start");
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log("mesh.name " + mesh.name);
        Vector3[] vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        // assign the array of colors to the Mesh.
        mesh.colors = colors;
    }
}