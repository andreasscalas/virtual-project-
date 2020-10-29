using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.UI;


public class TreatSelectionManager : MonoBehaviour
{



    public bool highlightingActivated;
    private Transform _outline;

    [HideInInspector] public Transform _selectedControlPoints;


    private Vector3 barCenter;
    [SerializeField] private Material barMaterial;

    private Vector3 colliderPosition; //can be useful to know which control point is hit
    [SerializeField] private Material defaultMaterial;
    [HideInInspector] public bool delete;
    private RaycastHit hit;

    public MeshCreateControlPoints meshCreateControlPoints;

    [HideInInspector] public GameObject obj;

    public Material OutlineMaterial1;
    public Material OutlineMaterial2;
    public Material highlightMaterial;

    private readonly List<Material> outlineMaterialGroup = new List<Material>();

    [HideInInspector] public bool select;
    [SerializeField] private readonly string selectableTag = "Selectable";

    [SerializeField] private Material Selected_Transparent;

    //public Transform _unselectedControlPoints;
    //[HideInInspector]
    public GameObject SelectedControlPoints;
    [SerializeField] private readonly string selectedParentTag = "SelectedParent";
    public List<Transform> selectionList = new List<Transform>();
    [SerializeField] private string unselectedParentTag = "UnselectedParent";

    public Text voiceControlCommand;

    /*****************Andreas Editing****************/
    public List<int> highlightedAnnotations = new List<int>();
    public GameObject modelGameObject;
    public GameObject cageGameObject;
    [HideInInspector] public bool segmentDelete;
    [HideInInspector] public bool segmentSelect;
    /************************************************/

    private LineRenderer lineRenderer;
    //public Text objectselected;
    //public Text objectstored;
    //public Text objectdeleted;
    //public Text objectremoved;
    private void Start()
    {
        //load material for the outline material
        for (var i = 0; i < meshCreateControlPoints.listTagsGroupedByIndex.Count; i++)
            outlineMaterialGroup.Add(Resources.Load("Outlined Material Group" + i, typeof(Material)) as Material);

        InitializeSelecObj();
        Bounds bb = modelGameObject.GetComponent<Collider>().bounds;
        float bbDiagonalLength = (bb.min - bb.max).magnitude;
        float bbLengthRatio = 0.05f;
        OutlineMaterial2.SetFloat("_Outline", bbLengthRatio * bbDiagonalLength);
        //InitializeUnselecObj();
        highlightingActivated = true;
        select = false;
        delete = false;

        var go = new GameObject();
        lineRenderer = go.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (_outline != null && !selectionList.Contains(hit.transform))
        {
            for (var i = 0; i < meshCreateControlPoints.cpDataList.Count; i++)
                if (_outline == meshCreateControlPoints.cpDataList[i].go.transform)
                    hit.transform.gameObject.GetComponent<MeshRenderer>().material =
                        meshCreateControlPoints.cpDataList[i].defautMaterial;
            _outline = null;
        }

        if (_outline != null && selectionList.Contains(hit.transform))
        {
            var selectionRenderer = _outline.GetComponent<Renderer>();
            selectionRenderer.material = highlightMaterial;
            _outline = null;
            //Debug.Log("treatselection manager _outline 1");
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
            voiceControlCommand.text = "discard";
        }
    }

    public void OnSelectSegment()
    {
        segmentSelect = true;
        voiceControlCommand.text = "Select (a set of) CPs";
    }


    public void OnDeleteSegment()
    {
        segmentDelete = true;
        voiceControlCommand.text = "Discard (a set of) CPs";
    }

    public void onActivateHighlighting()
    {
        highlightingActivated = true;
        voiceControlCommand.text = "Highlighting active";
    }



    public void onStopHighlighting()
    {
        highlightingActivated = false;
        voiceControlCommand.text = "Highlighting deactivated";
    }


    public void onHideCage()
    {
        cageGameObject.SetActive(false);
        voiceControlCommand.text = "Cage hidden";
    }

    public void onDisplayCage()
    {
        cageGameObject.SetActive(true);
        voiceControlCommand.text = "Cage displayed";
    }

