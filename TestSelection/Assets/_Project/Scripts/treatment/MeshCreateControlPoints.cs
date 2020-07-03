using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets._Project.Scripts.treatment;
using Leap.Unity.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MeshCreateControlPoints : MonoBehaviour
{
    //public TreatSelectionManager treatSelectionManager;
    public Transform _initializedControlPoints;
    private readonly List<Transform> _newPosCP = new List<Transform>();
    private readonly List<Vector3> _newScalePosCPs = new List<Vector3>();
    private Vector3 barCenter;
    [SerializeField] private Material barCenterCage;

    [HideInInspector] public Vector3[] cageVertices;

    public bool collision;

    private float collisionSliVal = new float();
    private List<GameObject> controlPointList = new List<GameObject>();
    [SerializeField] private Material defaultMaterial;
    private int goCounter = 1;

    [HideInInspector] public Vector3[] initialControlPointPosition;

    [HideInInspector] public GameObject InitializedControlPoints;

    [HideInInspector] public Vector3[] initialModelVerticesPosition;

    [HideInInspector] public List<InteractionBehaviour> interactCP = new List<InteractionBehaviour>();

    private Mesh meshCage;
    private Mesh meshModel;
    public Vector3[] modelVertices;
    private double[,] newMatrixPositionModel;

    public GameObject objCage;
    public GameObject objModel;

    [HideInInspector] private List<Transform> PositionControlPoints;

    public ReadFileComputeNewcage readFileComputeNewcage;
    public ReadJson readJson;

    [HideInInspector] public float scale;

    public Vector3 scaleCenter;

    [HideInInspector] public bool scaleGO;

    [SerializeField] private readonly string selectableTag = "Selectable";

    public Slider slider;
    public float sliderValue;
    private float sliValCol;
    [SerializeField] private string spawnSelectableTag = "InitializeParent";
    private int[] trisCage;
    public int[] trisModel;
    public List<Material> materialGroup = new List<Material>();
    public List<Material> outlineMaterialGroup = new List<Material>();
    public List<ControlPointsData> cpDataList=new List<ControlPointsData>();

    public List<IGrouping<Color,ControlPointsData>> listTagsGroupedByIndex = new List<IGrouping<Color, ControlPointsData>>();
    private bool UpdateModification;
    private bool InitializeMesh;


    private void Start()
    {
        UpdateModification = true;
        InitializeMesh = false;
        scale = 1;
        collision = false;
        InitializedControlPoints = new GameObject();
        InitializedControlPoints.name = "Initialized Control Points";
        InitializedControlPoints.tag = "InitializeParent";
        _initializedControlPoints = InitializedControlPoints.transform;

        CreateControlPoints();

        ComputeBarCenter(modelVertices);

        scaleCenter = barCenter;

    }


    /// <summary>
    ///     function to create control points
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
        for (var i = 0; i < meshCage.vertices.Length; i++)
        {
            GameObject ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var K = initialControlPointPosition[i];
            ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
            ControlPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ControlPoint.tag = selectableTag;
            ControlPoint.name = "Control Point " + goCounter;
            interactCP.Add(ControlPoint.AddComponent<InteractionBehaviour>());
            ControlPoint.GetComponent<Rigidbody>().useGravity = false;
            ControlPoint.GetComponent<Rigidbody>().isKinematic = true;
            ControlPoint.AddComponent<ChangeColor>();

            goCounter++;
            //ControlPoint.AddComponent<Rigidbody>().useGravity = false;
            ControlPoint.transform.parent = _initializedControlPoints;
            var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
            controlPointRenderer.material = defaultMaterial;
            //_newPosCP.Add(ControlPoint.transform);
            //PositionControlPoints.Add(ControlPoint.transform);

            //Destroy(ControlPoint.gameObject.GetComponent<Collider>());
        }

        //Merge the same CPs that have different tags(belong to different segments) 
        for (int i = 0; i < readJson.CageAllSegVertIndex.Count; i++)
        {
            for (int j = 0; j < readJson.CageAllSegVertIndex[i].Count; j++)
            {
                ControlPointsData cpData = new ControlPointsData();
                cpData = cpDataList.Find(x => (x.goIndex == readJson.CageAllSegVertIndex[i][j]));
                if (cpData != null)
                {
                    cpData.goTags.Add(readJson.segmentTags[i]);
                    cpData.goColor.Add(new Color(readJson.AllSegColors[i][0], readJson.AllSegColors[i][1], readJson.AllSegColors[i][2]));
                }
                else
                {
                    cpData = new ControlPointsData();
                    //cpData.go = cube;
                    cpData.goIndex = readJson.CageAllSegVertIndex[i][j];
                    cpData.goTags.Add(readJson.segmentTags[i]);
                    cpData.goColor.Add(new Color(readJson.AllSegColors[i][0], readJson.AllSegColors[i][1], readJson.AllSegColors[i][2]));
                    //Debug.Log("cpData.goTags vertices" + cpData.goTags.Last());
                    cpDataList.Add(cpData);
                }

            }
        }

        // compute(mix) the color for each Control Point
        for (int i = 0; i < cpDataList.Count; i++)
        {
            Color mycolor = Color.black;
            for (int j = 0; j < cpDataList[i].goColor.Count; j++)
            {
                mycolor += cpDataList[i].goColor[j];
            }
            mycolor /= cpDataList[i].goColor.Count;
            cpDataList[i].goColor.Clear();
            cpDataList[i].goColor.Add(mycolor);
            //Debug.Log("cpDataList[i].goColor "+ cpDataList[i].goColor[0]);
        }

        //foreach (var VARIABLE in cpDataList.FindAll(x => x.goTags.Count() > 1))
        //{
        //    Debug.Log("repeat cubes tags" + VARIABLE);
        //}


        //foreach (var VARIABLE in cpDataList.FindAll(x => x.goColor.Count() > 1))
        //{
        //    Debug.Log("repeat cubes color" + VARIABLE);
        //}
        CreateMaterial();

        //load the defaut materials for the segments on the cage
        for (int i = 0; i < listTagsGroupedByIndex.Count; i++)
        {
            materialGroup.Add(Resources.Load("Default Material Group" + i, typeof(Material)) as Material);
            outlineMaterialGroup.Add(Resources.Load("Outlined Material Group" + i, typeof(Material)) as Material);
            ////Debug.Log(string.Format("ControlPointsInfo[i].goIndex: {0} ControlPointsInfo[i].goTags.count: {1}", ControlPointsInfo[i].goIndex, ControlPointsInfo[i].goTags.Count));
        }

        //Create CPs
        for (var i = 0; i < cpDataList.Count; i++)
        {
            GameObject ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cpDataList[i].go = ControlPoint;
            ControlPoint.transform.position = cageVertices[cpDataList[i].goIndex];
            ControlPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ControlPoint.tag = selectableTag;
            ControlPoint.name = "Control Point " + goCounter;
            interactCP.Add(ControlPoint.AddComponent<InteractionBehaviour>());
            ControlPoint.GetComponent<Rigidbody>().useGravity = false;
            ControlPoint.GetComponent<Rigidbody>().isKinematic = true;
            //Add tag(the segment that they belong to) to each CP 
            ControlPoint.AddComponent<CustomTag>();
            var tagSystem = ControlPoint.GetComponent<CustomTag>();
            for (int j = 0; j < cpDataList[i].goTags.Count; j++)
            {
                tagSystem.Add(cpDataList[i].goTags[j]);
            }
            
            goCounter++;
            //ControlPoint.AddComponent<Rigidbody>().useGravity = false;
            ControlPoint.transform.parent = _initializedControlPoints;
            var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
            //Find the correct material for this CP
            for (int j = 0; j < materialGroup.Count; j++)
            {
                if (cpDataList[i].goColor[0] / 255 == materialGroup[j].color)
                {
                    controlPointRenderer.material = materialGroup[j];
                    //Debug.Log("this is a string inside add material to class 0");
                    cpDataList[i].defautMaterial = materialGroup[j];
                    //Debug.Log("this is a string inside add material to class 1");
                    cpDataList[i].outlineMaterial = outlineMaterialGroup[j];
                    //Debug.Log("this is a string inside add material to class 2");
                }
            }
            //Debug.Log("this is a string outside add material to class");
            controlPointList.Add(ControlPoint);
            //_newPosCP.Add(ControlPoint.transform);
            //PositionControlPoints.Add(ControlPoint.transform);
            //Destroy(ControlPoint.gameObject.GetComponent<Collider>());
        }

        for (int i = 0; i < cageVertices.Length; i++)
        {
            for (int j = 0; j < controlPointList.Count; j++)
            {
                if (/*Vector3.Distance(cageVertexPos[j], cageVertices[i]) < 0.01f*/ controlPointList[j].transform.position == cageVertices[i])
                {
                    _newPosCP.Add(controlPointList[j].transform);
                    PositionControlPoints.Add(controlPointList[j].transform);
                    //Debug.Log("ControlPoint.transform.position" + ControlPoint.transform.position);
                }
            }
        }

    }


    void CreateMaterial()
    {
        // regroup indexes of the same color

        var tagsGroupedByIndex = cpDataList.GroupBy(x => x.goColor[0]);
        //Debug.Log("This is the amount of different tags " + tagsGroupedByIndex.Count());
        //foreach (var group in tagsGroupedByIndex)
        //{
        //    Debug.Log("the vetex indexes of the color " + group.Key + ":");
        //    foreach (var x in group)
        //        Debug.Log("* " + x.goIndex);
        //}

        listTagsGroupedByIndex = tagsGroupedByIndex.ToList();

        // Create a simple material asset
        for (int i = 0; i < listTagsGroupedByIndex.Count(); i++)
        {
            var defautMaterial = new Material(Shader.Find("Diffuse"));
            var outlinedMaterial = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
            AssetDatabase.CreateAsset(defautMaterial, "Assets/Resources/" + "Default Material Group" + i + ".mat");
            AssetDatabase.CreateAsset(outlinedMaterial, "Assets/Resources/" + "Outlined Material Group" + i + ".mat");
            defautMaterial.color = listTagsGroupedByIndex[i].Key/255;
            //outlinedMaterial.color = listTagsGroupedByIndex[i].Key / 255;
            var colorSource = listTagsGroupedByIndex[i].Key / 255;
            outlinedMaterial.SetColor("_Color", new Color(colorSource.r, colorSource.g, colorSource.b, 0.8f ));
            //outlinedMaterial.SetFloat("_Outline", 0.015f);
            outlinedMaterial.SetColor("_OutlineColor", Color.yellow);
        }
    }



    // extract the postion in TransformList
    private List<Vector3> convertTransformPosition(List<Transform> listTransformInput)
    {
        var toReturn = new List<Vector3>();
        for (var i = 0; i < listTransformInput.Count; i++) toReturn.Add(listTransformInput[i].position);
        return toReturn;
    }

    private void Update()
    {
        sliderValue = slider.value;

        UpdateModification = false;

        for (int i = 0; i < _newPosCP.Count; i++)
        {
            if (_newPosCP[i].transform.position != cageVertices[i])
            {
                UpdateModification = true;
                break;
            }
        }

        if(!InitializeMesh || UpdateModification)
        { UpdateCage(cageVertices, _newPosCP, meshCage); }
        

        if (scaleGO) GetNewPos();
        scaleGO = false;

        if (!InitializeMesh || UpdateModification)
        {
            UpdateModel();
            Debug.Log("this is a update");
        }

        InitializeMesh = true;
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

        var vec = new List<Vector3>();
        for (var i = 0; i < PositionControlPoints /*initialControlPointPosition*/.Count; i++)
            vec.Add(PositionControlPoints /*initialControlPointPosition*/[i].position - scaleCenter);
        for (var i = 0; i < vec.Count; i++)
        {
            vec[i] *= scale;
            _newScalePosCPs.Add(vec[i] + scaleCenter);
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
        var sum = new Vector3(0, 0, 0);
        for (var i = 0; i < _vec.Length; i++) sum = _vec[i] + sum;
        barCenter = sum / _vec.Length;
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
        slider.value = 1;
        scale = 1;
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

    private void UpdateModelModification(Vector3[] vertices, double[,] matrixMprime, Mesh mesh)
    {
        //Debug.Log("vertices.Length\t" + vertices.Length);
        //Debug.Log("matrixMprime.count/3\t" + matrixMprime.Length / 3);
        var colors = new Color[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = (float) matrixMprime[i, 0];
            vertices[i].y = (float) matrixMprime[i, 1];
            vertices[i].z = (float) matrixMprime[i, 2];
            //colors[i] = Color.red;
        }

        //full fill color array
        for (var i = 0; i < readJson.AllSegVertsIndexes.Count ; i++)
        {
            var colorFromJson = readJson.AllSegColors[i];
            for (var j = 0; j < readJson.AllSegVertsIndexes[i].Count; j++)
            {
                //Debug.Log("this is a string inside j 1");
                colors[readJson.AllSegVertsIndexes[i][j]] =
                new Color(colorFromJson[0], colorFromJson[1], colorFromJson[2]) / 255;
            }
        }

        //modify the color array that should have mixed color
        for (int i = 0; i < readJson.AllSegVertsIndexes[4].Count; i++)
        {
            var colorFromJson4 = readJson.AllSegColors[4];
            var colorFromJson6 = readJson.AllSegColors[6];
            for (int j = 0; j < readJson.AllSegVertsIndexes[6].Count; j++)
            {
                if (readJson.AllSegVertsIndexes[4][i] == readJson.AllSegVertsIndexes[6][j])
                    colors[readJson.AllSegVertsIndexes[4][i]] =new Color(colorFromJson4[0] + colorFromJson6[0], colorFromJson4[1] + colorFromJson6[1], colorFromJson4[2] + colorFromJson6[2]) / 510;
            }
        }



        //Debug.Log("the colors "+ readJson.AllSegColors[i][0]+" "+ readJson.AllSegColors[i][1]+" "+ readJson.AllSegColors[i][2]);
        //colorCorrector += readJson.AllSegVertsIndexAmounts[i];

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisModel;
        mesh.colors = colors;

        //Debug.Log("color 2859 " + colors[trisModel[2859 * 3 + 0]]);
        //Debug.Log("color 2859 " + colors[trisModel[2859 * 3 + 0]]);
        //Debug.Log("color 2859 " + colors[trisModel[2859 * 3 + 0]]);


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