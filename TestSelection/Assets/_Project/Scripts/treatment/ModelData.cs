using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




namespace Assets._Project.Scripts.treatment
{
    public class ModelData
    {
        public int level;
        public string tag;
        public int[] color;
        public List<int> triangles;
        public List<List<int>> verticesIndex=new List<List<int>>();

        public ModelData()
        {

        }

        public ModelData(string tag, int level, int[] color, List<int> triangles, List<List<int>> verticesIndex)
        {
            this.tag = tag;
            this.level = level;
            this.color = color;
            this.triangles = triangles;
            this.verticesIndex = verticesIndex;
        }
    }
}