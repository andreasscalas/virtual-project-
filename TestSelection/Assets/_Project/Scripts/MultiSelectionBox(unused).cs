using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelectionBox : MonoBehaviour
{
    [SerializeField]
    private RectTransform selectSquareImage;
    Vector3 startPos;
    Vector3 endPos;

    void Start()
    {
        selectSquareImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift)))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                startPos = hit.point;
            }
        }

        if ((Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftShift)))
        {
            selectSquareImage.gameObject.SetActive(false);
        }

        if ((Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift)) ||(Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)))
        {
            if (!selectSquareImage.gameObject.activeInHierarchy)
            {
                selectSquareImage.gameObject.SetActive(true);
            }
            endPos = Input.mousePosition;
            Vector3 squareStart = Camera.main.WorldToScreenPoint(startPos);
            squareStart.z = 0f;

            Vector3 center=(squareStart + endPos)/2f;
            selectSquareImage.position = center;
            float sizeX = Mathf.Abs(squareStart.x - endPos.x);
            float sizeY = Mathf.Abs(squareStart.y - endPos.y);
            selectSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
        }

    }
}
