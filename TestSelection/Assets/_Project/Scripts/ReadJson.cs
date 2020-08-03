using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accord.Collections;
using Assets._Project.Scripts.treatment;
using Leap.Unity;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class ReadJson : MonoBehaviour
{
    private JsonData data1;

    private string jsonString1;

    [HideInInspector] private MeshCreateControlPoints meshCreateControlPoints;

    [HideInInspector] private ReadFileComputeNewcage readFileComputeNewcage;

    [HideInInspector] public List<int> ringFinger = new List<int>();

    [SerializeField] private Material segmentmMaterial;
    [SerializeField] private Material segmentmMaterial2;

    [HideInInspector] public bool switchSegment;

    [Range(-0.1f, 1.0f)] public double threshold;

    private double thresholdPrime;


    [HideInInspector] public List<int> trianCageSegmented = new List<int>();

    [HideInInspector] public List<int> trianModelSegmented = new List<int>();


    [HideInInspector] public List<ModelData> importedSegmentsOfDifferentLevels = new List<ModelData>();
    
    [HideInInspector] public Color[] colorArrayLevelx;
    

    [HideInInspector] public List<TreeNode> treeNodeLevelx = new List<TreeNode>();
    [HideInInspector] public int levelMax = new int();
    [HideInInspector] public int levelSelect;




    [HideInInspector] public int[] trisCage;
    [HideInInspector] public int[] trisModel;
    public GameObject objCage;
    public GameObject objModel;
    private Mesh meshCage;
    private Mesh meshModel;
    [HideInInspector] public Vector3[] modelVertices;
    [HideInInspector] public Vector3[] cageVertices;
    public List<string> segmentTags;
    //private List<int> level =new List<int>();
    //public List<int> modelLevel = new List<int>();
    public TreeNode rootNode;
    public bool levelChange;
    public Text levelRange;
    public Text level;
    public Text voiceLevelCommand;

    void Awake()
    {
        levelSelect = 0;
        levelChange = false;
        segmentTags = new List<string>();
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        meshModel = objModel.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        modelVertices = meshModel.vertices;
        trisCage = meshCage.triangles;
        trisModel = meshModel.triangles;
        colorArrayLevelx = new Color[modelVertices.Length];

        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        
        jsonString1 = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_hierarchical_nails.txt");

        data1 = JsonMapper.ToObject(jsonString1);


        


        //Instantiation of the the model's segments(instances of the segments) 
        for (var i = 0; i < data1["annotations"].Count; i++)
        {
            var datatest = JsonMapper.ToJson(data1["annotations"][i]);
            importedSegmentsOfDifferentLevels.Add(JsonMapper.ToObject<ModelData>(datatest));
            //Debug.Log("To see the colors  " + importedSegmentsOfDifferentLevels[i].color[0]);

        }


        var segmentLevel0= importedSegmentsOfDifferentLevels.Find(x => x.father==-1);

        rootNode = new TreeNode(segmentLevel0);
        //loop for levels
        for (int i = 0; i < importedSegmentsOfDifferentLevels.Count; i++)
        {
            if (importedSegmentsOfDifferentLevels[i].id==segmentLevel0.id)
            {
                continue;
            }
            var node = new TreeNode(importedSegmentsOfDifferentLevels[i]);
            var father= importedSegmentsOfDifferentLevels.Find(x => x.id == importedSegmentsOfDifferentLevels[i].father);


            if (rootNode.GetDescendent(father.id) != null)
            {
                rootNode.GetDescendent(father.id).Add(node);
            }
            
        }

        int idMax= importedSegmentsOfDifferentLevels.Max(x => x.id); ;


        levelMax = rootNode.GetDescendent(idMax).GetLevel();
        Debug.Log("levelMax "+levelMax);

        levelRange.text = "0-"+levelMax;
        TreatmentCPLevelx();


        var hierarchy = new GameObject();
        hierarchy.name = ("Hierarchy");
        rootNode.GetGameobject().transform.parent = hierarchy.transform;

        //BFS(rootNode);


        //Create the color arrays for the different-level-model








    }

    private void TreatmentCPLevelx()
    {
        //get the level x tree nodes
        treeNodeLevelx.Clear();
        for (int i = 0; i < importedSegmentsOfDifferentLevels.Count; i++)
        {
            var GetNode = rootNode.GetDescendent(importedSegmentsOfDifferentLevels[i].id);

            if (GetNode.GetLevel() == levelSelect /* x */)
            {
                treeNodeLevelx.Add(GetNode);
                //for (int j = 0; j < importedSegmentsOfDifferentLevels[i].verticesIndex.Count; j++)
                //{
                //    var Getcolor = importedSegmentsOfDifferentLevels[i].color;
                //    colorArrayLevelx.Add(new Color(Getcolor[0], Getcolor[1], Getcolor[2])/255);
                //}

                //Debug.Log("rootNode level x " + rootNode.GetDescendent(importedSegmentsOfDifferentLevels[i].id).GetData().tag);
            }
        }


        // find the vertices indexes for level x
        // loop for different segment
        for (var j = 0; j < treeNodeLevelx.Count; j++)
        {
            var GetSegmentTriangles = treeNodeLevelx[j].GetData().triangles;
            var GetSegmentVertexIndexes = treeNodeLevelx[j].GetData().verticesIndex;
            for (var i = 0; i < GetSegmentTriangles.Count; i++)
            {
                if (!GetSegmentVertexIndexes.Contains(trisModel[(GetSegmentTriangles[i]) * 3]))
                    GetSegmentVertexIndexes.Add(trisModel[(GetSegmentTriangles[i]) * 3]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[(GetSegmentTriangles[i]) * 3 + 1]))
                    GetSegmentVertexIndexes.Add(trisModel[(GetSegmentTriangles[i]) * 3 + 1]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[(GetSegmentTriangles[i]) * 3 + 2]))
                    GetSegmentVertexIndexes.Add(trisModel[(GetSegmentTriangles[i]) * 3 + 2]);
            }
        }

        //get the level x colors
        // loop for different segment
        Array.Clear(colorArrayLevelx, 0, colorArrayLevelx.Length);
        for (var i = 0; i < treeNodeLevelx.Count; i++)
        {
            var GetSegmentVertexIndexes = treeNodeLevelx[i].GetData().verticesIndex;
            var GetSegmentColors = treeNodeLevelx[i].GetData().color;

            for (var j = 0; j < GetSegmentVertexIndexes.Count; j++)
            {
                colorArrayLevelx[GetSegmentVertexIndexes[j]] +=
                    (new Color(GetSegmentColors[0], GetSegmentColors[1], GetSegmentColors[2], 255) / 255);
            }
        }

        //mix the colors
        for (int j = 0; j < colorArrayLevelx.Length; j++)
        {
            colorArrayLevelx[j] /= colorArrayLevelx[j].a;
        }


        TresholdComputeCageVertices();
    }

    // Start is called before the first frame update
    private void Start()
    {
        thresholdPrime = 1.1f;
        switchSegment = false;
        


        //for (int i = 0; i < AllSegVertsIndexes[0].Count; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //    cube.transform.position = modelVertices[AllSegVertsIndexes[0][i]];
        //    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //    //Debug.Log("the first segment cage vertices positions in readjson " + CageAllSegVertIndex[1][i] /*+ " " + CageAllSegVertIndex[0][i] + " " + CageAllSegVertIndex[i][2]*/);
        //}
    }

    private void TresholdComputeCageVertices()
    {
        //get level x cage vertices
        //compute the segments on the cage with a threshold 0.4(with hierarchy)
        for (int i = 0; i < treeNodeLevelx.Count; i++)
        {
            List<int> interLevelsCageSegVerts = new List<int>();
            interLevelsCageSegVerts.Clear();
            filterBarMatrix(0.4, treeNodeLevelx[i].GetData().verticesIndex, interLevelsCageSegVerts);
            treeNodeLevelx[i].GetData().cageVerticesIndex = interLevelsCageSegVerts;
            //for (int j = 0; j < treeNodeLevelx[i].GetData().cageVerticesIndex.Count; j++)
            //{
            //    Debug.Log("treeNodeLevelx[i].GetData().cageVerticesIndex:" + treeNodeLevelx[i].GetData().cageVerticesIndex[j]);
            //}
            //Debug.Log("separator 1:");
        }
    }

    private void Update()
    {
        if (thresholdPrime != threshold || switchSegment)
        {
            //MapSegModel(littleFinger);
            filterBarMatrix(threshold, trianModelSegmented, trianCageSegmented);
            deleteFilter();
            DrawFilter(trianCageSegmented);
            switchSegment = false;
            thresholdPrime = threshold;
        }

        if (levelChange)
        {
            TreatmentCPLevelx();
            level.text = levelSelect.ToString(); 
            //levelChange = false;
        }
    }

    public void ChangeLevel0()
    {
        voiceLevelCommand.text = "Level 0";
        levelSelect = 0;
        levelChange = true;
    }

    public void ChangeLevel1()
    {
        voiceLevelCommand.text = "Level 1";
        levelSelect = 1;
        levelChange = true;
    }

    public void ChangeLevel2()
    {
        voiceLevelCommand.text = "Level 2";
        levelSelect = 2;
        levelChange = true;
    }

    public void deleteFilter()
    {
        //destroy the previous segment.
        var objs = GameObject.FindGameObjectsWithTag("Segment");
        for (var i = 0; i < objs.Length; i++) Destroy(objs[i]);
    }

    private void DrawFilter(List<int> trianCageSegmented)
    {
        //draw new segment
        for (var i = 0; i < trianCageSegmented.Count; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<MeshRenderer>().material = segmentmMaterial;
            obj.transform.position = cageVertices[trianCageSegmented[i]];
            obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            obj.tag = "Segment";
        }
    }


    private void GetDataToLst(JsonData jsonData, List<int> Mylist)
    {
        for (var i = 0; i < jsonData.Count; i++)
        {
            var interData = Convert.ToString(jsonData[i]);
            //Debug.Log(interData);
            //Debug.Log(Int32.Parse(interData));
            var k = int.Parse(interData);
            Mylist.Add(k);
        }
    }


    /// <summary>
    ///     filter the Barycentric matrix by using a user defined threshold
    /// </summary>
    /// <param name="thresHold"></param>
    public void filterBarMatrix(double thres, List<int> ModelSegVertIndexesInput, List<int> ListCageSegOutput)
    {
        //Debug.Log("this is the a ModelData inside filter");
        ListCageSegOutput.Clear();
        for (var i = 0; i < ModelSegVertIndexesInput.Count; i++)
            //Debug.Log("this is the a ModelData inside for loop ModelSegVertIndexesInput.Count");
            //Debug.Log("this is the a ModelData before the loop readFileComputeNewcage.rowNumberUpdate " + readFileComputeNewcage.rowNumberUpdate);
        for (var j = 0; j < readFileComputeNewcage.columnNumberUpdate; j++)
        {
            if (readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j] > thres)//Debug.Log("readFileComputeNewcage.barMatrices " +j +" "+ readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j]);
            {
                if (!ListCageSegOutput.Contains(j))
                    ListCageSegOutput.Add(j);
            }
        }
        //Debug.Log("this is the positions of the elements less than threshold " + j);
        //Debug.Log("The related cage vertices amount: " + ListCageSegOutput.Count);
    }

    private void BFS(TreeNode start)
    {
        //LINE is the final order
        List<TreeNode> LINE = new List<TreeNode>();
        LINE.Add(start);

        //qq is a stack
        Queue qq = new Queue();
        qq.Enqueue(start);

        start.visited = true;

        TreeNode a = new TreeNode(new ModelData());

        while (qq.Count != 0)
        {
            a = new TreeNode(new ModelData());
            a = (TreeNode)qq.Dequeue();

            List<TreeNode> a_nbs = new List<TreeNode>();
            a_nbs = a.GetChildren();

            foreach (TreeNode tmp in a_nbs.Where(k => !k.visited).ToList())
            {
                qq.Enqueue(tmp);
                LINE.Add(tmp);
                tmp.visited = true;
            }
        }
    }



}