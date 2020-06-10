using System;
using System.Collections;
using System.Collections.Generic;
using Accord.Math;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Leap;
using Vector3 = UnityEngine.Vector3;

public class TreatSelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private string selectedParentTag = "SelectedParent";
    [SerializeField] private string unselectedParentTag = "UnselectedParent";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material barMaterial;
    [SerializeField] private Material Selected_Transparent;

    //Mesh mesh;
    //Vector3[] vertices;
    [HideInInspector]
    public GameObject obj;
    public List<Transform> selectionList = new List<Transform>();
    private List<Transform> interSelectionList = new List<Transform>();
    //private List<Vector3> _position = new List<Vector3>(100);
    //private List<Vector3> _scale = new List<Vector3>(100);

    private Vector3 mousePos1;
    private Vector3 mousePos2;
    private Vector3 colliderPosition;

    public Transform _selectedControlPoints;
    //public Transform _unselectedControlPoints;
    //[HideInInspector]
    public GameObject SelectedControlPoints;
    //[HideInInspector]
    //public GameObject UnselectedControlPoints;
    private Vector3 barCenter = new Vector3();


    public MeshCreateControlPoints meshCreateControlPoints;
    private InteractionBehaviour interactionSCPs = new InteractionBehaviour();
    //UnityEvent m_MyEvent;
    public Text voiceControlCommand;

    public HandModelBase rightHandModel;
    private bool select;
    private bool delete;


    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    void Start()
    {
        InitializeSelecObj();
        //InitializeUnselecObj();
        select = false;
        delete = false;
    }

    private void Update()
    {
        deleteControlPoint();
        storeControlPoint();
        if (select)
        { CastSelectRay(select); }
        if (delete)
        { CastSelectRay(delete); }
    }


    public void OnSelect()
    {
        delete = false;
        select = true;
    }
    public void OnDelete()
    {
        select = false;
        delete = true;
    }

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
                    Debug.Log("Evenery time click a control point" + obj.transform.position);
                    colliderPosition = hit.collider.transform.position;
                }

                if (selection.CompareTag(selectedParentTag))
                {
                    colliderPosition = hit.collider.transform.position;
                    Debug.Log("collider Position in selection function " + colliderPosition);
                }

                Destroy(selection.GetComponent<InteractionBehaviour>());
            }
        }


        //if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //    Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
        //    if (r == null)
        //    {
        //        r = SelectedControlPoints.AddComponent<Rigidbody>();
        //        r.useGravity = false;
        //        r.isKinematic = true;
        //    }
        //    if (mousePos1 != mousePos2)
        //    {
        //        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, (mousePos2.x - mousePos1.x), (mousePos2.y - mousePos1.y));
        //        ////get the control points positions(inside initializedControlPoints Gameobject) that are inside the rectangle draw with mouse
        //        foreach (Transform child in meshCreateControlPoints._initializedControlPoints)
        //        {
        //            if (selectRect.Contains(Camera.main.WorldToViewportPoint(child.position), true))
        //            {
        //                Debug.Log("Camera.main.WorldToViewportPoint(child.position)" + Camera.main.WorldToViewportPoint(child.position));
        //                selectionList.Add(child);
        //                interSelectionList.Add(child);
        //            }
        //        }

        //        ////put the control points into the SelectedControlPoints gameobject
        //        for (int i = 0; i < interSelectionList.Count; i++)
        //        {
        //            var child = interSelectionList[i];
        //            child.transform.parent = _selectedControlPoints;
        //            var selectionRenderer = child.GetComponent<Renderer>();

        //            if (selectionRenderer != null)
        //            {
        //                selectionRenderer.material = highlightMaterial;
        //            }
        //        }

        //        interSelectionList.Clear();
        //    }
        //}
    }

    private void deleteControlPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //deselect a control point, and put this control point into the unselected gameobject

        if (Input.GetMouseButtonDown(1))
        {
            DeleteParentCollider();
            //if rigidbody remains, we can not remove control points from Selected Control Points to other gameobject
            Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
            Destroy(r);

            if (Physics.Raycast(ray, out hit))
            {
                mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var selection = hit.transform;
                if (selection.CompareTag(selectedParentTag))
                {
                    obj = selection.gameObject;
                    //get the ray hit position 
                    Vector3 position = hit.collider.transform.position;
                    for (int i = 0; i < selection.childCount; i++)
                    {
                        Transform child = selection.GetChild(i);
                        if (Vector3.Distance(position, child.transform.position) < 0.1)
                        {
                            var selectionRenderer = child.GetComponent<Renderer>();
                            selectionRenderer.material = defaultMaterial;
                            Debug.Log(obj + " is deleted");
                            //objectdeleted.text = obj + "is deleted";
                            child.transform.parent = null;
                            child.transform.parent = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/;
                            selectionList.Remove(selection);
                            Debug.Log(obj + " is removed ");
                        }
                    }

                }

                if (selection.CompareTag(selectableTag))
                {
                    Debug.Log("To see if enter compareTag");
                    if (selectionList.Contains(selection))
                    {
                        obj = selection.gameObject;
                        var selectionRenderer = selection.GetComponent<Renderer>();
                        selectionRenderer.material = defaultMaterial;
                        Debug.Log(obj + " is deleted");
                        //objectdeleted.text = obj + "is deleted";
                        obj.transform.parent = null;
                        obj.transform.parent = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/;
                        selectionList.Remove(selection);
                        Debug.Log(obj + " is removed ");
                        obj.AddComponent<InteractionBehaviour>();
                        //objectremoved.text = obj + "is removed";
                    }

                }
            }
        }


        //if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //    if (mousePos1 != mousePos2)
        //    {
        //        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, (mousePos2.x - mousePos1.x), (mousePos2.y - mousePos1.y));
        //        ////get the control points positions(inside initializedControlPoints Gameobject) that are inside the rectangle draw with mouse
        //        foreach (Transform child in _selectedControlPoints)
        //        {
        //            Debug.Log("selectRect" + selectRect);
        //            if (selectRect.Contains(Camera.main.WorldToViewportPoint(child.position), true))
        //            {
        //                //Debug.Log("Camera.main.WorldToViewportPoint(child.position)" + Camera.main.WorldToViewportPoint(child.position));
        //                selectionList.Remove(child);
        //                interSelectionList.Add(child);
        //            }
        //        }
        //        ////put the control points into the SelectedControlPoints gameobject
        //        for (int i = 0; i < interSelectionList.Count; i++)
        //        {
        //            var child = interSelectionList[i];
        //            child.transform.parent = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/;
        //            var selectionRenderer = child.GetComponent<Renderer>();
        //            if (selectionRenderer != null)
        //            {
        //                selectionRenderer.material = defaultMaterial;
        //            }
        //        }
        //        interSelectionList.Clear();
        //    }
        //}
    }


    private void CastSelectRay(bool act)
    {
        if (rightHandModel.IsTracked)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            if (act == select)
            { Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.red); }
            if (act == delete)
            { Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.green); }

            Ray ray = new Ray(rightHand.Fingers[1].TipPosition.ToVector3(), rightHand.Fingers[1].Direction.ToVector3());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("this is a string in once hit happens");
                if (hit.transform.tag == "Selectable")
                {
                    if (select)
                    {
                        Debug.Log("select hit");/*hit.transform.gameObject.GetComponent<ChangeColor>().SetColor();*/
                        var selection = hit.transform;
                        if (selection.CompareTag(selectableTag))
                        {
                            obj = selection.gameObject;
                            var selectionRenderer = selection.GetComponent<Renderer>();
                            obj.transform.parent = null;
                            obj.transform.parent = _selectedControlPoints;
                            Debug.Log(obj + " gameobject is selected");

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
                            Debug.Log("Evenery time click a control point" + obj.transform.position);
                            colliderPosition = hit.collider.transform.position;
                        }

                        if (selection.CompareTag(selectedParentTag))
                        {
                            colliderPosition = hit.collider.transform.position;
                            Debug.Log("collider Position in selection function " + colliderPosition);
                        }

                        Destroy(selection.GetComponent<InteractionBehaviour>());
                        
                    }

                    if (delete)
                    {
                        Debug.Log("delete hit");/*hit.transform.gameObject.GetComponent<ChangeColor>().ResetColor();*/
                        DeleteParentCollider();
                        //if rigidbody remains, we can not remove control points from Selected Control Points to other gameobject
                        Rigidbody r = SelectedControlPoints.GetComponent<Rigidbody>();
                        Destroy(r);

                        if (Physics.Raycast(ray, out hit))
                        {
                            var selection = hit.transform;
                            if (selection.CompareTag(selectedParentTag))
                            {
                                obj = selection.gameObject;
                                //get the ray hit position 
                                Vector3 position = hit.collider.transform.position;
                                for (int i = 0; i < selection.childCount; i++)
                                {
                                    Transform child = selection.GetChild(i);
                                    if (Vector3.Distance(position, child.transform.position) < 0.1)
                                    {
                                        var selectionRenderer = child.GetComponent<Renderer>();
                                        selectionRenderer.material = defaultMaterial;
                                        Debug.Log(obj + " is deleted");
                                        //objectdeleted.text = obj + "is deleted";
                                        child.transform.parent = null;
                                        child.transform.parent = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/;
                                        selectionList.Remove(selection);
                                        Debug.Log(obj + " is removed ");
                                    }
                                }

                            }

                            if (selection.CompareTag(selectableTag))
                            {
                                Debug.Log("To see if enter compareTag");
                                if (selectionList.Contains(selection))
                                {
                                    obj = selection.gameObject;
                                    var selectionRenderer = selection.GetComponent<Renderer>();
                                    selectionRenderer.material = defaultMaterial;
                                    Debug.Log(obj + " is deleted");
                                    //objectdeleted.text = obj + "is deleted";
                                    obj.transform.parent = null;
                                    obj.transform.parent = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/;
                                    selectionList.Remove(selection);
                                    Debug.Log(obj + " is removed ");
                                    obj.AddComponent<InteractionBehaviour>();
                                    //objectremoved.text = obj + "is removed";
                                }

                            }
                        }
                        

                    }
                }
            }

        }
    }



    public void Rotation()
    {
        Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
        Destroy(SelectedControlPoints.GetComponent<rotateObj>());
        ComputeBarCenter(selectionList);
        //DeleteParentCollider();
        ComputeDisSetCPs(barCenter);
        //GetColliderCopy();
        SelectedControlPoints.AddComponent<rotateObj>();
        voiceControlCommand.text = "Rotation";
    }
    public void Translation()
    {
        Destroy(SelectedControlPoints.GetComponent<rotateObj>());
        Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
        Destroy(SelectedControlPoints.GetComponent<InteractionBehaviour>());
        ComputeBarCenter(selectionList);
        //DeleteParentCollider();
        ComputeDisSetCPs(colliderPosition);
        //GetColliderCopy();

        SelectedControlPoints.AddComponent<MoveObjectUpdate>();
        interactionSCPs = SelectedControlPoints.AddComponent<InteractionBehaviour>();
        interactionSCPs.OnContactBegin = () => { Debug.Log("hand touch SelecCPs..."); };

        //if (m_MyEvent == null)
        //    m_MyEvent = new UnityEvent();
        //m_MyEvent.AddListener(Ping);

        //SelectedControlPoints.GetComponent<InteractionBehaviour>();
        voiceControlCommand.text = "Translation";
    }



    public void Scale()
    {
        Destroy(SelectedControlPoints.GetComponent<rotateObj>());
        Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
        //DeleteParentCollider();
        meshCreateControlPoints.scaleCenter = colliderPosition;
        ComputeDisSetCPs(colliderPosition);
        Debug.Log("colliderPosition" + colliderPosition);
        //ComputeDisSetCPs(colliderPosition);
        //meshCreateControlPoints.GetNewPos();
        //SelectedControlPoints/*meshCreateControlPoints.InitializedControlPoints*/.AddComponent<DetectScaleCollision>();
        foreach (Transform child in meshCreateControlPoints._initializedControlPoints)
        {
            child.gameObject.AddComponent<DetectScaleCollision>();
        }
        voiceControlCommand.text = "Scale";
    }
    /// <summary>
    /// select and store data of gameobject(the control points) function.
    /// </summary>
   


    /// <summary>
    /// delecte and remove gameobject(the control points) function.
    /// </summary>
    

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
        //for (int i = 0; i < meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/.childCount; i++)
        //{
        //    var child = meshCreateControlPoints._initializedControlPoints/*_unselectedControlPoints*/.GetChild(i);
        //    child.transform.parent = null;
        //    child.transform.parent = meshCreateControlPoints._initializedControlPoints;
        //    i--;
        //}
        selectionList.Clear();
    }

    ///set self adopted colliders for the "selected control points"

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

    /// <summary>
    /// delete all the collders in the gameobject "Selected Contol Points"
    /// </summary>
    public void DeleteRecreateSelecObj()
    {        
        Destroy(SelectedControlPoints);
        InitializeSelecObj();
    }
    public void DeleteParentCollider ()
    {
        Collider[] colliderParent = SelectedControlPoints.GetComponents<Collider>();
        for (int i = 0; i < colliderParent.Length; i++)
        { Destroy(colliderParent[i]); }
    }

    public void GetColliderCopy()
    {
        //GetInChildren get all the elements in parent and children
        Collider[] Cols = SelectedControlPoints.GetComponentsInChildren<Collider>();
        Debug.Log("SelectedControlPoints.GetComponentsInChildren<Collider>()  " + Cols.Length);
        SphereCollider[] sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
        Debug.Log("SelectedControlPoints.GetComponents<SphereCollider>()  " + sphereCollider.Length);
        int k = sphereCollider.Length;
        //To eliminate the elements in parent
        for (int j = Cols.Length - SelectedControlPoints.transform.childCount; j < Cols.Length; j++)
        {

            //Debug.Log("the position of the children colliders\t" + col.transform.position);
            CopyComponent(Cols[j], SelectedControlPoints);


            sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();

            //delete the offset of transform(remember when move a gameobject, its transfrom changed)
            sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].center = Cols[j].transform.position - SelectedControlPoints.transform.position;
            //Debug.Log("Cols[j].transform.position  "+ Cols[j].transform.position);
            //Debug.Log("SelectedControlPoints.transform.position  " + SelectedControlPoints.transform.position);
            sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].radius = 0.48f;

        }
    }

    public void ComputeBarCenter(List<Transform> Lst)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        for (int i = 0; i < Lst.Count; i++)
        {
            sum = Lst[i].position + sum;
        }
        barCenter = (sum / Lst.Count);
        Debug.Log("barcenter  "+barCenter);

    }

    /// <summary>
    ///Compute the distaces between barcenter and the control points; set the Selected Control Points' transform to the cintrol points we click; apply the offset to the other control points
    /// </summary>
    public void ComputeDisSetCPs(Vector3 positionTransform)
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
        Vector3 offset = SelectedControlPoints.transform.position - positionTransform/*colliderPosition*//*barCenter*/;
        Debug.Log("colliderPosition in Selected Control point transform " + colliderPosition);
        SelectedControlPoints.transform.position = positionTransform /*colliderPosition*/ /*barCenter*/;

        for (int i = 0; i < SelectedControlPoints.transform.childCount; i++)
        {
            var child = SelectedControlPoints.transform.GetChild(i);
            child.position = child.position + offset;
            //child.transform.localScale= new Vector3(0.05f/disMax, 0.05f/disMax, 0.05f/disMax);
        }


        Debug.Log("disMax" + disMax);
        //SelectedControlPoints.transform.localScale = new Vector3(2 * disMax, 2 * disMax, 2 * disMax);
        //SelectedControlPoints.GetComponent<MeshRenderer>().material= Selected_Transparent;
        SelectedControlPoints.GetComponent<SphereCollider>().radius = 2f;
        ////SelectedControlPoints.GetComponent<SphereCollider>().center = SelectedControlPoints.transform.position;
    }

    void InitializeSelecObj()
    {
        //SelectedControlPoints = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SelectedControlPoints = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //GameObject SelectedControlPoints = new GameObject();
        SelectedControlPoints.transform.localScale = new Vector3(0.11f,0.11f,0.11f);
        var renderer = SelectedControlPoints.GetComponent<MeshRenderer>();
        renderer.material = barMaterial;
        SelectedControlPoints.name = "Selected Control Points";
        SelectedControlPoints.tag = "SelectedParent";
        _selectedControlPoints = SelectedControlPoints.transform;
        //SelectedControlPoints.AddComponent<MoveObjectUpdate>();
        //SelectedControlPoints.GetComponent<SphereCollider>().radius=0.048f;
       

    }

    //void InitializeUnselecObj()
    //{
    //    UnselectedControlPoints =new GameObject();
    //    UnselectedControlPoints.name = "Unselected Control Points";
    //    UnselectedControlPoints.tag = "UnselectedParent";

    //    _unselectedControlPoints = UnselectedControlPoints.transform;
    //    //Collider boxCol = SelectedControlPoints.AddComponent<BoxCollider>();
    //    meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
    //}
}