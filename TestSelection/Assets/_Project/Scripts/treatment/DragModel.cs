using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.UI;

public class DragModel : MonoBehaviour
{
    public HandModelBase leftHandModel;
    public HandModelBase rightHandModel;
    //public ControllerColliderHit hit=new ControllerColliderHit();
    float twoFingerDistance = 0.03f;
    public GameObject ObjToMove;
    private bool initialTrack;
    private bool graspDetect;
    private bool checkSegment;
    private Vector3 initialPosHand;
    private Vector3 previousPoshand;
    public Vector3 speed = new Vector3(0.2f, 0, 0);
    private ReadJson readJson;
    private TreatSelectionManager treatSelectionManager;
    private MeshCreateControlPoints meshCreateControlPoints;
    private int segment=-1;
    bool movable = false;
    private Vector3 offset ;
    Vector3 selectionCPsBarycenter = new Vector3();
    private int selectionlistCount;
    private List<Vector3> impossibleDirections;
    public bool FreeMovimentState;
    private Vector3 previousDragPos;
    private Camera cam;
    private Hand leftHand;
    private Hand rightHand;
    private LeapProvider provider;
    bool swipeUp;
    bool swipeDown;
    [HideInInspector] public bool canChangeLevel;
    [HideInInspector] public bool voiceChangeScale;
    [HideInInspector] public bool changeScaleCollision;
    public Text voiceControlCommand;
    public Text leapHandAction;

    const float deltaVelocity = 2f;
    const float smallestVelocity = 1.9f;
    const float deltaCloseFinger = 0.12f;

