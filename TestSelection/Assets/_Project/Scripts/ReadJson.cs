using System;
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




    public int[] trisCage;
    public int[] trisModel;
    public GameObject objCage;
    public GameObject objModel;
    private Mesh meshCage;
    private Mesh meshModel;
    public Vector3[] modelVertices;
    public Vector3[] cageVertices;
    public List<string> segmentTags;
    private List<int> level =new List<int>();
    public List<int> modelLevel = new List<int>();
    public TreeNode tree;

    void Awake()
    {
        segmentTags = new List<string>();
        meshCage = objCage.GetComponent<MeshFilter>().mesh;
        meshModel = objModel.GetComponent<MeshFilter>().mesh;
        cageVertices = meshCage.vertices;
        modelVertices = meshModel.vertices;
        trisCage = meshCage.triangles;
        trisModel = meshModel.triangles;

        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_correct(no hierarchy).txt");
        jsonString1 = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_correct.txt");

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
            level.Add(int.Parse(Convert.ToString(data1["annotations"][i]["level"])));
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

            //Create the GOs of levels
            if (!modelLevel.Contains(importedSegmentsOfDifferentLevels[i].level))
            {
                modelLevel.Add(importedSegmentsOfDifferentLevels[i].level);
            }
        }

        List<ModelData> interListOfImportedSegmentsOfDifferentLevels = new List<ModelData>();


        for (int j = 0; j <= modelLevel.Max(); j++)
        {
            interListOfImportedSegmentsOfDifferentLevels = importedSegmentsOfDifferentLevels.FindAll(x => x.level == j);
            //importedSegmentsOfDifferentLevels[i].level =
            differentLevelModelSegments.Add(interListOfImportedSegmentsOfDifferentLevels);
        }
        //
        // find the triangles indexes
        //for different level
        for (int l = 0; l < differentLevelModelSegments.Count; l++)
        {
            var interSegVertListOfLists = new List<List<int>>();
            //for different segment
            for (var j = 0; j < differentLevelModelSegments[l].Count; j++)
            {

                var interSegVertLists = new List<int>();
                //for different trianlges
                for (var i = 0; i < differentLevelModelSegments[l][j].triangles.Count; i++)
                {
                    //Debug.Log("littleFinger " +littleFinger[i] );
                    if (!interSegVertLists.Contains(trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3]))
                        interSegVertLists.Add(trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3]);
                    //Debug.Log("vertex 1 " + sfd.trisModel[littleFinger[i] * 3]);

                    if (!interSegVertLists.Contains(
                        trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3 + 1]))
                        interSegVertLists.Add(trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3 + 1]);
                    //Debug.Log("vertex 3 " + sfd.trisModel[littleFinger[i] * 3 + 1]);


                    if (!interSegVertLists.Contains(
                        trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3 + 2]))
                        interSegVertLists.Add(trisModel[(differentLevelModelSegments[l][j].triangles[i]) * 3 + 2]);
                    //Debug.Log("vertex 2 " + sfd.trisModel[littleFinger[i] * 3+2]);
                    //Debug.Log("little finger triangle index " + i + " " + littleFinger[i]);
                }
                
                interSegVertListOfLists.Add(interSegVertLists);
                //AllSegVertsIndexAmounts.Add(interSegVertLists.Count);
            }
            AllSegVertsIndexes1.Add(interSegVertListOfLists);
        }

        //Debug.Log("differentLevelModelSegments[0][0].triangles " + differentLevelModelSegments[0][0].triangles[0]);
        ///////////////////////////////////////////////////////////////////////////////
