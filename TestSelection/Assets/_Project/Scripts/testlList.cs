using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class testlList : MonoBehaviour
{
    public List<string> stringList = new List<string>();
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 20; i++)
            stringList.Add("Something" + i.ToString());
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Forward"))
            //Go up the next series of five
            for (int i = 0; i < 20; i++)
                Debug.Log(stringList[i]);

        if (GUI.Button(new Rect(10, 70, 50, 30), "Backward"))
            for (int i = 0; i < 20; i++)
                //Go down the next series of five    
                Debug.Log(stringList[i]);
    }
}