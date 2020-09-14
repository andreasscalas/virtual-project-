using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] public Material defaultMaterial;
    [SerializeField] public Material highlightMaterial;
    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    public bool leftHandtouch;
    public bool rightHandtouch;

    private TreatSelectionManager treatSelectionManager;

    void Start()
    {
        leftHandtouch = false;
        rightHandtouch = false;
        treatSelectionManager = GameObject.Find("Selection Manager").GetComponent<TreatSelectionManager>();
        defaultMaterial = Resources.Load("Object@Default", typeof(Material)) as Material;
        highlightMaterial = Resources.Load("Object@Selected", typeof(Material)) as Material;
    }

    //void Update()
    //{
    //    if (leftHandModel.IsTracked)
    //    {
            
    //    }
    //}


    public void SetColor()
    {
        gameObject.GetComponent<MeshRenderer>().material= defaultMaterial;
        Debug.Log("gameObject.name touch set color" + gameObject.name);
    }

    public void SetParent()
    {
        gameObject.transform.parent = null;
        gameObject.transform.parent = treatSelectionManager._selectedControlPoints;
        Debug.Log("gameObject.name touch set parent" + gameObject.name);
    }



    public void ResetColor()
    {
        gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
        Debug.Log("gameObject.name leave set recolor" + gameObject.name);
    }
}
