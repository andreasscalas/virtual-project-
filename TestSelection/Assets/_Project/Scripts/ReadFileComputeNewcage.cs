using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord;
using System.IO;
using System;
using Accord.Math;
using System.Linq;
using System.Transactions;
using JetBrains.Annotations;
using Vector3 = UnityEngine.Vector3;


public class ReadFileComputeNewcage : MonoBehaviour
{
    //public ReadCoord ReadCoord;
    public string modelFileName;
    public string cageFileName;
    public string barCoordFileName;

    [HideInInspector]
    public double[,] modelMatrices;
    [HideInInspector]
    public double[,] cageMatrices;
    [HideInInspector]
    public double[,] barMatrices;
    public MeshCreateControlPoints meshCreateControlPoints;

    //public List<Transform> lst = new List<Transform>();
    /*public*/
    int columnNumber;
    public int rowNumberUpdate;
    public int columnNumberUpdate;
    [HideInInspector]
    public List<int> order;
    //double[,] deformation;

    void Start() 
    {
        modelMatrices = ReadMatrixFromFile(modelFileName, false);
        Debug.Log("load matrix 1");

        cageMatrices = ReadMatrixFromFile(cageFileName, false);
        Debug.Log("load matrix 2");

        barMatrices = ReadMatrixFromFile(barCoordFileName, true);
        Debug.Log("load matrix 3");

        order = mapping(meshCreateControlPoints.initialControlPointPosition, cageMatrices);
        Debug.Log("this is cage mapping rule start");
        Debug.Log("cage mesh order.Count\t" + order.Count);
        bool flagcage = true;
        for (int i = 0; i < order.Count; i++)
        {
            //Debug.Log("this is model mapping rule\t" + i + "th vertex" + "→" + order[i] + "th vertex by reading .xyz");

            if (i != order[i])
            {
                flagcage = false;
            }
        }

        if (flagcage == true)
        {
            Debug.Log("We are sure we do not need re-mapping for cage");
        }

        order = mapping(meshCreateControlPoints.initialModelVerticesPosition, modelMatrices);
        Debug.Log("this is model mapping rule start");
        Debug.Log("model mesh order.Count\t" + order.Count);
        bool flagmodel = true;
        for (int i = 0; i < order.Count; i++)
        {
            //Debug.Log("this is model mapping rule\t" + i+"th vertex" + "→" + order[i]+"th vertex by reading .xyz");
            if (i != order[i])
            {
                flagmodel = false;
            }
        }
        if (flagmodel == true)
        {
            Debug.Log("We are sure we do not need re-mapping for model");
        }

        //computeProductBG(barMatrices, cageMatrices);
    }

    /// <summary>
    /// function to update the modifications of the model.
    /// </summary>
    /// <param name="cageMatrices"></param>
    /// <param name="barMatrices"></param>
    /// <returns></returns>
    public double[,] UpdateMesh(double[,] cageMatrices, double[,] barMatrices)
    {
        return Matrix.Dot(barMatrices, cageMatrices); 
    }

    /// <summary>
    /// function to compute the new cage with deformation applied
    /// </summary>
    /// <param name="selectedVertices"></param>
    /// <param name="deformation"></param>
    /// <param name="cageMatrices"></param>
    /// <returns></returns>
    private double[,] ComputeNewcage( List<int> selectedVertices, double[,] deformation, double[,] cageMatrices)
    {

        foreach (int element in selectedVertices)
        {
            cageMatrices[selectedVertices[element], 0] += deformation[element,0];
            cageMatrices[selectedVertices[element], 1] += deformation[element, 1];
            cageMatrices[selectedVertices[element], 2] += deformation[element, 2];
        }
        return cageMatrices;
    }

    /// <summary>
    /// To display and check M=B*G;
    /// </summary>
    /// <param name="barMatrices"></param>
    /// <param name="cageMatrices"></param>
    /// <returns></returns>
    public double[,] computeProductBG(double[,] barMatrices, double[,] cageMatrices)
    {
        var produit = Matrix.Dot(barMatrices, cageMatrices);
        //for (int i = 0; i < 20/*Matrix.Rows(produit)*/; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    { Debug.Log("produit[" + i + ", " + j + "]" + "\t" + produit[i, j]); }
        //}
        return (produit);
    }

