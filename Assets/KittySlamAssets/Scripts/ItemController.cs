using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ItemController : MonoBehaviour
{

    public float maxCollectionBonus = 100f;
    public float currentCollectionBonus = 100f;
    public TextMeshProUGUI collectionBonusText;
    public Sprite[] sprites;
    public SpriteRenderer[] spriteRenderers;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        int spriteIndex = Random.Range(0, sprites.Length);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    void FixedUpdate()
    {
        // decrease the collection bonus over time (the item is worth less the longer it's on the screen)
        // decrease exponentially, so it's worth 50% of the max value after 1 second, 25% after 2 seconds, etc.
        // limit the minimum value to 10% of the max value
        // GitHub Copilot - Formula
        currentCollectionBonus = Mathf.Max(maxCollectionBonus * Mathf.Pow(0.5f, Time.time - startTime), maxCollectionBonus * 0.1f);
        collectionBonusText.text = currentCollectionBonus.ToString("N0", CultureInfo.InvariantCulture);
    }

    public void OnPickup()
    {
        Destroy(gameObject);
    }
}
