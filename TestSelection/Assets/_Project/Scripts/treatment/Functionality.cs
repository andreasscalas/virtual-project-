using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Functionality : MonoBehaviour
{
    public GameObject hand_cage;
    // Start is called before the first frame update
    void Start()
    {
        hand_cage = GameObject.Find("hand_cage");
        //transform.horse_cage().Where(x => x.name.Contains("Patrol"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideCage()
    {
        hand_cage.SetActive(false); ;
    }

    public void DisplayCage()
    {
        hand_cage.SetActive(true); ;
    }


}
