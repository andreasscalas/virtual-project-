using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Assets.Scripts.Actions;
//using Assets.Scripts.Phases;
using Assets.Scripts.Support;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
//using Collider = Assets.Scripts.Support.Collider;

public class Translate : MonoBehaviour {

    public HandModelBase HandModel_R;
    private GameObject selectedObject;
    private Vector3 indexPosition;
    private Vector3 lastIndexPosition;
    private bool _extendedIndexActive;
    private Vector3 scaleParents;
    public ExtendedFingerDetector extendedFingerDetectorR;
    private float time;

    //Sarà da mettere globale in modo che valga per tutte le gesture (nello script che gestisce la scena principale)!
    private float gestureWaitingTime;

    void Start()
    {
        _extendedIndexActive = false;
        gestureWaitingTime = 0.0f;
        time = 0.0f;
    }

    void Update()
    {

        //selectedObject = GameObject.FindWithTag("Selected");
        selectedObject = GameObject.Find("Selected Control Points");


        if (selectedObject != null)
        {
            //Multiply scales of all parents of the selectedTargetObject
            scaleParents = selectedObject.transform.localScale;
            Transform tranformParent = selectedObject.transform.parent;
            while (tranformParent != null)
            {
                scaleParents = Vector3.Scale(scaleParents, tranformParent.localScale);
                tranformParent = tranformParent.parent;
            }


            /// When index finger is extended move the selectedObject following
            ///  the finger movement

            if (extendedFingerDetectorR != null && extendedFingerDetectorR.IsActive)
            {
                if (time >= gestureWaitingTime)
                {

                    /// When the gesture is avtive disable the scripts for the hand menu appearance to disambiguate the functionalities.
                    //GameObject.Find("Attachment Hands").GetComponent<HandMenuAppearance>().enabled = false;
                    //GameObject.Find("CenterEyeAnchor").GetComponent<Gaze>().enabled = false;

                    //selectedObject.gameObject.GetComponent<InteractionBehaviour>().ignoreGrasping = true;

                    //Store index position
                    indexPosition = HandModel_R.GetLeapHand().Fingers[1].TipPosition.ToVector3();

                    /// If it is the first frame in which there is the extended index (again) set "lastIndexPosition"
                    /// equals to "indexPosition" in order to have, at first, deltaIndexPosition=0 and not to move the 
                    /// object at the first time according to the last index position.
                    if (_extendedIndexActive == false)
                    {
                        var assemblies = GameObject.Find("Assemblies");
                        var soundPath = @"Audio/activation";
                        //FeedBack.AudioPlay(assemblies, soundPath);

                        lastIndexPosition = indexPosition;

                        // Save in a list the selectedObject and its transform before each transformation ( for the UNDO operation)
                        var position = new Vector3();
                        var rotation = new Quaternion();
                        var scale = new Vector3();

                        position = selectedObject.transform.localPosition;
                        scale = selectedObject.transform.localScale;
                        rotation = selectedObject.transform.localRotation;

                        TransformGameObject transformSelectedObject = new TransformGameObject(selectedObject, position,
                            rotation, scale);
                        //VoiceController.transformationsFlow.Add(transformSelectedObject);

                        //VoiceController.positionSelectedObject.Add(position);
                        //VoiceController.scaleSelectedObject.Add(scale);
                        //VoiceController.rotationSelectedObject.Add(rotation);
                    }


                    Vector3 deltaIndexPosition = (indexPosition - lastIndexPosition)*5.0f;

                    // Change the position of the object
                    //Vector3 translation = Vector3.Scale(scaleParents, deltaIndexPosition);
                    selectedObject.transform.position = selectedObject.transform.position + deltaIndexPosition;

                    lastIndexPosition = indexPosition;
                    _extendedIndexActive = true;
                }

            }
            else
            {
                _extendedIndexActive = false;
                             
                //selectedObject.gameObject.GetComponent<InteractionBehaviour>().ignoreGrasping = false;

                //GameObject.Find("Attachment Hands").GetComponent<HandMenuAppearance>().enabled = true;
                //GameObject.Find("CenterEyeAnchor").GetComponent<Gaze>().enabled = true;

                time = 0.0f;
               
            }

            time += Time.deltaTime;

        }


    }


}
