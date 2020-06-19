using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReadJson))]
public class SegmentEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

        ReadJson readJson = (ReadJson)target;

		GUILayout.BeginHorizontal();

        if (GUILayout.Button("thumb"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.thumb);
            readJson.switchSegment = true;
        }

        if (GUILayout.Button("Index finger"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.indexFinger);
            readJson.switchSegment = true;
            
        }
        

        if (GUILayout.Button("Medium finger"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.mediumFinger);
            readJson.switchSegment = true;
        }
        
        if (GUILayout.Button("Ring finger"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.ringFinger);
            readJson.switchSegment = true;
        }

        if (GUILayout.Button("Little finger"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.littleFinger);
            readJson.switchSegment = true;
        }


        if (GUILayout.Button("palm"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.palm);
            readJson.switchSegment = true;
        }

        if (GUILayout.Button("Wrist"))
        {
            readJson.trianCageSegmented.Clear();
            readJson.trianModelSegmented.Clear();
            readJson.MapSegModel(readJson.wrist);
            readJson.switchSegment = true;
        }

        //if (GUILayout.Button("Medium Finger"))
        //{
        //    readJson.Reset();
        //}

        //if (GUILayout.Button("Index Finger"))
        //{
        //    readJson.Reset();
        //}

        //if (GUILayout.Button("Thumb"))
        //{
        //    readJson.Reset();
        //}

        //if (GUILayout.Button("wrist"))
        //{
        //    readJson.Reset();
        //}

        //if (GUILayout.Button("palm"))
        //{
        //    readJson.Reset();
        //}

        GUILayout.EndHorizontal();
	}

}
