using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.AccessControl;
using Assets._Project.Scripts.treatment;
using Leap.Unity.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TreeEditor;

public class MeshCreateControlPoints : MonoBehaviour
{
    //public TreatSelectionManager treatSelectionManager;
    [HideInInspector] public Transform _initializedControlPoints;
    private readonly List<Transform> _newPosCP = new List<Transform>();
    private readonly List<Vector3> _newScalePosCPs = new List<Vector3>();
    private Vector3 barCenter;
    [SerializeField] private Material barCenterCage;

    [HideInInspector] public Vector3[] cageVertices;

    public bool collision;

    private float collisionSliVal = new float();
    private List<GameObject> controlPointList = new List<GameObject>();
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material outlineMaterial;
    private int goCounter = 1;

    [HideInInspector] public Vector3[] initialControlPointPosition;

    [HideInInspector] public GameObject InitializedControlPoints;

    [HideInInspector] public Vector3[] initialModelVerticesPosition;

    [HideInInspector] public List<InteractionBehaviour> interactCP = new List<InteractionBehaviour>();

    private Mesh meshCage;
    private Mesh meshModel;
    [HideInInspector] public Vector3[] modelVertices;
    private double[,] newMatrixPositionModel;

    public GameObject objCage;
    public GameObject objModel;

    [HideInInspector] private List<Transform> PositionControlPoints;

    [HideInInspector] public ReadFileComputeNewcage readFileComputeNewcage;
    [HideInInspector] public ReadJson readJson;
    [HideInInspector] public Functionality functionality;
    [HideInInspector] public TreatSelectionManager treatSelectionManager;

    [HideInInspector] public float scale;

    [HideInInspector] public Vector3 scaleCenter;

    [HideInInspector] public bool scaleGO;

    [SerializeField] private readonly string selectableTag = "Selectable";

    public Slider slider;
    public float sliderValue;
    private float sliValCol;
    [SerializeField] private string spawnSelectableTag = "InitializeParent";
    private int[] trisCage;
    [HideInInspector] public int[] trisModel;
    [HideInInspector] public List<Material> materialGroup1 = new List<Material>();
    [HideInInspector] public List<Material> outlineMaterialGroup1 = new List<Material>();
    [HideInInspector] public List<ControlPointsData> cpDataList=new List<ControlPointsData>();

    public List<IGrouping<Color,ControlPointsData>> listTagsGroupedByIndex = new List<IGrouping<Color, ControlPointsData>>();
    [HideInInspector] public int colorAmountsOfDifferentlevels;
    private bool UpdateModification;
    private bool InitializeMesh;


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
        InitializeMesh = false;
        scale = 1;
        collision = false;
        InitializedControlPoints = new GameObject();
        InitializedControlPoints.name = "Initialized Control Points";
        InitializedControlPoints.tag = "InitializeParent";
        _initializedControlPoints = InitializedControlPoints.transform;

        //CreateControlPoints();

        ComputeBarCenter(modelVertices);

