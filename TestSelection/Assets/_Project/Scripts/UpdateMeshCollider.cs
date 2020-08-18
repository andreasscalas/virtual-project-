using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMeshCollider : MonoBehaviour
{

    Mesh myMesh;
    public GameObject objCollider;
    public DragModel dragModel;
    // Start is called before the first frame update
    void Start()
    {
        //objCollider = GameObject.Find("hand");
        myMesh = objCollider.GetComponentInChildren<MeshFilter>().mesh;
        //dragModel= objCollider.GetComponent<DragModel>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (dragModel.grasping)
        //{
            myMesh.RecalculateBounds();

            MeshCollider myMC = objCollider.GetComponent<MeshCollider>();
            myMC.sharedMesh = null;
            myMC.sharedMesh = myMesh;
        //}
        
    }
}
