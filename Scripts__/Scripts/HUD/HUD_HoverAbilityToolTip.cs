using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HUD_HoverAbilityToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image AbilityToolTipWindow;
    [SerializeField] private Sprite NoVisual_Sprite;
    [SerializeField] private Sprite Window_Sprite;
    [SerializeField] private GameObject playerHeroClass;

    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI abilityCostText;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private TextMeshProUGUI abilityPerLevelText;

    [SerializeField] private string abilityInit;
    private HeroClass heroClass; private bool SetSkill = false;

    public void Start()
    {
        if (!SetSkill) { return; }
        playerHeroClass = GameObject.FindGameObjectWithTag("MyPlayer");
        heroClass = playerHeroClass.GetComponent<HeroClass>();
        
        AbilityToolTipWindow.sprite = NoVisual_Sprite;
        abilityNameText.text = "";
        abilityCostText.text = "";
        abilityDescriptionText.text = "";
        abilityPerLevelText.text = "";
    }
    public void SetHero(HeroClass playerSl)
    {
        heroClass = playerSl;
        AbilityToolTipWindow.sprite = NoVisual_Sprite;
        abilityNameText.text = "";
        abilityCostText.text = "";
        abilityDescriptionText.text = "";
        abilityPerLevelText.text = "";
        SetSkill = true;
    }
    public void UpdateAbilityTooltip()
    {
        if (!SetSkill) { return; }
        switch (heroClass.HeroName)
        {
            case "Ekard":
                switch (abilityInit)
                {
                    case "Q":
                        heroClass.GetUpdatedStats_Ekard(heroClass.Q_Ability, false);
                        abilityNameText.text = heroClass.Q_Ability.abilityName;
                        abilityCostText.text = heroClass.Q_Ability.abilityCooldown.ToString() + " secs.\n" + heroClass.Q_Ability.abilityCost.ToString() + " Mana";
                        abilityDescriptionText.text = heroClass.Q_Ability.abilityDescription;
                        abilityPerLevelText.text = heroClass.Q_Ability.abilityPerLevel;
                        break;
                    case "W":
                        heroClass.GetUpdatedStats_Ekard(heroClass.W_Ability, false);
                        abilityNameText.text = heroClass.W_Ability.abilityName;
                        abilityCostText.text = heroClass.W_Ability.abilityCooldown.ToString() + " secs.\n" + heroClass.W_Ability.abilityCost.ToString() + " Mana";
                        abilityDescriptionText.text = heroClass.W_Ability.abilityDescription;
                        abilityPerLevelText.text = heroClass.W_Ability.abilityPerLevel;
                        break;
                    case "E":
                        heroClass.GetUpdatedStats_Ekard(heroClass.E_Ability, false);
                        abilityNameText.text = heroClass.E_Ability.abilityName;
                        abilityCostText.text = heroClass.E_Ability.abilityCooldown.ToString() + " secs.\n" + heroClass.E_Ability.abilityCost.ToString() + " Mana";
                        abilityDescriptionText.text = heroClass.E_Ability.abilityDescription;
                        abilityPerLevelText.text = heroClass.E_Ability.abilityPerLevel;
                        break;
                    case "R":
                        heroClass.GetUpdatedStats_Ekard(heroClass.R_Ability, false);
                        abilityNameText.text = heroClass.R_Ability.abilityName;
                        abilityCostText.text = heroClass.R_Ability.abilityCooldown.ToString() + " secs.\n" + heroClass.R_Ability.abilityCost.ToString() + " Mana";
                        abilityDescriptionText.text = heroClass.R_Ability.abilityDescription;
                        abilityPerLevelText.text = heroClass.R_Ability.abilityPerLevel;
                        break;
                    default:
                        Debug.Log("Missing ability reference?");
                        break;
                }
                break;

            default:
                Debug.Log("Missing hero reference?", this);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilityToolTipWindow.sprite = Window_Sprite;
        UpdateAbilityTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityToolTipWindow.sprite = NoVisual_Sprite;
        abilityNameText.text = "";
        abilityCostText.text = "";
        abilityDescriptionText.text = "";
        abilityPerLevelText.text = "";
    }
}
