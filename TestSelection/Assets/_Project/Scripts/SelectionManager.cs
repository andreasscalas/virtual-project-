using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    Mesh mesh;
    Vector3[] vertices;
    [HideInInspector]
    public GameObject obj;
    private List<Transform> selectionList = new List<Transform>();

    public GameObject SelectedControlPoints;
    public GameObject UnselectedControlPoints;
    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    void start()
    {
        Collider boxCol = SelectedControlPoints.AddComponent<BoxCollider>();
    }

    private void Update()
    {
        deleteGameobject();
        storeGameobject();
        
    }
    /// <summary>
    /// select and store data of gameobject(the control points) function.
    /// </summary>
    private void storeGameobject()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0)&& Input.GetKey(KeyCode.X))
        {
            if (Physics.Raycast(ray, out hit) /*&& b == true*/)
            {
                var selection = hit.transform;
                if (selection.CompareTag(selectableTag))
                {
                    obj = selection.gameObject;
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    obj.transform.parent = null;
                    obj.transform.parent = SelectedControlPoints.transform;
                    Debug.Log(obj + " gameobject is selected");

                    //objectselected.text= obj+ "is selected";

                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial;
                    }
                    if (selectionList.Contains(selection) == false)
                    {
                        selectionList.Add(selection);
                        Debug.Log(obj+" gameobject is stored" );

                        //objectstored.text = obj+"is stored";
                    }
                }
            }
        }
    }
    /// <summary>
    /// delecte and remove gameobject(the control points) function.
    /// </summary>
    private void deleteGameobject()
    {
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //这里指的是如果处于退出选中状态，将会赋予初始。我们要做的是退出选中状态之后：我们会把选中的vertices的index从list里面删除）
        if (Input.GetMouseButtonDown(1)&& Input.GetKey(KeyCode.X))
        {
            if (Physics.Raycast(ray, out hit) /*&& b == true*/)
            {
                var selection = hit.transform;
                if (selection.CompareTag(selectableTag))
                {
                    if (selectionList.Contains(selection))
                    {
                        obj = selection.gameObject;
                        var selectionRenderer = selection.GetComponent<Renderer>();
                        selectionRenderer.material = defaultMaterial;
                        Debug.Log(obj + " is deleted");
                        //objectdeleted.text = obj + "is deleted";
                        obj.transform.parent = null;
                        obj.transform.parent = UnselectedControlPoints.transform;
                        selectionList.Remove(selection);
                        Debug.Log(obj + " is removed ");
                        //objectremoved.text = obj + "is removed";
                    }

                }
               
            }
        }
    }
}