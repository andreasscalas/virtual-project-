using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class testMatrixtoList : MonoBehaviour
{

    public double[,] matrix;
    // Start is called before the first frame update
    void Start()
    {

        double[,] matrix =
        {
            { 1, 2 ,3},
            { 3, 4 ,5},
            { 5, 6 ,7},
        };



        Vector3 KK = new Vector3(1, 2, 3);
        Vector3 K = new Vector3(11, 22, 33);
        Vector3[,] d = new Vector3[,]
        {
            {KK},
            {K},
            {K},

        };
        ///matrix to list
        List<Vector3> lst = d.Cast<Vector3>().ToList();
        List<double> KKK = matrix.Cast<double>().ToList();
        //var KKKKK = Matrix.Dot(d, d);
        //Debug.Log("these are the elements of a list d\t" + lst[0]+"\t"+d[0,0]+"\t"+ lst[1]);
        //Debug.Log("these are the elements of a list KKK\t" + KKK[0]+"\t"+ matrix[0,1]+ "\t"+KKK[4]);
        for (int i = 0; i < lst.Count; i++)
        {
           // Debug.Log("this is the elements in list:" + lst[i]);
        }
        //for (int i = 0; i < d.Length; i++)
        //{
        //    double K=d[i,2];
        //    Debug.Log("these are the elements of a list" + d[i, 2]);
        //    //for (int j = 0; j < 2; j++)
        //    //{
        //    //    Debug.Log("these are the elements of a list" + K);
        //    //}
        //}
        double[,] b = new double[lst.Count, 3/*c*/];

        Debug.Log("this is b.Length" + Matrix.Rows(b));
        Debug.Log("this is lst[0].y(33) " + lst[2].z);
        //list to matrix
        for (int i = 0; i < lst.Count; i++)
        {
            b[i , 0] = lst[i].x;
            b[i,  1] = lst[i].y;
            b[i,  2] = lst[i].z;
        }
        
        

        for (int i = 0; i < Matrix.Rows(b); i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.Log("this is matrix b" + i + "\t" + " :" + b[i, 0]);
            }
        }
    }




    //////////// Update is called once per frame
    //////////void Update()
    //////////{

    //////////}

    //////////static void Main()
    //////////{
    //////////    // Step 1: create list.
    //////////    List<double[,]> list = new List<double[,]>();
    //////////    list.Add(new double[,]
    //////////    {
    //////////        {1.0, 2.0,3.0},
    //////////        {11.0, 22.0,33.0},
    //////////        {111.0, 222.0,333.0},
    //////////        {1111.0, 2222.0,3333.0},
    //////////        {11111.0, 22222.0,33333.0}
    //////////    });


    //////////    //list.Add(new Vector3(0, 0, 0));

    //////////    //// Step 2: convert to string array.
    //////////    //Vector3[,] array = list.ToArray();
    //////////    //Test(array);
    //////////}

    //////////static void Test(Vector3[,] array)
    //////////{
    //////////    Debug.Log("Array received: " + array.Length);
}


