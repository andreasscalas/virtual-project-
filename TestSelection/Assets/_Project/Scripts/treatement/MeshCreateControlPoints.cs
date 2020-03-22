using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;


public class MeshCreateControlPoints : MonoBehaviour
{

    Mesh meshCage;
    Mesh meshModel;
    Vector3[] cageVertices;
    Vector3[] modelVertices;
    public GameObject objCage;
    public GameObject objModel;
    private int[] trisCage;
    private int[] trisModel;
    private double[,] newMatrixPositionControlPoints;
    private double[,] newMatrixPositionModel;
    GameObject ControlPoint /*= new GameObject()*/;
    Vector3[] initialControlPointPosition;
    private List<Transform> newListPositionControlPoints = new List<Transform>();
    public ReadFileComputeNewcage ReadFileComputeNewcage;
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material defaultMaterial;
    int goCounter=1;
    private List<int> _indexOrder = new List<int>();
    //private List<Vector3> Ceshi1 /*= new List<Vector3>()*/;

    void Start()
    {

        CreateControlPoints();

        //Ceshi1=convertTransfromPosition(newListPositionControlPoints);
        _indexOrder = mapping(initialControlPointPosition, ReadFileComputeNewcage.cageMatrices);
        //display mapping rule
        for (int i = 0; i < _indexOrder.Count; i++)
        {
            Debug.Log("this is mapping rule" + _indexOrder[i]);
        }
    }
    /// <summary>
    /// function to create control points
    /// </summary>
    private void CreateControlPoints()
    {
        //extract the information of the cage mesh(vertices, tris)
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        initialControlPointPosition = meshCage.vertices;
        trisCage = meshCage.triangles;
        //Debug.Log("the vertices" + vertices);

        //generate the control points
        for (int i = 0; i < meshCage.vertices.Length; i++)
        {
            ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 K = initialControlPointPosition[i];
            ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
            ControlPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ControlPoint.tag = selectableTag;
            ControlPoint.name = "\"Control Point\""+ goCounter.ToString();
            goCounter++;
            var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
            controlPointRenderer.material = defaultMaterial;
            newListPositionControlPoints.Add(ControlPoint.transform);
        }
        for (int i = 0; i < newListPositionControlPoints.Count; i++)
        {
            Debug.Log("newly recorded newListPositionControlPoints" + "\t" /*+ i + "\t" */+ newListPositionControlPoints[i].position.ToString("F6"));
        }
    }


    // extract the postion in TransformList
    private List<Vector3> convertTransfromPosition(List<Transform> listTransfromInput)
    {
        List<Vector3> toReturn = new List<Vector3>();
        for (int i = 0; i < listTransfromInput.Count; i++)
        {
            toReturn.Add(listTransfromInput[i].position);
        }
        return toReturn;
    }

    // returns List of int, which is the mapping rule from.
    private List<int> mapping(Vector3[] positionInUnity, double[,] matrixCage)
    {
        // list of int
        List<int> order = new List<int>();

        for (int i = 0; i < positionInUnity.Length; i++)
        {
            int j;
            for (j = 0; j < (matrixCage.Length) / 3; j++)
            {
                if (positionInUnity[i]./*position.*/x == matrixCage[j, 0])
                {
                    if (positionInUnity[i]./*position.*/y == matrixCage[j, 1])
                    {
                        if (positionInUnity[i]./*position.*/z == matrixCage[j, 2])
                        {
                            order.Add(j);
                        }
                    }
                }
            }
            //matrixCage.RemoveRow(j); can be optimized
        }
        return order;
    }




    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
            UpdateModification(cageVertices, newListPositionControlPoints, meshCage);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //////newMatrixPositionControlPoints = convertListtoMatrix(newListPositionControlPoints);
        //////newMatrixPositionModel = Matrix.Dot(ReadFileComputeNewcage.barMatrices, newMatrixPositionControlPoints);
        //////List<Vector3> newListPositionModel = newMatrixPositionModel.Cast<Vector3>().ToList();
        //////UpdateModelModification(modelVertices, newListPositionModel, meshModel);
        //}

    }
    /// <summary>
    /// Update modification to the cage.
    /// </summary>
    private void UpdateModification(Vector3[] vertices, List<Transform> newListPosition, Mesh mesh)
    {

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = newListPosition[i].position;
        }
        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisCage;
    }


    ////private void UpdateModelModification(Vector3[] vertices, List<Vector3> newListPosition, Mesh mesh)
    ////{

    ////    for (int i = 0; i < vertices.Length; i++)
    ////    {
    ////        vertices[i] = newListPosition[i];
    ////    }
    ////    // assign the local vertices array into the vertices array of the Mesh.
    ////    mesh.vertices = vertices;
    ////    mesh.RecalculateBounds();
    ////    mesh.triangles = trisCage;
    ////}





    ////////private void UpdateModification()
    ////////{
    ////////    for (var i = 0; i < cageVertices.Length; i++)
    ////////    {
    ////////        cageVertices[i] = newListPositionControlPoints[i].position;
    ////////    }
    ////////    // assign the local vertices array into the vertices array of the Mesh.
    ////////    meshCage.vertices = cageVertices;
    ////////    meshCage.RecalculateBounds();
    ////////    meshCage.triangles = trisCage;
    ////////}


    ///convert newPositionControlPoints into a matrix.
    ///
    //////private double[,] convertListtoMatrix(List<Transform> positionList)

    //////{
    //////    double[,] b = new double[positionList.Count, 3 /*c*/];

    //////    //list to matrix
    //////    for (int i = 0; i < positionList.Count; i++)
    //////    {
    //////        b[i, 0] = positionList[i].x;
    //////        b[i, 1] = positionList[i].y;
    //////        b[i, 2] = positionList[i].z;
    //////    }
    //////    return b;
    //////}


}
