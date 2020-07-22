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

 public class ReadJson : MonoBehaviour
{
    public List<List<int>> AllSegColors = new List<List<int>>();
    public List<List<int>> AllSegColors1 = new List<List<int>>();

    public List<List<List<int>>> AllSegColo = new List<List<List<int>>>();

    public List<List<int>> AllSegtrianIndexes = new List<List<int>>();
    public List<List<int>> AllSegtrianIndexes1 = new List<List<int>>();

    public List<int> AllSegtriansIndexAmounts = new List<int>();
    public List<int> AllSegtriansIndexAmounts1 = new List<int>();

    public List<int> AllSegVertsIndexAmounts = new List<int>();
    public List<List<int>> AllSegVertsIndexes = new List<List<int>>();
    public List<List<List<int>>> AllSegVertsIndexes1 = new List<List<List<int>>>();

    public List<List<int>> CageAllSegVertIndex = new List<List<int>>();

    private JsonData data;
    private JsonData data1;

    [HideInInspector] public List<int> indexFinger = new List<int>();

    private string jsonString;
    private string jsonString1;

    [HideInInspector] public List<int> littleFinger = new List<int>();

    [HideInInspector] public List<int> mediumFinger = new List<int>();

    //public List<int> interSegTrianLists = new List<int>();
    //public List<int> interSegVertLists = new List<int>();
    [HideInInspector] private MeshCreateControlPoints meshCreateControlPoints;

    [HideInInspector] public List<int> palm = new List<int>();

    [HideInInspector] private ReadFileComputeNewcage readFileComputeNewcage;

    [HideInInspector] public List<int> ringFinger = new List<int>();

    [SerializeField] private Material segmentmMaterial;
    [SerializeField] private Material segmentmMaterial2;

    [HideInInspector] public bool switchSegment;

    [Range(-0.1f, 1.0f)] public double threshold;

    private double thresholdPrime;

    [HideInInspector] public List<int> thumb = new List<int>();

    [HideInInspector] public List<int> trianCageSegmented = new List<int>();

    [HideInInspector] public List<int> trianModelSegmented = new List<int>();

    [HideInInspector] public List<int> wrist = new List<int>();


    [HideInInspector] public List<ModelData> importedSegmentsOfDifferentLevels = new List<ModelData>();
    [HideInInspector] public List<List<ModelData>> differentLevelModelSegments = new List<List<ModelData>>();
    [HideInInspector] public Color[] colorArrayLevelx;
    

    [HideInInspector] public List<TreeNode> treeNodeLevelx = new List<TreeNode>();
    [HideInInspector] public int levelMax = new int();





    public int[] trisCage;
    public int[] trisModel;
    public GameObject objCage;
    public GameObject objModel;
    private Mesh meshCage;
    private Mesh meshModel;
    public Vector3[] modelVertices;
    public Vector3[] cageVertices;
    public List<string> segmentTags;
    //private List<int> level =new List<int>();
    //public List<int> modelLevel = new List<int>();
    public TreeNode rootNode;

