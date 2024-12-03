using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    // references
    public TextMeshProUGUI levelText;
    public Image progressionCircle;

    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 1500f;
    public float levelUpMultiplier = 1.5f;

    // Event for when the character levels up (optional for other scripts to listen to)
    public delegate void OnLevelUp(int newLevel);
    public event OnLevelUp LevelUpEvent;

    // Update is called once per frame

    private void Start()
    {
        levelText.text = GetCurrentLevel().ToString();
        UpdateProgressionCircle(); 
    }

    public void AddXP(float xpAmount)
    {

        currentXP = currentXP + xpAmount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateProgressionCircle();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel; 
        xpToNextLevel *= levelUpMultiplier; 

        levelText.text = GetCurrentLevel().ToString(); 


        UpdateProgressionCircle(); 
        // Trigger the level-up event for other scripts to respond
        LevelUpEvent?.Invoke(currentLevel);
    }

    // Function to get the current level
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // Function to get XP progress as a percentage (useful for UI)
    public float GetXPProgress()
    {
        return currentXP / xpToNextLevel;
    }

    // Update the progression circle based on the XP progress
    private void UpdateProgressionCircle()
    {
        if (progressionCircle != null)
        {
            progressionCircle.fillAmount = GetXPProgress();
        }
    }
}
