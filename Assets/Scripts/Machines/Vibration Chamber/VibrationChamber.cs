using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationChamber : MonoBehaviour
{

    public float currentFuel = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void AddFuel(float amount)
    {
        currentFuel += amount;
        Debug.Log($"Drill refueled. Current fuel: {currentFuel}");
    }

    public float GetFuel()
    {
        return currentFuel;
    }
}
