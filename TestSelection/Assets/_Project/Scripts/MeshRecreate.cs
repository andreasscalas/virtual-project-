using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRecreate : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    //public GameObject model;
    public GameObject cage;
    //public ReadFileComputeNewcage ReadFileComputeNewcage;
    private int[] tris;
    GameObject ControlPoint /*= new GameObject()*/;
    Vector3[] initialControlPointPosition;
    Vector3[] movedPosition;
    private List<Transform> cageVertexesPosition = new List<Transform>();
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material defaultMaterial;

    void Start()
    {

        CreateControlPoints(cage);

    }
    /// <summary>
    /// function to create control points
    /// </summary>
    private void CreateControlPoints(GameObject cage)
    {
        mesh = cage.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        initialControlPointPosition = mesh.vertices;
        tris = mesh.triangles;
        Debug.Log("the vertices" + vertices);

            //generate the control points
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 K = initialControlPointPosition[i];
            ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
            ControlPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ControlPoint.tag = selectableTag;
            //obj = ControlPoint.gameObject;
            var ControlPointRenderer = ControlPoint.GetComponent<Renderer>();
            ControlPointRenderer.material = defaultMaterial;
            //var renderer = ControlPoint.GetComponent<MeshRenderer>();
            //renderer.material.SetColor("_Color", Color.blue);
            cageVertexesPosition.Add(ControlPoint.transform);
        }
        

    }

    void Update()
    {

        UpdateModification(cage);
        //UpdateModification(model);

    }
    /// <summary>
    /// Update modification to the cage.
    /// </summary>
    void UpdateModification(GameObject obj)
    {
        //mesh= obj.GetComponent<MeshFilter>().mesh;
        //List<Transform> myList = new List<Transform>();
        //if (obj.name.Contains("cage"))
        //{
         //   myList = cageVertexesPosition;
        //}
        //else
        //    myList = ReadFileComputeNewcage.lst;


        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = cageVertexesPosition[i].position;
        }
        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = tris;
    }



}
