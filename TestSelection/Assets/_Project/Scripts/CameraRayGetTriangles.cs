using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets._Project.Scripts.treatment;

public class CameraRayGetTriangles : MonoBehaviour
{
    Camera cam;
    private ReadJson readJson;
    private Functionality functionality;
    private MeshCreateControlPoints meshCreateControlPoints;
    private TreatSelectionManager treatSelectionManager;
    private VoiceController voiceController;
    [HideInInspector] public List<ControlPointsData> myList = new List<ControlPointsData>();
    private bool hitMeshCollider;
    private int flagSegment;
    private List<int> intersectSegmentIndexes = new List<int>();
    private List<int> intersectSegmentIndexesOld = new List<int>();
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material OutlineMaterial2;
    [SerializeField] private Material SelectedMaterial;
    private List<ControlPointsData> controlPointsBySegments = new List<ControlPointsData>();
    private bool match;

    void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        functionality = GameObject.Find("Selection Manager").GetComponent<Functionality>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        treatSelectionManager = GameObject.Find("Selection Manager").GetComponent<TreatSelectionManager>();
        voiceController = GameObject.Find("Selection Manager").GetComponent<VoiceController>();

        cam = GetComponent<Camera>();
        hitMeshCollider = false;
        flagSegment = -1;

        //controlPointsBySegments
        //meshHand = GameObject.Find("hand").GetComponent<MeshCollider>();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, 1000 * Camera.main.transform.forward);

        hitMeshCollider = Physics.Raycast(ray, out hit);

