using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets._Project.Scripts.treatment;
using Leap.Unity.Interaction;
using UnityEditor;
using UnityEngine;

public class MeshCreateControlPoints : MonoBehaviour
{
    private readonly List<Transform> _newPosCP = new List<Transform>();

    [SerializeField] private readonly string selectableTag = "Selectable";

    //public TreatSelectionManager treatSelectionManager;
    [HideInInspector] public Transform _initializedControlPoints;
    //private Vector3 barCenter;
    [SerializeField] private Material barCenterCage;

    [HideInInspector] public Vector3[] cageVertices;

    //public bool collision;

    //private float collisionSliVal = new float();
    [HideInInspector] public int colorAmountsOfDifferentlevels;
    private readonly List<GameObject> controlPointList = new List<GameObject>();
    [HideInInspector] public List<ControlPointsData> cpDataList = new List<ControlPointsData>();
    [SerializeField] private Material defaultMaterial;
    [HideInInspector] public Functionality functionality;
    private int goCounter = 1;

    [HideInInspector] public Vector3[] initialControlPointPosition;

    [HideInInspector] public GameObject InitializedControlPoints;

    [HideInInspector] public Vector3[] initialModelVerticesPosition;

    [HideInInspector] public List<InteractionBehaviour> interactCP = new List<InteractionBehaviour>();

    [HideInInspector] public List<IGrouping<Color, ControlPointsData>> listTagsGroupedByIndex =
        new List<IGrouping<Color, ControlPointsData>>();

    [HideInInspector] public List<Material> materialGroup1 = new List<Material>();

    private Mesh meshCage;
    private Mesh meshModel;
    [HideInInspector] public Vector3[] modelVertices;
    private double[,] newMatrixPositionModel;

    public GameObject objCage;
    public GameObject objModel;
    [SerializeField] private Material outlineMaterial;
    [HideInInspector] public List<Material> outlineMaterialGroup1 = new List<Material>();

    [HideInInspector] private List<Transform> PositionControlPoints;

    [HideInInspector] public ReadFileComputeNewcage readFileComputeNewcage;
    [HideInInspector] public ReadJson readJson;

    //[HideInInspector] public float scale;

    //[HideInInspector] public Vector3 scaleCenter;

    //[HideInInspector] public bool scaleGO; //scaleGO to stop scaling while mesh collides with walls

    //public Slider slider;
    //public float sliderValue;
    private float sliValCol;
    [SerializeField] private string spawnSelectableTag = "InitializeParent";
    [HideInInspector] public TreatSelectionManager treatSelectionManager;
    private int[] trisCage;
    [HideInInspector] public int[] trisModel;

    private bool UpdateModification;
    //private bool InitializeMesh;

    private void Start()
    {
        PositionControlPoints = new List<Transform>();
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        meshModel = objModel.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        modelVertices = meshModel.vertices;
        initialControlPointPosition = meshCage.vertices;
        initialModelVerticesPosition = meshModel.vertices;
        trisCage = meshCage.triangles;
        trisModel = meshModel.triangles;

        functionality = GameObject.Find("Selection Manager").GetComponent<Functionality>();
        readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        treatSelectionManager = GameObject.Find("Selection Manager").GetComponent<TreatSelectionManager>();
        UpdateModification = true;
        //InitializeMesh = false;
        //scale = 1;
        //collision = false;
        InitializedControlPoints = new GameObject();
        InitializedControlPoints.name = "Initialized Control Points";
        InitializedControlPoints.tag = "InitializeParent";
        _initializedControlPoints = InitializedControlPoints.transform;

        //CreateControlPoints();

        ComputeBarCenter(modelVertices);
        CreateControlPoints();

        //scaleCenter = barCenter;
    }


    /// <summary>
    ///     function to create control points
    /// </summary>
    public void CreateControlPoints()
    {
        //Merge the same CPs that have different tags(belong to different segments)(with hierarchy and tree node)
        //loop for the different segments
        cpDataList.Clear();
        controlPointList.Clear();

        for(var i = 0; i < cageVertices.Length; i++)
        {
            var cpData = new ControlPointsData();
            cpData.goIndex = i;
            cpData.defautMaterial = new Material(Shader.Find("Diffuse"));
            cpData.outlineMaterial = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
            cpData.defautMaterialAssetPath = "Assets/Resources/" + string.Format("level{0}, Default Material Group {1}", cpData.goLevel, cpData.goIndex) + ".mat";
            cpData.outlineMaterialAssetPath = "Assets/Resources/" + string.Format("level{0}, outlined Material Group {1}", cpData.goLevel, cpData.goIndex) + ".mat";

            AssetDatabase.CreateAsset(cpData.defautMaterial, cpData.defautMaterialAssetPath);
            AssetDatabase.CreateAsset(cpData.outlineMaterial, cpData.outlineMaterialAssetPath);

            cpData.defautMaterial.color = new Color(0f, 0f, 0f, 1f);
            cpData.outlineMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));

