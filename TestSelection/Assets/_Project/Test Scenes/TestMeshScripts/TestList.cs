using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestList : MonoBehaviour
{
    public List<int> cageVertices0 = new List<int>();
    public List<int> cageVertices1 = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        cageVertices0.Add(0);
        cageVertices0.Add(1);
        cageVertices0.Add(2);
        cageVertices0.Add(3);
        cageVertices0.Add(4);
        cageVertices0.Add(5);
        Debug.Log("cageVertices0[0]  0:" + cageVertices0[0]);
        cageVertices1 = new List<int> {5, 4, 3, 2, 1, 0};
        cageVertices0 = cageVertices1;
        cageVertices1 = new List<int> { 8, 8, 8, 8, 8, 8 };
        Debug.Log("cageVertices0[0]  1:"+ cageVertices0[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
