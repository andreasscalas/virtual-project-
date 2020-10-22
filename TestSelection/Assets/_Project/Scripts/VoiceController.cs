using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class VoiceController : MonoBehaviour
{
    private readonly Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public DragModel dragModel;
    private KeywordRecognizer keywordRecognizer;
    public MeshCreateControlPoints meshCreateControlPoints;
    [HideInInspector] public ReadJson readJson;
    [HideInInspector] public bool segmentDelete;
    [HideInInspector] public bool segmentSelect;
    public TreatSelectionManager treatslectionManager;
    public Text voiceSelection;

    private void Start()
    {
        segmentSelect = false;
        readJson = GameObject.Find("Selection Manager").GetComponent<ReadJson>();
        //dragModel = GameObject.Find("hand").GetComponent<DragModel>();
        dragModel = GameObject.Find("flowered_teapot_simplified").GetComponent<DragModel>();
        actions.Add("select", treatslectionManager.OnSelect);
        actions.Add("discard", treatslectionManager.OnDelete);
        actions.Add("select segment", treatslectionManager.OnSelectSegment);
        actions.Add("discard segment", treatslectionManager.OnDeleteSegment);
        actions.Add("level zero", readJson.ChangeLevel0);
        actions.Add("level one", readJson.ChangeLevel1);
        actions.Add("level two", readJson.ChangeLevel2); //"lowest level"; "highest level" as key word
        actions.Add("change level", dragModel.SwitchLevel);
        actions.Add("scale", dragModel.ChangeScaleOfModel);
        actions.Add("stop scaling", dragModel.StopScaleModel);


        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }


}