            Bounds bb = objModel.transform.parent.gameObject.GetComponent<Collider>().bounds;
            float bbDiagonalLength = (bb.min - bb.max).magnitude;
            float bbOutlineLengthRatio = 0.05f;
            cpData.outlineMaterial.SetFloat("_Outline", bbOutlineLengthRatio * bbDiagonalLength);
            cpData.outlineMaterial.SetColor("_OutlineColor", Color.yellow);
            //Debug.Log("cpData.goTags vertices" + cpData.goTags.Last());
            cpDataList.Add(cpData);
            var ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ControlPoint.tag = selectableTag;
            var K = initialControlPointPosition[i];
            ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
            float bbSphereLengthRatio = 0.02f;
            ControlPoint.transform.localScale = new Vector3(bbSphereLengthRatio * bbDiagonalLength, bbSphereLengthRatio * bbDiagonalLength, bbSphereLengthRatio * bbDiagonalLength);
            ControlPoint.name = "Control Point " + goCounter;
            interactCP.Add(ControlPoint.AddComponent<InteractionBehaviour>());
            ControlPoint.GetComponent<Rigidbody>().useGravity = false;
            ControlPoint.GetComponent<Rigidbody>().isKinematic = true;
            ControlPoint.AddComponent<ChangeColor>();
            goCounter++;
            ControlPoint.transform.parent = _initializedControlPoints;
            controlPointList.Add(ControlPoint);
            cpData.go = ControlPoint;
            ControlPoint.SetActive(false);
        }

    }

    //create materials for model
    private void CreateMaterial()
    {
        // regroup indexes of the same color
        var tagsGroupedByIndex = cpDataList.GroupBy(x => x.goColor[0]);
        listTagsGroupedByIndex = tagsGroupedByIndex.ToList();
        colorAmountsOfDifferentlevels = tagsGroupedByIndex.ToList().Count;

        // Create a material asset
        for (var i = 0; i < listTagsGroupedByIndex.Count(); i++)
        {
            var defautMaterial = new Material(Shader.Find("Diffuse"));
            var outlinedMaterial = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
            AssetDatabase.CreateAsset(defautMaterial,
                "Assets/Resources/" + string.Format("level{0}, Default Material Group {1}", cpDataList[0].goLevel, i) +
                ".mat");
            AssetDatabase.CreateAsset(outlinedMaterial,
                "Assets/Resources/" + string.Format("level{0}, outlined Material Group {1}", cpDataList[0].goLevel, i) +
                ".mat");

            defautMaterial.color = listTagsGroupedByIndex[i].Key;
            var colorSource = listTagsGroupedByIndex[i].Key;
            //set colors for the outline materials
            outlinedMaterial.SetColor("_Color", new Color(colorSource.r, colorSource.g, colorSource.b, 0.8f));
            outlinedMaterial.SetColor("_OutlineColor", Color.yellow);
        }
    }


    private void Update()
    {
        if (readJson.levelChange)
        {
            for (var i = 0; i < cpDataList.Count; i++)
            {
                cpDataList[i].go.SetActive(true);
                cpDataList[i].goTags.Clear();
                cpDataList[i].goColor.Clear();
            }
            

            for (var i = 0; i < readJson.treeNodeLevelx.Count; i++)
                for (var j = 0; j < readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Count; j++)
                {
                    var cpData = cpDataList.Find(x => x.goIndex == readJson.treeNodeLevelx[i].GetData().cageVerticesIndex[j]);
                    if (cpData != null)
                    {
                        var getColor = readJson.treeNodeLevelx[i].GetData().color;
                        cpData.goTags.Add(readJson.treeNodeLevelx[i].GetData().id.ToString());
                        cpData.goLevel = readJson.treeNodeLevelx[i].GetLevel();
                        cpData.goColor.Add(new Color(getColor[0], getColor[1], getColor[2], 255) / 255);
                    }
                }

            // compute(mix) the color for each Control Point; add level for each CP
            for (var i = 0; i < cpDataList.Count; i++)
            {
                var mycolor = new Color(0, 0, 0, 0);
                for (var j = 0; j < cpDataList[i].goColor.Count; j++) mycolor += cpDataList[i].goColor[j];
                mycolor /= cpDataList[i].goColor.Count;
                cpDataList[i].goColor.Clear();
                cpDataList[i].goColor.Add(mycolor);
                cpDataList[i].defautMaterial.SetColor("_Color", mycolor);
                cpDataList[i].outlineMaterial.SetColor("_Color", mycolor);
            }


            //color, add tags to CPs of different
            for (var i = 0; i < cpDataList.Count; i++)
            {
                var tagSystem0 = cpDataList[i].go.GetComponent<CustomTag>();
                if (tagSystem0 == null) cpDataList[i].go.AddComponent<CustomTag>();
                var tagSystem1 = cpDataList[i].go.GetComponent<CustomTag>();
                tagSystem1.Clear();

                for (var k = 0; k < cpDataList[i].goTags.Count; k++) tagSystem1.Add(cpDataList[i].goTags[k]);

                //var controlPointRenderer = cpDataList[i].go.GetComponent<MeshRenderer>();
                if (treatSelectionManager.selectionList.Contains(cpDataList[i].go.transform))
                    cpDataList[i].go.GetComponent<MeshRenderer>().material = treatSelectionManager.highlightMaterial;
                else
                {
                    cpDataList[i].go.GetComponent<MeshRenderer>().material = cpDataList[i].defautMaterial;

                }
            }

            //make the transforms inside the _newPosCP have the right order
            _newPosCP.Clear();
            for (var i = 0; i < cageVertices.Length; i++)
                for (var j = 0; j < controlPointList.Count; j++)
                    if (controlPointList[j].transform.position == cageVertices[i])
                    {
                        _newPosCP.Add(controlPointList[j].transform);
                        PositionControlPoints.Add(controlPointList[j].transform);
                        break;
                    }
        }

        //sliderValue = slider.value;

        UpdateModification = false;

        for (var i = 0; i < _newPosCP.Count; i++)
            if (_newPosCP[i].transform.position != cageVertices[i])
            {
                UpdateModification = true;
                break;
            }

        if (UpdateModification || readJson.levelChange /*functionality.levelsChange*/
        ) UpdateCage(cageVertices, _newPosCP, meshCage);


        //if (scaleGO) GetNewPos();
        //scaleGO = false;

        if ( /*!InitializeMesh ||*/ UpdateModification || readJson.levelChange /*functionality.levelsChange*/)
        {
            UpdateModel();
            readJson.levelChange = false;
        }

        //InitializeMesh = true;
    }


    private void UpdateModel()
    {
        double[,] newMatrixPositionControlPoints;
        newMatrixPositionControlPoints = ConvertListToMatrix(_newPosCP);
        var newMatrixPositionModelVertices = new double[readFileComputeNewcage.columnNumberUpdate,
            readFileComputeNewcage.columnNumberUpdate];
        // compute the model matrix M with the verticesPosition after deformation
        newMatrixPositionModelVertices =
            readFileComputeNewcage.computeProductBG(readFileComputeNewcage.barMatrices, newMatrixPositionControlPoints);
        UpdateModelModification(modelVertices, newMatrixPositionModelVertices, meshModel);
    }

    //public void GetNewPos()
    //{
    //    _newScalePosCPs.Clear();
    //    //for (int i = 0; i < initialControlPointPosition.Length; i++)
    //    //{
    //    //    double coefA = Math.Pow((initialControlPointPosition[i].x - scaleCenter/*barCenter*/.x), 2) + Math.Pow((initialControlPointPosition[i].y - scaleCenter/*barCenter*/.y), 2) + Math.Pow((initialControlPointPosition[i].z - scaleCenter/*barCenter*/.z), 2);
    //    //    double coefC = -Math.Pow(scaleRatio * Vector3.Distance(scaleCenter/*barCenter*/, initialControlPointPosition[i]), 2);
    //    //    var t = (Math.Sqrt(-4 * coefA * coefC)) / (2 * coefA);
    //    //    //Debug.Log("t " + t);
    //    //    _newScalePosCPs.Add(new Vector3((float)(scaleCenter/*barCenter*/.x + (initialControlPointPosition[i].x - scaleCenter/*barCenter*/.x) * t),
    //    //        (float)(scaleCenter/*barCenter*/.y + (initialControlPointPosition[i].y - scaleCenter/*barCenter*/.y) * t),
    //    //        (float)(scaleCenter/*barCenter*/.z + (initialControlPointPosition[i].z - scaleCenter/*barCenter*/.z) * t)));
    //    //    //Debug.Log("_newScalePosCPs "+ _newScalePosCPs[i]);
    //    //    _newPosCP[i].transform.position = _newScalePosCPs[i];
    //    //}

    //    var vec = new List<Vector3>();
    //    for (var i = 0; i < PositionControlPoints /*initialControlPointPosition*/.Count; i++)
    //        vec.Add(PositionControlPoints /*initialControlPointPosition*/[i].position - scaleCenter);
    //    for (var i = 0; i < vec.Count; i++)
    //    {
    //        //vec[i] *= scale;
    //        _newScalePosCPs.Add(vec[i] + scaleCenter);
    //        _newPosCP[i].transform.position = _newScalePosCPs[i];
    //    }
    //}

    //public void AdjustScaleRatio()
    //{
    //    var newColorBlock = slider.colors;
    //    if (!collision)
    //    {
    //        //to avoid the cumulative scale, the ratio should be divided by the previous slider value
    //        scale = slider.value / sliderValue;
    //        //record the slider value until collision happens
    //        sliValCol = sliderValue;
    //        scaleGO = true;
    //    }

    //    if (collision)
    //    {
    //        //Debug.Log("scaleRatio " + scaleRatio);
    //        //Debug.Log("scaleRatio * sliValCol " + scaleRatio * sliValCol);
    //        //slider.colors= ColorBlock.defaultColorBlock;
    //        Debug.Log("collision, color change!");
    //        newColorBlock.pressedColor = Color.red;
    //        newColorBlock.selectedColor = Color.red;
    //        slider.colors = newColorBlock;
    //    }

    //    if (collision && slider.value < scale * sliValCol)
    //    {
    //        scale = slider.value / sliderValue;
    //        scaleGO = true;
    //        newColorBlock.pressedColor = Color.white;
    //        newColorBlock.selectedColor = Color.white;
    //        slider.colors = newColorBlock;
    //    }
    //}


    private void ComputeBarCenter(Vector3[] _vec)
    {
        var sum = new Vector3(0, 0, 0);
        for (var i = 0; i < _vec.Length; i++) sum = _vec[i] + sum;
        //barCenter = sum / _vec.Length;
        //Debug.Log("barcenter  " + barCenter);
    }

    private double[,] ConvertListToMatrix(List<Transform> myList)
    {
        var myMatrix = new double[myList.Count, 3];
        for (var i = 0; i < myList.Count; i++)
        {
            myMatrix[i, 0] = myList[i].position.x;
            myMatrix[i, 1] = myList[i].position.y;
            myMatrix[i, 2] = myList[i].position.z;
        }

        return myMatrix;
    }

    public void ClickResetMesh()
    {
        //Reset Cage mesh and the control points
        ResetCageMesh(cageVertices, initialControlPointPosition, meshCage);
        //Reset Model mesh 
        meshModel.vertices = initialModelVerticesPosition;
        meshModel.triangles = trisModel;
        //Reset slider
        //slider.value = 1;
        //scale = 1;
        //Reset the control points
    }

    private void ResetCageMesh(Vector3[] vertices, Vector3[] DefaultPosition, Mesh mesh)
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = DefaultPosition[i];
            _newPosCP[i].position = initialControlPointPosition[i];
        }

        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisCage;
    }

    /// <summary>
    ///     Update modification to the model
    /// </summary>
    private void UpdateModelModification(Vector3[] vertices, double[,] matrixMprime, Mesh mesh)
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = (float) matrixMprime[i, 0];
            vertices[i].y = (float) matrixMprime[i, 1];
            vertices[i].z = (float) matrixMprime[i, 2];
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisModel;
        mesh.colors = readJson.colorArrayLevelx;

    }

    /// <summary>
    ///     Update modification to the cage.
    /// </summary>
    private void UpdateCage(Vector3[] vertices, List<Transform> newListPosition, Mesh mesh)
    {
        for (var i = 0; i < vertices.Length; i++) vertices[i] = newListPosition[i].position;
        // assign the local vertices array into the vertices array of the Mesh.
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisCage;
    }
}