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
    [SerializeField] private Material Selected_Transparent;

    //Mesh mesh;
    //Vector3[] vertices;
    [HideInInspector]
    public GameObject obj;
    public List<Transform> selectionList = new List<Transform>();
    private List<Transform> interSelectionList = new List<Transform>();
    private List<Vector3> _position = new List<Vector3>(100);
    private List<Vector3> _scale = new List<Vector3>(100);

    private Vector3 mousePos1;
    private Vector3 mousePos2;

    public Transform _selectedControlPoints;
    public Transform _unselectedControlPoints;
    [HideInInspector]
    public GameObject SelectedControlPoints;
    [HideInInspector]
    public GameObject UnselectedControlPoints;
    private Vector3 barCenter = new Vector3();

    public MeshCreateControlPoints meshCreateControlPoints;
    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    void Start()
    {
        InitializeSelecObj();
        InitializeUnselecObj();
    }

    private void Update()
    {
        deleteControlPoint();
        storeControlPoint();
        for (int i = 0; i < selectionList.Count; i++)
        {
            _position.Add(selectionList[i].position);
            _scale.Add(selectionList[i].localScale);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ComputeBarCenter();
            //ComputeCreateCollider();
        }

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
            Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
            if (r == null)
            {
                r = SelectedControlPoints.AddComponent<Rigidbody>();
                r.useGravity = false;
                r.isKinematic = true;
            }
            if (Physics.Raycast(ray, out hit) /*&& b == true*/)
            {
                mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
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
                        Debug.Log(obj + " gameobject is stored");
                        //objectstored.text = obj+"is stored";
                        //CopyComponent(SelectedControlPoints.GetComponentInChildren<Collider>(), SelectedControlPoints);
                    }
                }
            }

        }

        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
            if (r == null)
            {
                r = SelectedControlPoints.AddComponent<Rigidbody>();
                r.useGravity = false;
                r.isKinematic = true;
            }
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
            Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
            Destroy(r);
            //Debug.Log("看哈进来没有");
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("看哈射线出来没有");
                mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var selection = hit.transform;
                //Debug.Log("看哈闯到没有，闯到的点位置"+ selection.position);

                //for (int i = 0; i < selection.childCount; i++)    
                //{
                //    var child = selection.GetChild(i);
                //    // do whatever you want with child transform object here
                //    if (child.CompareTag(selectableTag))
                //    {
                //        //Debug.Log("看哈撞到可选择没有");
                //        if (selectionList.Contains(child))
                //        {
                //            //Debug.Log("看哈撞到在list里的没有");
                //            obj = child.gameObject;
                //            var selectionRenderer = child.GetComponent<Renderer>();
                //            selectionRenderer.material = defaultMaterial;
                //            Debug.Log(obj + " is deleted");
                //            //objectdeleted.text = obj + "is deleted";
                //            obj.transform.parent = null;
                //            obj.transform.parent = _unselectedControlPoints;
                //            selectionList.Remove(child);
                //            Debug.Log(obj + " is removed ");
                //            //objectremoved.text = obj + "is removed";
                //        }

                //    }

                //}
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
        ////Put the control points of Unselected Control Points into the Initialized Control Points parent
        for (int i = 0; i < _unselectedControlPoints.childCount; i++)
        {
            var child = _unselectedControlPoints.GetChild(i);
            child.transform.parent = null;
            child.transform.parent = meshCreateControlPoints._initializedControlPoints;
            i--;
        }
        selectionList.Clear();
    }

    /////////////////set self adopted colliders for the "selected control points"

    //////////////Component CopyComponent(Component original, GameObject destination)
    //////////////{
    //////////////    System.Type type = original.GetType();
    //////////////    Component copy = destination.AddComponent(type);
    //////////////    // Copied fields can be restricted with BindingFlags
    //////////////    System.Reflection.FieldInfo[] fields = type.GetFields();
    //////////////    foreach (System.Reflection.FieldInfo field in fields)
    //////////////    {
    //////////////        field.SetValue(copy, field.GetValue(original));
    //////////////    }

    //////////////    return copy;
    //////////////}

    /// <summary>
    /// delete all the collders in the gameobject "Selected Contol Points"
    /// </summary>
    public void DeleteRecreateSelecObj()
    {        
        Destroy(SelectedControlPoints);
        InitializeSelecObj();
    }


    ////////////public void GetColliderCopy()
    ////////////{
    ////////////    //GetInChildren get all the elements in parent and children
    ////////////    Collider[] Cols = SelectedControlPoints.GetComponentsInChildren<Collider>();
    ////////////    SphereCollider[] sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
    ////////////    
    ////////////    int k = sphereCollider.Length;
    ////////////    //To eliminate the elements in parent
    ////////////    for (int j = Cols.Length - SelectedControlPoints.transform.childCount; j < Cols.Length; j++)
    ////////////    {

    ////////////        //Debug.Log("the position of the children colliders\t" + col.transform.position);
    ////////////        CopyComponent(Cols[j], SelectedControlPoints);
    ////////////        

    ////////////        sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
    ////////////       
    ////////////        //delete the offset of transform(remember when move a gameobject, its transfrom changed)
    ////////////        sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].center = Cols[j].transform.position - SelectedControlPoints.transform.position;
    ////////////        sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].radius = 0.06f;
    ////////////        
    ////////////    }
    ////////////}

    public void ComputeBarCenter()
    {
        Vector3 sum = new Vector3(0, 0, 0);
        for (int i = 0; i < selectionList.Count; i++)
        {
            sum = selectionList[i].position + sum;
        }
        barCenter = (sum / selectionList.Count);

    }
    public void ComputeCreateCollider()
    {
        float disMax = 0;
        for (int i = 0; i < selectionList.Count; i++)
        {
            float dis = Vector3.Distance(barCenter, selectionList[i].position);
            if (disMax < dis)
            {
                disMax = dis;
            }
        }
        //set the transform of the Selected Control Points to the barycentric point, compensate the offset to its children. 
        Vector3 offset = SelectedControlPoints.transform.position - barCenter;
        SelectedControlPoints.transform.position = barCenter;

        for (int i = 0; i < SelectedControlPoints.transform.childCount; i++)
        {
            var child = SelectedControlPoints.transform.GetChild(i);
            child.position = child.position + offset;
            //child.transform.localScale= new Vector3(0.05f/disMax, 0.05f/disMax, 0.05f/disMax);
        }


        Debug.Log("disMax" + disMax);
        //SelectedControlPoints.transform.localScale = new Vector3(2 * disMax, 2 * disMax, 2 * disMax);
        //SelectedControlPoints.GetComponent<MeshRenderer>().material= Selected_Transparent;
        SelectedControlPoints.AddComponent<SphereCollider>();
        SelectedControlPoints.GetComponent<SphereCollider>().radius = disMax;
    }

    void InitializeSelecObj()
    {  
        //SelectedControlPoints = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SelectedControlPoints = new GameObject();
        SelectedControlPoints.name = "Selected Control Points";
        _selectedControlPoints = SelectedControlPoints.transform;
        SelectedControlPoints.AddComponent<DragObject>();
    }

    void InitializeUnselecObj()
    {
        UnselectedControlPoints = new GameObject();
        UnselectedControlPoints.name = "Unselected Control Points";
        _unselectedControlPoints = UnselectedControlPoints.transform;
        //Collider boxCol = SelectedControlPoints.AddComponent<BoxCollider>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
    }
}