    private void CastSelectRay(bool act)
    {
        if (true /*rightHandModel.IsTracked*/)
        {
            //Debug.DrawRay(Camera.main.transform.position, 1000 * Camera.main.transform.forward, Color.green);
			clearHighlighting();
            var ray = new Ray(Camera.main.transform.position, 1000 * Camera.main.transform.forward);
            //lineRenderer.SetPosition(0, Camera.main.transform.position);
            //lineRenderer.SetPosition(1, Camera.main.transform.position + 1000 * Camera.main.transform.forward );

            if(Physics.Raycast(ray, out hit))
            {
                var selection = hit.transform;

                if (hit.transform.tag == "Selectable")
                {
                    //outline the gameobject
                    if (!selectionList.Contains(hit.transform))
                        //hit.transform.gameObject.GetComponent<MeshRenderer>().material = outlineMaterialGroup[0]/*OutlineMaterial1*/;
                        for (var i = 0; i < meshCreateControlPoints.cpDataList.Count; i++)
                            if (hit.transform.gameObject == meshCreateControlPoints.cpDataList[i].go)
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material =
                                    meshCreateControlPoints.cpDataList[i].outlineMaterial;

                    if (selectionList.Contains(hit.transform) && highlightingActivated)
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = OutlineMaterial2;

                    if (select)
                    {
                        Debug.Log(hit.transform.name);
                        if (selection.CompareTag(selectableTag))
                        {
                            obj = selection.gameObject;
                            var selectionRenderer = selection.GetComponent<Renderer>();
                            obj.transform.parent = null;
                            obj.transform.parent = _selectedControlPoints;
                            //Debug.Log(obj + " gameobject is selected");

                            if (selectionRenderer != null && highlightingActivated) selectionRenderer.material = OutlineMaterial2;
                            else if(selectionRenderer != null) selectionRenderer.material = highlightMaterial;
                            if (selectionList.Contains(selection) == false)
                                selectionList.Add(selection);
                            //Debug.Log(obj + " gameobject is stored");
                            //objectstored.text = obj+"is stored";
                            //CopyComponent(SelectedControlPoints.GetComponentInChildren<Collider>(), SelectedControlPoints);
                            Debug.Log(obj.transform.position);
                            colliderPosition = hit.collider.transform.position;
                        }

                        if (selection.CompareTag(selectedParentTag))
                        {
                            colliderPosition = hit.collider.transform.position;
                            Debug.Log("collider Position in selection function " + colliderPosition);
                        }

                        Destroy(selection.GetComponent<InteractionBehaviour>());
                        //selected = true;
                    }

                    if (delete)
                    {
                        Debug.Log("delete hit"); /*hit.transform.gameObject.GetComponent<ChangeColor>().ResetColor();*/
                        DeleteParentCollider();
                        //if rigidbody remains, we can not remove control points from Selected Control Points to other gameobject
                        var r = SelectedControlPoints.GetComponent<Rigidbody>();
                        Destroy(r);

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (selection.CompareTag(selectedParentTag))
                            {
                                obj = selection.gameObject;
                                //get the ray hit position 
                                var position = hit.collider.transform.position;
                                for (var i = 0; i < selection.childCount; i++)
                                {
                                    var child = selection.GetChild(i);
                                    if (Vector3.Distance(position, child.transform.position) < 0.1)
                                    {
                                        var selectionRenderer = child.GetComponent<Renderer>();
                                        selectionRenderer.material = defaultMaterial;
                                        Debug.Log(obj + " is deleted");
                                        //objectdeleted.text = obj + "is deleted";
                                        child.transform.parent = null;
                                        child.transform.parent =
                                            meshCreateControlPoints
                                                ._initializedControlPoints /*_unselectedControlPoints*/;
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
                                    obj.transform.parent =
                                        meshCreateControlPoints._initializedControlPoints /*_unselectedControlPoints*/;
                                    selectionList.Remove(selection);
                                    Debug.Log(obj + " is removed ");
                                    obj.AddComponent<InteractionBehaviour>();
                                    //objectremoved.text = obj + "is removed";
                                }
                            }
                        }

                    }

                    _outline = selection;

                } else if (hit.transform.name.Equals(modelGameObject.name) && highlightingActivated)
                {
                    for (int i = 0; i < meshCreateControlPoints.readJson.treeNodeLevelx.Count; i++)
                    {
                        for (int j = 0; j < meshCreateControlPoints.readJson.treeNodeLevelx[i].GetData().triangles.Count; j++)
                        {
                            if ((meshCreateControlPoints.readJson.treeNodeLevelx[i].GetData().triangles[j] == hit.triangleIndex))
                            {
                                highlightedAnnotations.Add(i);
                                OutlineControPoints(i);
                                break;
                            }
                        }

                    }
                }

                select = false;
                delete = false;
                segmentSelect = false;
                segmentDelete = false;
            }
        }
    }

