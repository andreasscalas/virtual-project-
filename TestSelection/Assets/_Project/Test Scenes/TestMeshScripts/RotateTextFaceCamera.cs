using UnityEngine;
// This complete script can be attached to a camera to make it
// continuously point at another object.

public class RotateTextFaceCamera : MonoBehaviour
{
    //public Transform target;

    void Start()
    {
        var theText = new GameObject();

        var textMesh = theText.AddComponent<TextMesh>();
        var meshRenderer = theText.AddComponent<MeshRenderer>();

        // do some settings here that are needed in the component
        // set the text
        textMesh.text = "Worldddddddddddd!";
        textMesh.fontSize = 89;
        textMesh.anchor = TextAnchor.MiddleCenter;
        theText.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);

    }

    void Update()
    {
        // Rotate the camera every frame so it keeps looking at the target
        this.transform.rotation=Camera.main.transform.rotation;

        // Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
        //transform.LookAt(target, Vector3.left);
    }
}

