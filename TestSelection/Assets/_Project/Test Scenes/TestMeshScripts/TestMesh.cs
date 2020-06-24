using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.transform.position = new Vector3(0, 0, 0);
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[6];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 1, 0);
        vertices[2] = new Vector3(1, 0, 0);
        vertices[3] = new Vector3(1, 0, 0);
        vertices[4] = new Vector3(0, 1, 0);
        vertices[5] = new Vector3(1, 0, 1);

        int[] triangles=new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 3;
        triangles[4] = 4;
        triangles[5] = 5;

        Color[] colors = new Color[vertices.Length];

        colors[0] = Color.green; 
        colors[1] = Color.green; 
        colors[2] = Color.green;
        colors[3] = Color.red;
        colors[4] = Color.red;
        colors[5] = Color.red;


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        //for (int i = 0; i < mesh.vertices.Length; i++)
        //{
        //    var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.position = mesh.vertices[i];
        //    cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //}


    }

    // Update is called once per frame
}
