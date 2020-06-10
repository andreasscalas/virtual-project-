using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class HandRaySelect : MonoBehaviour
{
    //public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    //public ControllerColliderHit hit=new ControllerColliderHit();
    float twoFingerDistance = 0.03f;
    public GameObject Obj;
    private bool select;
    private bool delete;

    void Start()
    {
        select = false;
        delete = false;
    }

    void Update()
    {
        if(select)
        { CastSelectRay(select);}
        if (delete)
        { CastSelectRay(delete); }
    }

    public void OnSelect()
    {
        delete = false;
        select = true;
    }
    public void OnDelete()
    {
        select = false;
        delete = true;
    }

    private void CastSelectRay(bool act)
    {
        if (rightHandModel.IsTracked)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            if (act == select) 
            { Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.red);}
            if (act == delete)
            { Debug.DrawRay(rightHand.Fingers[1].TipPosition.ToVector3(), 1000 * rightHand.Fingers[1].Direction.ToVector3(), Color.green); }

            Ray ray = new Ray(rightHand.Fingers[1].TipPosition.ToVector3(), rightHand.Fingers[1].Direction.ToVector3());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name != "ground")
                {
                    Debug.Log("hit.transform.gameObject.GetComponent<ChangeColor>() != null ");
                    if (select) 
                    { Debug.Log("select");/*hit.transform.gameObject.GetComponent<ChangeColor>().SetColor();*/}
                    if (delete)
                    { Debug.Log("delete");/*hit.transform.gameObject.GetComponent<ChangeColor>().ResetColor();*/ }
                }
            }

        }
    }

}