        // if the mesh is not hit
        if (!hitMeshCollider /* && var bool ==true*/)
        {
            //get all Control Points Data///////////////////
            for (int i = 0; i < meshCreateControlPoints.cpDataList.Count; i++)
            {
                var controlPoints = meshCreateControlPoints.cpDataList[i];
                //if the control points are selected, reset default material 
                if (treatSelectionManager.selectionList.Contains(controlPoints.go.transform))
                {
                    controlPoints.go.GetComponent<MeshRenderer>().material = SelectedMaterial ;
                }

                //the not selected CPs

                if (!treatSelectionManager.selectionList.Contains(controlPoints.go.transform))
                {
                    controlPoints.go.GetComponent<MeshRenderer>().material = controlPoints.defautMaterial;
                }
                
                
                //A bool var becomes false here/////////////////////
            }

        }
        // if the mesh is hit
        else
        {
            for (int k = 0; k < meshCreateControlPoints.cpDataList.Count; k++)
            {
                if (!match)
                {
                    Debug.Log("not match");
                    for (int x = 0; x < intersectSegmentIndexesOld.Count; x++)
                    {
                        //find out the control points that has te same tags(belong to a same segment) 
                        var controlPointsOfSegmentsHitOld = meshCreateControlPoints.cpDataList.FindAll(y => y.goTags.Contains(readJson.treeNodeLevelx[intersectSegmentIndexesOld[x]].GetData().tag));

                        if (meshCreateControlPoints.cpDataList[k].goTags.Contains(readJson.treeNodeLevelx[intersectSegmentIndexesOld[x]].GetData().tag))
                        {
                            //if the control point has been already selected
                            if (treatSelectionManager.selectionList.Contains(meshCreateControlPoints.cpDataList[k].go.transform))
                            {

                                foreach (var VARIABLE in controlPointsOfSegmentsHitOld)
                                {
                                    VARIABLE.go.GetComponent<MeshRenderer>().material = SelectedMaterial;
                                }
                            }
                            if (!treatSelectionManager.selectionList.Contains(meshCreateControlPoints.cpDataList[k].go.transform))
                            {
                                foreach (var VARIABLE in controlPointsOfSegmentsHitOld)
                                {
                                    VARIABLE.go.GetComponent<MeshRenderer>().material = meshCreateControlPoints.cpDataList[k].defautMaterial;
                                }
                            }

                        }
                    }
                    intersectSegmentIndexesOld.Clear();
                    for (int i = 0; i < intersectSegmentIndexes.Count; i++)
                    {
                        intersectSegmentIndexesOld.Add(intersectSegmentIndexes[i]);
                    }
                    
                }

            }

            //Debug.Log("intersectSegmentIndexesOld " + intersectSegmentIndexesOld.Count);
            intersectSegmentIndexes.Clear();

            //go through the all segments
            for (int i = 0; i < readJson.treeNodeLevelx.Count; i++)
            {
                //go though all the triangles indexes of a segment
                for (int j = 0; j < readJson.treeNodeLevelx[i].GetData().triangles.Count; j++)
                {
                    //ray hits a segment, save the segment number in a list
                    if (hit.triangleIndex == readJson.treeNodeLevelx[i].GetData().triangles[j])
                    {
                        //intersectSegmentIndexesOld = intersectSegmentIndexes;

                        intersectSegmentIndexes.Add(i);

                        match = CheckMatch(intersectSegmentIndexesOld, intersectSegmentIndexes);
                        Debug.Log("intersectSegmentIndexes " + intersectSegmentIndexes[0]);
                        // high light these segments
                        // go though the control points


                        





                        //go though all the control points instances
                        for (int k = 0; k < meshCreateControlPoints.cpDataList.Count; k++)
                        {
                            //go through all the segments that are hit
                            for (int x = 0; x < intersectSegmentIndexes.Count; x++)
                            {
                                //find out the control points that has te same tags(belong to a same segment) 
                                var controlPointsOfSegmentsHit = meshCreateControlPoints.cpDataList.FindAll(y => y.goTags.Contains(readJson.treeNodeLevelx[intersectSegmentIndexes[x]].GetData().tag));

                                if (meshCreateControlPoints.cpDataList[k].goTags.Contains(readJson.treeNodeLevelx[intersectSegmentIndexes[x]].GetData().tag))
                                {
                                    //if the control point has been already selected
                                    if (treatSelectionManager.selectionList.Contains(meshCreateControlPoints.cpDataList[k].go.transform))
                                    {
                                        foreach (var VARIABLE in controlPointsOfSegmentsHit)
                                        {
                                            VARIABLE.go.GetComponent<MeshRenderer>().material = OutlineMaterial2;
                                        }
                                        
                                    }
                                    if (!treatSelectionManager.selectionList.Contains(meshCreateControlPoints.cpDataList[k].go.transform))
                                    {
                                        foreach (var VARIABLE in controlPointsOfSegmentsHit)
                                        {
                                            VARIABLE.go.GetComponent<MeshRenderer>().material = meshCreateControlPoints.cpDataList[k].outlineMaterial;
                                        }
                                    }

                                }
                            }

                        }

                        

                    }
                    ////ray hits mesh but not a segment on the mesh
                    //if (intersectSegmentIndexes == null)
                    //{
                    //    // a repeating part ///////////////////
                    //    for (int k = 0; k < meshCreateControlPoints.cpDataList.Count; k++)
                    //    {
                    //        var controlPoints = meshCreateControlPoints.cpDataList[k];
                    //        //if the control points are selected, reset default material 
                    //        if (treatSelectionManager.selectionList.Contains(controlPoints.go.transform))
                    //        {
                    //            controlPoints.go.GetComponent<MeshRenderer>().material = controlPoints.defautMaterial;
                    //        }

                    //        //the not selected CPs
                    //        else
                    //        {
                    //            controlPoints.go.GetComponent<MeshRenderer>().material = SelectedMaterial;
                    //        }
                    //        //A bool var becomes false here/////////////////////
                    //    }
                    //}

                }
            }

            
        }




    }

    private bool CheckMatch(List<int> l1, List<int> l2)
    {
        if (l1.Count != l2.Count)
            return false;
        for (int i = 0; i < l1.Count; i++)
        {
            if (l1[i] != l2[i])
                return false;
        }
        return true;
    }

    // Funzione apparentemente non utilizzata!?!
    private void OutlineControPoints(int flagSegment)
    {
        //var myList = meshCreateControlPoints.cpDataListLevels1[k].FindAll(x =>new Color( x.goColor[0].r, x.goColor[0].g , x.goColor[0].b )/255 == new Color(substitut[0], substitut[1], substitut[2])/255 );
        float[] segmentColorSubstitute= readJson.treeNodeLevelx[flagSegment].GetData().color;
        myList = meshCreateControlPoints.cpDataList.FindAll(x => x.goColor[0] == new Color(segmentColorSubstitute[0], segmentColorSubstitute[1], segmentColorSubstitute[2],255)/255/*x.goTags.Contains(readJson.treeNodeLevelx[flagSegment].GetData().tag)*/);

        for (int m = 0; m < myList.Count; m++)
        {
            var controlPointRenderer = myList[m].go.GetComponent<MeshRenderer>();

            for (int j = 0; j < meshCreateControlPoints.materialGroup1.Count; j++)
            {
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color )
                {
                    controlPointRenderer.material = meshCreateControlPoints.outlineMaterialGroup1[j];
                }
                
            }
        }
        voiceController.segmentSelect = false;
    }
    

    private void DefaultControPoints(int flagSegment)
    {
        //var myList = meshCreateControlPoints.cpDataListLevels1[k].FindAll(x =>new Color( x.goColor[0].r, x.goColor[0].g , x.goColor[0].b )/255 == new Color(substitut[0], substitut[1], substitut[2])/255 );
        float[] segmentColorSubstitute = readJson.treeNodeLevelx[flagSegment].GetData().color;
        myList = meshCreateControlPoints.cpDataList.FindAll(x => x.goColor[0] == new Color(segmentColorSubstitute[0], segmentColorSubstitute[1], segmentColorSubstitute[2], 255) / 255);
        for (int m = 0; m < myList.Count; m++)
        {
            var controlPointRenderer = myList[m].go.GetComponent<MeshRenderer>();

            for (int j = 0; j < meshCreateControlPoints.materialGroup1.Count; j++)
            {
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color )
                {
                    
                }
                
            }
        }
        
    }


}