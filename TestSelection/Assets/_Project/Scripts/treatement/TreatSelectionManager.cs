using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatSelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    //Mesh mesh;
    //Vector3[] vertices;
    [HideInInspector]
    public GameObject obj;
    public List<Transform> selectionList = new List<Transform>();
    private List<Transform> interSelectionList = new List<Transform>();

    private Vector3 mousePos1;
    private Vector3 mousePos2;

    public Transform _selectedControlPoints;
    public Transform _unselectedControlPoints;
    [HideInInspector]
    public GameObject SelectedControlPoints;
    [HideInInspector]
    public GameObject UnselectedControlPoints;


    public MeshCreateControlPoints meshCreateControlPoints;
    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    void Start()
    {
        SelectedControlPoints = new GameObject();
        SelectedControlPoints.name = "Selected Control Points";
        _selectedControlPoints = SelectedControlPoints.transform;
        SelectedControlPoints.AddComponent<DragObject>();

        UnselectedControlPoints = new GameObject();
        UnselectedControlPoints.name = "Unselected Control Points";
        _unselectedControlPoints = UnselectedControlPoints.transform;
        //Collider boxCol = SelectedControlPoints.AddComponent<BoxCollider>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();

    }

    private void Update()
    {
        deleteControlPoint();
        storeControlPoint();
    }
    /// <summary>
    /// select and store data of gameobject(the control points) function.
    /// </summary>
    private void storeControlPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit) /*&& b == true*/)
            {
                mousePos1 =Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var selection = hit.transform;
                if (selection.CompareTag(selectableTag))
                {
                    obj = selection.gameObject;
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    obj.transform.parent = null;
                    obj.transform.parent = _selectedControlPoints;
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
                        //CopyComponent(SelectedControlPoints.GetComponentInChildren<Collider>(), SelectedControlPoints);
                    }                                     
                }
            }
            
        }

        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos1 != mousePos2)
            {
                Rect selectRect = new Rect(mousePos1.x, mousePos1.y, (mousePos2.x - mousePos1.x), (mousePos2.y - mousePos1.y));
                ////get the control points positions(inside initializedControlPoints Gameobject) that are inside the rectangle draw with mouse
                foreach (Transform child in meshCreateControlPoints._initializedControlPoints)
                {
                    if (selectRect.Contains(Camera.main.WorldToViewportPoint(child.position), true))
                    {
                        Debug.Log("Camera.main.WorldToViewportPoint(child.position)" + Camera.main.WorldToViewportPoint(child.position));
                        selectionList.Add(child);
                        interSelectionList.Add(child);
                    }
                }
                ////get the control points positions(inside unSelectedControlPoints Gameobject) that are inside the rectangle draw with mouse
                foreach (Transform child in _unselectedControlPoints)
                {
                    if (selectRect.Contains(Camera.main.WorldToViewportPoint(child.position), true))
                    {
                        Debug.Log("Camera.main.WorldToViewportPoint(child.position)" + Camera.main.WorldToViewportPoint(child.position));
                        selectionList.Add(child);
                        interSelectionList.Add(child);
                    }
                }
                ////put the control points into the SelectedControlPoints gameobject
                for (int i = 0; i < interSelectionList.Count; i++)
                {
                    var child = interSelectionList[i];
                    child.transform.parent = _selectedControlPoints;
                    var selectionRenderer = child.GetComponent<Renderer>();

                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial;
                    }
                }
                
                interSelectionList.Clear();
            }
        }       
    }


    /// <summary>
    /// delecte and remove gameobject(the control points) function.
    /// </summary>
    private void deleteControlPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //deselect a control point, and put this control point into the unselected gameobject
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit) /*&& b == true*/)
            {
                mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
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
                        obj.transform.parent = _unselectedControlPoints;
                        selectionList.Remove(selection);
                        Debug.Log(obj + " is removed ");
                        //objectremoved.text = obj + "is removed";
                    }

                }
               
            }
        }

        if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftControl))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos1 != mousePos2)
            {
                Rect selectRect = new Rect(mousePos1.x, mousePos1.y, (mousePos2.x - mousePos1.x), (mousePos2.y - mousePos1.y));
                ////get the control points positions(inside initializedControlPoints Gameobject) that are inside the rectangle draw with mouse
                foreach (Transform child in _selectedControlPoints)
                {
                    Debug.Log("selectRect" + selectRect);
                    if (selectRect.Contains(Camera.main.WorldToViewportPoint(child.position), true))
                    {
                        Debug.Log("Camera.main.WorldToViewportPoint(child.position)" + Camera.main.WorldToViewportPoint(child.position));
                        selectionList.Remove(child);
                        interSelectionList.Add(child);
                    }
                }
                ////put the control points into the SelectedControlPoints gameobject
                for (int i = 0; i < interSelectionList.Count; i++)
                {
                    var child = interSelectionList[i];
                    child.transform.parent = _unselectedControlPoints;
                    var selectionRenderer = child.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = defaultMaterial;
                    }
                }
                interSelectionList.Clear();
            }
        }
    }

    public void ClearControlPoints()
    {
        ////Reset the color for the selected control points and put them into the initialized Control points parent
        for (int i = 0; i < _selectedControlPoints.childCount; i++)
        {
            var child = _selectedControlPoints.GetChild(i);
            var selectionRenderer = child.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            child.transform.parent = null;
            child.transform.parent = meshCreateControlPoints._initializedControlPoints;
            i--;
        }
        ////Put the control points into the initialized Control points parent
        for (int i = 0; i < _unselectedControlPoints.childCount; i++)
        {
            var child = _unselectedControlPoints.GetChild(i);
            child.transform.parent = null;
            child.transform.parent = meshCreateControlPoints._initializedControlPoints;
            i--;
        }
        selectionList.Clear();
    }

    ///set self adopted colliders for the "selected control points"
    private void SetColliders()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "SomeChildObj")
            {
                Collider col = child.GetComponent<Collider>();
                col.enabled = false;
            }
        }
    }

    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));            
        }
        
        return copy;
    }
    //T CopyComponent<T>(T original, GameObject destination) where T : Component
    //{
    //    System.Type type = original.GetType();
    //    Component copy = destination.AddComponent(type);
    //    System.Reflection.FieldInfo[] fields = type.GetFields();
    //    foreach (System.Reflection.FieldInfo field in fields)
    //    {
    //        field.SetValue(copy, field.GetValue(original));
    //    }
    //    return copy as T;
    //}
 