    private void OutlineControPoints(int flagSegment)
    {
        //var myList = meshCreateControlPoints.cpDataListLevels1[k].FindAll(x =>new Color( x.goColor[0].r, x.goColor[0].g , x.goColor[0].b )/255 == new Color(substitut[0], substitut[1], substitut[2])/255 );
        //float[] segmentColorSubstitute= readJson.treeNodeLevelx[flagSegment].GetData().color;
        //myList = meshCreateControlPoints.cpDataList.FindAll(x => x.goColor[0] == new Color(segmentColorSubstitute[0], segmentColorSubstitute[1], segmentColorSubstitute[2],255)/255/*x.goTags.Contains(readJson.treeNodeLevelx[flagSegment].GetData().tag)*/);
        //find out the control points instances that belong to s segment of levelx.
        List<int> segmentVertexSubstitute = meshCreateControlPoints.readJson.treeNodeLevelx[flagSegment].GetData().cageVerticesIndex;
        var myList = meshCreateControlPoints.cpDataList.FindAll(x => segmentVertexSubstitute.Contains(x.goIndex));


        //Debug.Log("Sto per cambiare colore a " + myList.Count + " CPs");
        for (int m = 0; m < myList.Count; m++)
        {
            var controlPointRenderer = myList[m].go.GetComponent<MeshRenderer>();
            if (segmentSelect)
            {
                selectionList.Add(myList[m].go.transform);
                if(highlightingActivated)
                    myList[m].go.GetComponent<MeshRenderer>().material = OutlineMaterial2;
                else
                    myList[m].go.GetComponent<MeshRenderer>().material = highlightMaterial;
                myList[m].go.transform.parent = null;
                myList[m].go.transform.parent = _selectedControlPoints;
            } else if (segmentDelete)
            {
                selectionList.Remove(myList[m].go.transform);
                myList[m].go.GetComponent<MeshRenderer>().material = myList[m].defautMaterial;
                myList[m].go.transform.parent = null;
                myList[m].go.transform.parent = meshCreateControlPoints._initializedControlPoints;
            } else if (selectionList.Contains(myList[m].go.transform)) 
                if(highlightingActivated)
                    controlPointRenderer.material = OutlineMaterial2;
                else 
                    controlPointRenderer.material = highlightMaterial;
            else if(!highlightingActivated)
                controlPointRenderer.material = myList[m].defautMaterial;
            else
                controlPointRenderer.material = myList[m].outlineMaterial;
        }
    }


    public void clearHighlighting()
    {

        for (int i = 0; i < highlightedAnnotations.Count; i++)
        {

            List<int> associatedCageVertices = meshCreateControlPoints.readJson.treeNodeLevelx[highlightedAnnotations[i]].GetData().cageVerticesIndex;
            var associatedControlPointsData = meshCreateControlPoints.cpDataList.FindAll(x => associatedCageVertices.Contains(x.goIndex));
            foreach (var cpData in associatedControlPointsData)
            {
                if (selectionList.Contains(cpData.go.transform) == false)
                    cpData.go.GetComponent<MeshRenderer>().material = cpData.defautMaterial;
                else
                    cpData.go.GetComponent<MeshRenderer>().material = highlightMaterial;
            }


        }
        highlightedAnnotations.Clear();
    }
    // translation, rotation and scaling for mouse version

    //public void Rotation()
    //{
    //    Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
    //    Destroy(SelectedControlPoints.GetComponent<rotateObj>());
    //    ComputeBarCenter(selectionList);
    //    //DeleteParentCollider();
    //    ComputeDisSetCPs(barCenter);
    //    //GetColliderCopy();
    //    SelectedControlPoints.AddComponent<rotateObj>();
    //    voiceControlCommand.text = "Rotate";
    //}
    //public void Translation()
    //{
    //    Destroy(SelectedControlPoints.GetComponent<rotateObj>());
    //    Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
    //    Destroy(SelectedControlPoints.GetComponent<InteractionBehaviour>());
    //    ComputeBarCenter(selectionList);
    //    //DeleteParentCollider();
    //    ComputeDisSetCPs(colliderPosition);
    //    //GetColliderCopy();

    //    SelectedControlPoints.AddComponent<MoveObjectUpdate>();
    //    interactionSCPs = SelectedControlPoints.AddComponent<InteractionBehaviour>();
    //    interactionSCPs.OnContactBegin = () => { Debug.Log("hand touch SelecCPs..."); };

    //    //if (m_MyEvent == null)
    //    //    m_MyEvent = new UnityEvent();
    //    //m_MyEvent.AddListener(Ping);

    //    //SelectedControlPoints.GetComponent<InteractionBehaviour>();
    //    voiceControlCommand.text = "Translate";
    //}


