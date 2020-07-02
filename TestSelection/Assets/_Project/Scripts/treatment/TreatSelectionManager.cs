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
    [SerializeField] private Material OutlineMaterial1;
    [SerializeField] private Material OutlineMaterial2;
    [SerializeField] private Material barMaterial;
    [SerializeField] private Material Selected_Transparent;

    [HideInInspector]
    public GameObject obj;
    public List<Transform> selectionList = new List<Transform>();
    private List<Transform> interSelectionList = new List<Transform>();

    private Vector3 mousePos1;
    private Vector3 mousePos2;
    private Vector3 colliderPosition;

    public Transform _selectedControlPoints;
    //public Transform _unselectedControlPoints;
    //[HideInInspector]
    public GameObject SelectedControlPoints;
    //[HideInInspector]

    private Vector3 barCenter = new Vector3();


    public MeshCreateControlPoints meshCreateControlPoints;
    private InteractionBehaviour interactionSCPs = new InteractionBehaviour();

    public Text voiceControlCommand;

    public HandModelBase rightHandModel;
    private bool select;
    private bool delete;
    private bool rayhitted;
    //private bool selected;
    RaycastHit hit;


    private Transform _outline;

    private List<Material> outlineMaterialGroup=new List<Material>();
    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    void Start()
    {
        //load material for the outline material
        for (int i = 0; i < meshCreateControlPoints.listTagsGroupedByIndex.Count; i++)
        {
            outlineMaterialGroup.Add(Resources.Load("Outlined Material Group" + i, typeof(Material)) as Material);
        }

        InitializeSelecObj();
        //InitializeUnselecObj();
        select = false;
        delete = false;
        rayhitted = false;
    }

    private void Update()
    {
        
        if (_outline != null && !selectionList.Contains(hit.transform))
        {
            var selectionRenderer = _outline.GetComponent<Renderer>();

            for (int i = 0; i < meshCreateControlPoints.cpDataList.Count; i++)
            {
                if (_outline == meshCreateControlPoints.cpDataList[i].go.transform)
                {
                    hit.transform.gameObject.GetComponent<MeshRenderer>().material = meshCreateControlPoints.cpDataList[i].defautMaterial;
                }
            }
            //selectionRenderer.material = defaultMaterial;
            _outline = null;
        }

        if (_outline != null && selectionList.Contains(hit.transform))
        {
            var selectionRenderer = _outline.GetComponent<Renderer>();
            selectionRenderer.material = highlightMaterial;
            _outline = null;
        }

        CastSelectRay(select);
        CastSelectRay(delete);
    }


    public void OnSelect()
    {
        if (hit.transform.tag == "Selectable")
        {
            delete = false;
            select = true;
            voiceControlCommand.text = "Select";
        }
    }
    public void OnDelete()
    {
        if (hit.transform.tag == "Selectable")
        {
            select = false;
            delete = true;
            voiceControlCommand.text = "Delete";
        }
    }


    private void CastSelectRay(bool act)
    {
        if (true/*rightHandModel.IsTracked*/)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            Debug.DrawRay(Camera.main.transform.position, 1000 * Camera.main.transform.forward, Color.green);
            
            Ray ray = new Ray(Camera.main.transform.position, 1000 * Camera.main.transform.forward);

            if (Physics.Raycast(ray, out hit))
            {
                rayhitted = true;
                var selection = hit.transform;
                //Debug.Log("this is a string in once hit happens");
                if (hit.transform.tag == "Selectable")
                {
                    //outline the gameobject
                    if (!selectionList.Contains(hit.transform))
                    {
                        //hit.transform.gameObject.GetComponent<MeshRenderer>().material = outlineMaterialGroup[0]/*OutlineMaterial1*/;
                        for (int i = 0; i < meshCreateControlPoints.cpDataList.Count; i++)
                        {
                            if (hit.transform.gameObject == meshCreateControlPoints.cpDataList[i].go)
                            {
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = meshCreateControlPoints.cpDataList[i].outlineMaterial;

                            }
                        }
                    }

                    if (selectionList.Contains(hit.transform))
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = OutlineMaterial2;
                    }

                    if (select)
                    {
                        Debug.Log("select hit");/*hit.transform.gameObject.GetComponent<ChangeColor>().SetColor();*/
                        Debug.Log(hit.transform.name);
                        if (selection.CompareTag(selectableTag))
                        {
                            obj = selection.gameObject;
                            var selectionRenderer = selection.GetComponent<Renderer>();
                            obj.transform.parent = null;
                            obj.transform.parent = _selectedControlPoints;
                            //Debug.Log(obj + " gameobject is selected");

                            if (selectionRenderer != null)
                            {
                                selectionRenderer.material = OutlineMaterial2;
                            }
                            if (selectionList.Contains(selection) == false)
                            {
                                selectionList.Add(selection);
                                //Debug.Log(obj + " gameobject is stored");
                                //objectstored.text = obj+"is stored";
                                //CopyComponent(SelectedControlPoints.GetComponentInChildren<Collider>(), SelectedControlPoints);
                            }
                            Debug.Log(obj.transform.position);
                            colliderPosition = hit.collider.transform.position;
                        }

                        if (selection.CompareTag(selectedParentTag))
                        {
                            colliderPosition = hit.collider.transform.position;
                            Debug.Log("collider Position in selection function " + colliderPosition);
                        }

                        Destroy(selection.GetComponent<InteractionBehaviour>());
                        select = false;
                        //selected = true;
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
                        delete = false;
                    }
                    _outline = selection;
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
        voiceControlCommand.text = "Rotate";
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
        voiceControlCommand.text = "Translate";
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