    /// <summary>
    /// Read the files(M,B,G) and judge if they exist or not
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public StreamReader readFile(string fileName)
    {
        StreamReader r;
        string path = (Application.streamingAssetsPath + "/" + fileName + ".txt");
        FileInfo fInfo = new FileInfo(path);
        if (fInfo.Exists)
        {
            r = new StreamReader(path);
        }
        else
        {
            Debug.Log("File does not exist");
            return null;
        }
        return r;
    }
    /// <summary>
    /// extract the data(M,B,G) from different files
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="isCoordinateFile"></param>
    /// <returns></returns>
    public double[,] ReadMatrixFromFile(string fileName, bool isCoordinateFile)
    {
        int rowNumber;
        //int columnNumber;
        int startingRowIndex;

        var r = readFile(fileName);
        var fileContent = r.ReadToEnd();
        var fileLine = fileContent.Split('\n');
       
        //We set the number of row and column according to the file we are reading.
        if (isCoordinateFile)
        {
            startingRowIndex = 2;
            var firstFileLine = fileLine.Get(1);
            //Read the number of rows and columns from the second line of the file
            string[] matrixData = firstFileLine.Split(' ');
            rowNumber = Int32.Parse(matrixData.Get(0));
            rowNumberUpdate = Int32.Parse(matrixData.Get(0));
            Debug.Log("This is matrix B's row size:" + rowNumber);
            columnNumber = Int32.Parse(matrixData.Get(1));
            columnNumberUpdate = Int32.Parse(matrixData.Get(1));
            Debug.Log("This is matrix B's column size:" + columnNumber);
            double[,] Barycentric = Matrix.Create(new double[rowNumber, columnNumber]);
            for (int i = 2; i < rowNumber + 2; i++)
            {
                var line = fileLine.Get(i);
                int k = i - 1;
                //Debug.Log("To see the " + k + "th " + "row:" + line);
                var eachData = line.Split(' ');
                for (int j = 0; j < columnNumber; j++)
                {
                    Barycentric[i - 2, j] = float.Parse(eachData.Get(j));
                    int l = j + 1;
                    //Debug.Log("To see [" + k + "," + l + "] element in Matrix B:" + Barycentric[i - 2, j].ToString("F11"));
                }
            }
            return Barycentric;
        }
        else
        {
            startingRowIndex = 0;
            rowNumber = fileLine.Length - 1;
            columnNumber = 3;
        }


        //Initialize a Matrix (size specification)
        var MatrixMandG = Matrix.Create(new double[rowNumber, columnNumber]);
       
        
        //assign the matrix with the data in file
        for (int i = startingRowIndex; i < rowNumber; i++)
        {
            var line = fileLine.Get(i);
            int k = i + 1;
            //Debug.Log("To see the " + k + "th " + "row:" + line);
           
            var eachData = line.Split(' ');
       
            for (int j = 0; j < columnNumber; j++)
            {
                MatrixMandG[i, j] = float.Parse(eachData.Get(j));
                ////int l = j + 1;
                ////if (fileName.Contains("cage"))
                ////{ Debug.Log("To see [" + k + "," + l + "] element in Matrix G:" + MatrixMandG[i, j]/*.ToString("F6")*/); }
                //////else if (fileName.Contains("coord"))
                //////{ }
                ////else /*if (fileName.Contains("coord")!=true)*/
                ////{
                ////    Debug.Log("To see [" + k + "," + l + "] element in Matrix M:" + MatrixMandG[i, j]/*.ToString("F6")*/);
                ////}
            }
        }

        return MatrixMandG;
    }

    List<int> mapping(Vector3[] positionInUnity, double[,] matrixCage)
    {
        // list of int
        List<int> order = new List<int>();
        //Debug.Log("initialControlPointPosition.Length\t" + positionInUnity.Length);
        //Debug.Log("matrixCage.Length/3\t" + matrixCage.Length/3);
        for (int i = 0; i < positionInUnity.Length; i++)
        {
            int j;
            for (j = 0; j < (matrixCage.Length) / 3; j++)
            {
                //Debug.Log("positionInUnity[i].x" + positionInUnity[i]./*position.*/x);
                //Debug.Log("matrixCage[j, 0]" + matrixCage[j, 0]);
                double deltaX = positionInUnity[i]. /*position.*/x + matrixCage[j, 0];
                if (Mathf.Abs((float)deltaX) <0.00001)
                {
                    double deltaY = positionInUnity[i]. /*position.*/y - matrixCage[j, 1];
                    if (Mathf.Abs((float)deltaY) < 0.00001)
                    {
                        double deltaZ = positionInUnity[i]. /*position.*/z - matrixCage[j, 2];
                        if (Mathf.Abs((float)deltaZ) < 0.00001)
                        {
                            order.Add(j);
                        }
                    }
                }
            }
            //matrixCage.RemoveRow(j); can be optimized
        }
        return order;

    }
}
