using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    public TMP_Text anthillHealthText;
    public TMP_Text antsText;
    public TMP_Text foodText;
    public TMP_Text woodText;
    public TMP_Text stonesText;
    public TMP_Text webText;
    public TMP_Text spacingValue;
    public Slider spacingSlider;

    public GameObject mainMenu, inGameUi;

    private void Start()
    {
        inGameUi.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void SetStats(int health, int ants, int food, int wood, int stones, int web)
    {
        anthillHealthText.text = health.ToString();
        antsText.text = ants.ToString();
        foodText.text = food.ToString();
        woodText.text = wood.ToString();
        stonesText.text = stones.ToString();
        webText.text = web.ToString();
    }

    public void OnSpacingSliderValueChanged()
    {
        GameManager.instance.player.formationSpacing = spacingSlider.value;
        spacingValue.text = spacingSlider.value.ToString("F1");
    }
}
