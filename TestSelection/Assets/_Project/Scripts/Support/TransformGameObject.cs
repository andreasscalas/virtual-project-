using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Support
{
    class TransformGameObject
    {
        public GameObject GameObject;

        public Vector3 Position = new Vector3();

        public Quaternion Rotation = new Quaternion();

        public Vector3 Scale = new Vector3();

        public TransformGameObject()
        {
            
        }

        public TransformGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.GameObject = gameObject;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }




    }
}
