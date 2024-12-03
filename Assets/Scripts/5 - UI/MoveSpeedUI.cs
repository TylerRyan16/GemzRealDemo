using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSpeedUI : MonoBehaviour
{

    public CameraController cameraController;
    public TextMeshProUGUI moveSpeedText;


    void Update()
    {
        int displaySpeed = Mathf.RoundToInt(cameraController.moveSpeed);
        moveSpeedText.text = displaySpeed.ToString();   
    }
}
