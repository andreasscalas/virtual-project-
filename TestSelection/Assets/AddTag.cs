using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddTag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string tag = "I'm a tag";
        var customTag0= gameObject.GetComponent<CustomTag>();
        //customTag0.GetTags();
        //var customTag1= gameObject.GetComponent<CustomTag>();
        //customTag0 = "verbatim C# text";
        //var customTag2= gameObject.tag;
        Debug.Log("customTag");
        customTag0.Add("verbatim C# text");
        //Debug.Log("customTag " + customTag0.GetAtIndex(3));
        Debug.Log("customTag" + customTag0);
        //Debug.Log("GetTags().Count"+customTag.GetTags().Count());
        //Debug.Log("HasTag()a"+customTag.HasTag("Finish"));
        //Debug.Log("HasTag()b"+customTag.HasTag("Fiinish"));
        //Debug.Log(customTag.GetTags());
        //for (int i = 0; i < customTag.Count; i++)
        //{
        //    customTag[i] = tag;
        //}
    }


}
