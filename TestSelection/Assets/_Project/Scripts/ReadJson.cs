using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

 public class ReadJson : MonoBehaviour
{
    public List<List<int>> AllSegColors = new List<List<int>>();

    public List<List<int>> AllSegtrianIndexes = new List<List<int>>();
    public List<int> AllSegtriansIndexAmounts = new List<int>();
    public List<int> AllSegVertsIndexAmounts = new List<int>();
    public List<List<int>> AllSegVertsIndexes = new List<List<int>>();

    public List<List<int>> CageAllSegVertIndex = new List<List<int>>();

    private JsonData data;

    [HideInInspector] public List<int> indexFinger = new List<int>();

    private string jsonString;

    [HideInInspector] public List<int> littleFinger = new List<int>();

    [HideInInspector] private readonly List<int> Lst = new List<int>();

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

    public int[] trisCage;
    public int[] trisModel;
    public GameObject objCage;
    public GameObject objModel;
    private Mesh meshCage;
    private Mesh meshModel;
    public Vector3[] modelVertices;
    public Vector3[] cageVertices;
    public List<string> segmentTags;




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

        data = JsonMapper.ToObject(jsonString);

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

    }


    // Start is called before the first frame update
    private void Start()
    {
        //for (int i = 0; i < AllSegtrianIndexes.Count; i++)
        //{
        //    Debug.Log("segement separation ");
        //    for (int j = 0; j < AllSegtrianIndexes[i].Count; j++)
        //    {
        //        Debug.Log("AllSegtrianIndexes[i] "+i+" "+ AllSegtrianIndexes[i][j]);
        //    }
        //}


        //////////////////segmentTags = new List<string>();
        //////////////////meshCage = objCage.GetComponent<MeshFilter>().mesh;
        //////////////////meshModel = objModel.GetComponent<MeshFilter>().mesh;
        //////////////////cageVertices = meshCage.vertices;
        //////////////////modelVertices = meshModel.vertices;
        //////////////////trisCage = meshCage.triangles;
        //////////////////trisModel = meshModel.triangles;

        //////////////////meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        //////////////////readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        //////////////////jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_correct.txt");

        //////////////////data = JsonMapper.ToObject(jsonString);

        //Debug.Log(data["annotations"][0]["triangles"]);

        GetDataToLst(data["annotations"][0]["triangles"], littleFinger);
        GetDataToLst(data["annotations"][1]["triangles"], ringFinger);
        GetDataToLst(data["annotations"][2]["triangles"], mediumFinger);
        GetDataToLst(data["annotations"][3]["triangles"], indexFinger);
        GetDataToLst(data["annotations"][4]["triangles"], thumb);
        GetDataToLst(data["annotations"][5]["triangles"], wrist);
        GetDataToLst(data["annotations"][6]["triangles"], palm);

        var k = littleFinger.Count + ringFinger.Count + mediumFinger.Count + indexFinger.Count + thumb.Count +
                wrist.Count + palm.Count;

        //Debug.Log(littleFinger.Count);
        //Debug.Log(ringFinger.Count);
        //Debug.Log(wrist.Count);
        //Debug.Log(palm.Count);

        ////////////////////////////for (var i = 0; i < data["annotations"].Count; i++)
        ////////////////////////////{
        ////////////////////////////    var interSegTrianLists = new List<int>();
        ////////////////////////////    var interSegColors = new List<int>();
        ////////////////////////////    interSegTrianLists.Clear();
        ////////////////////////////    interSegColors.Clear();
        ////////////////////////////    GetDataToLst(data["annotations"][i]["triangles"], interSegTrianLists);
        ////////////////////////////    GetDataToLst(data["annotations"][i]["color"], interSegColors);
        ////////////////////////////    segmentTags.Add(Convert.ToString(data["annotations"][i]["tag"]));


        ////////////////////////////    for (int j = 0; j < interSegTrianLists.Count; j++)
        ////////////////////////////    {
        ////////////////////////////        Debug.Log(interSegTrianLists[j]);
        ////////////////////////////    }
        ////////////////////////////    AllSegtrianIndexes.Add(interSegTrianLists);
        ////////////////////////////    AllSegColors.Add(interSegColors);
        ////////////////////////////    AllSegtriansIndexAmounts.Add(interSegTrianLists.Count);
        ////////////////////////////}

        //for (int i = 0; i < AllSegtrianIndexes.Count; i++)
        //{
        //    Debug.Log("this is a separator for different segments**********************************");
        //    for (int j = 0; j < AllSegtrianIndexes[i].Count; j++)
        //    {
        //        Debug.Log(AllSegtrianIndexes[i][j]);
        //    }
        //}

        //for (int i = 0; i < AllSegtrianIndexes[0].Count; i++)
        //{
        //    Debug.Log(AllSegtrianIndexes[0][i]);
        //}

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
                if (!interSegVertLists.Contains(trisModel[(AllSegtrianIndexes[j][i] - 1) * 3]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i] - 1) * 3]);
                //Debug.Log("vertex 1 " + sfd.trisModel[littleFinger[i] * 3]);

                if (!interSegVertLists.Contains(
                    trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 1]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 1]);
                //Debug.Log("vertex 3 " + sfd.trisModel[littleFinger[i] * 3 + 1]);


                if (!interSegVertLists.Contains(
                    trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 2]))
                    interSegVertLists.Add(trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 2]);
                //Debug.Log("vertex 2 " + sfd.trisModel[littleFinger[i] * 3+2]);
                //Debug.Log("little finger triangle index " + i + " " + littleFinger[i]);
            }

            AllSegVertsIndexes.Add(interSegVertLists);
            AllSegVertsIndexAmounts.Add(interSegVertLists.Count);
        }

        //compute the segments on the cage with a threshold 0.4
        //Debug.Log("AllSegVertsIndexes[0].Count " + AllSegVertsIndexes[0].Count);

        //for (int i = 0; i < AllSegVertsIndexes[0].Count; i++)
        //{
        //    Debug.Log("AllSegVertsIndexes[0][i] "+ AllSegVertsIndexes[0][i]);
        //}


        
        for (var i = 0; i < AllSegVertsIndexes.Count; i++)
        {
            List<int> InterCageSegVerts = new List<int>();
            //InterCageSegVerts.Clear();
            ////////////////Debug.Log("AllSegVertsIndexes.Count " + AllSegVertsIndexes.Count);
            filterBarMatrix(0.4, AllSegVertsIndexes[i], InterCageSegVerts);
            //Debug.Log("InterCageSegVerts " + InterCageSegVerts.Count);
            CageAllSegVertIndex.Add(InterCageSegVerts);
        }

        //Debug.Log("the first segment cage vertices positions in readjson ");
        //Debug.Log("CageAllSegVertIndex.Count " + CageAllSegVertIndex.Count);
        //Debug.Log("CageAllSegVertIndex[6].Count " + CageAllSegVertIndex[6].Count);
        //Debug.Log("CageAllSegVertIndex[5].Count " + CageAllSegVertIndex[5].Count);

        //for (int i = 0; i < CageAllSegVertIndex[2].Count; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //    cube.transform.position = cageVertices[CageAllSegVertIndex[2][i]];
        //    cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    //Debug.Log("the first segment cage vertices positions in readjson " + CageAllSegVertIndex[1][i] /*+ " " + CageAllSegVertIndex[0][i] + " " + CageAllSegVertIndex[i][2]*/);
        //}
        //for (int i = 0; i < AllSegVertsIndexes[0].Count; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //    cube.transform.position = modelVertices[AllSegVertsIndexes[0][i]];
        //    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //    //Debug.Log("the first segment cage vertices positions in readjson " + CageAllSegVertIndex[1][i] /*+ " " + CageAllSegVertIndex[0][i] + " " + CageAllSegVertIndex[i][2]*/);
        //}
                

        for (var i = 0; i < littleFinger.Count; i++)
        {
            //Debug.Log("littleFinger " +littleFinger[i] );
            if (!Lst.Contains(trisModel[(littleFinger[i] - 1) * 3]))
                Lst.Add(trisModel[(littleFinger[i] - 1) * 3]);
            //Debug.Log("vertex 1 " + sfd.trisModel[littleFinger[i] * 3]);

            if (!Lst.Contains(trisModel[(littleFinger[i] - 1) * 3 + 1]))
                Lst.Add(trisModel[(littleFinger[i] - 1) * 3 + 1]);
            //Debug.Log("vertex 3 " + sfd.trisModel[littleFinger[i] * 3 + 1]);


            if (!Lst.Contains(trisModel[(littleFinger[i] - 1) * 3 + 2]))
                Lst.Add(trisModel[(littleFinger[i] - 1) * 3 + 2]);
            //Debug.Log("vertex 2 " + sfd.trisModel[littleFinger[i] * 3+2]);
            //Debug.Log("little finger triangle index " + i + " " + littleFinger[i]);
        }

        //int colorCorrector = 0;
        //for (int i = 0; i < AllSegVertsIndexAmounts.Count; i++)
        //{
        //    for (int j = 0; j < AllSegVertsIndexAmounts[i]; j++)
        //    {
        //        Debug.Log("this is a string inside j 1");
        //        colors[i + colorCorrector] = Color.red;
        //        Debug.Log("this is a string inside j 2");
        //    }

        //    colorCorrector += AllSegVertsIndexAmounts[i];
        //}

        //////for (int i = 0; i < Lst.Count; i++)
        //////{
        //////    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //////    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //////    cube.transform.position = sfd.modelVertices[Lst[i]];
        //////    Debug.Log("little finger Vertices " + i + " " + sfd.modelVertices[Lst[i]].ToString(("F6")));

        //////    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //////}
        //Debug.Log("first triangle " + Lst[0] + " "+Lst[1]+ " "+ Lst[2]);
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
            obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
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
    ///     Get the segmentation triangles of the model by using the "triant" file data
    /// </summary>
    /// <param name="segmentations"></param>
    public void MapSegModel(List<int> segmentations)
    {
        for (var i = 0; i < segmentations.Count; i++)
        {
            if (!trianModelSegmented.Contains(trisModel[(segmentations[i] - 1) * 3]))
                trianModelSegmented.Add(trisModel[(segmentations[i] - 1) * 3]);

            if (!trianModelSegmented.Contains(trisModel[(segmentations[i] - 1) * 3 + 2]))
                trianModelSegmented.Add(trisModel[(segmentations[i] - 1) * 3 + 2]);

            if (!trianModelSegmented.Contains(trisModel[(segmentations[i] - 1) * 3 + 1]))
                trianModelSegmented.Add(trisModel[(segmentations[i] - 1) * 3 + 1]);
        }
    }

    /// <summary>
    ///     filter the Barycentric matrix by using a user defined threshold
    /// </summary>
    /// <param name="thresHold"></param>
    public void filterBarMatrix(double thres, List<int> ModelSegVertIndexesInput, List<int> ListCageSegOutput)
    {
        //Debug.Log("this is the a string inside filter");
        ListCageSegOutput.Clear();
        for (var i = 0; i < ModelSegVertIndexesInput.Count; i++)
            //Debug.Log("this is the a string inside for loop ModelSegVertIndexesInput.Count");
            //Debug.Log("this is the a string before the loop readFileComputeNewcage.rowNumberUpdate " + readFileComputeNewcage.rowNumberUpdate);
        for (var j = 0; j < readFileComputeNewcage.columnNumberUpdate; j++)
            if (readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j] > thres)
                //Debug.Log("readFileComputeNewcage.barMatrices " +j +" "+ readFileComputeNewcage.barMatrices[ModelSegVertIndexesInput[i], j]);
                if (!ListCageSegOutput.Contains(j))
                    ListCageSegOutput.Add(j);
        //Debug.Log("this is the positions of the elements less than threshold " + j);
        //Debug.Log("The related cage vertices amount: " + ListCageSegOutput.Count);
    }
}