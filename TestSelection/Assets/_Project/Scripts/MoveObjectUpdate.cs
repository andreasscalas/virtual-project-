using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveObjectUpdate : MonoBehaviour
{
    private Transform transform;
    private bool FreeMovimentState;
    private bool CollisionState;
    private float mZCoord;
    private Vector3 mOffSet;
    private List<Vector3> impossibleDirections;
    private Vector3 previousDragPos;
    private bool isMouseCloseObjet;

    // Use this for initialization
    void Start()
    {
        transform = this.GetComponent<Transform>();
        FreeMovimentState = true;
        impossibleDirections = new List<Vector3>();
    }
        
    // Update is called once per frame
    void Update()
    {
    }


    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffSet = gameObject.transform.position - GetMouseWorldPos();
        previousDragPos = GetMouseWorldPos() + mOffSet;
        isMouseCloseObjet = true;

    }

    void OnMouseDrag()
    {

        if (FreeMovimentState)
        {
            transform.position = GetMouseWorldPos() + mOffSet;
        }
        else if (CollisionState)
        {
            var possibleMoviment = previousDragPos - GetMouseWorldPos() - mOffSet;
            possibleMoviment = Vector3.Normalize(possibleMoviment);
            previousDragPos = GetMouseWorldPos() + mOffSet;


            var distance = Vector3.Distance(transform.position, GetMouseWorldPos() + mOffSet);
            if (distance > 0.3)
            {
                isMouseCloseObjet = false;
            }
            else
            {
                isMouseCloseObjet = true;
            }
            

            var move = false;
            foreach (var direction in impossibleDirections)
            {
                var directionNorm = Vector3.Normalize(direction);
                var isParall = Vector3.Dot(possibleMoviment, directionNorm);

                if (Math.Abs(isParall + 1) < 0.2)
                {
                    move = true;
                    break;
                }
            }
            
            if (move && isMouseCloseObjet)
            {
                transform.position = GetMouseWorldPos() + mOffSet;
            }
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        FreeMovimentState = false;
        CollisionState = true;

        foreach (var contact in collision.contacts)
        {
            impossibleDirections.Add(contact.normal);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        FreeMovimentState = true;
        CollisionState = false;
    }

    void OnCollisionStay(Collision collision)
    {
        CollisionState = true;

    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}