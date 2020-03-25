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
    public Vector3[] initialControlPointPosition;
    public Vector3[] initialModelVerticesPosition;

    public GameObject objCage;
    public GameObject objModel;
    private int[] trisCage;
    private int[] trisModel;
    double[,] newMatrixPositionModel;
    GameObject ControlPoint /*= new GameObject()*/;
    
    private List<Transform> newListPositionControlPoints = new List<Transform>();
    private List<Transform> newListPositionVerticesModel = new List<Transform>(100);
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material defaultMaterial;
    int goCounter=1;
    List<int> _indexOrder = new List<int>();
    //private List<Vector3> Ceshi1 /*= new List<Vector3>()*/;
    public ReadFileComputeNewcage readFileComputeNewcage;


    void Start()
    {
        CreateControlPoints();
    }
    /// <summary>
    /// function to create control points
    /// </summary>
    private void CreateControlPoints()
    {
        //extract the information of the cage mesh(vertices, tris)
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        meshModel = objModel.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        modelVertices = meshModel.vertices;
        initialControlPointPosition = meshCage.vertices;
        initialModelVerticesPosition = meshModel.vertices;
        trisCage = meshCage.triangles;
        trisModel = meshModel.triangles;
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
            Debug.Log("newly recorded newListPositionControlPoints" + "\t" + i + "\t" + newListPositionControlPoints[i].position.ToString("F6"));
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdateCageModification(cageVertices, newListPositionControlPoints, meshCage);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // assign the moved position of control points to the mesh vertices.
            double[,] newMatrixPositionControlPoints = new double[newListPositionControlPoints.Count,3];

            double[,] newMatrixPositionModelVertices = new double[readFileComputeNewcage.columnNumberUpdate,
                                                                 readFileComputeNewcage.columnNumberUpdate];

            for (int i = 0; i < newListPositionControlPoints.Count; i++)
            {
                newMatrixPositionControlPoints[i, 0]=newListPositionControlPoints[i].position.x;
                newMatrixPositionControlPoints[i, 1]=newListPositionControlPoints[i].position.y;
                newMatrixPositionControlPoints[i, 2]=newListPositionControlPoints[i].position.z;
            }
            // compute the model matrix M with the verticesPosition after deformation

            newMatrixPositionModelVertices = readFileComputeNewcage.computeProductBG(readFileComputeNewcage.barMatrices, newMatrixPositionControlPoints);
            /////convert the position from  matrix to accessible transform list（disposal step）

            ////////Debug.Log("newMatrixPositionControlPoints.Length/3\t" + newMatrixPositionModelVertices.Length / 3);
            ////////for (int i = 0; i < newMatrixPositionModelVertices.Length/3; i++)
            ////////{
            ////////    Debug.Log("问题在这里");
            ////////    double x = newListPositionVerticesModel[i].position.x;
            ////////    Debug.Log("newListPositionVerticesModel[i].position.x\t" + x);

            ////////    double y = newListPositionVerticesModel[i].position.y;
            ////////    double z = newListPositionVerticesModel[i].position.z;
            ////////    x = newMatrixPositionModelVertices[i, 0];
            ////////    y = newMatrixPositionModelVertices[i, 0];
            ////////    z = newMatrixPositionModelVertices[i, 0];
            ////////}

            // call the mesh modification function to update the mesh of the model cage
            UpdateModelModification(modelVertices, newMatrixPositionModelVertices, meshModel);

        }

    }

    private void UpdateModelModification(Vector3[] vertices, double[,] matrixMprime , Mesh mesh)
    {
        Debug.Log("vertices.Length\t" + vertices.Length);
        Debug.Log("matrixMprime.count/3\t" + matrixMprime.Length / 3);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = (float)matrixMprime[i, 0];
            vertices[i].y = (float)matrixMprime[i, 1];
            vertices[i].z = (float)matrixMprime[i, 2];
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisModel;
    }
    /// <summary>
    /// Update modification to the cage.
    /// </summary>
    private void UpdateCageModification(Vector3[] vertices, List<Transform> newListPosition, Mesh mesh)
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
}
