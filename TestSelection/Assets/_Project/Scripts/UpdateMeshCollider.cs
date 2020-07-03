using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMeshCollider : MonoBehaviour
{

    Mesh myMesh;
    private GameObject objCollider;
    // Start is called before the first frame update
    void Start()
    {
        objCollider = GameObject.Find("hand");
        myMesh = objCollider.GetComponentInChildren<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
        myMesh.RecalculateBounds();

        MeshCollider myMC = objCollider.GetComponent<MeshCollider>();
        myMC.sharedMesh = null;
        myMC.sharedMesh = myMesh;
    }
}
