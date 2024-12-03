using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawOre : MonoBehaviour
{
    public int totalGemCount = 3;


    public int rarity;
    public string oreName;
    public GameObject roughPrefab;

    public ParticleSystem hitParticleEffect;


    public int GetGemsRemaining()
    {
        return totalGemCount;
    }

    public void DecrementOreCount(int count)
    {
        totalGemCount -= count;
    }


    public string GetOreName()
    {
        return oreName;
    }

    public int GetOreRarity()
    {
        return rarity;
    }

    public GameObject GetRoughPrefab()
    {
        return roughPrefab;
    }

    public void PlayParticleEffect()
    {
        hitParticleEffect.Play();
    }

}
