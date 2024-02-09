using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public long energy = 0;
    public TextMeshProUGUI[] energyTexts;

    private long displayedEnergy = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        displayedEnergy = (long)Mathf.Ceil(Mathf.Lerp(displayedEnergy, energy, 0.06f));
        UpdateEnergyTexts();
    }

    public void IncreaseEnergyInt(int amount)
    {
        energy += (long)amount;
        UpdateEnergyTexts();
    }

    public void IncreaseEnergyFloat(float amount)
    {
        energy += (long)Mathf.Ceil(amount);
        UpdateEnergyTexts();
    }

    public void DecreaseEnergy(int amount)
    {
        energy -= (long)amount;
        UpdateEnergyTexts();
    }

    private void UpdateEnergyTexts()
    {
        // localised number formatting
        foreach (TextMeshProUGUI energyText in energyTexts)
        {
            energyText.text = displayedEnergy.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}