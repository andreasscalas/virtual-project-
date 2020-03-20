using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;



public class SwitchCameraMouse : MonoBehaviour

{

    public GameObject FPSController;

    public static bool camlock = false;

    // Start is called before the first frame update

    void Start()

    {

        FPSController = GameObject.Find("FPSController");

    }



    void Update()

    {

        if (Input.GetKeyDown(KeyCode.C))

        {



            if (camlock == false)

            {

                FPSController.GetComponent<FirstPersonController>().enabled = false;

                Cursor.lockState = CursorLockMode.None;

                Cursor.visible = true;

                camlock = true;

            }

            else

            {

                FPSController.GetComponent<FirstPersonController>().enabled = true;

                camlock = false;



            }

        }



    }

}