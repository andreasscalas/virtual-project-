using System.Collections.Generic;
using System.IO;
using Accord.Math;
using UnityEngine;
//using Vector3 = UnityEngine.Vector3;

public class ReadFileComputeNewcage : MonoBehaviour
{
    public string barCoordFileName; 

    [HideInInspector] public double[,] barMatrices; //the matrix B

    public string cageFileName;

    [HideInInspector] public double[,] cageMatrices; //the matrix G

    [HideInInspector] public int columnNumber;

    public int columnNumberUpdate;

    public MeshCreateControlPoints meshCreateControlPoints;

    //public ReadCoord ReadCoord;
    public string modelFileName;
    //public string annotationFileName;

    [HideInInspector] public double[,] modelMatrices; //the matrix M

    [HideInInspector] public List<int> order;

    public int rowNumberUpdate;
    //double[,] deformation;

    private void Start()
    {
        modelMatrices = ReadMatrixFromFile(modelFileName, false);
        //Debug.Log("load matrix 1");

        cageMatrices = ReadMatrixFromFile(cageFileName, false);
        //Debug.Log("load matrix 2");

        barMatrices = ReadMatrixFromFile(barCoordFileName, true);
        //Debug.Log("load matrix 3");

    }


    /// <summary>
    ///     To display and check M=B*G;
    /// </summary>
    /// <param name="barMatrices"></param>
    /// <param name="cageMatrices"></param>
    /// <returns></returns>
    public double[,] computeProductBG(double[,] barMatrices, double[,] cageMatrices)
    {
        var produit = barMatrices.Dot(cageMatrices);
        return produit;
    }

    /// <summary>
    ///     Read the files(M,B,G) and judge if they exist or not
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public StreamReader readFile(string fileName)
    {
        StreamReader r;
        var path = Application.streamingAssetsPath + "/" + fileName + ".txt";
        var fInfo = new FileInfo(path);
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
    ///     extract the data(M,B,G) from different files
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
            var matrixData = firstFileLine.Split(' ');
            rowNumber = int.Parse(matrixData.Get(0));
            rowNumberUpdate = int.Parse(matrixData.Get(0));
            //Debug.Log("This is matrix B's row size:" + rowNumber);
            columnNumber = int.Parse(matrixData.Get(1));
            columnNumberUpdate = int.Parse(matrixData.Get(1));
            //Debug.Log("This is matrix B's column size:" + columnNumber);
            var Barycentric = Matrix.Create(new double[rowNumber, columnNumber]);
            for (var i = 2; i < rowNumber + 2; i++)
            {
                var line = fileLine.Get(i);
                var k = i - 1;
                //Debug.Log("To see the " + k + "th " + "row:" + line);
                var eachData = line.Split(' ');
                for (var j = 0; j < columnNumber; j++)
                {
                    Barycentric[i - 2, j] = float.Parse(eachData.Get(j));
                    var l = j + 1;
                    //Debug.Log("To see [" + k + "," + l + "] element in Matrix B:" + Barycentric[i - 2, j].ToString("F11"));
                }
            }

            return Barycentric;
        }

        startingRowIndex = 0;
        rowNumber = fileLine.Length - 1;
        columnNumber = 3;


        //Initialize a Matrix (size specification)
        var MatrixMandG = Matrix.Create(new double[rowNumber, columnNumber]);


        //assign the matrix with the data in file
        for (var i = startingRowIndex; i < rowNumber; i++)
        {
            var line = fileLine.Get(i);
            var k = i + 1;
            //Debug.Log("To see the " + k + "th " + "row:" + line);

            var eachData = line.Split(' ');

            for (var j = 0; j < columnNumber; j++)
                MatrixMandG[i, j] = float.Parse(eachData.Get(j));
        }

        return MatrixMandG;
    }

    ////check if the vertices got by using mesh.vertices are the same as the vertices got in Meshlab  
    //private List<int> mapping(Vector3[] positionInUnity, double[,] matrixCage)
    //{
    //    // list of int
    //    var order = new List<int>();
    //    //Debug.Log("initialControlPointPosition.Length\t" + positionInUnity.Length);
    //    //Debug.Log("matrixCage.Length/3\t" + matrixCage.Length/3);
    //    for (var i = 0; i < positionInUnity.Length; i++)
    //    {
    //        int j;
    //        for (j = 0; j < matrixCage.Length / 3; j++)
    //        {
    //            //Debug.Log("positionInUnity[i].x" + positionInUnity[i]./*position.*/x);
    //            //Debug.Log("matrixCage[j, 0]" + matrixCage[j, 0]);
    //            var deltaX = positionInUnity[i]. /*position.*/x + matrixCage[j, 0];
    //            if (Mathf.Abs((float) deltaX) < 0.00001)
    //            {
    //                var deltaY = positionInUnity[i]. /*position.*/y - matrixCage[j, 1];
    //                if (Mathf.Abs((float) deltaY) < 0.00001)
    //                {
    //                    var deltaZ = positionInUnity[i]. /*position.*/z - matrixCage[j, 2];
    //                    if (Mathf.Abs((float) deltaZ) < 0.00001) order.Add(j);
    //                }
    //            }
    //        }

    //        //matrixCage.RemoveRow(j); can be optimized
    //    }

    //    return order;
    //}
}