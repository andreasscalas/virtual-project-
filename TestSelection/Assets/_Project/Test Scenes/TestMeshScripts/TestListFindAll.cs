using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListFindAll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<int> integers = new List<int>(){1,1,1,2,3,4,5,6,7,8,9};
        var a = integers.FindAll(x => x > 7);
        Debug.Log(a.Count);
        foreach (var VARIABLE in a)
        {
            Debug.Log(VARIABLE);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
