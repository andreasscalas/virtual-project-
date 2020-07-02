using Assets._Project.Scripts.treatment;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LinqGroup
{
    class program : MonoBehaviour
    {
         void Start()
         {
            var users = new List<User>()
            {
                new User { Name = "John Doe", Age = 42, HomeCountry = "USA" },
                new User { Name = "Jane Doe", Age = 38, HomeCountry = "USA" },
                new User { Name = "Joe Doe", Age = 19, HomeCountry = "Germany" },
                new User { Name = "Jenna Doe", Age = 19, HomeCountry = "Germany" },
                new User { Name = "James Doe", Age = 8, HomeCountry = "USA" },
            };

            var cpDataList = new List<ControlPointsData>();
            cpDataList.Add(new ControlPointsData {/*go = obj0,*/ goTags = { "little finger", "palm" }, goColor = { Color.red }, goIndex = 0 });
            cpDataList.Add(new ControlPointsData {/*go = obj1,*/ goTags = { "little finger" }, goColor = { Color.red }, goIndex = 1 });
            cpDataList.Add(new ControlPointsData {/*go = obj2,*/ goTags = { "pinky" }, goColor = { Color.red }, goIndex = 0 });
            cpDataList.Add(new ControlPointsData {/*go = obj3,*/ goTags = { "ring finger" }, goColor = { Color.red }, goIndex = 0 });
            cpDataList.Add(new ControlPointsData {/*go = obj4,*/ goTags = { "palm" }, goColor = { Color.red }, goIndex = 1 });

            //var usersGroupedByCountry = users.GroupBy(user => user.HomeCountry);
            //foreach (var group in usersGroupedByCountry)
            //{
            //    Debug.Log("Users from " + group.Key + ":");
            //    foreach (var user in group)
            //        Debug.Log("* " + user.Name);
            //}

            var tagsGroupedByIndex = cpDataList.GroupBy(x => x.goColor[0]);
            foreach (var group in tagsGroupedByIndex)
            {
                Debug.Log("Users from " + group.Key + ":");
                foreach (var x in group)
                    Debug.Log("* " + x.goIndex);
            }
         }

        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string HomeCountry { get; set; }
        }
    }
}