using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.treatment
{
    public class ControlPointsData
    {
        //public GameObject go;
        public List<string> goTags=new List<string>();
        public int goIndex;
        public List<Color> goColor = new List<Color>();

        public ControlPointsData()
        {

        }

        public ControlPointsData(/*GameObject go,*/ List<string> goTags, List<Color> goColor,int goIndex)
        {
            //this.go = go;
            this.goTags = goTags;
            this.goColor = goColor;
            this.goIndex = goIndex;

        }
    }
}
