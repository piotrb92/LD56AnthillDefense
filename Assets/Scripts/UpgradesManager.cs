using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    public int anthillUpgradeRequiredSticks = 2;
    public int anthillUpgradeRequiredStones = 2;
    public int anthillUpgradeRequiredFood = 20;
    public int antsUpgradeRequiredFood = 20;

    public float speedBonus = 0;
    public float weightBonus = 0;
    public float healthBonus = 0;
    public float damageBonus = 0;

    public int currentAnthillLevel = 1, currentAntsLevel = 1;

    public TMP_Text anthillLevelText, antsLevelText, anthillReqSticksText, anthillReqStonesText, anthillReqFoodText, antsReqFoodText;

    private void Start()
    {
        UpdateTexts();
    }
    public void UpgradeAnthill()
    {
        if (!CanUpgradeAnthill()) return;
        currentAnthillLevel++;
        GameManager.instance.anthill.currentFood -= anthillUpgradeRequiredFood;
        GameManager.instance.anthill.wood -= anthillUpgradeRequiredSticks;
        GameManager.instance.anthill.stone -= anthillUpgradeRequiredStones;
        AntHill anthill = GameManager.instance.anthill;
        anthill.maxHealth *= 1.1f;
        anthill.health *= 1.1f;
        GameManager.instance.anthill.antSpawnInterval = GameManager.instance.anthill.antSpawnInterval * 0.9f;
        anthillUpgradeRequiredFood = (int)Mathf.Round(anthillUpgradeRequiredFood * 1.5f);
        anthillUpgradeRequiredSticks = (int)Mathf.Round(anthillUpgradeRequiredSticks * 1.5f);
        anthillUpgradeRequiredStones = (int)Mathf.Round(anthillUpgradeRequiredStones * 1.5f);
        UpdateTexts();
       
    }

    public bool CanUpgradeAnthill()
    {
        if (anthillUpgradeRequiredFood > GameManager.instance.anthill.currentFood) return false;
        if (anthillUpgradeRequiredSticks > GameManager.instance.anthill.wood) return false;
        if (anthillUpgradeRequiredStones > GameManager.instance.anthill.stone) return false;
        return true;
    }
    
    public bool CanUpgradeAnts()
    {
        if (antsUpgradeRequiredFood > GameManager.instance.anthill.currentFood) return false;
        return true;
    }

    public void UpdateTexts()
    {
        anthillLevelText.text = currentAnthillLevel.ToString();
        antsLevelText.text = currentAntsLevel.ToString();
        anthillReqSticksText.text = anthillUpgradeRequiredSticks.ToString();
        anthillReqStonesText.text = anthillUpgradeRequiredStones.ToString();
        anthillReqFoodText.text = anthillUpgradeRequiredFood.ToString();
        antsReqFoodText.text = antsUpgradeRequiredFood.ToString();
    }

    public void UpgradeAnts()
    {
        if (!CanUpgradeAnts()) return;
        currentAntsLevel++;
        GameManager.instance.anthill.currentFood -= antsUpgradeRequiredFood;
        speedBonus += 0.1f;
        weightBonus += 0.5f;
        healthBonus += currentAntsLevel;
        damageBonus += 0.5f;
        antsUpgradeRequiredFood = (int)Mathf.Round(antsUpgradeRequiredFood * 1.5f);
        UpdateTexts();
        for (int i = 0; i < GameManager.instance.playersCreatures.Count; i++)
        {
            GameManager.instance.playersCreatures[i].maxHealth += currentAntsLevel;
            GameManager.instance.playersCreatures[i].health += currentAntsLevel;
            GameManager.instance.playersCreatures[i].controller.rb.mass += 0.5f;
        }
    }
}
