using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceController : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions= new Dictionary<string, Action>();
    public TreatSelectionManager treatslectionManager;
    public MeshCreateControlPoints meshCreateControlPoints;
    [HideInInspector] public ReadJson readJson;
    public bool segmentSelect;
    public bool segmentDelete;

    void Start()
    {
        segmentSelect = false;
        GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        actions.Add("select", treatslectionManager.OnSelect);
        actions.Add("remove", treatslectionManager.OnDelete);
        actions.Add("translate", treatslectionManager.Translation);
        //actions.Add("rotate", treatslectionManager.Rotation);
        //actions.Add("scale", treatslectionManager.Scale);
        //actions.Add("change scale", treatslectionManager.ChangeScale);
        
        actions.Add("select segment", this.SelectSegment);
        actions.Add("delete segment", this.DeleteSegment);
        actions.Add("level zero", readJson.ChangeLevel0);
        actions.Add("level one", readJson.ChangeLevel1);
        actions.Add("level two", readJson.ChangeLevel2);


        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    public void SelectSegment()
    {
        segmentSelect = true;
    }

    public void DeleteSegment()
    {
        segmentDelete = true;
    }

}
