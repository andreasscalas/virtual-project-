using System.Collections;
using System.Collections.Generic;
using Assets._Project.Scripts.treatment;
using UnityEngine;

public class TestMergeList : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj0 = new GameObject() ;
        GameObject obj1 = new GameObject() ;
        GameObject obj2 = new GameObject() ;
        GameObject obj3 = new GameObject() ;
        GameObject obj4 = new GameObject() ;

        List<ControlPointsData> cpDataList = new List<ControlPointsData>();
        cpDataList.Add(new ControlPointsData{/*go = obj0,*/ goTags ={"little finger"}, goColor={Color.red}, goIndex = 0} );
        cpDataList.Add(new ControlPointsData{/*go = obj1,*/ goTags ={"little finger"}, goColor={Color.red}, goIndex = 1} );
        cpDataList.Add(new ControlPointsData{/*go = obj2,*/ goTags ={"pinky"}, goColor={Color.red}, goIndex = 0} );
        cpDataList.Add(new ControlPointsData{/*go = obj3,*/ goTags ={"ring finger"}, goColor={Color.red}, goIndex = 0} );
        cpDataList.Add(new ControlPointsData{/*go = obj4,*/ goTags ={"palm"}, goColor={Color.red}, goIndex = 1} );

        List<ControlPointsData> result = new List<ControlPointsData>();

        while (cpDataList.Count > 0)
        {
            ControlPointsData m = cpDataList[0];
            cpDataList.RemoveAt(0);
            List<ControlPointsData> listB = new List<ControlPointsData>();
            foreach (var VARIABLE in cpDataList)
            {
                if (VARIABLE.goIndex.Equals(m.goIndex))
                {
                    m.goTags.Add(VARIABLE.goTags[0]);
                    m.goColor[0] = (VARIABLE.goColor[0] + m.goColor[0]) / 2;
                }
                else
                {
                    listB.Add(VARIABLE);
                }
            }
            cpDataList = listB;
            result.Add(m);
        }

        foreach (var VARIABLE in result)
        {
            Debug.Log("start of segment");

            Debug.Log(VARIABLE.goIndex.ToString());

            foreach (var item in VARIABLE.goTags)
            {
                Debug.Log(item.ToString());
            }
            Debug.Log("finish of segment");
        }

    }
    
    
}
