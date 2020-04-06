using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;



public class SwitchCameraMouse : MonoBehaviour

{
    public GameObject FPSController;
    public GameObject selectionManager;
    public static bool camSeleclock = false;
    // Start is called before the first frame update
    void Start()
    {
        FPSController = GameObject.Find("FPSController");
        //selectionManager = GameObject.Find("Selection Manager");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (camSeleclock == false)
            {
                FPSController.GetComponent<FirstPersonController>().enabled = false;
                //selectionManager.GetComponent<MultiSelection>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                camSeleclock = true;
            }
            else
            {
                FPSController.GetComponent<FirstPersonController>().enabled = true;
                //selectionManager.GetComponent<MultiSelection>().enabled = true;
                camSeleclock = false;
            }
        }
    }
}