    //public void Scale()
    //{
    //    Destroy(SelectedControlPoints.GetComponent<rotateObj>());
    //    Destroy(SelectedControlPoints.GetComponent<MoveObjectUpdate>());
    //    //DeleteParentCollider();
    //    meshCreateControlPoints.scaleCenter = colliderPosition;
    //    ComputeDisSetCPs(colliderPosition);
    //    Debug.Log("colliderPosition" + colliderPosition);
    //    //ComputeDisSetCPs(colliderPosition);
    //    //meshCreateControlPoints.GetNewPos();
    //    //SelectedControlPoints/*meshCreateControlPoints.InitializedControlPoints*/.AddComponent<DetectScaleCollision>();
    //    foreach (Transform child in meshCreateControlPoints._initializedControlPoints)
    //    {
    //        child.gameObject.AddComponent<DetectScaleCollision>();
    //    }
    //    voiceControlCommand.text = "Scale";
    //}


    public void ClearControlPoints()
    {
        ////Reset the color for the selected control points and put them into the initialized Control points parent
        for (var i = 0; i < _selectedControlPoints.childCount; i++)
        {
            var child = _selectedControlPoints.GetChild(i);
            var selectionRenderer = child.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            child.transform.parent = null;
            child.transform.parent = meshCreateControlPoints._initializedControlPoints;
            i--;
        }

        selectionList.Clear();
    }

    ///set self adopted colliders for the "selected control points"
    private Component CopyComponent(Component original, GameObject destination)
    {
        var type = original.GetType();
        var copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        var fields = type.GetFields();
        foreach (var field in fields) field.SetValue(copy, field.GetValue(original));

        return copy;
    }

    public void DeleteParentCollider()
    {
        var colliderParent = SelectedControlPoints.GetComponents<Collider>();
        for (var i = 0; i < colliderParent.Length; i++) Destroy(colliderParent[i]);
    }

    public void GetColliderCopy()
    {
        //GetInChildren get all the elements in parent and children
        var Cols = SelectedControlPoints.GetComponentsInChildren<Collider>();
        Debug.Log("SelectedControlPoints.GetComponentsInChildren<Collider>()  " + Cols.Length);
        var sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();
        Debug.Log("SelectedControlPoints.GetComponents<SphereCollider>()  " + sphereCollider.Length);
        var k = sphereCollider.Length;
        //To eliminate the elements in parent
        for (var j = Cols.Length - SelectedControlPoints.transform.childCount; j < Cols.Length; j++)
        {
            CopyComponent(Cols[j], SelectedControlPoints);
            sphereCollider = SelectedControlPoints.GetComponents<SphereCollider>();

            //delete the offset of transform(remember when move a gameobject, its transfrom changed)
            sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].center =
                Cols[j].transform.position - SelectedControlPoints.transform.position;
            sphereCollider[j - (Cols.Length - SelectedControlPoints.transform.childCount) + k].radius = 0.48f;
        }
    }

    public void ComputeBarCenter(List<Transform> Lst)
    {
        var sum = new Vector3(0, 0, 0);
        for (var i = 0; i < Lst.Count; i++) sum = Lst[i].position + sum;
        barCenter = sum / Lst.Count;
        Debug.Log("barcenter  " + barCenter);
    }

    /// <summary>
    ///     Compute the distances between barcenter and the control points; set the Selected Control Points' transform to the
    ///     control points we click; apply the offset to the other control points
    /// </summary>
    public void ComputeDisSetCPs(Vector3 positionTransform)
    {
        float disMax = 0;
        for (var i = 0; i < selectionList.Count; i++)
        {
            var dis = Vector3.Distance(barCenter, selectionList[i].position);
            if (disMax < dis) disMax = dis;
        }

        //set the transform of the Selected Control Points to the barycentric point, compensate the offset to its children. 
        var offset = SelectedControlPoints.transform.position - positionTransform /*colliderPosition*/ /*barCenter*/;
        Debug.Log("colliderPosition in Selected Control point transform " + colliderPosition);
        SelectedControlPoints.transform.position = positionTransform /*colliderPosition*/ /*barCenter*/;

        for (var i = 0; i < SelectedControlPoints.transform.childCount; i++)
        {
            var child = SelectedControlPoints.transform.GetChild(i);
            child.position = child.position + offset;
        }


        Debug.Log("disMax" + disMax);
    }

    private void InitializeSelecObj()
    {
        SelectedControlPoints = new GameObject();
        SelectedControlPoints.name = "Selected Control Points";
        SelectedControlPoints.tag = "SelectedParent";

        _selectedControlPoints = SelectedControlPoints.transform;
    }
}