using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class AddListener : MonoBehaviour
{

    [HideInInspector]
    public MeshCreateControlPoints meshCreateControlPoints;
    private List<GameObject> LstCPs = new List<GameObject>();

    public List<InteractionBehaviour> LstInteractBes = new List<InteractionBehaviour>();
    //public ChangeColor changeColor;
    void Start()
    {
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        //Debug.Log("meshCreateControlPoints._initializedControlPoints.childCount " + meshCreateControlPoints._initializedControlPoints.childCount);

        for (int i = 0; i < meshCreateControlPoints._initializedControlPoints.childCount; i++)
        {
            int k = i + 1;
            var obj = GameObject.Find("Initialized Control Points/Control Point " + k);
            LstCPs.Add(obj);
            LstInteractBes.Add(obj.GetComponent<InteractionBehaviour>());

            LstInteractBes[i].OnContactBegin += obj.GetComponent<ChangeColor>().SetColor;
            LstInteractBes[i].OnContactBegin += obj.GetComponent<ChangeColor>().SetParent;
            LstInteractBes[i].OnContactEnd += obj.GetComponent<ChangeColor>().ResetColor;
        }

        //Obj1 = GameObject.Find("Initialized Control Points/Control Point 1");
        //Debug.Log("Obj1 " + Obj1.name);

        //Obj2 = GameObject.Find("Initialized Control Points/Control Point 2");
        //Debug.Log("Obj2 " + Obj2.name);


        //InteractionBehaviour interaction1 = Obj1.GetComponent<InteractionBehaviour>();
        //InteractionBehaviour interaction2 = Obj2.GetComponent<InteractionBehaviour>();
        //interaction1.OnContactBegin += OnControlPointSelect1;
        //interaction2.OnContactBegin += OnControlPointSelect2;
        //interaction1.OnContactEnd += OnControlPointDeselect1;
        //interaction2.OnContactEnd += OnControlPointDeselect2;


    }




    //private Action OnControlPointSelect(GameObject obj)
    //{
    //    obj.GetComponent<ChangeColor>().SetColor();
    //    return null;
    //}




    //private void OnControlPointDeselect1()
    //{
    //    Obj1.GetComponent<ChangeColor>().ResetColor();
    //}

    //private void OnControlPointDeselect2()
    //{
    //    Obj2.GetComponent<ChangeColor>().ResetColor();
    //}

    //private void OnControlPointSelect1()
    //{
    //    Obj1.GetComponent<ChangeColor>().SetColor();
    //}

    //private void OnControlPointSelect2()
    //{
    //    Obj2.GetComponent<ChangeColor>().SetColor();
    //}
}
