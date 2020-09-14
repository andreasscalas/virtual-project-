using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

public class DisableAttachmentHand : MonoBehaviour
{
    public HandModelBase leftHandModel;
    private GameObject attachmentHands;
    void Start()
    {
        attachmentHands = GameObject.Find("Attachment Hands");
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandModel.IsTracked)
        {
            attachmentHands.SetActive(true);
            Debug.Log("left hand tracked");
        }

        if (!leftHandModel.IsTracked)
        {
            attachmentHands.SetActive(false);
            //Debug.Log("left hand is not tracked");

        }

    }
}

