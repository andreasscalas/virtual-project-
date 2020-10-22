using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets._Project.Scripts.treatment;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class ReadJson : MonoBehaviour
{
    [HideInInspector] public Vector3[] cageVertices;

    [HideInInspector] public Color[] colorArrayLevelx;
    private JsonData data1;


    //[SerializeField] private Material segmentmMaterial;
    //[SerializeField] private Material segmentmMaterial2;
    //[HideInInspector] public bool switchSegment;
    //[Range(-0.1f, 1.0f)] public double threshold;

    //private double thresholdPrime;

    [HideInInspector] public List<ModelData> importedSegmentsOfDifferentLevels = new List<ModelData>();

    private string jsonString1;
    public Text level;
    public bool levelChange;
    [HideInInspector] public int levelMax;
    public Text levelRange;
    [HideInInspector] public int levelSelect;
    private Mesh meshCage;

    [HideInInspector] private MeshCreateControlPoints meshCreateControlPoints;
    private Mesh meshModel;
    [HideInInspector] public Vector3[] modelVertices;
    public GameObject objCage;
    public GameObject objModel;

    [HideInInspector] private ReadFileComputeNewcage readFileComputeNewcage;

    //private List<int> level =new List<int>();
    //public List<int> modelLevel = new List<int>();
    public TreeNode rootNode;
    public List<string> segmentTags;

    [HideInInspector] public List<TreeNode> treeNodeLevelx = new List<TreeNode>();

    [HideInInspector] public int[] trisCage;
    [HideInInspector] public int[] trisModel;
    public Text voiceLevelCommand;

    private void Awake()
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
    }

    public void init() 
    { 
        jsonString1 =
            //File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_hierarchical_nails.txt");
            File.ReadAllText(Application.streamingAssetsPath + "/" + "flowered_teapot_simplified.ant.txt");

        data1 = JsonMapper.ToObject(jsonString1);

        //Instantiation of the the model's segments(instances of the segments) 
        for (var i = 0; i < data1["annotations"].Count; i++)
        {
            var datatest = JsonMapper.ToJson(data1["annotations"][i]);
            importedSegmentsOfDifferentLevels.Add(JsonMapper.ToObject<ModelData>(datatest));

            //UnityEngine.Debug.Log("To see the colors  " + importedSegmentsOfDifferentLevels[i].color[0]);
        }


        var segmentLevel0 = importedSegmentsOfDifferentLevels.Find(x => x.father == -1);

        rootNode = new TreeNode(segmentLevel0);
        //Andreas: Questa cosa è a dir poco oscena, ma sono costretto a farla per non riscrivere il codice 
        { 
            var GetSegmentTriangles = rootNode.GetData().triangles;
            var GetSegmentVertexIndexes = rootNode.GetData().verticesIndex;
            for (var j = 0; j < GetSegmentTriangles.Count; j++)
            {
                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3 + 1]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3 + 1]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3 + 2]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3 + 2]);

            }

            TresholdComputeCageVertices(rootNode);
        }
        //loop for levels
        for (var i = 0; i < importedSegmentsOfDifferentLevels.Count; i++)
        {
            if (importedSegmentsOfDifferentLevels[i].id == segmentLevel0.id) continue;
            var node = new TreeNode(importedSegmentsOfDifferentLevels[i]);
            
            var GetSegmentTriangles = node.GetData().triangles;
            var GetSegmentVertexIndexes = node.GetData().verticesIndex;
            for (var j = 0; j < GetSegmentTriangles.Count; j++)
            {
                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3 + 1]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3 + 1]);

                if (!GetSegmentVertexIndexes.Contains(trisModel[GetSegmentTriangles[j] * 3 + 2]))
                    GetSegmentVertexIndexes.Add(trisModel[GetSegmentTriangles[j] * 3 + 2]);

            }
            TresholdComputeCageVertices(node);
            

            var father =
                importedSegmentsOfDifferentLevels.Find(x => x.id == importedSegmentsOfDifferentLevels[i].father);


            if (rootNode.GetDescendent(father.id) != null) rootNode.GetDescendent(father.id).Add(node);
        }

        var idMax = importedSegmentsOfDifferentLevels.Max(x => x.id);
        UnityEngine.Debug.Log("levelMax detected " + idMax);


        levelMax = rootNode.GetDescendent(idMax).GetLevel();
        UnityEngine.Debug.Log("levelMax " + levelMax);

        levelRange.text = "0-" + levelMax;
        TreatmentCPLevelx();


        var hierarchy = new GameObject();
        hierarchy.name = "Hierarchy";
        rootNode.GetGameobject().transform.parent = hierarchy.transform;

        //BFS(rootNode);


        //Create the color arrays for the different-level-model
    }

    private void TreatmentCPLevelx()
    {
        //get the level x tree nodes
        treeNodeLevelx.Clear();
        for (var i = 0; i < importedSegmentsOfDifferentLevels.Count; i++)
        {
            var GetNode = rootNode.GetDescendent(importedSegmentsOfDifferentLevels[i].id);

            if (GetNode.GetLevel() == levelSelect /* x */)
                treeNodeLevelx.Add(GetNode);
        }


        

        //get the level x colors
        // loop for different segment
        Array.Clear(colorArrayLevelx, 0, colorArrayLevelx.Length);
        for (var i = 0; i < treeNodeLevelx.Count; i++)
        {
            var GetSegmentVertexIndexes = treeNodeLevelx[i].GetData().verticesIndex;
            var GetSegmentColors = treeNodeLevelx[i].GetData().color;

            for (var j = 0; j < GetSegmentVertexIndexes.Count; j++)
                colorArrayLevelx[GetSegmentVertexIndexes[j]] +=
                    new Color(GetSegmentColors[0], GetSegmentColors[1], GetSegmentColors[2], 255) / 255;
        }

        //mix the colors
        for (var j = 0; j < colorArrayLevelx.Length; j++) colorArrayLevelx[j] /= colorArrayLevelx[j].a;


    }

    // Start is called before the first frame update
    private void Start()
    {
        //thresholdPrime = 1.1f;
        //switchSegment = false;
    }

    private void TresholdComputeCageVertices(TreeNode n)
    {
        //get level x cage vertices
        //compute the segments on the cage with a threshold 0.4(with hierarchy)
        
        var interLevelsCageSegVerts = new List<int>();
        interLevelsCageSegVerts.Clear();
        filterBarMatrix(0.2, n.GetData().verticesIndex, interLevelsCageSegVerts);
        n.GetData().cageVerticesIndex = interLevelsCageSegVerts;

    }

    private void Update()
    {
        if (levelChange)
        {
            TreatmentCPLevelx();
            level.text = levelSelect.ToString();
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
        Console.WriteLine("Riconosco di dover cambiare livello");
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

    //used for the visualization of different segments(testing)
    //public void deleteFilter()
    //{
    //    //destroy the previous segment.
    //    var objs = GameObject.FindGameObjectsWithTag("Segment");
    //    for (var i = 0; i < objs.Length; i++) Destroy(objs[i]);
    //}

    //private void DrawFilter(List<int> trianCageSegmented)
    //{
    //    //draw new segment
    //    for (var i = 0; i < trianCageSegmented.Count; i++)
    //    {
    //        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //        obj.GetComponent<MeshRenderer>().material = segmentmMaterial;
    //        obj.transform.position = cageVertices[trianCageSegmented[i]];
    //        obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    //        obj.tag = "Segment";
    //    }
    //}


    private void GetDataToLst(JsonData jsonData, List<int> Mylist)
    {
        for (var i = 0; i < jsonData.Count; i++)
        {
            var interData = Convert.ToString(jsonData[i]);
            //UnityEngine.Debug.Log(interData);
            //UnityEngine.Debug.Log(Int32.Parse(interData));
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
        //UnityEngine.Debug.Log("this is the a ModelData inside filter");

        ListCageSegOutput.Clear();
        for (var i = 0; i < ModelSegVertIndexesInput.Count; i++)
        {
            for (var j = 0; j < readFileComputeNewcage.columnNumberUpdate; j++)
            {
                if (readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j] > thres)
                {
                    if (!ListCageSegOutput.Contains(j))
                        ListCageSegOutput.Add(j);
                }
                    
            }
                
        }
        
        //UnityEngine.Debug.Log("this is the positions of the elements less than threshold " + j);
        //UnityEngine.Debug.Log("The related cage vertices amount: " + ListCageSegOutput.Count);
    }

    private void BFS(TreeNode start)
    {
        //LINE is the final order
        var LINE = new List<TreeNode>();
        LINE.Add(start);

        //qq is a stack
        var qq = new Queue();
        qq.Enqueue(start);

        start.visited = true;

        var a = new TreeNode(new ModelData());

        while (qq.Count != 0)
        {
            a = new TreeNode(new ModelData());
            a = (TreeNode) qq.Dequeue();

            var a_nbs = new List<TreeNode>();
            a_nbs = a.GetChildren();

            foreach (var tmp in a_nbs.Where(k => !k.visited).ToList())
            {
                qq.Enqueue(tmp);
                LINE.Add(tmp);
                tmp.visited = true;
            }
        }
    }
}