using System;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;

public class DragModel : MonoBehaviour
{
    private const float deltaVelocity = 2f;
    private const float smallestVelocity = 1.9f;
    private const float deltaCloseFinger = 0.12f;
    [HideInInspector] public bool canChangeLevel;
    [HideInInspector] public bool changeScaleCollision;
    private bool checkSegment;

    public bool FreeMovimentState;

    //public ControllerColliderHit hit=new ControllerColliderHit();
    public bool graspDetect;
    [HideInInspector] public bool grasping;
    private List<Vector3> impossibleDirections;
    private Vector3 initialPosHand;
    public Text leapHandAction;
    private Hand leftHand;
    public HandModelBase leftHandModel;
    private MeshCreateControlPoints meshCreateControlPoints;
    private bool movable;
    private Vector3 offset;
    private Vector3 previousDragPos;
    private Vector3 previousPoshand;
    private LeapProvider provider;
    private ReadJson readJson;
    private Hand rightHand;
    public HandModelBase rightHandModel;
    private int segment = -1;
    private Vector3 selectionCPsBarycenter;
    private int selectionlistCount;
    private bool swipeDown;
    private bool swipeUp;
    private TreatSelectionManager treatSelectionManager;
    [HideInInspector] public bool voiceChangeScale;
    public Text voiceControlCommand;

