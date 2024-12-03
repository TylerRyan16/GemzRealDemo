using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreClicker : MonoBehaviour
{

    public StatsManager statsManager;
    public AudioSource audioSource;
    public PrefabManager prefabManager;
    public ItemCardsDisplay itemCardsDisplay;

    // audio
    public AudioClip clickSound;
    public AudioClip itemAddedSound;
    public AudioClip cannotHitSound;


    private Dictionary<GameObject, int> oreClickCounts = new Dictionary<GameObject, int>(); // To track clicks on each ore

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on this GameObject.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            CheckForOreClick();
        }
    }

    private void CheckForOreClick()
    {
        if (prefabManager.GetCurrentItem() != null)
        {
            return;
        }

        // dont check for clicks if info card open
        if (itemCardsDisplay.IsInfoCardOpen()) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the "ore" tag
            if (hit.collider.CompareTag("ore"))
            {
                // Get the RawOre component
                RawOre rawOre = hit.collider.GetComponent<RawOre>();

                if (rawOre != null)
                {
                    // shake the ore
                    StartCoroutine(ShakeOre(hit.collider.gameObject));

                    // ONLY DO ANYTHING IF ITS COAL
                    if (rawOre.GetOreName() == "Coal")
                    {
                        // Increment the click count for this ore
                        if (!oreClickCounts.ContainsKey(hit.collider.gameObject))
                        {
                            oreClickCounts[hit.collider.gameObject] = 0;
                        }
                        oreClickCounts[hit.collider.gameObject]++;

                        // Play the click sound
                        PlaySound(clickSound);

                        // Reduce totalItemCount every 3 clicks
                        if (oreClickCounts[hit.collider.gameObject] % 3 == 0)
                        {
                            rawOre.DecrementOreCount(statsManager.GetOreAmountPerCycle());

                            if (rawOre.GetOreName() == "Coal")
                            {
                                rawOre.PlayParticleEffect();

                                statsManager.AddCoalToInventory(statsManager.GetOreAmountPerCycle());
                                statsManager.UpdateStatsInfo();
                            }

                            // Play the item added sound
                            PlaySound(itemAddedSound);

                            // If no items are remaining, destroy the ore
                            if (rawOre.GetGemsRemaining() <= 0)
                            {
                                Destroy(hit.collider.gameObject);
                                oreClickCounts.Remove(hit.collider.gameObject);
                            }
                        }
                    
                    } 
                    else
                    {
                        PlaySound(cannotHitSound);
                    }
                }
            }
        }
    }

    private IEnumerator ShakeOre(GameObject ore)
    {
        Vector3 originalPosition = ore.transform.position;
        float duration = 0.14f; // Duration of the shake
        float magnitude = 0.05f; // Magnitude of the shake
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);

            ore.transform.position = new Vector3(
                originalPosition.x + offsetX,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to the original position
        ore.transform.position = originalPosition;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}

