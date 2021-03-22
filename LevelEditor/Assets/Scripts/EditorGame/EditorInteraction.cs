using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorInteraction : MonoBehaviour
{
    public float scaleOffset;

    public GameObject editorObjects;

    public GameObject axisPosition;
    public GameObject axisRotation;
    public GameObject axisScale;

    public TextMesh text;


    private GameObject currentObj;

    private RaycastHit hit;

    private void Update()
    {
        editorObjects.SetActive(currentObj != null);

        if (currentObj != null)
        {
            editorObjects.transform.position = currentObj.transform.position;
            editorObjects.transform.rotation = currentObj.transform.rotation;

            text.text = currentObj.name;

            text.transform.LookAt(transform);

            float scale = Vector3.Distance(currentObj.transform.position, transform.position) * scaleOffset;
            editorObjects.transform.localScale = new Vector3(scale, scale, scale);
        }    
        else
        {
            text.text = "";
        }

        if(Input.GetButtonDown("Fire1"))
        {
            SelectRay();
        }
    }

    private void SelectRay()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out hit))
        {
            currentObj = hit.transform.gameObject;
        }
        else
        {
            currentObj = null;
        }
    }
}
