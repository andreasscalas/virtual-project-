using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Leap.Unity.Interaction;
using TMPro;
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
    List<Transform> PositionControlPoints;
    private Vector3 barCenter;
    public Vector3 scaleCenter;

    public GameObject objCage;
    public GameObject objModel;
    private int[] trisCage;
    private int[] trisModel;
    double[,] newMatrixPositionModel;
    GameObject ControlPoint /*= new GameObject()*/;
    private List<Transform> _newPosCP = new List<Transform>();
    private List<Vector3> _newScalePosCPs = new List<Vector3>();
    
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private string spawnSelectableTag = "InitializeParent";
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material barCenterCage;
    int goCounter=1;

    public ReadFileComputeNewcage readFileComputeNewcage;
    //public TreatSelectionManager treatSelectionManager;
   
    
    public Transform _initializedControlPoints;
    [HideInInspector]
    public GameObject InitializedControlPoints;

    public Slider slider;
    public float sliderValue;

    [HideInInspector]
    public float scale;
    [HideInInspector]
    public bool scaleGO;
    public bool collision;

    float collisionSliVal = new float();
    float sliValCol;

    [HideInInspector]
    public List<InteractionBehaviour> interactCP = new List<InteractionBehaviour>();

    void Start()
    {
        scale = 1;
        collision = false;
        InitializedControlPoints = new GameObject();
        InitializedControlPoints.name = "Initialized Control Points";
        InitializedControlPoints.tag = "InitializeParent";
        _initializedControlPoints = InitializedControlPoints.transform;
        //InitializedControlPoints.AddComponent<InteractionBehaviour>();
        //InitializedControlPoints.GetComponent<Rigidbody>().useGravity = false;
        CreateControlPoints();

        ComputeBarCenter(modelVertices);
        //VectorBarCagevertices();

        //ComputeBarCenter(modelVertices);
        //GameObject Bar = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //Bar.transform.position = barCenter;
        scaleCenter = barCenter;
        //Bar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //MeshRenderer meshBar = Bar.GetComponent<MeshRenderer>();
        //meshBar.material = barCenterCage;

    }
    /// <summary>
    /// function to create control points
    /// </summary>
    public void CreateControlPoints()
    {
        //extract the information of the cage mesh(vertices, tris)
        //List<Transform> PositionControlPoints= new List<Transform>();
        PositionControlPoints = new List<Transform>();
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
            ControlPoint.name = "Control Point "+ goCounter;
            interactCP.Add( ControlPoint.AddComponent<InteractionBehaviour>());
            ControlPoint.GetComponent<Rigidbody>().useGravity = false;
            ControlPoint.GetComponent<Rigidbody>().isKinematic = true;

            goCounter++;
            //ControlPoint.AddComponent<Rigidbody>().useGravity = false;
            ControlPoint.transform.parent = _initializedControlPoints;
            var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
            controlPointRenderer.material = defaultMaterial;
            _newPosCP.Add(ControlPoint.transform);
            PositionControlPoints.Add(ControlPoint.transform);
            
            //Destroy(ControlPoint.gameObject.GetComponent<Collider>());
        }
        //for (int i = 0; i < newListPositionControlPoints.Count; i++)
        //{
        //    Debug.Log("newly recorded newListPositionControlPoints" + "\t" + i + "\t" + newListPositionControlPoints[i].position.ToString("F6"));
        //}

    }


    // extract the postion in TransformList
    private List<Vector3> convertTransformPosition(List<Transform> listTransformInput)
    {
        List<Vector3> toReturn = new List<Vector3>();
        for (int i = 0; i < listTransformInput.Count; i++)
        {
            toReturn.Add(listTransformInput[i].position);
        }
        return toReturn;
    }

    void Update()
    {
        sliderValue = slider.value;
        UpdateCage(cageVertices, _newPosCP, meshCage);

        if (scaleGO)
        {
            GetNewPos();
        }
        scaleGO = false;

        UpdateModel();
        
    }

    private void UpdateModel()
    {
        double[,] newMatrixPositionControlPoints;
        newMatrixPositionControlPoints = ConvertListToMatrix(_newPosCP);
        double[,] newMatrixPositionModelVertices = new double[readFileComputeNewcage.columnNumberUpdate,
            readFileComputeNewcage.columnNumberUpdate];
        // compute the model matrix M with the verticesPosition after deformation
        newMatrixPositionModelVertices = readFileComputeNewcage.computeProductBG(readFileComputeNewcage.barMatrices, newMatrixPositionControlPoints);
        UpdateModelModification(modelVertices, newMatrixPositionModelVertices, meshModel);
    }

    public void GetNewPos()
    {
        _newScalePosCPs.Clear();
        //for (int i = 0; i < initialControlPointPosition.Length; i++)
        //{
        //    double coefA = Math.Pow((initialControlPointPosition[i].x - scaleCenter/*barCenter*/.x), 2) + Math.Pow((initialControlPointPosition[i].y - scaleCenter/*barCenter*/.y), 2) + Math.Pow((initialControlPointPosition[i].z - scaleCenter/*barCenter*/.z), 2);
        //    double coefC = -Math.Pow(scaleRatio * Vector3.Distance(scaleCenter/*barCenter*/, initialControlPointPosition[i]), 2);
        //    var t = (Math.Sqrt(-4 * coefA * coefC)) / (2 * coefA);
        //    //Debug.Log("t " + t);
        //    _newScalePosCPs.Add(new Vector3((float)(scaleCenter/*barCenter*/.x + (initialControlPointPosition[i].x - scaleCenter/*barCenter*/.x) * t),
        //        (float)(scaleCenter/*barCenter*/.y + (initialControlPointPosition[i].y - scaleCenter/*barCenter*/.y) * t),
        //        (float)(scaleCenter/*barCenter*/.z + (initialControlPointPosition[i].z - scaleCenter/*barCenter*/.z) * t)));
        //    //Debug.Log("_newScalePosCPs "+ _newScalePosCPs[i]);
        //    _newPosCP[i].transform.position = _newScalePosCPs[i];
        //}

        List<Vector3> vec = new List<Vector3>();
        for (int i = 0; i < PositionControlPoints/*initialControlPointPosition*/.Count; i++)
        {
            vec.Add(PositionControlPoints/*initialControlPointPosition*/[i].position - scaleCenter);
        }
        for (int i = 0; i < vec.Count; i++)
        {
            vec[i]*=scale;
            _newScalePosCPs.Add(vec[i]+ scaleCenter);
            _newPosCP[i].transform.position = _newScalePosCPs[i];
        }

    }

    public void AdjustScaleRatio()
    {
        var newColorBlock = slider.colors;
        if (!collision)
        {
            //to avoid the cumulative scale, the ratio should be divided by the previous slider value
            scale = slider.value / sliderValue;
            //record the slider value until collision happens
            sliValCol = sliderValue;
            scaleGO = true;
        }

        if (collision)
        {
            //Debug.Log("scaleRatio " + scaleRatio);
            //Debug.Log("scaleRatio * sliValCol " + scaleRatio * sliValCol);
            //slider.colors= ColorBlock.defaultColorBlock;
            Debug.Log("collision, color change!");
            newColorBlock.pressedColor = Color.red;
            newColorBlock.selectedColor = Color.red;
            slider.colors = newColorBlock;
        }

        if (collision && slider.value < scale * sliValCol)
        {
            scale = slider.value / sliderValue;
            scaleGO = true;
            newColorBlock.pressedColor = Color.white;
            newColorBlock.selectedColor = Color.white;
            slider.colors = newColorBlock;
        }

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
        //Reset slider
        slider.value = 1;
        scale = 1;

    }

    private void ResetCageMesh(Vector3[] vertices, Vector3[] DefaultPosition, Mesh mesh)
    {
       
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = DefaultPosition[i];
            _newPosCP[i].position = initialControlPointPosition[i];
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
    private void UpdateCage(Vector3[] vertices, List<Transform> newListPosition, Mesh mesh)
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