    void Awake()
    {
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
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_correct(no hierarchy).txt");
        jsonString1 = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_hierarchical_nails.txt");

        data = JsonMapper.ToObject(jsonString);
        data1 = JsonMapper.ToObject(jsonString1);


        for (var i = 0; i < data["annotations"].Count; i++)
        {
            var interSegTrianLists = new List<int>();
            var interSegColors = new List<int>();
            interSegTrianLists.Clear();
            interSegColors.Clear();
            GetDataToLst(data["annotations"][i]["triangles"], interSegTrianLists);
            GetDataToLst(data["annotations"][i]["color"], interSegColors);
            segmentTags.Add(Convert.ToString(data["annotations"][i]["tag"]));
            //for (int j = 0; j < interSegTrianLists.Count; j++)
            //{
            //    Debug.Log(interSegTrianLists[j]);
            //}
            AllSegtrianIndexes.Add(interSegTrianLists);
            AllSegColors.Add(interSegColors);
            AllSegtriansIndexAmounts.Add(interSegTrianLists.Count);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        for (var i = 0; i < data1["annotations"].Count; i++)
        {
            var interSegTrianLists = new List<int>();
            var interSegColors = new List<int>();
            interSegTrianLists.Clear();
            interSegColors.Clear();
            GetDataToLst(data1["annotations"][i]["triangles"], interSegTrianLists);
            GetDataToLst(data1["annotations"][i]["color"], interSegColors);
            segmentTags.Add(Convert.ToString(data1["annotations"][i]["tag"]));
            //level.Add(int.Parse(Convert.ToString(data1["annotations"][i]["level"])));
            //for (int j = 0; j < interSegTrianLists.Count; j++)
            //{
            //    Debug.Log(interSegTrianLists[j]);
            //}
            AllSegtrianIndexes1.Add(interSegTrianLists);
            AllSegColors1.Add(interSegColors);
            AllSegtriansIndexAmounts1.Add(interSegTrianLists.Count);
        }






        ////Instantiation of the the model's segments(instances of the segments) 
        //var hierarchy = new GameObject();
        //hierarchy.name = ("Hierarchy");
        //List<int> modelLevel = new List<int>();
        //modelLevel.Add(-1);
        //for (var i = 0; i < data1["annotations"].Count; i++)
        //{
        //    var datatest = JsonMapper.ToJson(data1["annotations"][i]);
        //    importedSegmentsOfDifferentLevels.Add(JsonMapper.ToObject<ModelData>(datatest));
        //    //Debug.Log("To see the triangle indexes " + importedSegmentsOfDifferentLevels[i].triangles[0]);

        //    //Create the GOs of levels
        //    if (!modelLevel.Contains(importedSegmentsOfDifferentLevels[i].level))
        //    {
        //        GameObject level = new GameObject();
        //        level.name = "level " + importedSegmentsOfDifferentLevels[i].level;
        //        level.transform.parent = hierarchy.transform;
        //    }
        //    modelLevel.Add(importedSegmentsOfDifferentLevels[i].level);


        //    //create the GOs for the segements of different levels
        //    GameObject hierarchicalSegs = new GameObject();
        //    hierarchicalSegs.name = importedSegmentsOfDifferentLevels[i].tag + " (level " + importedSegmentsOfDifferentLevels[i].level + ")";

        //    if (hierarchicalSegs.name.Contains(Convert.ToString(importedSegmentsOfDifferentLevels[i].level)))
        //    {
        //        hierarchicalSegs.transform.parent =
        //            GameObject.Find("level " + Convert.ToString(importedSegmentsOfDifferentLevels[i].level)).transform;
        //    }
        //}

        //List<ModelData> interListOfImportedSegmentsOfDifferentLevels = new List<ModelData>();
        //Debug.Log("handModellevel.Max() " + modelLevel.Max());
        //Debug.Log("handModellevel.Max() " + modelLevel[0]);


        //Instantiation of the the model's segments(instances of the segments) 
        
        //modelLevel.Add(-1);
        for (var i = 0; i < data1["annotations"].Count; i++)
        {
            var datatest = JsonMapper.ToJson(data1["annotations"][i]);
            importedSegmentsOfDifferentLevels.Add(JsonMapper.ToObject<ModelData>(datatest));
            //Debug.Log("To see the colors  " + importedSegmentsOfDifferentLevels[i].color[0]);

        }


        //
        

        //Debug.Log("differentLevelModelSegments[0][0].triangles " + differentLevelModelSegments[0][0].triangles[0]);
        ///////////////////////////////////////////////////////////////////////////////

        //// level-->element(segment) position in this level-->vertexe
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[0][0][0]);
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[1][0][1]);
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[1][0][2]);


        //for (int i = 0; i < differentLevelModelSegments.Count; i++)
        //{
        //    for (int j = 0; j < differentLevelModelSegments[i].Count; j++)
        //    {
        //        for (int k = 0; k < AllSegVertsIndexes1[i][j].Count; k++)
        //        {
        //            //certain level certain segment
        //            differentLevelModelSegments[i][j].verticesIndex.Add(AllSegVertsIndexes1[i][j][k]);
        //        }

        //    }
        //}

        //for (int i = 0; i < AllSegVertsIndexes1[1][4].Count; i++)
        //{

        //    for (int j = 0; j <AllSegVertsIndexes1[1][6].Count; j++)
        //    {
        //        //if (readJson.AllSegVertsIndexes1[1][4][i] == readJson.AllSegVertsIndexes1[1][6][j])
        //        //    colors1[readJson.differentLevelModelSegments[1][4].verticesIndex[j]] = new Color(intColor4[0] + intColor6[0], intColor4[1] + intColor6[1], intColor4[2] + intColor6[2]) / 510;
        //      
        //    }
        //}
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

        //get the level x tree nodes
        for (int i = 0; i < importedSegmentsOfDifferentLevels.Count; i++)
        {
            var GetNode = rootNode.GetDescendent(importedSegmentsOfDifferentLevels[i].id);
            
            if (GetNode.GetLevel() == 1/* x */)
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
        for (var i = 0; i < treeNodeLevelx.Count; i++)
        {
            var GetSegmentVertexIndexes = treeNodeLevelx[i].GetData().verticesIndex;
            var GetSegmentColors = treeNodeLevelx[i].GetData().color;

            for (var j = 0; j < GetSegmentVertexIndexes.Count; j++)
            {
                colorArrayLevelx[GetSegmentVertexIndexes[j]] += (new Color(GetSegmentColors[0], GetSegmentColors[1], GetSegmentColors[2], 255) / 255);
            }
        }
        //mix the colors
        for (int j = 0; j < colorArrayLevelx.Length; j++)
        {
            colorArrayLevelx[j] /= colorArrayLevelx[j].a;
        }




        //for (int i = 0; i < treeNodeLevelx.Count; i++)
        //{
        //    List<int> interLevelsCageSegVerts = new List<int>();
        //    interLevelsCageSegVerts.Clear();
        //    filterBarMatrix(0.4, treeNodeLevelx[i].GetData().verticesIndex, interLevelsCageSegVerts);
        //    Debug.Log("InterLevelsCageSegVerts 0:" + interLevelsCageSegVerts.Count);
        //    treeNodeLevelx[i].GetData().cageVerticesIndex = interLevelsCageSegVerts;
        //    Debug.Log("InterLevelsCageSegVerts 1:" + interLevelsCageSegVerts.Count);
        //}

        //Debug.Log("treeNodeLevelx cage verts 0:"+ treeNodeLevelx[0].GetData().cageVerticesIndex.Count);
        //foreach (var VARIABLE in treeNodeLevelx[0].GetData().cageVerticesIndex)
        //{
        //    Debug.Log("treeNodeLevelx cage verts " + VARIABLE);
        //}
        


        var hierarchy = new GameObject();
        hierarchy.name = ("Hierarchy");
        rootNode.GetGameobject().transform.parent = hierarchy.transform;

        //BFS(rootNode);


        //Create the color arrays for the different-level-model








    }

    // Start is called before the first frame update
    private void Start()
    {
        //populate the rootNode with segments
        
        //Debug.Log("hello here 2" + rootNode.GetChild("Item 4"));

        //GetDataToLst(data["annotations"][0]["triangles"], littleFinger);
        //GetDataToLst(data["annotations"][1]["triangles"], ringFinger);
        //GetDataToLst(data["annotations"][2]["triangles"], mediumFinger);
        //GetDataToLst(data["annotations"][3]["triangles"], indexFinger);
        //GetDataToLst(data["annotations"][4]["triangles"], thumb);
        //GetDataToLst(data["annotations"][5]["triangles"], wrist);
        //GetDataToLst(data["annotations"][6]["triangles"], palm);

        //var allCount = littleFinger.Count + ringFinger.Count + mediumFinger.Count + indexFinger.Count + thumb.Count +
        //        wrist.Count + palm.Count;



        thresholdPrime = 1.1f;
        switchSegment = false;
        

        //get level x cage vertices
        //compute the segments on the cage with a threshold 0.4(with hierarchy)
        for (int i = 0; i < treeNodeLevelx.Count; i++)
        {
            List<int> interLevelsCageSegVerts = new List<int>();
            interLevelsCageSegVerts.Clear();
            filterBarMatrix(0.4, treeNodeLevelx[i].GetData().verticesIndex, interLevelsCageSegVerts);
            treeNodeLevelx[i].GetData().cageVerticesIndex = interLevelsCageSegVerts;
            //Debug.Log("InterLevelsCageSegVerts 1:" + interLevelsCageSegVerts.Count);
        }




        //for (int i = 0; i < AllSegVertsIndexes[0].Count; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //    cube.transform.position = modelVertices[AllSegVertsIndexes[0][i]];
        //    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //    //Debug.Log("the first segment cage vertices positions in readjson " + CageAllSegVertIndex[1][i] /*+ " " + CageAllSegVertIndex[0][i] + " " + CageAllSegVertIndex[i][2]*/);
        //}
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
    ///     Get the segmented triangles of the model by using the "triant" file data
    /// </summary>
    /// <param name="segmentations"></param>
    public void MapSegModel(List<int> segmentations)
    {
        for (var i = 0; i < segmentations.Count; i++)
        {
            if (!trianModelSegmented.Contains(trisModel[(segmentations[i]-1 ) * 3]))
                trianModelSegmented.Add(trisModel[(segmentations[i]-1 ) * 3]);

            if (!trianModelSegmented.Contains(trisModel[(segmentations[i]-1 ) * 3 + 2]))
                trianModelSegmented.Add(trisModel[(segmentations[i]-1 ) * 3 + 2]);

            if (!trianModelSegmented.Contains(trisModel[(segmentations[i]-1 ) * 3 + 1]))
                trianModelSegmented.Add(trisModel[(segmentations[i]-1 ) * 3 + 1]);
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