using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    public float speed = 5;
    private Transform transform;

    // Use this for initialization
    void Start()
    {
        transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime);
    }
}