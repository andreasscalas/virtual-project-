using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDragObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    public Transform sphere;
    public float distanceFromCamera;
    Rigidbody r;

    void Start()
    {
        distanceFromCamera = Vector3.Distance(sphere.position,cam.transform.position);
        //gameObject.AddComponent<Rigidbody>();
        r = GetComponent<Rigidbody>();
    }
    Vector3 lastPos;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = distanceFromCamera;
            pos = cam.ScreenToWorldPoint(pos);
            sphere.position = pos;
            //r.velocity = (pos - sphere.position) * 10;
        }
    }
}
