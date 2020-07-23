using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets._Project.Scripts.treatment;

public class CameraRayGetTriangles : MonoBehaviour
{
    Camera cam;
    private ReadJson readJson;
    public Functionality functionality;
    public MeshCreateControlPoints meshCreateControlPoints;
    public List<ControlPointsData> myList = new List<ControlPointsData>();
    private bool hitMeshCollider;
    private int flagSegment;
    private Transform _selection;
    void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        functionality = GameObject.Find("Selection Manager").GetComponent<Functionality>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();

        cam = GetComponent<Camera>();
        hitMeshCollider = false;
        flagSegment = -1;

    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, 1000 * Camera.main.transform.forward);

       
        if (!Physics.Raycast(ray, out hit))
        {
            
            hitMeshCollider = false;
            Debug.Log("hitMeshCollider:" + hitMeshCollider);
            return ;
        }
        else
        {
            Debug.Log("hit.transform.gameObject.name:" + hit.transform.gameObject.name);
            hitMeshCollider = true;
            Debug.Log("hitMeshCollider:" + hitMeshCollider);
        }

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            hitMeshCollider = false;
            return;
        }
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];


        //for (int k = 0; k < functionality.levels.Count; k++)
        //{
            //if (functionality.levels[k] == true)
            //{

                //Debug.Log("meshCollider.name "+ meshCollider.name);
                //Debug.Log("hit.collider.name " + hit.collider.name);
                //Debug.Log("hit.transform " + hit.transform);
                Debug.Log("hitMeshCollider " + hitMeshCollider);
                if (/*flagSegment >= 0 ||*/ !hitMeshCollider /*|| _selection!=null*/)
                {
                    DefaultControPoints(flagSegment);
                    Debug.Log("CP has been set default material");
                    flagSegment = -1;
                    //_selection = null;
                }

                else
                //if (hitMeshCollider)
                {
                    for (int i = 0; i < readJson.treeNodeLevelx.Count; i++)
                    {

                        if ((readJson.treeNodeLevelx[i].GetData().triangles.Contains(hit.triangleIndex)))
                        {
                            //Debug.Log("flagSegment:" );
                            OutlineControPoints(i);
                            Debug.Log("CP has been set highlight material");
                            flagSegment = i;
                            //_selection = hit.transform;
                        }

                    }
                }
                
            //}
        //}


        //Vector3 p0 = vertices[triangles[hit.triangles * 3 + 0]];
        //Vector3 p1 = vertices[triangles[hit.triangles * 3 + 1]];
        //Vector3 p2 = vertices[triangles[hit.triangles * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);
        //Debug.DrawRay(cam.transform.position, 1000 * Camera.main.transform.forward);
    }

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
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color)
                {
                    controlPointRenderer.material = meshCreateControlPoints.outlineMaterialGroup1[j];
                }
            }
        }
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
                if (myList[m].goColor[0] == meshCreateControlPoints.materialGroup1[j].color)
                {
                    controlPointRenderer.material = meshCreateControlPoints.materialGroup1[j];
                }
            }
        }
    }


}