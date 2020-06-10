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
    public HandRaySelect handRaySelect;
    //public MeshCreateControlPoints meshCreateControlPoints;

    void Start()
    {
        actions.Add("select", handRaySelect.OnSelect);
        actions.Add("delete", handRaySelect.OnDelete);
        actions.Add("translate", treatslectionManager.Translation);
        actions.Add("rotate", treatslectionManager.Rotation);
        actions.Add("scale", treatslectionManager.Scale);
        //actions.Add("change scale", treatslectionManager.ChangeScale);
        

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