        scaleCenter = barCenter;
    }


    /// <summary>
    ///     function to create control points
    /// </summary>
    public void CreateControlPoints()
    {
        //Merge the same CPs that have different tags(belong to different segments)(with hierarchy and tree node)
        //loop for the different segments
        cpDataList.Clear();

        for (int i = 0; i < readJson.treeNodeLevelx.Count; i++)
        {
            for (int j = 0; j < readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Count; j++)
            {
                ControlPointsData cpData = new ControlPointsData();
                cpData = cpDataList.Find(x => (x.goIndex == readJson.treeNodeLevelx[i].GetData().cageVerticesIndex[j]));
                if (cpData != null)
                {
                    var getColor = readJson.treeNodeLevelx[i].GetData().color;
                    cpData.goTags.Add(readJson.treeNodeLevelx[i].GetData().tag);

                    cpData.goLevel = readJson.treeNodeLevelx[i].GetLevel();

                    cpData.goColor.Add(new Color(getColor[0], getColor[1], getColor[2],255)/255);
                }
                else
                {
                    cpData = new ControlPointsData();
                    //cpData.go = cube;
                    var getColor = readJson.treeNodeLevelx[i].GetData().color;
                    cpData.goIndex = readJson.treeNodeLevelx[i].GetData().cageVerticesIndex[j];
                    cpData.goTags.Add(readJson.treeNodeLevelx[i].GetData().tag);

                    cpData.goLevel = readJson.treeNodeLevelx[i].GetLevel();

                    cpData.goColor.Add(new Color(getColor[0], getColor[1], getColor[2],255)/255);
                    //Debug.Log("cpData.goTags vertices" + cpData.goTags.Last());
                    cpDataList.Add(cpData);
                }

                //for (int k = 0; k < cpDataList.Count; k++)
                //{
                //    Debug.Log("cpdatalist[k] " + cpDataList[k].goIndex);
                //}
                //Debug.Log("seperator");
            }
        }

        // compute(mix) the color for each Control Point; add level for each CP
        for (int i = 0; i < cpDataList.Count; i++)
        {
            Color mycolor = new Color(0,0,0,0);
            for (int j = 0; j < cpDataList[i].goColor.Count; j++)
            {
                mycolor += cpDataList[i].goColor[j];
            }
            mycolor /= cpDataList[i].goColor.Count;
            cpDataList[i].goColor.Clear();
            cpDataList[i].goColor.Add(mycolor);
            //Debug.Log("cpDataList[i].goColor          "+ cpDataList[i].goColor[0]);
        }





        //foreach (var VARIABLE in cpDataList.FindAll(x => x.goTags.Count() > 1))

        //foreach (var VARIABLE in cpDataList.FindAll(x => x.goColor.Count() > 1))

        CreateMaterial();
        //CreateLevelsMaterials();


        //load the defaut materials for the segments on the cage(with hierarchy)
        materialGroup1.Clear();
        outlineMaterialGroup1.Clear();
        for (int k = 0; k < colorAmountsOfDifferentlevels; k++)
        {
            materialGroup1.Add(Resources.Load(String.Format("level{0}, Default Material Group {1}", cpDataList.Last().goLevel, k), typeof(Material)) as Material);
            outlineMaterialGroup1.Add(Resources.Load(String.Format("level{0}, Outlined Material Group {1}", cpDataList.Last().goLevel, k), typeof(Material)) as Material);
        }
        //Debug.Log("materialGroup1.count" + materialGroup1[0]);

        //create all CPs
        if(controlPointList.Count==0)
        {
            for (var i = 0; i < meshCage.vertices.Length; i++)
            {
                GameObject ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var K = initialControlPointPosition[i];
                ControlPoint.transform.position = new Vector3(K[0], K[1], K[2]);
                ControlPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                ControlPoint.tag = selectableTag;

                // add cpdata that does not belong to a segment,they have only materials and gameobject
                //for (int j = 0; j < cpDataList.Count; j++)
                //{
                //    if (cageVertices[cpDataList[j].goIndex] != ControlPoint.transform.position)
                //    {
                //        ControlPointsData cpData = new ControlPointsData();
                //        cpData.go = ControlPoint;
                //        cpData.defautMaterial = defaultMaterial;
                //        cpData.outlineMaterial = null;
                //        cpDataList.Add(cpData);
                //    }
                //}


                ControlPoint.name = "Control Point " + goCounter;
                interactCP.Add(ControlPoint.AddComponent<InteractionBehaviour>());
                ControlPoint.GetComponent<Rigidbody>().useGravity = false;
                ControlPoint.GetComponent<Rigidbody>().isKinematic = true;
                ControlPoint.AddComponent<ChangeColor>();

                goCounter++;
                //ControlPoint.AddComponent<Rigidbody>().useGravity = false;
                ControlPoint.transform.parent = _initializedControlPoints;
                //var controlPointRenderer = ControlPoint.GetComponent<MeshRenderer>();
                //controlPointRenderer.material = defaultMaterial;
                controlPointList.Add(ControlPoint);
                //_newPosCP.Add(ControlPoint.transform);
                //PositionControlPoints.Add(ControlPoint.transform);

                //Destroy(ControlPoint.gameObject.GetComponent<Collider>());
            }
        }
        



        ////color segments CPs
        for (var i = 0; i < cpDataList.Count; i++)
        {
            //GameObject ControlPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            for (int j = 0; j < controlPointList.Count; j++)
            {
                if (controlPointList[j].transform.position == cageVertices[cpDataList[i].goIndex])
                {
                    cpDataList[i].go = controlPointList[j];
                    var tagSystem0 = cpDataList[i].go.GetComponent<CustomTag>();
                    if (tagSystem0 == null)
                    {
                        cpDataList[i].go.AddComponent<CustomTag>();
                    }
                    var tagSystem1 = cpDataList[i].go.GetComponent<CustomTag>();
                    tagSystem1.Clear();

                    for (int k = 0; k < cpDataList[i].goTags.Count; k++)
                    {
                        tagSystem1.Add(cpDataList[i].goTags[k]);
                    }

                    var controlPointRenderer = cpDataList[i].go.GetComponent<MeshRenderer>();
                    //Find the correct material for this CP
                    for (int k = 0; k < materialGroup1.Count; k++)
                    {
                        if (!treatSelectionManager.selectionList.Contains(cpDataList[i].go.transform))
                        {
                            if (cpDataList[i].goColor[0] == materialGroup1[k].color)
                            {
                                controlPointRenderer.material = materialGroup1[k];
                                //Debug.Log("this is a string inside add material to class 0");
                                cpDataList[i].defautMaterial = materialGroup1[k];
                                //Debug.Log("this is a string inside add material to class 1");
                                cpDataList[i].outlineMaterial = outlineMaterialGroup1[k];
                            }
                        }

                 
                    }


                }

                //if (controlPointList[j].transform.position != cageVertices[cpDataList[i].goIndex])
                //{
                //    var cpData = new ControlPointsData();
                //    cpData.go = controlPointList[j];
                //    cpData.defautMaterial = defaultMaterial;
                //    cpData.outlineMaterial = null;
                //    cpDataList.Add(cpData);
                //    Debug.Log("CPs that not bekong to a segment");
                //}


            }
        }
        //for (int j = 0; j < cpDataList.Count; j++)
        //{
        //    for (int k = 0; k < controlPointList.Count; k++)
        //    {
        //        if (controlPointList[k].transform.position != cageVertices[cpDataList[j].goIndex])
        //        {
        //            var cpData = new ControlPointsData();
        //            cpData.go = controlPointList[k];
        //            cpDataList.Add(cpData);
        //            Debug.Log("not equal");
        //        }
        //    }
        //}
        for (int i = 0; i < controlPointList.Count; i++)
        {
            var cpData = new ControlPointsData();
            cpData.go = controlPointList[i];
            //Debug.Log("cpDataList.Count x " + controlPointList[i].name);
            var controlPointsOfNoSegment = cpDataList.Find(x => x.go.name == controlPointList[i].name);
            if (controlPointsOfNoSegment == null && !treatSelectionManager.selectionList.Contains(cpData.go.transform)) //belong to the non=annotated segment
            {
                cpData.go.GetComponent<MeshRenderer>().material = defaultMaterial;
                cpData.defautMaterial = defaultMaterial;
                cpData.outlineMaterial = outlineMaterial;
                cpData.goColor.Add(new Color(0, 0, 0, 1));
                cpDataList.Add(cpData);
            }
        }

        _newPosCP.Clear();
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

    //create mats for model without hiearchy
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
        colorAmountsOfDifferentlevels=tagsGroupedByIndex.ToList().Count;

        // Create a simple material asset
        for (int i = 0; i < listTagsGroupedByIndex.Count(); i++)
        {
            var defautMaterial = new Material(Shader.Find("Diffuse"));
            var outlinedMaterial = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
            //AssetDatabase.CreateAsset(defautMaterial, "Assets/Resources/" + "Default Material Group" + i + ".mat");

            AssetDatabase.CreateAsset(defautMaterial, "Assets/Resources/" + String.Format("level{0}, Default Material Group {1}", cpDataList[0].goLevel,i) + ".mat");
            AssetDatabase.CreateAsset(outlinedMaterial, "Assets/Resources/" + String.Format("level{0}, outlined Material Group {1}", cpDataList[0].goLevel,i) + ".mat");

            //AssetDatabase.CreateAsset(outlinedMaterial, "Assets/Resources/" + "Outlined Material Group" + i + ".mat");
            defautMaterial.color = listTagsGroupedByIndex[i].Key;
            //outlinedMaterial.color = listTagsGroupedByIndex[i].Key / 255;
            var colorSource = listTagsGroupedByIndex[i].Key;
            outlinedMaterial.SetColor("_Color", new Color(colorSource.r, colorSource.g, colorSource.b, 0.8f));
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
        if (readJson.levelChange)
        {
            CreateControlPoints();

        }

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

        if (/*!InitializeMesh ||*/ UpdateModification || readJson.levelChange/*functionality.levelsChange*/)
        {

            UpdateCage(cageVertices, _newPosCP, meshCage);
        }
        

        if (scaleGO) GetNewPos();
        scaleGO = false;

        if (/*!InitializeMesh ||*/ UpdateModification || readJson.levelChange/*functionality.levelsChange*/)
        {
            UpdateModel();
            readJson.levelChange = false;
        }

        InitializeMesh = true;

    }




    //private void UpdataControlPoints()
    //{
    //    //obtain all the CPs
    //    for (int i = 0; i < cpDataListLevels1[0].Count; i++)
    //    {
            

    //        //judge if level is changed,if level is changed, change the color of the CPs
    //        for (int k = 0; k < functionality.levels.Count; k++)
    //        {
    //            if (functionality.levels[k] == true)
    //            {
    //                var controlPointRenderer = cpDataListLevels1[k][i].go.GetComponent<MeshRenderer>();

    //                for (int j = 0; j < materialGroup1[k].Count; j++)
    //                {
    //                    if (cpDataListLevels1[k][i].goColor[0] / 255 == materialGroup1[k][j].color)
    //                    {
    //                        //Debug.Log("cpDataListLevels1[k][i].goColor[0] / 255 "+ cpDataListLevels1[k][i].goColor[0] / 255);
    //                        //Debug.Log("materialGroup1[k][j].color " + materialGroup1[k][j].color);
    //                        controlPointRenderer.material = materialGroup1[k][j];
    //                        //Debug.Log("this is a string inside add material to class 0");
    //                        cpDataListLevels1[k][i].defautMaterial = materialGroup1[k][j];
    //                        //Debug.Log("this is a string inside add material to class 1");
    //                        cpDataListLevels1[k][i].outlineMaterial = outlineMaterialGroup1[k][j];
    //                        //Debug.Log("this is a string inside add material to class 2");
    //                    }
    //                }
                    
    //            }
    //        }
            
            
    //    }

    //}




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
        
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = (float) matrixMprime[i, 0];
            vertices[i].y = (float) matrixMprime[i, 1];
            vertices[i].z = (float) matrixMprime[i, 2];
            //colors[i] = Color.red;
        }

        ////full fill color array
        //var colors = new Color[vertices.Length];

        //for (var i = 0; i < readJson.AllSegVertsIndexes.Count; i++)
        //{
        //    var colorFromJson = readJson.AllSegColors[i];
        //    for (var j = 0; j < readJson.AllSegVertsIndexes[i].Count; j++)
        //    {
        //        //Debug.Log("this is a string inside j 1");
        //        colors[readJson.AllSegVertsIndexes[i][j]] =
        //        new Color(colorFromJson[0], colorFromJson[1], colorFromJson[2]) / 255;
        //    }
        //}

        //modify the color array that should have mixed color
        //Debug.Log("readJson.AllSegVertsIndexes[4].Count: " + readJson.AllSegVertsIndexes[4].Count);
        //Debug.Log("readJson.AllSegVertsIndexes[4].Count " + readJson.AllSegVertsIndexes[6].Count);
        //for (int i = 0; i < readJson.AllSegVertsIndexes[4].Count; i++)
        //{
        //    var colorFromJson4 = readJson.AllSegColors[4];
        //    var colorFromJson6 = readJson.AllSegColors[6];
        //    for (int j = 0; j < readJson.AllSegVertsIndexes[4].Count; j++)
        //    {
        //        if (readJson.AllSegVertsIndexes[4][i] == readJson.AllSegVertsIndexes[6][j])
        //            colors[readJson.AllSegVertsIndexes[4][i]] = new Color(colorFromJson4[0] + colorFromJson6[0], colorFromJson4[1] + colorFromJson6[1], colorFromJson4[2] + colorFromJson6[2]) / 510;
        //    }
        //}

        //Debug.Log("color not in rootNode(correct): " + colors[vertices.Length - 1]);
        //Debug.Log("color not in rootNode(correct) 1: " + colors[100]);

        ////full fill color array for level 0
        //var colors0 = new Color[vertices.Length];
        //for (var i = 0; i < readJson.AllSegVertsIndexes1[0].Count; i++)
        //{
        //    //first level, i-th segemnt [1][i]
        //    var intColor = readJson.rootNode.GetData().color;

        //    for (var j = 0; j < readJson.AllSegVertsIndexes1[0][i].Count; j++)
        //    {
        //        //Debug.Log("this is a string inside j 1");
        //        colors0[readJson.differentLevelModelSegments[0][i].verticesIndex[j]] = new Color(intColor[0] / 250, (float)intColor[1] / 250, (float)intColor[2] / 250);
        //    }
        //}



        ////full fill color array for level 1
        //var colors1 = new Color[vertices.Length];
        ////Debug.Log("readJson.AllSegVertsIndexes1[1] " + readJson.AllSegVertsIndexes1[1].Count);
        //for (var i = 0; i < readJson.AllSegVertsIndexes1[1].Count; i++)
        //{
        //    //first level, i-th segemnt [1][i]
        //    var intColor = readJson.rootNode.GetChild(readJson.differentLevelModelSegments[1][i]).ID.color;

        //    for (var j = 0; j < readJson.AllSegVertsIndexes1[1][i].Count; j++)
        //    {
        //        //Debug.Log("this is a string inside j 1");
        //        colors1[readJson.differentLevelModelSegments[1][i].verticesIndex[j]] = new Color(intColor[0] / 250, (float)intColor[1] / 250, (float)intColor[2] / 250);
        //    }
        //}


        ////modify the color array that should have mixed color
        //var intColor4 = readJson.differentLevelModelSegments[1][4].color;
        //var intColor6 = readJson.differentLevelModelSegments[1][6].color;

        //for (int i = 0; i < readJson.AllSegVertsIndexes1[1][4].Count; i++)
        //{

        //    for (int j = 0; j < readJson.AllSegVertsIndexes1[1][6].Count; j++)
        //    {
        //        if (readJson.AllSegVertsIndexes1[1][4][i] == readJson.AllSegVertsIndexes1[1][6][j])
        //            colors1[readJson.differentLevelModelSegments[1][4].verticesIndex[i]] = new Color(intColor4[0] + intColor6[0], intColor4[1] + intColor6[1], intColor4[2] + intColor6[2]) / 510;

        //    }
        //}

        //var levelsColors = new List<Color[]>();
        //levelsColors.Add(colors0);
        //levelsColors.Add(colors1);

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.triangles = trisModel;
        mesh.colors = readJson.colorArrayLevelx;

        //for (int i = 0; i < functionality.levels.Count; i++)
        //{
        //    if (functionality.levels[i] == true)
        //    {
        //        //mesh.colors = levelsColors[i];
        //    }
        //}


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