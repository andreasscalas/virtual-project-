// This script draws a debug line around mesh triangles
// as you move the mouse over them.
using UnityEngine;
using System.Collections;

public class CameraRayGetTriangles : MonoBehaviour
{
    Camera cam;
    private ReadJson readJson;

    void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(/*Camera.main.transform.forward*/Input.mousePosition), out hit))
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];


        for (int i = 0; i < readJson.AllSegtrianIndexes.Count; i++)
        {
            for (int j = 0; j < readJson.AllSegtrianIndexes[i].Count; j++)
            {
                if ((readJson.AllSegtrianIndexes[i][j]-1) == hit.triangleIndex)
                {
                    Debug.Log("the i-th group that is being hit "+ i);
                    //Debug.Log("the position in the segment "+ j);
                    //Debug.Log("hit.triangleIndex " + hit.triangleIndex);
                }
            }
        }

        //Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        //Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        //Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);
        //Debug.DrawRay(cam.transform.position, 1000 * Camera.main.transform.forward);
    }
}