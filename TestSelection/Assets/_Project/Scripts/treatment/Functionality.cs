using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functionality : MonoBehaviour
{
    public GameObject hand_cage;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        hand_cage = GameObject.Find("hand_cage");
        //transform.horse_cage().Where(x => x.name.Contains("Patrol"));
        image = GameObject.Find("Hide/Display Cage").GetComponent<Image>();
        image.color = new Color(0f, 0.6f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideDisplayCage()
    {
        
        if (hand_cage.activeSelf)
        {
            hand_cage.SetActive(false);
            image.color = new Color(0f, 1f, 0.6f, 1f);
        }
        else
        {
            hand_cage.SetActive(true);
            image.color = new Color(0f, 0.6f, 1f, 1f);
        }
    }
}
