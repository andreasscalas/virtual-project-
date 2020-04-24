using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rotateObj : MonoBehaviour
{
    float rotSpeed = 5;
    private Vector3 startDragPosition;
    private Vector3 previousDragPos;
    private Vector3 currentDragPos;
    private List<Vector3> currentDragPosLst = new List<Vector3>();
    private float mZCoord;
    private Vector3 axis;
    private float angle;
    private Vector3 mOffset;
    private float delta;
    private float alfa;

    void OnMouseDown()
    {
        Debug.Log("mouse is pressed");
        delta = 0.1f;
        //deselect a control point, and put this control point into the unselected gameobject
        if (true/*Input.GetKey(KeyCode.Z)*/)
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Debug.Log("mZCoord" + mZCoord);
            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
            startDragPosition= gameObject.transform.position;
            Debug.Log("startDragPosition" + startDragPosition);
            currentDragPosLst.Add(new Vector3(0,0,0));
            currentDragPosLst.Add(startDragPosition);
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
        Vector3 cameraDirection = Camera.main.transform.forward;
        currentDragPos = (GetMouseAsWorldPoint() + mOffset);
        currentDragPosLst.Add(currentDragPos);
        currentDragPosLst.Remove(currentDragPosLst[0]);
        Debug.Log("dragPosition"+ currentDragPos);

        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        //Debug.Log("dragPosition " + dragPosition);
        //Debug.Log("Input.GetAxis(\"Mouse X\") " + Input.GetAxis("Mouse X"));
        //Debug.Log("angle "+ angle);
        axis = Vector3.Normalize(Vector3.Cross((currentDragPos - startDragPosition), cameraDirection));
        //transform.RotateAround(Vector3.up, -rotX);      
        //transform.RotateAround(Vector3.right, rotY);
 
        //var dragDistance = Vector3.Distance(previousDragPos, currentDragPos);
        var belta = Vector3.Angle(startDragPosition, currentDragPos) * Mathf.Deg2Rad;
        //if (dragDistance > delta)
        //{
        //    alfa = Vector3.Angle(previousDragPos, currentDragPos) * Mathf.Deg2Rad;
        //    previousDragPos = currentDragPos;
        //}
        //transform.Rotate(Vector3.forward, angle*Time.deltaTime, Space.World);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetAngels, 10 * Time.deltaTime);
        //transform.localEulerAngles = new Vector3(30.0f, 60.0f, 90.0f);
        //transform.rotation = Quaternion.Euler(100.0f, 100.0f, 100.0f);
        if (currentDragPosLst[currentDragPosLst.Count-2] != currentDragPos)
        {
            transform.RotateAround(axis, 10 * belta);
        }
    }

}