    void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        treatSelectionManager = GameObject.Find("Selection Manager").GetComponent<TreatSelectionManager>();
        meshCreateControlPoints = GameObject.Find("Selection Manager").GetComponent<MeshCreateControlPoints>();
        //offset = treatSelectionManager._selectedControlPoints.position - initialPosHand;
        checkSegment = false;
        cam = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
        impossibleDirections = new List<Vector3>();
        FreeMovimentState = true;
        swipeUp = false;
        swipeDown = false;
        canChangeLevel = false;
        voiceChangeScale = false;
        changeScaleCollision = false;
    }

    void Update()
    {
        //swipe hands to change level
        SwipeChangeLevel();

        //get hands
        if (leftHandModel.IsTracked||rightHandModel.IsTracked)
        {
            leftHand = leftHandModel.GetLeapHand();
            rightHand = rightHandModel.GetLeapHand();
        }


        //if selection list changed,compute the barycenter of the selection list
        if (selectionlistCount!= treatSelectionManager.selectionList.Count)
        {
            selectionlistCount = treatSelectionManager.selectionList.Count;
            ComputeBaryenter(treatSelectionManager.selectionList);
        }


        if (checkSegment  /*a condition that make FindSegment run once*/)
        {
            //check if the CPs belong to a segment that is touched left hand
            FindSegment();
            checkSegment = false;
            Debug.Log(segment);
        }

        // Allow to move(deform) the mesh of the model if the mesh(corresponding to the selected control points) is grasped by hand 
        if (graspDetect)
        {
            MoveObject();
        }

        //check if scale model is allowed, if yes, scale the model
        if (voiceChangeScale)
        {
            Scale();
        }


    }

    private void ComputeBaryenter(List<Transform> myList)
    {
        for (int i = 0; i < myList.Count; i++)
        {
            selectionCPsBarycenter += myList[i].position;
        }

        selectionCPsBarycenter /= myList.Count;
    }


    public void MoveObject()
    {
        /*bool value =true*/
        //var offset = treatSelectionManager._selectedControlPoints.position - initialPosHand;

        for (int i = 0; i < treatSelectionManager.selectionList.Count; i++)
        {
            for (int j = 0; j < readJson.treeNodeLevelx[segment].GetData().cageVerticesIndex.Count; j++)
            {
                //if the segment that hand touch has at least a control point selected
                if (treatSelectionManager.selectionList[i].position == meshCreateControlPoints.cageVertices[readJson.treeNodeLevelx[segment].GetData().cageVerticesIndex[j]])
                {
                    //We can move the control points now
                    movable = true;
                    break;
                }
            }
        }

        Debug.Log("movable " + movable);




        if (movable && FreeMovimentState)
        {
            treatSelectionManager.SelectedControlPoints.transform.position = leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
            //treatSelectionManager.SelectedControlPoints.transform.Rotate(cam.transform.forward, 10);
            //Debug.DrawRay(leftHand.PalmPosition.ToVector3(), 100*leftHand.PalmNormal.ToVector3(),Color.red);
            //Debug.Log("mesh deforming... " + leftHand.PalmNormal.ToVector3());
            //Debug.Log("mesh deforming... "+ treatSelectionManager.SelectedControlPoints.transform.position);
        }

        if (movable && !FreeMovimentState)
        {
            var move = false;
            foreach (var direction in impossibleDirections)
            {
                var possibleMoviment = previousDragPos - leftHand.Fingers[0].TipPosition.ToVector3() + selectionCPsBarycenter + offset;
                possibleMoviment = Vector3.Normalize(possibleMoviment);
                previousDragPos = leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset; ;

                var directionNorm = Vector3.Normalize(direction);
                var isParall = Vector3.Dot(possibleMoviment, directionNorm);

                if (Math.Abs(isParall + 1) < 0.2)
                {
                    move = true;
                    break;
                }
            }

            if (move)
            {
                treatSelectionManager.SelectedControlPoints.transform.position = leftHand.Fingers[0].TipPosition.ToVector3() - selectionCPsBarycenter - offset;
            }
        }
        //Debug.Log("previousPoshand" + previousPoshand);


        //ObjToMove.transform.position += speed*Time.deltaTime; /*initial vector value*/

        //previousPoshand = leftHand.Fingers[0].TipPosition.ToVector3();
        //Debug.Log("leftHand.Fingers[0].TipPosition.ToVector3()！" + leftHand.Fingers[0].TipPosition.ToVector3());




    }

    //Detect if leap hand(left) hits the corresponding segment of the selected CPs;  
    public void FindSegment()
    {
        /*bool value =true*/
        //Debug.Log("leftHand.Fingers[0].TipPosition.ToVector3()！" + leftHand.Fingers[0].TipPosition.ToVector3());


        float distance = 100;

        for (int i = 0; i < readJson.treeNodeLevelx.Count; i++)
        {

            //var variable = readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Min(x =>
            //    Vector3.Distance(readJson.cageVertices[x], leftHand.Fingers[0].TipPosition.ToVector3()));
            for (int j = 0; j < readJson.treeNodeLevelx[i].GetData().cageVerticesIndex.Count; j++)
            {
                var dis = Vector3.Distance(leftHand.Fingers[0].TipPosition.ToVector3(), readJson.cageVertices[readJson.treeNodeLevelx[i].GetData().cageVerticesIndex[j]]);
                //find the nearest vertex of a certain segment on the cage(here it would be more precise if we find the nearest vertex on the model)
                //if we want to make both two hands move the model's mesh, we also need to get the distance between right hand and vertex, if 
                if (distance > dis)
                {
                    distance = dis;
                    segment = i;
                }
            }

        }
        initialPosHand = leftHand.Fingers[0].TipPosition.ToVector3();
        offset = initialPosHand - selectionCPsBarycenter;


    }


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
    }


    void OnCollisionEnter(Collision collision)
    {
        changeScaleCollision = true;
        FreeMovimentState = false;
        //CollisionState = true;
        Debug.Log("collision model and wall");
        foreach (var contact in collision.contacts)
        {
            impossibleDirections.Add(contact.normal);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        changeScaleCollision = false;
        Debug.Log("collision exit model and wall");
        FreeMovimentState = true;
        //CollisionState = false;
    }

    void OnCollisionStay(Collision collision)
    {
        changeScaleCollision = true;
        //CollisionState = true;
        FreeMovimentState = false;
    }


    public void SwipeChangeLevel()
    {


        Frame frame = provider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsLeft || hand.IsRight)
            {
                swipeUp = false;
                swipeDown = false;

                if (canChangeLevel)
                {
                    if (isMoveUp(hand))
                    {
                        swipeUp = true;
                    }
                    if (isMoveDown(hand))
                    {
                        swipeDown = true;
                    }

                    if (swipeUp && !swipeDown ) 
                    {
                        //level+1
                        if (readJson.levelSelect < readJson.levelMax)
                        {
                            readJson.levelSelect += 1;
                            readJson.levelChange = true;
                            canChangeLevel = false;
                            swipeUp = false;
                            voiceControlCommand.text = "Change level(+1)";
                            leapHandAction.text = "Swiped up";


                        }
                    }
                    if (!swipeUp && swipeDown )
                    {
                        //level-1
                        if (readJson.levelSelect > 0)
                        {
                            readJson.levelSelect -= 1;
                            readJson.levelChange = true;
                            canChangeLevel = false;
                            swipeDown = false;
                            voiceControlCommand.text = "Change level(-1)";
                            leapHandAction.text = "Swiped down";
                        }
                    }
                }
                
            }
        }

    }

    //method that will be recalled by voice controllor when "change level" key word is spoke out
    public void SwitchLevel()
    {
        canChangeLevel = true;
        voiceControlCommand.text = "Swipe to change level";
    }


    //method that will be recalled by voice controllor when "change level" key word is spoke out
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


    protected bool isMoveUp(Hand hand)// hand swipe to up
    {
        return hand.PalmVelocity.y > deltaVelocity && !isStationary(hand);
    }


    protected bool isMoveDown(Hand hand)   // hand swipe to down
    {
        //velocity along y   deltaVelocity = 0.7f    isStationary (hand)  check if hand is stationary 
        return hand.PalmVelocity.y < -deltaVelocity && !isStationary(hand);
    }

    protected bool isStationary(Hand hand)// is stationary
    {
        return hand.PalmVelocity.Magnitude < smallestVelocity;
    }




    //method to scale the whole model
    public void Scale()
    {
        Frame frame = provider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (isOpenFullHand(hand) && !changeScaleCollision) 
            {
                Debug.Log("zoom in");
                var initialCPs = GameObject.Find("Initialized Control Points");
                var SelectedCPs = GameObject.Find("Selected Control Points");

                Vector3 value = initialCPs.transform.localScale;
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

                Vector3 value = initialCPs.transform.localScale;
                value -= new Vector3(value.x * 0.05f, value.y * 0.05f, value.z * 0.05f);
                //    Debug.Log(value);
                initialCPs.transform.localScale = value;
                SelectedCPs.transform.localScale = value;
                changeScaleCollision = false;
                leapHandAction.text = "leap-hand is clenched";
            }
        }
    }




    protected bool isCloseHand(Hand hand)     //check if make a fist
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        { //check all fingers
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger)    //float deltaCloseFinger = 0.05f;
            {
                count++;
                //  if (finger.Type == Finger.FingerType.TYPE_THUMB)
                //  Debug.Log ((finger.TipPosition - hand.PalmPosition).Magnitude);
            }
        }
        Debug.Log("deltaCloseFinger:" + deltaCloseFinger);
        return (count == 5);
    }

    protected bool isOpenFullHand(Hand hand)         //hand is fully open
    {
        //Debug.Log (hand.GrabStrength + " " + hand.PalmVelocity + " " + hand.PalmVelocity.Magnitude);
        return hand.GrabStrength == 0;
    }



}