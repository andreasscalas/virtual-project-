using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functionality : MonoBehaviour
{
    public GameObject model;
    private Image image;

    private ReadJson readJson;
    //public List<bool> levels=new List<bool>();
    [HideInInspector] public List<bool> levels=new List<bool>();
    [HideInInspector] public bool levelsChange=new bool();
    private List<GameObject> controlPoints = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        levelsChange = true;
        model = GameObject.Find("hand_cage");
        //transform.horse_cage().Where(x => x.name.Contains("Patrol"));
        image = GameObject.Find("Hide/show Cage&CPs").GetComponent<Image>();
        image.color = new Color(0f, 0.6f, 1f, 1f);

        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        for (int i = 0; i <= readJson.levelMax; i++)
        {
            levels.Add(false);
        }
        levels[0] = true;


        GameObject[] objs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        
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

    public void HideDisplayCage()
    {
        
        if (model.activeSelf)
        {
            model.SetActive(false);
            foreach (var VARIABLE in controlPoints)
            {
                VARIABLE.SetActive(false);
            }
            image.color = new Color(0f, 1f, 0.6f, 1f);
        }
        else
        {
            model.SetActive(true);
            foreach (var VARIABLE in controlPoints)
            {
                VARIABLE.SetActive(true);
            }
            image.color = new Color(0f, 0.6f, 1f, 1f);
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
