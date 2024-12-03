using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CursorVisibility : MonoBehaviour
{
    public CameraController cameraController;
    private bool cursorHidden = false;

    void Start()
    {
        Cursor.visible = true; // Start with the cursor visible
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingRightClick())
        {
            if (!cursorHidden)
            {
 
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                cursorHidden = true;
            }
            
        } else
        {
            if (cursorHidden)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                cursorHidden = false;
            }
            
        }
    }

    private bool isHoldingRightClick()
    {
        return Input.GetMouseButton(1);
    }
}