    private void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        treatSelectionManager = GameObject.Find("Selection Manager").GetComponent<TreatSelectionManager>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        checkSegment = false;
        provider = FindObjectOfType<LeapProvider>();
        impossibleDirections = new List<Vector3>();
        FreeMovimentState = true;
        swipeUp = false;
        swipeDown = false;
        canChangeLevel = false;
        voiceChangeScale = false;
        changeScaleCollision = false;
        graspDetect = false;
        grasping = false;
        selectionCPsBarycenter = new Vector3();
    }

    private void Update()
    {
        //swipe hands to change level
        SwipeChangeLevel();

        //get hands
        if (leftHandModel.IsTracked || rightHandModel.IsTracked)
        {
            leftHand = leftHandModel.GetLeapHand();
            rightHand = rightHandModel.GetLeapHand();
        }


        //if selection list changed, compute the barycenter of the selection list
        if (selectionlistCount != treatSelectionManager.selectionList.Count)
        {
            selectionlistCount = treatSelectionManager.selectionList.Count;
            ComputeBaryenter(treatSelectionManager.selectionList);
        }


        if (checkSegment /*a condition that make FindSegment run once*/)
        {
            //check if the CPs belong to a segment that is touched by left hand
            FindSegment();
            checkSegment = false;
        }

        // Allow to move(deform) the mesh of the model if the mesh(corresponding to the selected control points) is grasped by hand 
        if (graspDetect) MoveObject();

        //check if scale model is allowed, if yes, scale the model
        if (voiceChangeScale) Scale();
    }

    //compute barycenter
    private void ComputeBaryenter(List<Transform> myList)
    {
        selectionCPsBarycenter = new Vector3(0, 0, 0);
        for (var i = 0; i < myList.Count; i++) selectionCPsBarycenter += myList[i].position;

        selectionCPsBarycenter /= myList.Count;
    }


    public void MoveObject()
    {
        /*bool value =true*/
        //var offset = treatSelectionManager._selectedControlPoints.position - initialPosHand;

        for (var i = 0; i < treatSelectionManager.selectionList.Count; i++)
        for (var j = 0; j < readJson.treeNodeLevelx[segment].GetData().cageVerticesIndex.Count; j++)
            //if the segment that hand touch has at least a control point selected
            if (treatSelectionManager.selectionList[i].position ==
                meshCreateControlPoints.cageVertices[readJson.treeNodeLevelx[segment].GetData().cageVerticesIndex[j]])
            {
                //We can move the control points now
                movable = true;
                break;
            }

        Debug.Log("movable " + movable);

        if (movable && FreeMovimentState)
        {
            Debug.Log("selectionCPsBarycenter " + selectionCPsBarycenter);
            Debug.Log("offset " + offset);
            treatSelectionManager.SelectedControlPoints.transform.position =
                leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
            Debug.Log("treatSelectionManager.SelectedControlPoints.transform.position " +
                      treatSelectionManager.SelectedControlPoints.transform.position);
        }

        if (movable && !FreeMovimentState)
        {
            var move = false;
            foreach (var direction in impossibleDirections)
            {
                var possibleMoviment = previousDragPos - leftHand.Fingers[0].TipPosition.ToVector3() +
                                       selectionCPsBarycenter + offset;
                possibleMoviment = Vector3.Normalize(possibleMoviment);
                previousDragPos = leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
                ;

                var directionNorm = Vector3.Normalize(direction);
                var isParall = Vector3.Dot(possibleMoviment, directionNorm);

                if (Math.Abs(isParall + 1) < 0.2)
                {
                    move = true;
                    break;
                }
            }

            if (move)
                treatSelectionManager.SelectedControlPoints.transform.position =
                    leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
        }
    }

    //Detect if leap hand(left) hits the corresponding segment of the selected CPs;  
    public void FindSegment()
    {
        float distance = 100;

        for (var i = 0; i < readJson.treeNodeLevelx.Count; i++)
            //var variable = readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Min(x =>
            //    Vector3.Distance(readJson.cageVertices[x], leftHand.Fingers[0].TipPosition.ToVector3()));
        for (var j = 0; j < readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Count; j++)
        {
            var dis = Vector3.Distance(leftHand.Fingers[0].TipPosition.ToVector3(),
                readJson.cageVertices[readJson.treeNodeLevelx[i].GetData().cageVerticesIndex[j]]);
            //find the nearest vertex of a certain segment on the cage(here it would be more precise if we find the nearest vertex on the model)
            //if we want to make both two hands move the model's mesh, we also need to get the distance between right hand and vertex, if 
            if (distance > dis)
            {
                distance = dis;
                segment = i;
            }
        }

        initialPosHand = leftHand.Fingers[0].TipPosition.ToVector3();
        offset = initialPosHand -
                 selectionCPsBarycenter; //a distance between the transform position and hand fingertip's position
    }

    //event functions
    public void DetectGrasp()
    {
        Debug.Log("DetectGrasp");
        leapHandAction.text = "Model's mesh grasped";
        graspDetect = true;
        checkSegment = true;
        previousDragPos = leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
    }

    public void DetectRelease()

    {
        Debug.Log("Detect hand release");
        leapHandAction.text = "Model's mesh released";
        graspDetect = false;
        movable = false;
        grasping = false;
    }

    public void DetectOnGrasp()

    {
        grasping = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        changeScaleCollision = true;
        FreeMovimentState = false;
        //CollisionState = true;
        Debug.Log("collision model and wall");
        foreach (var contact in collision.contacts) impossibleDirections.Add(contact.normal);
    }

    private void OnCollisionExit(Collision collision)
    {
        changeScaleCollision = false;
        Debug.Log("collision exit model and wall");
        FreeMovimentState = true;
        //CollisionState = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        changeScaleCollision = true;
        //CollisionState = true;
        FreeMovimentState = false;
    }

    // swipe hand to change the level of the model
    public void SwipeChangeLevel()
    {
        var frame = provider.CurrentFrame;
        foreach (var hand in frame.Hands)
            if (hand.IsLeft || hand.IsRight)
            {
                swipeUp = false;
                swipeDown = false;

                if (canChangeLevel)
                {
                    if (isMoveUp(hand)) swipeUp = true;
                    if (isMoveDown(hand)) swipeDown = true;

                    if (swipeUp && !swipeDown)
                        //level+1
                        if (readJson.levelSelect < readJson.levelMax)
                        {
                            readJson.levelSelect += 1;
                            readJson.levelChange = true;
                            canChangeLevel = false;
                            swipeUp = false;
                            voiceControlCommand.text = "Change level(+1)";
                            leapHandAction.text = "Swiped upwards";
                        }

                    if (!swipeUp && swipeDown)
                        //level-1
                        if (readJson.levelSelect > 0)
                        {
                            readJson.levelSelect -= 1;
                            readJson.levelChange = true;
                            canChangeLevel = false;
                            swipeDown = false;
                            voiceControlCommand.text = "Change level(-1)";
                            leapHandAction.text = "Swiped downwards";
                        }
                }
            }
    }

    //method that will be recalled by voice controller with "change level" key word 
    public void SwitchLevel()
    {
        canChangeLevel = true;
        voiceControlCommand.text = "Swipe to change level";
    }


    //method that will be recalled by voice controller with "change level" key word 
    public void ChangeScaleOfModel()
    {
        voiceChangeScale = true;
        voiceControlCommand.text = "Clench or open hand to scale model";
    }

    public void StopScaleModel()
    {
        voiceChangeScale = false;
        voiceControlCommand.text = "Scale mode is deactivated";
    }

    // hand swipe up
    protected bool isMoveUp(Hand hand)
    {
        return hand.PalmVelocity.y > deltaVelocity && !isStationary(hand);
    }

    // hand swipe down
    protected bool isMoveDown(Hand hand)
    {
        //velocity along y   deltaVelocity = 0.7f    isStationary (hand)  check if hand is stationary 
        return hand.PalmVelocity.y < -deltaVelocity && !isStationary(hand);
    }

    protected bool isStationary(Hand hand) // is stationary
    {
        return hand.PalmVelocity.Magnitude < smallestVelocity;
    }

    //method to scale the whole model(scale the selected control points and the initialized control points)
    public void Scale()
    {
        var frame = provider.CurrentFrame;
        foreach (var hand in frame.Hands)
        {
            if (isOpenFullHand(hand) && !changeScaleCollision)
            {
                Debug.Log("zoom in");
                var initialCPs = GameObject.Find("Initialized Control Points");
                var SelectedCPs = GameObject.Find("Selected Control Points");

                var value = initialCPs.transform.localScale;
                value += new Vector3(value.x * 0.05f, value.y * 0.05f, value.z * 0.05f);
                //    Debug.Log(value);
                initialCPs.transform.localScale = value;
                SelectedCPs.transform.localScale = value;
                leapHandAction.text = "leap-hand is open";
            }

            Debug.Log("isCloseHand(hand):" + isCloseHand(hand));
            if (isCloseHand(hand))
            {
                Debug.Log("zoom out");
                var initialCPs = GameObject.Find("Initialized Control Points");
                var SelectedCPs = GameObject.Find("Selected Control Points");

                var value = initialCPs.transform.localScale;
                value -= new Vector3(value.x * 0.05f, value.y * 0.05f, value.z * 0.05f);
                //    Debug.Log(value);
                initialCPs.transform.localScale = value;
                SelectedCPs.transform.localScale = value;
                changeScaleCollision = false;
                leapHandAction.text = "leap-hand is clenched";
            }
        }
    }

    //check if hand makes a fist
    protected bool isCloseHand(Hand hand)
    {
        var listOfFingers = hand.Fingers;
        var count = 0;
        for (var f = 0; f < listOfFingers.Count; f++)
        {
            //check all fingers
            var finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger) //float deltaCloseFinger = 0.05f;
                count++;
        }

        Debug.Log("deltaCloseFinger:" + deltaCloseFinger);
        return count == 5;
    }

    //hand is fully open
    protected bool isOpenFullHand(Hand hand)
    {
        //Debug.Log (hand.GrabStrength + " " + hand.PalmVelocity + " " + hand.PalmVelocity.Magnitude);
        return hand.GrabStrength == 0;
    }
}