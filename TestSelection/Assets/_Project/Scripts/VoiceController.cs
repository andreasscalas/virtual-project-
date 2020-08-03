using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class VoiceController : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions= new Dictionary<string, Action>();
    public TreatSelectionManager treatslectionManager;
    public MeshCreateControlPoints meshCreateControlPoints;
    [HideInInspector] public ReadJson readJson;
    public DragModel dragModel;
    [HideInInspector] public bool segmentSelect;
    [HideInInspector] public bool segmentDelete;
    public Text voiceSelection;

    void Start()
    {
        segmentSelect = false;
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        dragModel = GameObject.Find("hand").GetComponent<DragModel>();
        actions.Add("select", treatslectionManager.OnSelect);
        actions.Add("discard", treatslectionManager.OnDelete);
        //actions.Add("translate", treatslectionManager.Translation);
        //actions.Add("rotate", treatslectionManager.Rotation);
        //actions.Add("scale", treatslectionManager.Scale);
        //actions.Add("change scale", treatslectionManager.ChangeScale);
        
        actions.Add("select segment", this.SelectSegment);
        actions.Add("discard segment", this.DeleteSegment);
        actions.Add("level zero", readJson.ChangeLevel0);
        actions.Add("level one", readJson.ChangeLevel1);
        actions.Add("level two", readJson.ChangeLevel2);
        actions.Add("change level", dragModel.SwitchLevel);
        actions.Add("scale", dragModel.ChangeScaleOfModel);
        actions.Add("stop", dragModel.StopScaleModel);


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
        voiceSelection.text = "Select (a set of) CPs";
    }

    public void DeleteSegment()
    {
        segmentDelete = true;
        voiceSelection.text = "Discard (a set of) CPs";
    }

}
