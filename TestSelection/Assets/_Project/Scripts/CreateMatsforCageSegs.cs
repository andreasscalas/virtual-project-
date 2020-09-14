using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CreateMatsforCageSegs : MonoBehaviour
{
    // Start is called before the first frame update
    //static public ReadJson readJson;
    private List<Color> AllSegColors;

    void Start()
    {
         AllSegColors =new List <Color> {Color.red, Color.green, Color.blue};
        //readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        CreateMaterial();
    }

    void CreateMaterial()
    {
        // Create a simple material asset
        
        for (int i = 0; i < AllSegColors.Count; i++)
        {
            var newMaterial = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
            AssetDatabase.CreateAsset(newMaterial, "Assets/" + "Material Test"+i + ".mat");
            newMaterial.SetColor("_Color", AllSegColors[i] );
            newMaterial.SetColor("_OutlineColor", AllSegColors[2-i]);
        }
        
        
    }
}
