using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Scripts.treatment;

public class CameraRayGetTrianglesOrigin : MonoBehaviour
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
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material OutlineMaterial2;
    [SerializeField] private Material SelectedMaterial;


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
        //meshHand = GameObject.Find("hand").GetComponent<MeshCollider>();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, 1000 * Camera.main.transform.forward);

        hitMeshCollider = Physics.Raycast(ray, out hit);


        
        if (/*flagSegment >= 0 */intersectSegmentIndexes!=null)
        {
            foreach (var VARIABLE in intersectSegmentIndexes)
            {
                //if selection list not contains the CPs, we default(variable)!!!!!!!!!!
                DefaultControPoints(VARIABLE);

            }

            //DefaultControPoints(flagSegment);
            //    Debug.Log("CP has been set default material");
            //intersectSegmentIndexes = null;
            intersectSegmentIndexes.Clear();

        }


        if (hitMeshCollider)
        {
            for (int i = 0; i < readJson.treeNodeLevelx.Count; i++)
            {
                for (int j = 0; j < readJson.treeNodeLevelx[i].GetData().triangles.Count; j++)
                {
                    if ((readJson.treeNodeLevelx[i].GetData().triangles[j]==hit.triangleIndex))
                    {
                        //Debug.Log("flagSegment:" );
                        intersectSegmentIndexes.Add(i);
                        foreach (var VARIABLE in intersectSegmentIndexes)
                        {
                            OutlineControPoints(VARIABLE);
                        }
                        //flagSegment = i;
                        
                    }
                }
                
            }
            
        }


    }

    private void OutlineControPoints(int flagSegment)
    {
        //var myList = meshCreateControlPoints.cpDataListLevels1[k].FindAll(x =>new Color( x.goColor[0].r, x.goColor[0].g , x.goColor[0].b )/255 == new Color(substitut[0], substitut[1], substitut[2])/255 );
        //float[] segmentColorSubstitute= readJson.treeNodeLevelx[flagSegment].GetData().color;
        //myList = meshCreateControlPoints.cpDataList.FindAll(x => x.goColor[0] == new Color(segmentColorSubstitute[0], segmentColorSubstitute[1], segmentColorSubstitute[2],255)/255/*x.goTags.Contains(readJson.treeNodeLevelx[flagSegment].GetData().tag)*/);
        //find out the control points instances that belong to s segment of levelx.
        List<int> segmentVertexSubstitute = readJson.treeNodeLevelx[flagSegment].GetData().cageVerticesIndex;
        myList = meshCreateControlPoints.cpDataList.FindAll(x => segmentVertexSubstitute.Contains(x.goIndex));



        for (int m = 0; m < myList.Count; m++)
        {
            var controlPointRenderer = myList[m].go.GetComponent<MeshRenderer>();

            for (int j = 0; j < meshCreateControlPoints.materialGroup1.Count; j++)
            {
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color && !treatSelectionManager.selectionList.Contains(myList[m].go.transform))
                {
                    controlPointRenderer.material = meshCreateControlPoints.outlineMaterialGroup1[j];
                    if (voiceController.segmentSelect)
                    {
                        treatSelectionManager.selectionList.Add(myList[m].go.transform);
                        myList[m].go.GetComponent<MeshRenderer>().material = OutlineMaterial2;
                        myList[m].go.transform.parent = null;
                        myList[m].go.transform.parent = treatSelectionManager._selectedControlPoints;
                    }
                }

                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color && treatSelectionManager.selectionList.Contains(myList[m].go.transform))
                {
                    controlPointRenderer.material = OutlineMaterial2;
                }



            }
        }
        voiceController.segmentSelect = false;
    }


    private void DefaultControPoints(int flagSegment)
    {
        //var myList = meshCreateControlPoints.cpDataListLevels1[k].FindAll(x =>new Color( x.goColor[0].r, x.goColor[0].g , x.goColor[0].b )/255 == new Color(substitut[0], substitut[1], substitut[2])/255 );
        //float[] segmentColorSubstitute = readJson.treeNodeLevelx[flagSegment].GetData().color;
        //myList = meshCreateControlPoints.cpDataList.FindAll(x => x.goColor[0] == new Color(segmentColorSubstitute[0], segmentColorSubstitute[1], segmentColorSubstitute[2], 255) / 255);
        //find out the control points instances that belong to s segment of levelx.
        List<int> segmentVertexSubstitute = readJson.treeNodeLevelx[flagSegment].GetData().cageVerticesIndex;
        myList = meshCreateControlPoints.cpDataList.FindAll(x => segmentVertexSubstitute.Contains(x.goIndex));

        for (int m = 0; m < myList.Count; m++)
        {
            var controlPointRenderer = myList[m].go.GetComponent<MeshRenderer>();

            for (int j = 0; j < meshCreateControlPoints.materialGroup1.Count; j++)
            {
                //if the color of the segments is the same as some colors in the materials group(so they belong to a same group)
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color && !treatSelectionManager.selectionList.Contains(myList[m].go.transform))
                {
                    controlPointRenderer.material = meshCreateControlPoints.materialGroup1[j];
                    //if (voiceController.segmentDelete)
                    //{
                    //    treatSelectionManager.selectionList.Remove(myList[m].go.transform);
                    //    myList[m].go.transform.parent = null;
                    //    myList[m].go.transform.parent = meshCreateControlPoints._initializedControlPoints;
                    //    myList[m].go.GetComponent<MeshRenderer>().material = meshCreateControlPoints.materialGroup1[j];
                    //}
                }


                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color && treatSelectionManager.selectionList.Contains(myList[m].go.transform))
                {
                    controlPointRenderer.material = SelectedMaterial;
                    if (voiceController.segmentDelete)
                    {
                        treatSelectionManager.selectionList.Remove(myList[m].go.transform);
                        myList[m].go.transform.parent = null;
                        myList[m].go.transform.parent = meshCreateControlPoints._initializedControlPoints;
                        myList[m].go.GetComponent<MeshRenderer>().material = meshCreateControlPoints.materialGroup1[j];
                    }
                }

            }
        }
        voiceController.segmentDelete = false;
    }


}