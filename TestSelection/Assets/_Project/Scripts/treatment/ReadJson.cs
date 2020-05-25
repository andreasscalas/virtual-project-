using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Accord.Math;
using LitJson;

public class ReadJson : MonoBehaviour
{
    private string jsonString;
    private JsonData data;
    private List<int> littleFinger = new List<int>();
    private int[] ringFinger;
    private int[] mediumFinger;
    private int[] indexFinger;
    private int[] thumb;
    private int[] wrist;
    private int[] palm;
    // Start is called before the first frame update
    void Start()
    {
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/" + "hand_segmentation.txt");
        //if (jsonString != null)
        //{ Debug.Log(jsonString); }

        data = JsonMapper.ToObject(jsonString);

        Debug.Log(data["annotations"][0]["triangles"]);

        for (int i = 0; i < data["annotations"][0]["triangles"].Count; i++)
        {

            string interData = Convert.ToString(data["annotations"][0]["triangles"][i]);
            //Debug.Log(interData);
            //Debug.Log(Int32.Parse(interData));
            int k = Int32.Parse(interData);
            littleFinger.Add(k);
        }
        Debug.Log(littleFinger[littleFinger.Count - 1]);
        //for (int i = 0; i < data["annotations"][1]["triangles"].Count; i++)
        //{
        //    ringFinger.Add(Convert.ToDouble(data["annotations"][1]["triangles"][i]));
        //}

        //for (int i = 0; i < data["annotations"][2]["triangles"].Count; i++)
        //{
        //    mediumFinger.Add(Convert.ToDouble(data["annotations"][2]["triangles"][i]));
        //}

        //for (int i = 0; i < data["annotations"][3]["triangles"].Count; i++)
        //{
        //    indexFinger.Add(Convert.ToDouble(data["annotations"][3]["triangles"][i]));
        //}

        //for (int i = 0; i < data["annotations"][4]["triangles"].Count; i++)
        //{
        //    thumb.Add(Convert.ToDouble(data["annotations"][4]["triangles"][i]));
        //}

        //for (int i = 0; i < data["annotations"][5]["triangles"].Count; i++)
        //{
        //    wrist.Add(Convert.ToDouble(data["annotations"][5]["triangles"][i]));
        //}

        //for (int i = 0; i < data["annotations"][6]["triangles"].Count; i++)
        //{
        //    palm.Add(Convert.ToDouble(data["annotations"][6]["triangles"][i]));
        //}
        //Debug.Log(data["annotations"][0]["triangles"]);

        //Debug.Log(littleFinger[littleFinger.Length]);
    }

}
