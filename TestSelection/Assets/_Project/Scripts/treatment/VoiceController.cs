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

    void Start()
    {
        actions.Add("translation", treatslectionManager.Translation);
        actions.Add("rotation", treatslectionManager.Rotation);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void forward()
    {
        transform.Translate(1, 0, 0);
    }

    private void backward()
    {
        transform.Translate(-1, 0, 0);
    }
}
