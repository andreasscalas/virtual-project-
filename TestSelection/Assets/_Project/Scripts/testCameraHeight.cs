using UnityEngine;

public class testCameraHeight : MonoBehaviour
{
    //movement speed in units per second
    private float movementSpeed = 5f;
 

    void Update()
    {
        //get the Input from Horizontal axis

        if (Input.GetKey(KeyCode.O))
        {
            transform.localScale += new Vector3(0,0.5f,0) * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.P))
        {
            transform.localScale += new Vector3(0,-0.5f, 0) * Time.deltaTime * 5; ;
        }

        //output to log the position change
        //Debug.Log("inside update"+transform.position);
    }
    //Debug.Log("update li de weizhi"+trans.position);
}