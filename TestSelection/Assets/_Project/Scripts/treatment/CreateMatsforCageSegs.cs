using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CreateMatsforCageSegs : MonoBehaviour
{
    // Start is called before the first frame update
    static public ReadJson readJson;

    void Start()
    {
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        CreateMaterial();
    }

    static void CreateMaterial()
    {
        // Create a simple material asset
        
        for (int i = 0; i < readJson.AllSegColors.Count; i++)
        {
            var newMaterial = new Material(Shader.Find("Diffuse"));
            AssetDatabase.CreateAsset(newMaterial, "Assets/Resources/" + "MaterialSeg"+i + ".mat");
            newMaterial.color =new Color(readJson.AllSegColors[i][0],readJson.AllSegColors[i][1],readJson.AllSegColors[i][2])/255;
        }
        
        
    }
}
