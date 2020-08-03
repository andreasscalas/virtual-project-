using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functionality : MonoBehaviour
{
    public GameObject modelCage;
    private Image imageCage;
    private Image imageControlPoints;
    Text hideDisplayCage;
    Text hideDisplayCPs;


    private ReadJson readJson;
    //public List<bool> levels=new List<bool>();
    [HideInInspector] public List<bool> levels=new List<bool>();
    [HideInInspector] public bool levelsChange=new bool();
    private List<GameObject> controlPoints = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        levelsChange = true;
        modelCage = GameObject.Find("hand_cage");
        modelCage.SetActive(false);

        //transform.horse_cage().Where(x => x.name.Contains("Patrol"));
        imageCage = GameObject.Find("Hide/show Cage").GetComponent<Image>();
        hideDisplayCage = GameObject.Find("Hide/show Cage").transform.Find("Text").GetComponent<Text>();
        imageCage.color = new Color(0f, 0.6f, 1f, 1f);

        imageControlPoints = GameObject.Find("Hide/show CPs").GetComponent<Image>();
        hideDisplayCPs = GameObject.Find("Hide/show CPs").transform.Find("Text").GetComponent<Text>();
        imageControlPoints.color = new Color(0f, 0.6f, 1f, 1f);

        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        for (int i = 0; i <= readJson.levelMax; i++)
        {
            levels.Add(false);
        }
        levels[0] = true;


        GameObject[] objs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        
        //after resetting, need to do this again
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].name.Contains("Control Point"))
            {
                controlPoints.Add(objs[i]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        levelsChange = false;
    }

    public void HideDisplayControlPoints()
    {
        foreach (var VARIABLE in controlPoints)
        {

            if (VARIABLE.activeSelf)
            {
                VARIABLE.SetActive(false);
                hideDisplayCPs.text = "Display CPs";
                imageControlPoints.color = new Color(0f, 1f, 0.6f, 1f);
            }

            else
            {
                VARIABLE.SetActive(true);
                imageControlPoints.color = new Color(0f, 0.6f, 1f, 1f);
                hideDisplayCPs.text = "Hide CPs";
            }
        }

        //imageControlPoints.color = new Color(0f, 1f, 0.6f, 1f);

        //foreach (var VARIABLE in controlPoints)
        //{
        //    if (!VARIABLE.activeSelf)
        //    {
        //        VARIABLE.SetActive(true);
        //    }
        //}
        //imageControlPoints.color = new Color(0f, 0.6f, 1f, 1f);

    }

    public void HideDisplayCage()
    {

        if (modelCage.activeSelf)
        {
            modelCage.SetActive(false);
            //foreach (var VARIABLE in controlPoints)
            //{
            //    VARIABLE.SetActive(false);
            //}
            hideDisplayCage.text = "Display cage";
            imageCage.color =  new Color(0f, 0.6f, 1f, 1f);
        }
        else
        {
            modelCage.SetActive(true);
            //foreach (var VARIABLE in controlPoints)
            //{
            //    VARIABLE.SetActive(true);
            //}
            hideDisplayCage.text = "Hide cage";
            imageCage.color = new Color(0f, 1f, 0.6f, 1f);
        }
    }


    //void OnGUI()
    //{
    //    //find out the levels, so that we can decide how many bouttons we need.



    //    for (int i = 0; i <= readJson.levelMax; i++)
    //    {
    //        var myButton = GUI.Button(new Rect(10, 30 * i, 50, 30), "level " + i);
    //        if (myButton)
    //        {
    //            levelsChange = true;
    //            for (int j = 0; j < levels.Count; j++)
    //            {
    //                levels[j] = false;
    //            }
    //            levels[i] = true;
    //            Debug.Log(String.Format("button{0} is pressed: ", i));
    //        }
    //    }
    //}   
}
