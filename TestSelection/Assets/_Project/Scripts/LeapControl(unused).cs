using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class LeapControl : MonoBehaviour
{

    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    public InteractionBehaviour intObj;
    float twoFingerDistance = 0.03f;
    public MeshCreateControlPoints meshCreateControlPoints;
    private object hit;

    // Update is called once per frame
    void Update()
    {
        if (rightHandModel.IsTracked)
        {
            Hand rightHand = rightHandModel.GetLeapHand();
            // float twoFingerDistance = 0.07f;
            
            if ((rightHand.Fingers[1].TipPosition - new Vector(0,0,0)).Magnitude < twoFingerDistance)
            {
                Debug.Log("rightHand.Fingers[0].TipPosition" + rightHand.Fingers[0].TipPosition);
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(0.25f, 10, 0.25f);
                obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                obj.AddComponent<Rigidbody>().useGravity = true;
                obj.AddComponent<InteractionBehaviour>();
                
                print("Pinch detected！");
            }
        }

        for (int i = 0; i < meshCreateControlPoints.interactCP.Count; i++)
        {
            meshCreateControlPoints.interactCP[i].OnContactBegin = () =>
            {

                var selection = hit;



                Debug.Log("Leap hand touched with a CP! "+ hit);
            };
        }
    }
}
