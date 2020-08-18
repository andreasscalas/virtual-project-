using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functionality : MonoBehaviour
{
    private readonly List<GameObject> controlPoints = new List<GameObject>();
    private Text hideDisplayCage;
    private Text hideDisplayCPs;
    private Image imageCage;
    private Image imageControlPoints;

    public GameObject modelCage;
    private GameObject[] objs;

    private ReadJson readJson;


    // Start is called before the first frame update
    private void Start()
    {
        //modelCage = GameObject.Find("hand_cage");
        modelCage.SetActive(false);
        imageCage = GameObject.Find("Hide/show Cage").GetComponent<Image>();
        hideDisplayCage = GameObject.Find("Hide/show Cage").transform.Find("Text").GetComponent<Text>();
        imageCage.color = new Color(0f, 0.6f, 1f, 1f);

        imageControlPoints = GameObject.Find("Hide/show CPs").GetComponent<Image>();
        hideDisplayCPs = GameObject.Find("Hide/show CPs").transform.Find("Text").GetComponent<Text>();
        imageControlPoints.color = new Color(0f, 0.6f, 1f, 1f);

        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();

        //after resetting, need to do this again////////////////////
        FindControlPoints();
    }

    public void FindControlPoints()
    {
        controlPoints.Clear();
        objs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        for (var i = 0; i < objs.Length; i++)
            if (objs[i].name.Contains("Control Points"))
            {
                controlPoints.Add(objs[i]);
                Debug.Log("Find control points 0"+ objs[i].name);
            }
    }


    public void HideDisplayControlPoints()
    {
        Debug.Log("VARIABLE " + controlPoints.Count);
        foreach (var VARIABLE in controlPoints)
        {
            Debug.Log("VARIABLE "+ VARIABLE.name);
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

    }

    public void HideDisplayCage()
    {
        if (modelCage.activeSelf)
        {
            modelCage.SetActive(false);
            hideDisplayCage.text = "Display cage";
            imageCage.color = new Color(0f, 0.6f, 1f, 1f);
        }
        else
        {
            modelCage.SetActive(true);
            hideDisplayCage.text = "Hide cage";
            imageCage.color = new Color(0f, 1f, 0.6f, 1f);
        }
    }
}