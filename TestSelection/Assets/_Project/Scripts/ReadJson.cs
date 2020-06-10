using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class ReadJson : MonoBehaviour
{
    private string jsonString;
    private JsonData data;
    private List<int> littleFinger = new List<int>();
    private List<int> ringFinger = new List<int>();
    private List<int> mediumFinger = new List<int>();
    private List<int> indexFinger = new List<int>();
    private List<int> thumb = new List<int>();
    private List<int> wrist = new List<int>();
    private List<int> palm = new List<int>();
    private MeshCreateControlPoints meshCreateControlPoints;
    private ReadFileComputeNewcage readFileComputeNewcage;
    List<int> trianModelSegmented = new List<int>();
    List<int> trianCageSegmented = new List<int>();
    [Range(-0.1f,1.0f)]
    public double threshold;
    private double thresholdPrime;
    [SerializeField] private Material segmentmMaterial;

    // Start is called before the first frame update
    void Start()
    {
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        readFileComputeNewcage = GameObject.Find("Selection Manager").GetComponent<ReadFileComputeNewcage>();
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation.txt");

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
        //Debug.Log("total length "+ k);
        //MapSegModel(littleFinger);
        //filterBarMatrix(threshold);
        //foreach (var VARIABLE in littleFinger)
        //{
        //    Debug.Log("littleFinger " + VARIABLE);
        //}
        //DrawFilter(trianCageSegmented);
        thresholdPrime = 1.1f;
    }

    void Update()
    {
        if (thresholdPrime != threshold)
        {
            MapSegModel(ringFinger);
            filterBarMatrix(threshold);
            deleteFilter();
            DrawFilter(trianCageSegmented);
        }
        thresholdPrime = threshold;
    }

    private void deleteFilter()
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
    private void MapSegModel(List<int> segmentations)
    {
        for (int i = 0; i < segmentations.Count; i++)
        {
            trianModelSegmented.Add(meshCreateControlPoints.trisModel[segmentations[i] * 3]);
            trianModelSegmented.Add(meshCreateControlPoints.trisModel[segmentations[i] * 3+2]);
            trianModelSegmented.Add(meshCreateControlPoints.trisModel[segmentations[i] * 3 + 1]);
        }
    }

    /// <summary>
    /// filter the Barycentric matrix by using a user defined threshold
    /// </summary>
    /// <param name="thresHold"></param>
    private void filterBarMatrix(double thres)
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
                    Debug.Log("readFileComputeNewcage.barMatrices " +j +" "+ readFileComputeNewcage.barMatrices[trianModelSegmented[i], j]);
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
