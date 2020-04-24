using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    Rigidbody r;
    public float speed;
    //public Transform transform;
    private bool state;
    private Vector3 position;
    private List<Vector3> _resetPosition= new List<Vector3>();
    private Vector3 resetPosition;

    private void Start()
    {
        
        InitializeDrag();
    }

    void Update()
    {
        //if (r == null && rigidbogyState)
        //{
        //    InitializeDrag();
        //}
    }


    void OnMouseDown()
    {
        Debug.Log("mouse is pressed");

        //deselect a control point, and put this control point into the unselected gameobject
        if (true/*Input.GetKey(KeyCode.Z)*/)
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Debug.Log("mZCoord"+ mZCoord);
            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
            state = true;
            _resetPosition.Clear();
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
 
    void OnMouseDrag()
    {
        if (/*Input.GetKey(KeyCode.Z) &&*/ state)
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Debug.Log("mZCoord" + mZCoord);
            //mOffset = new Vector3(0,0,0);
            //transform.position = GetMouseAsWorldPoint() + mOffset;

            position = (GetMouseAsWorldPoint() + mOffset);
            Debug.Log("position"+ position);
            _resetPosition.Add(position);
            //transform.Translate(new Vector3(position.x, position.y, position.z) * speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            //Debug.Log("transform.position/resetPosition"+ transform.position);

        }
    }


    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(string.Format("Collision of {0} with {1}", this.name, collision.gameObject.name));
            r.velocity = Vector3.zero;
            state = false;
        }

        resetPosition = _resetPosition[_resetPosition.Count-3];
        transform.position = resetPosition;
    }

    private void InitializeDrag()
    {
        gameObject.AddComponent<Rigidbody>();
        r = gameObject.GetComponent<Rigidbody>();
        r.useGravity = false;
        r.isKinematic = true;
        r.drag = 1;
        r.constraints = RigidbodyConstraints.FreezeRotation;
        state = true; // if the gameobject does not collide with others
        speed = 20;
    }

}