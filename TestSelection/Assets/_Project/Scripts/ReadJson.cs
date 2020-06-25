using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using LitJson;
using Debug = UnityEngine.Debug;


public class ReadJson : MonoBehaviour
{
    private string jsonString;
    private JsonData data;
    [HideInInspector]
    public List<int> littleFinger = new List<int>();
    [HideInInspector]
    public List<int> ringFinger = new List<int>();
    [HideInInspector]
    public List<int> mediumFinger = new List<int>();
    [HideInInspector]
    public List<int> indexFinger = new List<int>();
    [HideInInspector]
    public List<int> thumb = new List<int>();
    [HideInInspector]
    public List<int> wrist = new List<int>();
    [HideInInspector]
    public List<int> palm = new List<int>();

    public List<List<int>> AllSegtrianIndexes = new List<List<int>>();
    public List<List<int>> AllSegVertsIndexes = new List<List<int>>();
    public List<int> AllSegtriansIndexAmounts = new List<int>();
    public List<int> AllSegVertsIndexAmounts = new List<int>();
    public List<List<int>> AllSegColors = new List<List<int>>();
    //public List<int> interSegTrianLists = new List<int>();
    //public List<int> interSegVertLists = new List<int>();
    [HideInInspector]
    private MeshCreateControlPoints meshCreateControlPoints;
    [HideInInspector]
    private ReadFileComputeNewcage readFileComputeNewcage;

    [HideInInspector]
    public List<int> trianModelSegmented = new List<int>();
    [HideInInspector]
    public List<int> trianCageSegmented = new List<int>();
    [Range(-0.1f,1.0f)]
    public double threshold;
    private double thresholdPrime;
    [HideInInspector]
    public bool switchSegment;
    [SerializeField] private Material segmentmMaterial;
    [SerializeField] private Material segmentmMaterial2;
    [HideInInspector]
    List<int> Lst = new List<int>();

