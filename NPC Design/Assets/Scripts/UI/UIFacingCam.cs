using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFacingCam : MonoBehaviour
{
    private Canvas canvas;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        canvas = GetComponent<Canvas>();

        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = cam;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
