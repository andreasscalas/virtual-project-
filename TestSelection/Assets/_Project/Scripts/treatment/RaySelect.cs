using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class RaySelect : MonoBehaviour
{
    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    //public ControllerColliderHit hit=new ControllerColliderHit();
    float twoFingerDistance = 0.03f;
    public GameObject Obj;


    void Update()
    {

        //if (leftHandModel.IsTracked)
        //{
        //    Hand leftHand = leftHandModel.GetLeapHand();
        //    // float twoFingerDistance = 0.07f;
        //    //if ((leftHand.Fingers[0].TipPosition - leftHand.Fingers[1].TipPosition).Magnitude < twoFingerDistance)
        //    //{
        //    //    //Debug.Log("leftHand.Fingers[0].TipPosition" + leftHand.Fingers[0].TipPosition);
        //    //    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //    obj.transform.position = new Vector3(0.25f, 10, 0.25f);
        //    //    obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        //    //    obj.AddComponent<Rigidbody>().useGravity = true;
        //    //    obj.AddComponent<InteractionBehaviour>();

        //    //    print("Pinch detected！");
        //    //}
        //    Debug.DrawRay(leftHand.Fingers[1].TipPosition.ToVector3(), leftHand.Fingers[1].Direction.ToVector3(), Color.red);
        //    Ray ray = new Ray(leftHand.Fingers[1].TipPosition.ToVector3(), leftHand.Fingers[1].Direction.ToVector3());
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        hit.transform.gameObject.GetComponent<ChangeColor>().SetColor();
        //    }
            
        //}

        //if (rightHandModel.IsTracked)
        //{
        //    Hand rightHand = rightHandModel.GetLeapHand();
        //    Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000*rightHand.Fingers[1].Direction.ToVector3() , Color.green);
        //    Ray ray = new Ray(rightHand.Fingers[1].TipPosition.ToVector3(), rightHand.Fingers[1].Direction.ToVector3());
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        Debug.Log("hit ");
        //        if (hit.transform.name != "ground")
        //        {
        //            Debug.Log("hit.transform.gameObject.GetComponent<ChangeColor>() != null ");
        //            hit.transform.gameObject.GetComponent<ChangeColor>().ResetColor();
        //        }
        //    }

        //}

        CastSelectRay();
    }

    private void CastSelectRay()
    {
        if (rightHandModel.IsTracked)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.red);
            Ray ray = new Ray(rightHand.Fingers[1].TipPosition.ToVector3(), rightHand.Fingers[1].Direction.ToVector3());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("hit ");
                if (hit.transform.name != "ground")
                {
                    Debug.Log("hit.transform.gameObject.GetComponent<ChangeColor>() != null ");
                    hit.transform.gameObject.GetComponent<ChangeColor>().SetColor();
                }
            }

        }
    }


    private void CastDeselectRay()
    {
        if (rightHandModel.IsTracked)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.green);
            Ray ray = new Ray(rightHand.Fingers[1].TipPosition.ToVector3(), rightHand.Fingers[1].Direction.ToVector3());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("hit ");
                if (hit.transform.name != "ground")
                {
                    Debug.Log("hit.transform.gameObject.GetComponent<ChangeColor>() != null ");
                    hit.transform.gameObject.GetComponent<ChangeColor>().ResetColor();
                }
            }

        }
    }


    //void OnTriggerEnter(Collider hit)
    //{
    //    FingerModel finger = hit.GetComponentInParent<FingerModel>();
    //    if (finger)
    //    {
    //        Obj.GetComponent<MeshRenderer>().enabled = false;
    //        Debug.Log("We find out the collision!!");
    //        Debug.Log("display hit " + hit.gameObject.name);
    //    }
    //    //Debug.Log("We find out the collision!!");
    //    //Debug.Log("display hit " + hit.gameObject.name);
    //}

    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    Debug.Log("We find out the collision!!");
    //    Debug.Log("display hit " + hit.gameObject.name);

    //}
}