    public Color[] colors ;
    // Start is called before the first frame update
    void Start()
    {
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation_correct.txt");

        data = JsonMapper.ToObject(jsonString);

        //Debug.Log(data["annotations"][0]["triangles"]);

        GetDataToLst(data["annotations"][0]["triangles"], littleFinger);
        GetDataToLst(data["annotations"][1]["triangles"], ringFinger);
        GetDataToLst(data["annotations"][2]["triangles"], mediumFinger);
        GetDataToLst(data["annotations"][3]["triangles"], indexFinger);
        GetDataToLst(data["annotations"][4]["triangles"], thumb);
        GetDataToLst(data["annotations"][5]["triangles"], wrist);
        GetDataToLst(data["annotations"][6]["triangles"], palm);

        int k = littleFinger.Count + ringFinger.Count + mediumFinger.Count + indexFinger.Count + thumb.Count +
                wrist.Count + palm.Count;

        //Debug.Log(littleFinger.Count);
        //Debug.Log(ringFinger.Count);
        //Debug.Log(wrist.Count);
        //Debug.Log(palm.Count);

        for (int i = 0; i < data["annotations"].Count; i++)
        {
            List<int> interSegTrianLists = new List<int>();
            List<int> interSegColors = new List<int>();
            interSegTrianLists.Clear();
            interSegColors.Clear();
            GetDataToLst(data["annotations"][i]["triangles"], interSegTrianLists);
            GetDataToLst(data["annotations"][i]["color"], interSegColors);

            //for (int j = 0; j < interSegTrianLists.Count; j++)
            //{
            //    Debug.Log(interSegTrianLists[j]);
            //}
            AllSegtrianIndexes.Add(interSegTrianLists);
            AllSegColors.Add(interSegColors);
            AllSegtriansIndexAmounts.Add(interSegTrianLists.Count);
        }

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
        //DrawFilter(trianCageSegmented);
        thresholdPrime = 1.1f;
        switchSegment = false;

        for (int j = 0; j < AllSegtrianIndexes.Count; j++)
        {
            List<int> interSegVertLists = new List<int>();
            interSegVertLists.Clear();
            
            for (int i = 0; i < AllSegtrianIndexes[j].Count; i++)
            {
                //Debug.Log("littleFinger " +littleFinger[i] );
                if (!interSegVertLists.Contains(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3]))
                { interSegVertLists.Add(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3]); }
                //Debug.Log("vertex 1 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3]);

                if (!interSegVertLists.Contains(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 1]))
                { interSegVertLists.Add(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 1]); }
                //Debug.Log("vertex 3 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3 + 1]);


                if (!interSegVertLists.Contains(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 2]))
                { interSegVertLists.Add(meshCreateControlPoints.trisModel[(AllSegtrianIndexes[j][i] - 1) * 3 + 2]); }
                //Debug.Log("vertex 2 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3+2]);
                //Debug.Log("little finger triangle index " + i + " " + littleFinger[i]);
            }
            AllSegVertsIndexes.Add(interSegVertLists);
            AllSegVertsIndexAmounts.Add(interSegVertLists.Count);
        }
        
        //for (int i = 0; i < AllSegVertsIndexes[0].Count; i++)
        //{
        //    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    var renderer = cube.GetComponent<MeshRenderer>().material = segmentmMaterial;
        //    cube.transform.position = meshCreateControlPoints.modelVertices[AllSegVertsIndexes[0][i]];
        //    //Debug.Log("little finger Vertices " + i+" " + meshCreateControlPoints.modelVertices[Lst[i]].ToString(("F6")));

        //    cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //}
        //Debug.Log("first triangle " + Lst[0] + " " + Lst[1] + " " + Lst[2]);


        for (int i = 0; i < littleFinger.Count; i++)
        {
            //Debug.Log("littleFinger " +littleFinger[i] );
            if (!Lst.Contains(meshCreateControlPoints.trisModel[(littleFinger[i]-1) * 3 ])) 
            { Lst.Add(meshCreateControlPoints.trisModel[(littleFinger[i]-1) * 3]); }
            //Debug.Log("vertex 1 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3]);

            if (!Lst.Contains(meshCreateControlPoints.trisModel[(littleFinger[i] - 1) * 3 + 1]))
            { Lst.Add(meshCreateControlPoints.trisModel[(littleFinger[i] - 1) * 3 + 1]); }
            //Debug.Log("vertex 3 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3 + 1]);


            if (!Lst.Contains(meshCreateControlPoints.trisModel[(littleFinger[i] - 1) * 3 + 2]))
            { Lst.Add(meshCreateControlPoints.trisModel[(littleFinger[i] - 1) * 3 + 2]); }
            //Debug.Log("vertex 2 " + meshCreateControlPoints.trisModel[littleFinger[i] * 3+2]);
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
        //////    cube.transform.position = meshCreateControlPoints.modelVertices[Lst[i]];
        //////    Debug.Log("little finger Vertices " + i + " " + meshCreateControlPoints.modelVertices[Lst[i]].ToString(("F6")));

        //////    cube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //////}
        //Debug.Log("first triangle " + Lst[0] + " "+Lst[1]+ " "+ Lst[2]);

    }

    void Update()
    {
        if (thresholdPrime != threshold || switchSegment)
        {
            //MapSegModel(littleFinger);
            filterBarMatrix(threshold);
            deleteFilter();
            DrawFilter(trianCageSegmented);
            switchSegment = false;
            thresholdPrime = threshold;
        }

    }

    public void deleteFilter()
    {
        //destroy the previous segment.
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Segment");
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
    }

    private void DrawFilter(List<int> trianCageSegmented)
    {

        //draw new segment
        for (int i = 0; i < trianCageSegmented.Count; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<MeshRenderer>().material = segmentmMaterial;
            obj.transform.position = meshCreateControlPoints.cageVertices[trianCageSegmented[i]];
            obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            obj.tag = "Segment";
        }
       
    }


    private void GetDataToLst(JsonData jsonData, List<int> Mylist)
    {
        for (int i = 0; i < jsonData.Count; i++)
        {

            string interData = Convert.ToString(jsonData[i]);
            //Debug.Log(interData);
            //Debug.Log(Int32.Parse(interData));
            int k = Int32.Parse(interData);
            Mylist.Add(k);
        }
    }
    /// <summary>
    /// Get the segmentation triangles of the model by using the "triant" file data
    /// </summary>
    /// <param name="segmentations"></param>
    public void MapSegModel(List<int> segmentations)
    {
        for (int i = 0; i < segmentations.Count; i++)
        {
            if (!trianModelSegmented.Contains(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3]))
            {
                trianModelSegmented.Add(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3]);

            }

            if (!trianModelSegmented.Contains(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3 + 2]))
            {
                trianModelSegmented.Add(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3 + 2]);
            }

            if (!trianModelSegmented.Contains(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3 + 1]))
            {
                trianModelSegmented.Add(meshCreateControlPoints.trisModel[(segmentations[i] - 1) * 3 + 1]);
            }
        }
    }

    /// <summary>
    /// filter the Barycentric matrix by using a user defined threshold
    /// </summary>
    /// <param name="thresHold"></param>
    public void filterBarMatrix(double thres)
    {
        //Debug.Log("this is the a string inside filter");
        trianCageSegmented.Clear();
        for (int i = 0; i < trianModelSegmented.Count; i++)
        {
            //Debug.Log("this is the a string inside for loop trianModelSegmented.Count");
            //Debug.Log("this is the a string before the loop readFileComputeNewcage.rowNumberUpdate " + readFileComputeNewcage.rowNumberUpdate);
            for (int j = 0; j < readFileComputeNewcage.columnNumberUpdate; j++)
            {

                if (readFileComputeNewcage.barMatrices[trianModelSegmented[i], j] > thres)
                {
                    //Debug.Log("readFileComputeNewcage.barMatrices " +j +" "+ readFileComputeNewcage.barMatrices[trianModelSegmented[i], j]);
                    if (!trianCageSegmented.Contains(j))
                    {
                        trianCageSegmented.Add(j);
                        Debug.Log("this is the positions of the elements less than threshold " + j);
                    }

                }
            }
        }
        Debug.Log("The related cage vertices amount: "+ trianCageSegmented.Count);
    }

}
