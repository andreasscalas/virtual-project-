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
        public int id;
        public string tag;
        public int level;
        public int father;
        public float[] color;
        public List<int> triangles;
        public List<int> verticesIndex=new List<int>();
        public List<int> cageVerticesIndex=new List<int>();
        public Material defautMaterial;
        public Material outlineMaterial;

        public ModelData()
        {

        }

        public ModelData(int id, string tag, int level, int father, float[] color, List<int> triangles, List<int> verticesIndex, List<int> cageVerticesIndex)
        {
            this.id = id;
            this.tag = tag;
            this.level = level;
            this.father = father;
            this.color = color;
            this.triangles = triangles;
            this.verticesIndex = verticesIndex;
            this.cageVerticesIndex = cageVerticesIndex;
        }
    }
}