;
        //// level-->element(segment) position in this level-->vertexe
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[0][0][0]);
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[1][0][1]);
        //Debug.Log("AllSegVertsIndexes1: " + AllSegVertsIndexes1[1][0][2]);


        for (int i = 0; i < differentLevelModelSegments.Count; i++)
        {
            for (int j = 0; j < differentLevelModelSegments[i].Count; j++)
            {
                for (int k = 0; k < AllSegVertsIndexes1[i][j].Count; k++)
                {
                    //certain level certain segment
                    differentLevelModelSegments[i][j].verticesIndex.Add(AllSegVertsIndexes1[i][j][k]);
                }
                
            }
        }

        //for (int i = 0; i < AllSegVertsIndexes1[1][4].Count; i++)
        //{

        //    for (int j = 0; j <AllSegVertsIndexes1[1][6].Count; j++)
        //    {
        //        //if (readJson.AllSegVertsIndexes1[1][4][i] == readJson.AllSegVertsIndexes1[1][6][j])
        //        //    colors1[readJson.differentLevelModelSegments[1][4].verticesIndex[j]] = new Color(intColor4[0] + intColor6[0], intColor4[1] + intColor6[1], intColor4[2] + intColor6[2]) / 510;
        //        Debug.Log("hehe: ");
        //    }
        //}


        tree = new TreeNode(differentLevelModelSegments[0][0]);
        //loop for levels 
        for (int i = 1; i < differentLevelModelSegments.Count; i++)
        {
            //loop for the segments of a same level,take the first element as the head of this level, others are level+1
            for (int j = 0/*1*/; j < differentLevelModelSegments[i].Count; j++)
            {

                tree./*GetChild(differentLevelModelSegments[i][0]).*/Add(new TreeNode(differentLevelModelSegments[i][j]));
                //Debug.Log("tree.GetChild(differentLevelModelSegments[i,1][j]).ID.tag:" + tree.GetChild(differentLevelModelSegments[i][0]).GetChild(differentLevelModelSegments[i][j]).ID.tag);
                //Debug.Log("tree.GetChild(differentLevelModelSegments[i,1][j]).ID.tag:" + tree.GetChild(differentLevelModelSegments[i][j]).ID.tag);
            }
        }


        var hierarchy = new GameObject();
        hierarchy.name = ("Hierarchy");
        GameObject rootObj = new GameObject();
        rootObj.name = tree.ID.tag;
        rootObj.transform.parent = hierarchy.transform;
        for (int i = 0; i < modelLevel.Count; i++)
        {
            if (i > 0)
            {
                for (int j = 0; j < tree.Count; j++)
                {
                    GameObject ChildrenObj = new GameObject();
                    ChildrenObj.name = tree.GetChild(differentLevelModelSegments[i][j]).ID.tag;
                    ChildrenObj.transform.parent = rootObj.transform;
                    //Debug.Log("tree.GetChild(differentLevelModelSegments[1][i]).ID.tag:" + tree.GetChild(differentLevelModelSegments[1][j]).ID.tag);
                }
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //populate the tree with segments
        
        //Debug.Log("hello here 2" + tree.GetChild("Item 4"));

        GetDataToLst(data["annotations"][0]["triangles"], littleFinger);
        GetDataToLst(data["annotations"][1]["triangles"], ringFinger);
        GetDataToLst(data["annotations"][2]["triangles"], mediumFinger);
        GetDataToLst(data["annotations"][3]["triangles"], indexFinger);
        GetDataToLst(data["annotations"][4]["triangles"], thumb);
        GetDataToLst(data["annotations"][5]["triangles"], wrist);
        GetDataToLst(data["annotations"][6]["triangles"], palm);

        var allCount = littleFinger.Count + ringFinger.Count + mediumFinger.Count + indexFinger.Count + thumb.Count +
                wrist.Count + palm.Count;


        //Debug.Log("total length "+ k);
        //MapSegModel(littleFinger);
        //filterBarMatrix(threshold);
        //foreach (var VARIABLE in littleFinger)
        //{
        //    Debug.Log("littleFinger " + VARIABLE);
        //}
        //DrawFilter(ListCageSegOutput);
        thresholdPrime = 1.1f;
        switchSegment = false;
        for (var j = 0; j < AllSegtrianIndexes.Count; j++)
        {
            var interSegVertLists = new List<int>();
            interSegVertLists.Clear();

            for (var i = 0; i < AllSegtrianIndexes[j].Count; i++)
            {
                //Debug.Log("littleFinger " +littleFinger[i] );
                if (!interSegVertLists.Contains(trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3]);
                //Debug.Log("vertex 1 " + sfd.trisModel[littleFinger[i] * 3]);

                if (!interSegVertLists.Contains(
                    trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3 + 1]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3 + 1]);
                //Debug.Log("vertex 3 " + sfd.trisModel[littleFinger[i] * 3 + 1]);


                if (!interSegVertLists.Contains(
                    trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3 + 2]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i]-1 ) * 3 + 2]);
                //Debug.Log("vertex 2 " + sfd.trisModel[littleFinger[i] * 3+2]);
                //Debug.Log("little finger triangle index " + i + " " + littleFinger[i]);
            }

            AllSegVertsIndexes.Add(interSegVertLists);
            AllSegVertsIndexAmounts.Add(interSegVertLists.Count);
        }

        //compute the segments on the cage with a threshold 0.4(no hierarchy)
        for (var i = 0; i < AllSegVertsIndexes.Count; i++)
        {
            //Debug.Log("AllSegVertsIndexes[0][i] " + AllSegVertsIndexes[0][i]);
            List<int> InterCageSegVerts = new List<int>();
            //InterCageSegVerts.Clear();
            ////////////////Debug.Log("AllSegVertsIndexes.Count " + AllSegVertsIndexes.Count);
            filterBarMatrix(0.4, AllSegVertsIndexes[i], InterCageSegVerts);
            //Debug.Log("InterCageSegVerts " + InterCageSegVerts.Count);
            CageAllSegVertIndex.Add(InterCageSegVerts);
        }

        //compute the segments on the cage with a threshold 0.4(with hierarchy)
        //loop for the (i)levels
        for (int i = 0; i < modelLevel.Count; i++)
        {
            List<int> InterLevelsCageSegVerts = new List<int>();
            InterLevelsCageSegVerts.Clear();
            ////////////////Debug.Log("AllSegVertsIndexes.Count " + AllSegVertsIndexes.Count);
            if (i == 0)
            {
                filterBarMatrix(0.4, tree.ID.verticesIndex, InterLevelsCageSegVerts);
                for (int j = 0; j < InterLevelsCageSegVerts.Count; j++)
                {
                    differentLevelModelSegments[0][0].cageVerticesIndex.Add(InterLevelsCageSegVerts[j]);
                }

            }

            if (i == 1)
            {
                for (int j = 0; j < tree.Count; j++)
                {
                    InterLevelsCageSegVerts.Clear();
                    filterBarMatrix(0.4, tree.GetChild(differentLevelModelSegments[1][j]).ID.verticesIndex, InterLevelsCageSegVerts);

                    for (int k = 0; k < InterLevelsCageSegVerts.Count; k++)
                    {
                        differentLevelModelSegments[1][j].cageVerticesIndex.Add(InterLevelsCageSegVerts[k]);
                    }
                }
            }
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
            if (readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j] > thres)
                //Debug.Log("readFileComputeNewcage.barMatrices " +j +" "+ readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j]);
                if (!ListCageSegOutput.Contains(j))
                    ListCageSegOutput.Add(j);
        //Debug.Log("this is the positions of the elements less than threshold " + j);
        //Debug.Log("The related cage vertices amount: " + ListCageSegOutput.Count);
    }
}