
using System;
using System.Collections;
using System.Collections.Generic;
//using Assets.Scripts.Actions;
//using Assets.Scripts.Phases;
using UnityEngine;
using Assets.Scripts.Support;
using Leap.Unity.Interaction;


namespace Leap.Unity
{
    /// <summary>
    /// Use this component on a Game Object to allow it to be rotated when
    /// palms face each other



    public class Rotate : MonoBehaviour
    {


        public HandModelBase HandModel_L;
        public HandModelBase HandModel_R;
        private GameObject selectedObject;
        private Vector3 palmPosition_L;
        private Vector3 palmPosition_R;
        private Vector3 palmsDirection;
        private Vector3 lastPalmsDirection;
        private bool _rotationActive;
        private Vector3 scaleParents;
        private Quaternion rotationsParent;        
        public ExtendedFingerDetector extendedFingerDetectorL;
        public ExtendedFingerDetector extendedFingerDetectorR;
        private float time = 0.0f;

        // Use this for initialization
        void Start()
        {

            _rotationActive = false;
            time = 0;

        }

        

        void Update()
        {

            selectedObject = GameObject.FindWithTag("Selected");
           
            if (selectedObject != null)
            {

                ///Create box collider of the selected object if it doesn't exist yet

                if (selectedObject.GetComponent<BoxCollider>() == null)
                {
                    if (selectedObject.transform.childCount == 0)
                    {
                        //Assets.Scripts.Support.Collider.CreateBoxColliderOfPart(selectedObject);

                    }
                    else
                    {
                        //Assets.Scripts.Support.Collider.CreateBoxColliderOfComponent(selectedObject);

                    }

                }

                var bbox = selectedObject.GetComponent<BoxCollider>();

                //Coordinates of the center of the box collider
                Vector3 barycenterSelectedObject = bbox.center;

                //Scale of the selectedObject
                Vector3 scaleFactor = selectedObject.transform.localScale;  ///---> actually the scale is homogeneus on the three dimensions

                //Multiply scales of all parents of the selectedObject
                Transform tranformParent = selectedObject.transform.parent;
                scaleParents = new Vector3(1, 1, 1);
                rotationsParent = Quaternion.Euler(0, 0, 0);
                while (tranformParent != null)
                {
                    rotationsParent = tranformParent.localRotation * rotationsParent;
                    scaleParents = Vector3.Scale(scaleParents, tranformParent.localScale);
                    tranformParent = tranformParent.parent;

                }

                

                if (extendedFingerDetectorL != null && extendedFingerDetectorL.IsActive &&
                    extendedFingerDetectorR != null && extendedFingerDetectorR.IsActive)
                {
                    
                    //if (time > ReferencialDisplay.gestureWaitingTime)
                    {
                        /// When the gesture is avtive disable the scripts for the hand menu appearance to disambiguate the functionalities.
                        //GameObject.Find("Attachment Hands").GetComponent<HandMenuAppearance>().enabled = false;
                        //GameObject.Find("CenterEyeAnchor").GetComponent<Gaze>().enabled = false;


                        selectedObject.gameObject.GetComponent<InteractionBehaviour>().ignoreGrasping = true;

                        /// Store direction between palms
                        palmPosition_L = HandModel_L.GetLeapHand().PalmPosition.ToVector3();
                        palmPosition_R = HandModel_R.GetLeapHand().PalmPosition.ToVector3();

                        palmsDirection = palmPosition_R - palmPosition_L;

                        /// If it is the first frame in which there are two palms (again) set "lastpalmsDirection"
                        /// equals to "palmsDirection" in order to have, at first, deltaAngle=0 and not to rotate the object at
                        /// the first time according to the direction of palms.                  
                        if (_rotationActive == false)
                        {
                            var assemblies = GameObject.Find("Assemblies");
                            var soundPath = @"Audio/activation";
                            //FeedBack.AudioPlay(assemblies, soundPath);

                            lastPalmsDirection = palmsDirection;

                            // Save in a list the selectedObject and its transform before each transformation ( for the UNDO operation)
                            var position = new Vector3();
                            var rotation = new Quaternion();
                            var scale = new Vector3();

                            position = selectedObject.transform.localPosition;
                            scale = selectedObject.transform.localScale;
                            rotation = selectedObject.transform.localRotation;

                            TransformGameObject transformSelectedObject = new TransformGameObject(selectedObject, position, rotation, scale);
                           //VoiceController.transformationsFlow.Add(transformSelectedObject);

                            _rotationActive = true;

                        }

                        Quaternion handRot = Quaternion.FromToRotation(lastPalmsDirection, palmsDirection);

                        /// Rotate and replace the object. Two different cases: if the object is an assembly or
                        /// if it is a comonent/part. The assembly must be set in the origin (0,0,0) before rotate,
                        /// the component/part no because locally it is in (0,0,0) yet.
                        if (selectedObject.transform.parent.name == "Assemblies")
                        {
                            // Translation of the selectedObject from the _userPlace (add this vector after the rotation to replace the object in its initial position)
                            //Vector3 translation = selectedObject.transform.position - ReferencialDisplay._userPlace + selectedObject.transform.localRotation * Vector3.Scale(barycenterSelectedObject, scaleFactor);

                            // Put the selectedObject in the origin (0,0,0)
                            selectedObject.transform.position = -Vector3.Scale(barycenterSelectedObject, scaleFactor);

                            // Rotate the selectedObject
                            selectedObject.transform.rotation = handRot * selectedObject.transform.rotation;

                            // Replace the selectedObject in the origin (0,0,0) by applying the inverse transformation to the position
                            selectedObject.transform.position = selectedObject.transform.localRotation * selectedObject.transform.position;

                            // Put the selectedObject in the initial position (userPlace + translation) 
                            //selectedObject.transform.position = selectedObject.transform.position + ReferencialDisplay._userPlace + translation;
                        }
                        else
                        {
                            // Calculate the translation of the selectedObject after a scale (if the object hasn't been scaled scaleTranslation = 0 ) 
                            // (add this vector, correctly rotated, to the final position in order to replace the object in its initial position )  
                            Vector3 translationScale = -barycenterSelectedObject * (selectedObject.transform.localScale.x - 1);

                            // Translation of the selectedObject from its position in the assembly (add this vector after the rotation to replace the object in its initial position)
                            Vector3 translation = selectedObject.transform.localPosition - (barycenterSelectedObject - selectedObject.transform.localRotation * barycenterSelectedObject + selectedObject.transform.localRotation * translationScale);

                            // Rotate the selectedObject 
                            selectedObject.transform.rotation = handRot * selectedObject.transform.rotation;

                            // Replace the selectedObject in the initial position (locally)
                            selectedObject.transform.localPosition = barycenterSelectedObject - selectedObject.transform.localRotation * barycenterSelectedObject + selectedObject.transform.localRotation * translationScale + translation;

                        }


                        lastPalmsDirection = palmsDirection;
                       

                    }

                }
                else
                {
                    _rotationActive = false;                                   

                    selectedObject.gameObject.GetComponent<InteractionBehaviour>().ignoreGrasping = false;
                    
                    //GameObject.Find("Attachment Hands").GetComponent<HandMenuAppearance>().enabled = true;
                    //GameObject.Find("CenterEyeAnchor").GetComponent<Gaze>().enabled = true;


                    time = 0.0f;
                }

                time += Time.deltaTime;

            }

        }

        //public IEnumerator GestureConfirmation()
        //{
        //    yield return new WaitForSeconds(10);
        //    _rotationActive = true;
        //}
    }
}

