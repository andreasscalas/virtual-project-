using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;


public class MeshCreateControlPoints : MonoBehaviour
{

    Mesh meshCage;
    Mesh meshModel;
    [HideInInspector]
    public Vector3[] cageVertices;
    Vector3[] modelVertices;
    [HideInInspector]
    public Vector3[] initialControlPointPosition;
    [HideInInspector]
    public Vector3[] initialModelVerticesPosition;
    [HideInInspector]
    private Vector3 barCenter = new Vector3();

    public GameObject objCage;
    public GameObject objModel;
    private int[] trisCage;
    private int[] trisModel;
    double[,] newMatrixPositionModel;
    GameObject ControlPoint /*= new GameObject()*/;
    private List<Transform> _newPositionControlPoints = new List<Transform>();
    
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private string spawnSelectableTag = "InitializeParent";
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material barCenterCage;
    int goCounter=1;
    List<int> _indexOrder = new List<int>();
    //private List<Vector3> Ceshi1 /*= new List<Vector3>()*/;
    public ReadFileComputeNewcage readFileComputeNewcage;
    public TreatSelectionManager treatSelectionManager;
    public Transform _initializedControlPoints;
    [HideInInspector]
    public GameObject InitializedControlPoints;
    void Start()
    {
        InitializedControlPoints = new GameObject();
        InitializedControlPoints.name = "Initialized Control Points";
        InitializedControlPoints.tag = "InitializeParent";
        _initializedControlPoints = InitializedControlPoints.transform;

        CreateControlPoints();
    }
    /// <summary>
    /// function to create control points
    /// </summary>
    public void CreateControlPoints()
    {
        //extract the information of the cage mesh(vertices, tris)
        List<Transform> PositionControlPoints= new List<Transform>();
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        meshModel = objModel.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        modelVertices = meshModel.vertices;
        initialControlPointPosition = meshCage.vertices;
        initialModelVerticesPosition = meshModel.vertices;
        trisCage = meshCage.triangles;
        trisModel = meshModel.triangles;
        //Debug.Log("the vertices" + vertices);
        ////generate the control points
        for (int i = 0; i < meshCage.vertices.Length; i++)
        {
            ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 K = initialControlPointPosition[i];
            ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
            ControlPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ControlPoint.tag = selectableTag;
            ControlPoint.name = "\"Control Point\""+ goCounter;
            goCounter++;
            //ControlPoint.AddComponent<Rigidbody>().useGravity = false;
            ControlPoint.transform.parent = _initializedControlPoints;
            var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
            controlPointRenderer.material = defaultMaterial;
            _newPositionControlPoints.Add(ControlPoint.transform);
            PositionControlPoints.Add(ControlPoint.transform);
            //Destroy(ControlPoint.gameObject.GetComponent<Collider>());
        }
        //for (int i = 0; i < newListPositionControlPoints.Count; i++)
        //{
        //    Debug.Log("newly recorded newListPositionControlPoints" + "\t" + i + "\t" + newListPositionControlPoints[i].position.ToString("F6"));
        //}

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
        //if (Input.GetKeyDown(KeyCode.V))
        //{
            UpdateCageModification(cageVertices, _newPositionControlPoints, meshCage);
        //}
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ComputeBarCenter(modelVertices);
            GameObject Bar = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Bar.transform.position = barCenter;
            Bar.transform.localScale= new Vector3(0.1f,0.1f,0.1f);
            MeshRenderer meshBar = Bar.GetComponent<MeshRenderer>();
            meshBar.material = barCenterCage;
        }
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        // assign the moved position of control points to the mesh vertices.
        double[,] newMatrixPositionControlPoints;
            newMatrixPositionControlPoints =ConvertListToMatrix(_newPositionControlPoints);
            

            double[,] newMatrixPositionModelVertices = new double[readFileComputeNewcage.columnNumberUpdate,
                                                                 readFileComputeNewcage.columnNumberUpdate];
            // compute the model matrix M with the verticesPosition after deformation
            newMatrixPositionModelVertices = readFileComputeNewcage.computeProductBG(readFileComputeNewcage.barMatrices, newMatrixPositionControlPoints);
            UpdateModelModification(modelVertices, newMatrixPositionModelVertices, meshModel);
        //}
    }

    private void ComputeBarCenter(Vector3[] _vec)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        for (int i = 0; i < _vec.Length; i++)
        {
            sum = _vec[i] + sum;
        }
        barCenter = (sum / _vec.Length);
        Debug.Log("barcenter  " + barCenter);
    }

    private double[,] ConvertListToMatrix(List<Transform> myList)
    {
        double[,] myMatrix = new double[myList.Count, 3];
        for (int i = 0; i < myList.Count; i++)
        {
            myMatrix[i, 0] = myList[i].position.x;
            myMatrix[i, 1] = myList[i].position.y;
            myMatrix[i, 2] = myList[i].position.z;
        }
        return(myMatrix);

    }

    public void ClickResetMesh()
    {
        //Reset Cage mesh and the control points
        ResetCageMesh(cageVertices, initialControlPointPosition, meshCage);
        //Reset Model mesh 
        meshModel.vertices = initialModelVerticesPosition;
        meshModel.triangles = trisModel;
        

        ////GameObject cuube;
        ////cuube = GameObject.Find("Cube");
        ////Destroy(cuube.gameObject.GetComponent<Rigidbody>());
       
    }

    private void ResetCageMesh(Vector3[] vertices, Vector3[] DefaultPosition, Mesh mesh)
    {
       
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = DefaultPosition[i];
            _newPositionControlPoints[i].position = initialControlPointPosition[i];
        }
        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisCage;
        
    }

    private void UpdateModelModification(Vector3[] vertices, double[,] matrixMprime , Mesh mesh)
    {
        //Debug.Log("vertices.Length\t" + vertices.Length);
        //Debug.Log("matrixMprime.count/3\t" + matrixMprime.Length / 3);
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