/// <summary>
/// delete all the collders in the gameobject "Selected Contol Points"
/// </summary>
    public void DeleteAllColliders()
    {
        foreach (Collider c in SelectedControlPoints.GetComponents<Collider>())
        {
            Destroy(c);
        }
    }


    public void GetColliderCopy()
    {
        //GetInChildren get all the elements in parent and children
        Collider[] Cols = SelectedControlPoints.GetComponentsInChildren<Collider>();
        SphereCollider[] sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
        Debug.Log("看看sphereCollider进入之前的size\t" + sphereCollider.Length);
        int k = sphereCollider.Length;
        //To eliminate the elements in parent
        for (int j = Cols.Length-SelectedControlPoints.transform.childCount; j < Cols.Length; j++)
        {
            
            //Debug.Log("the position of the children colliders\t" + col.transform.position);
            CopyComponent(Cols[j], SelectedControlPoints);
            //Debug.Log("看看各个控制点的位置\t"+ Cols[j].transform.position);

            sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
            Debug.Log("看看sphereCollider的size\t" + sphereCollider.Length);
            //delete the offset of transform(remember when move a gameobject, its transfrom changed)
            sphereCollider[j-(Cols.Length - SelectedControlPoints.transform.childCount) + k].center = Cols[j].transform.position- SelectedControlPoints.transform.position;
            sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].radius = 0.05f;
            //Debug.Log("看看有多少个控制点\t" + (j - (Cols.Length - SelectedControlPoints.transform.childCount)));
        }











        ////////////////////foreach (Collider col in Cols)
        ////////////////////{
        ////////////////////    Debug.Log("col.gameObject.GetInstanceID\t" + col.gameObject.GetInstanceID());
        ////////////////////    Debug.Log("GetInstanceID\t" + GetInstanceID());

        ////////////////////    if (col.gameObject.GetInstanceID() != GetInstanceID())
        ////////////////////    {
        ////////////////////        Debug.Log("the position of the children colliders\t" + col.transform.position);
        ////////////////////        CopyComponent(col, SelectedControlPoints);
        ////////////////////        SphereCollider sphereCollider = SelectedControlPoints.GetComponent<SphereCollider>();
        ////////////////////        sphereCollider.center = col.transform.position;
        ////////////////////        Debug.Log("看看进来了几次\t" );
        ////////////////////    }
        ////////////////////}
        //foreach (Collider c in SelectedControlPoints.GetComponentsInChildren<Collider>())
        //{
        //    Debug.Log("the position of the children colliders\t"+c.transform.position);
        //    CopyComponent(c, SelectedControlPoints);
        //    //SphereCollider sphereCollider = SelectedControlPoints.GetComponent<SphereCollider>();
        //    //sphereCollider.center = c.transform.position;
        //    //Debug.Log("sphereCollider.center \t" + sphereCollider.center);
        //    //coll = Instantiate(c, SelectedControlPoints.transform);
        //    //coll.transform.parent = null;
        //    //coll.transform.parent = SelectedControlPoints.transform;
        //    //Debug.Log("Instantiate succeed\t" + coll.transform.position);
        //    //coll.GetComponent<Collider>();
        //    //myCollider = GetComponent<SphereCollider>();
        //    //col.transform.position = c.transform.position;
        //    //Debug.Log("the position of the slected control points colliders\t" + SelectedControlPoints.GetComponent<Collider>().transform.position);
        //}
    }
}