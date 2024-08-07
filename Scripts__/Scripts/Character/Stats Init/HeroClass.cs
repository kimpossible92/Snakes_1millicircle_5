using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PlayerSystem;
using Photon.Realtime;

public class HeroClass : MonoBehaviourPun, IPunObservable
{
    public enum HeroAssigner { Ekard, Jawn }
    public HeroAssigner heroAssigner;
    [DisplayWithoutEdit] public string HeroName;
    [DisplayWithoutEdit] public Sprite heroHUDSprite;
    public string setLocalId;
    [Header("Hero Stats")]
    [DisplayWithoutEdit] public int heroLevel;
    [DisplayWithoutEdit] public int heroExp;
    [DisplayWithoutEdit] public int heroExpToNext;
    [DisplayWithoutEdit] public int heroSkillPoints;
    [DisplayWithoutEdit] public bool heroCanLevelUltimate;

    [SerializeField] public float heroHealth;
    [DisplayWithoutEdit] public float heroMaxHealth;

    [DisplayWithoutEdit] public float heroDef;
    [DisplayWithoutEdit] public float heroRes;

    [DisplayWithoutEdit] public float heroMana;
    [DisplayWithoutEdit] public float heroMaxMana;

    [DisplayWithoutEdit] public float heroAttackDmg;
    [DisplayWithoutEdit] public float heroAbilityDmg;

    [DisplayWithoutEdit] public float heroAttackSpeed;
    [DisplayWithoutEdit] public float heroAttackTime;
    [DisplayWithoutEdit] public float heroCritChance;

    [DisplayWithoutEdit] public float heroLifesteal;
    [DisplayWithoutEdit] public float heroVamp;

    [DisplayWithoutEdit] public float heroSpeed;
    [DisplayWithoutEdit] public float heroTenacity;

    [DisplayWithoutEdit] public float heroHealthRegen;
    [DisplayWithoutEdit] public float heroManaRegen;

    [DisplayWithoutEdit] public float heroCooldownReduction;

    [Header("Hero Stats Per Level")]
    [DisplayWithoutEdit] public float heroMaxHealthPerLevel;
    [DisplayWithoutEdit] public float heroMaxManaPerLevel;

    [DisplayWithoutEdit] public float heroDefPerLevel;
    [DisplayWithoutEdit] public float heroResPerLevel;

    [DisplayWithoutEdit] public float heroAttackDmgPerLevel;
    [DisplayWithoutEdit] public float heroAttackSpeedPerLevel;

    [Header("Q Ability")]
    public AbilityClass Q_Ability;
    [SerializeField] private Sprite Q_Indicator;
    [SerializeField] private Image Q_HudIcon;
    [SerializeField] private Sprite Q_Range;

    [Header("W Ability")]
    public AbilityClass W_Ability;
    [SerializeField] private Sprite W_Indicator;
    [SerializeField] private Image W_HudIcon;
    [SerializeField] private Sprite W_Range;

    [Header("E Ability")]
    public AbilityClass E_Ability;
    [SerializeField] private Sprite E_Indicator;
    [SerializeField] private Image E_HudIcon;
    [SerializeField] private Sprite E_Range;

    [Header("R Ability")]
    public AbilityClass R_Ability;
    [SerializeField] private Sprite R_Indicator;
    [SerializeField] private Image R_HudIcon;
    [SerializeField] private Sprite R_Range;

    [Header("Misc. Components")]
    [SerializeField] GameObject GameControlScriptObject;

    GameControl_Script gsScript;
    HeroCombat heroCombatScript;

    private CharacterMovementScript moveScript;

    private KeyCode Q_Ability_Hotkey;
    private KeyCode W_Ability_Hotkey;
    private KeyCode E_Ability_Hotkey;
    private KeyCode R_Ability_Hotkey;

    HeroClass()
    {
        heroLevel = 1;
        heroExp = 0;
        heroExpToNext = 50;
        heroSkillPoints = 1;

        heroHealth = 100f;
        heroMaxHealth = heroHealth;

        heroMana = 100f;
        heroMaxMana = heroMana;

        heroAttackDmg = 5f;
        heroAbilityDmg = 5f;

        heroAttackSpeed = 1;
        heroAttackTime = 1.4f;

        heroDef = 1f;
        heroRes = 1f;

        heroCritChance = 0f;
        heroLifesteal = 0f;
        heroVamp = 0f;

        heroSpeed = 5f;
        heroTenacity = 0f;

        heroHealthRegen = 1.0f;
        heroManaRegen = 2.0f;

        heroCooldownReduction = 0.0f;
    }

    void Awake()
    {
        GetPlayerHotkeys();
        HeroName = heroAssigner.ToString();
        CallHeroInit(heroAssigner);

        moveScript = GetComponent<CharacterMovementScript>();
        heroCombatScript = GetComponent<HeroCombat>();
    }

    void Update()
    {
        heroSpeed = moveScript.agent.speed;
        
        foreach (Player2 p in PhotonNetwork.PlayerList)
        {
            //if (p.IsLocal)
            //{ 
            //    FindObjectOfType<OverviewPanel>().setHealth(p.ActorNumber, Photon.Pun.PhotonNetwork.NickName, (int)heroHealth); 
            //}
            if (p.IsLocal)
            {
                FindObjectOfType<OverviewPanel>().setHealth(p.ActorNumber, Photon.Pun.PhotonNetwork.NickName, (int)heroHealth);
            }
            else 
            { 
                FindObjectOfType<OverviewPanel>().setHealth(p.ActorNumber, "playerlocal", (int)heroHealth); 
            }
        }
    }

    void GetPlayerHotkeys()
    {
        GameControlScriptObject = FindObjectOfType<GameControl_Script>().gameObject;
        gsScript = GameControlScriptObject.GetComponent<GameControl_Script>();
        Q_Ability_Hotkey = gsScript.Q_Ability_Keycode;
        W_Ability_Hotkey = gsScript.W_Ability_Keycode;
        E_Ability_Hotkey = gsScript.E_Ability_Keycode;
        R_Ability_Hotkey = gsScript.R_Ability_Keycode;
    }

    void CallHeroInit(HeroAssigner hero)
    {
        switch (heroAssigner)
        {
            case HeroAssigner.Ekard:
                Hero_Ekard_Ability_Init();
                //Debug.Log("Assigned Hero is " + HeroName);
                break;

            case HeroAssigner.Jawn:
                //Debug.Log("Assigned Hero is " + HeroName);
                break;

            default:
                //Debug.Log("No Hero Assigned!");
                break;
        }
    }

    void Load_HeroHUDSprite(HeroAssigner _assignedHero)
    {
        heroAssigner = _assignedHero;

        switch (heroAssigner)
        {
            case (HeroAssigner.Ekard):
                heroHUDSprite = Resources.Load<Sprite>("HeroHUDIcons/Ekard_HUD_Sprite");
                //Debug.Log("Loaded " + heroAssigner.ToString() + " sprite");
                break;

            default:
                Debug.Log("No Hero found?!");
                break;
        }
    }

    // Hero inits

    public void GetUpdatedStats_Ekard(AbilityClass _ability, bool levelingAbility)
    {
        if (_ability == Q_Ability)
        {   
            if (levelingAbility)
            {
                // Update CDR TEMP
                // Q_Ability.abilityCooldown = Q_Ability.abilityCooldown - (Q_Ability.abilityCooldown * (heroCooldownReduction / 100));
                // Debug.Log($"{Q_Ability.abilityCooldown} - {Q_Ability.abilityCooldown} * {heroCooldownReduction}");

                switch (Q_Ability.abilityLevel)
                {
                    case 1:
                        Q_Ability.abilityBaseDamage = Q_Ability.abilityDamagePerLevel[0];
                        Q_Ability.abilityCost = Q_Ability.abilityCostPerLevel[0];
                        break;

                    case 2:
                        Q_Ability.abilityBaseDamage = Q_Ability.abilityDamagePerLevel[1];
                        Q_Ability.abilityCost = Q_Ability.abilityCostPerLevel[1];
                        break;

                    case 3:
                        Q_Ability.abilityBaseDamage = Q_Ability.abilityDamagePerLevel[2];
                        Q_Ability.abilityCost = Q_Ability.abilityCostPerLevel[2];
                        break;

                    case 4:
                        Q_Ability.abilityBaseDamage = Q_Ability.abilityDamagePerLevel[3];
                        Q_Ability.abilityCost = Q_Ability.abilityCostPerLevel[3];
                        break;

                    case 5:
                        Q_Ability.abilityBaseDamage = Q_Ability.abilityDamagePerLevel[4];
                        Q_Ability.abilityCost = Q_Ability.abilityCostPerLevel[4];
                        break;
                }
            }

            Debug.Log("Stats Updated!", this);

            Q_Ability.abilityDescription = Q_Ability.abilityDescription = string.Format(
                "{0} casts a medium range fireball in a line, dealing {1} (+{2}) ability damage to the first enemy hit.", HeroName, Q_Ability.abilityBaseDamage, heroAbilityDmg * Q_Ability.abilityScaling);
        }
        else if (_ability == W_Ability)
        {
            if (levelingAbility)
            {
                switch (W_Ability.abilityLevel)
                {
                    case 1:
                        W_Ability.abilityBaseDamage = W_Ability.abilityDamagePerLevel[0];
                        W_Ability.abilityCost = W_Ability.abilityCostPerLevel[0];
                        W_Ability.abilityCooldown = W_Ability.abilityCooldownPerLevel[0];
                        break;

                    case 2:
                        W_Ability.abilityBaseDamage = W_Ability.abilityDamagePerLevel[1];
                        W_Ability.abilityCost = W_Ability.abilityCostPerLevel[1];
                        W_Ability.abilityCooldown = W_Ability.abilityCooldownPerLevel[1];
                        break;

                    case 3:
                        W_Ability.abilityBaseDamage = W_Ability.abilityDamagePerLevel[2];
                        W_Ability.abilityCost = W_Ability.abilityCostPerLevel[2];
                        W_Ability.abilityCooldown = W_Ability.abilityCooldownPerLevel[2];
                        break;

                    case 4:
                        W_Ability.abilityBaseDamage = W_Ability.abilityDamagePerLevel[3];
                        W_Ability.abilityCost = W_Ability.abilityCostPerLevel[3];
                        W_Ability.abilityCooldown = W_Ability.abilityCooldownPerLevel[3];
                        break;

                    case 5:
                        W_Ability.abilityBaseDamage = W_Ability.abilityDamagePerLevel[4];
                        W_Ability.abilityCost = W_Ability.abilityCostPerLevel[4];
                        W_Ability.abilityCooldown = W_Ability.abilityCooldownPerLevel[4];
                        break;
                }
            }

            W_Ability.abilityDescription = string.Format(
                "{0} creates an explosion of light at a target location, dealing {1} (+{2}) AOE ability damage. Can be cast while moving.", HeroName, W_Ability.abilityBaseDamage, heroAbilityDmg * W_Ability.abilityScaling);
        }
        else if (_ability == E_Ability)
        {
            if (levelingAbility)
            {
                switch (E_Ability.abilityLevel)
                {
                    case 1:
                        E_Ability.abilityBuffPercentage = E_Ability.abilityBuffPercentagePerLevel[0];
                        E_Ability.abilityDuration = E_Ability.abilityDurationPerLevel[0];
                        break;

                    case 2:
                        E_Ability.abilityBuffPercentage = E_Ability.abilityBuffPercentagePerLevel[1];
                        E_Ability.abilityDuration = E_Ability.abilityDurationPerLevel[1];
                        break;

                    case 3:
                        E_Ability.abilityBuffPercentage = E_Ability.abilityBuffPercentagePerLevel[2];
                        E_Ability.abilityDuration = E_Ability.abilityDurationPerLevel[2];
                        break;

                    case 4:
                        E_Ability.abilityBuffPercentage = E_Ability.abilityBuffPercentagePerLevel[3];
                        E_Ability.abilityDuration = E_Ability.abilityDurationPerLevel[3];
                        break;

                    case 5:
                        E_Ability.abilityBuffPercentage = E_Ability.abilityBuffPercentagePerLevel[4];
                        E_Ability.abilityDuration = E_Ability.abilityDurationPerLevel[4];
                        break;
                }
            }

            E_Ability.abilityDescription = string.Format(
                "{0} makes haste, gaining a burst of ({1}%) movement speed and attack speed for ({2}) seconds. Acts as an auto-attack reset.", HeroName, E_Ability.abilityBuffPercentage * 100, E_Ability.abilityDuration);
        }
        else if (_ability == R_Ability)
        {
            if (levelingAbility)
            {
                switch (R_Ability.abilityLevel)
                {
                    case 1:
                        R_Ability.abilityBaseDamage = R_Ability.abilityDamagePerLevel[0];
                        R_Ability.abilityCooldown = R_Ability.abilityCooldownPerLevel[0];
                        break;

                    case 2:
                        R_Ability.abilityBaseDamage = R_Ability.abilityDamagePerLevel[1];
                        R_Ability.abilityCooldown = R_Ability.abilityCooldownPerLevel[1];
                        break;

                    case 3:
                        R_Ability.abilityBaseDamage = R_Ability.abilityDamagePerLevel[2];
                        R_Ability.abilityCooldown = R_Ability.abilityCooldownPerLevel[2];
                        break;
                }
            }

            R_Ability.abilityDescription = string.Format(
                "{0} conjures a massive fire bomb at an enemy, dealing {1} (+{2}) ability damage. ", HeroName, R_Ability.abilityBaseDamage, heroAbilityDmg * R_Ability.abilityScaling);
        }
        else
        {
            //Debug.Log("Missing ability reference", this);
        }
    }

    void Hero_Ekard_Ability_Init()
    {
        ///
        /// Hero - Ekard Stat Init
        /// 

        heroLevel = 5;
        heroExp = 0;
        heroExpToNext = 50;
        Load_HeroHUDSprite(HeroAssigner.Ekard);

        heroHealth = 450f;
        heroMaxHealth = heroHealth;

        heroMana = 275f;
        heroMaxMana = heroMana;

        heroAttackDmg = 25f;
        heroAbilityDmg = 0f;

        heroAttackSpeed = .85f;
        heroAttackTime = 1.4f;

        heroDef = 4f;
        heroRes = 3f;

        heroCritChance = 0f;
        heroLifesteal = 0f;
        heroVamp = 0f;

        heroSpeed = 5f; // this needs to be set in the NavMesh Agent
        heroTenacity = 0f;

        heroHealthRegen = 2.0f;
        heroManaRegen = 5.0f;

        heroCooldownReduction = 0.0f;

        // Per Level Scalings
        heroMaxHealthPerLevel = 45f;
        heroMaxManaPerLevel = 55f;

        heroDefPerLevel = 1.5f;
        heroResPerLevel = 1.0f;

        heroAttackDmgPerLevel = 1.5f;
        heroAttackSpeedPerLevel = .05f;

        ///
        /// Q Abililty Init
        ///

        /// Ability Summary:
        /// Shoots a medium range projectile in a line, dealing ability damage to first enemy hit.

        Q_Ability = new AbilityClass();
        Q_Ability.typeOfAbilityCast = AbilityClass.TypeOfAbilityCast.Skillshot;
        Q_Ability.typeOfAbilityCC = AbilityClass.TypeOfAbilityCC.None;
        Q_Ability.typeOfAbilityDamage = AbilityClass.TypeOfAbilityDamage.AbilityDamage;

        Q_Ability.abilityName = "Fireball";
        Q_Ability.abilityCooldown = 2.5f;
        Q_Ability.HUDIcon = Q_HudIcon;
        Q_Ability.Indicator = Q_Indicator;
        Q_Ability.Range = Q_Range;
        Q_Ability.abilityKeyCode = Q_Ability_Hotkey;

        Q_Ability.abilityMaxLevel = 5;

        Q_Ability.abilityRangeNum = 6.5f;

        Q_Ability.abilityDuration = 0f;
        Q_Ability.abilityBuffPercentage = 0f;

        Q_Ability.abilitySpeed = 600f; // average speed
        Q_Ability.abilityCastTime = 0f;

        Q_Ability.abilityBaseDamage = 60f;
        Q_Ability.abilityScaling = 1f;

        Q_Ability.abilityCost = 15f;
        Q_Ability.abilityDescription = string.Format("{0} casts a medium range fireball in a line, dealing {1} (+{2}) ability damage to the first enemy hit.", HeroName, Q_Ability.abilityBaseDamage, heroAbilityDmg * Q_Ability.abilityScaling);
        Q_Ability.abilityPerLevel = string.Format("Damage: 60 / 80 / 110 / 130 / 150\nCost: 15 / 20 / 25 / 30 / 30");

        Q_Ability.abilityDamagePerLevel = new float[5];
        Q_Ability.abilityDamagePerLevel[0] = 60;
        Q_Ability.abilityDamagePerLevel[1] = 80;
        Q_Ability.abilityDamagePerLevel[2] = 110;
        Q_Ability.abilityDamagePerLevel[3] = 130;
        Q_Ability.abilityDamagePerLevel[4] = 150;

        Q_Ability.abilityCostPerLevel = new float[5];
        Q_Ability.abilityCostPerLevel[0] = 15;
        Q_Ability.abilityCostPerLevel[1] = 20;
        Q_Ability.abilityCostPerLevel[2] = 25;
        Q_Ability.abilityCostPerLevel[3] = 30;
        Q_Ability.abilityCostPerLevel[4] = 30;

        ///
        /// W Abililty Init
        ///

        /// Ability Summary:
        /// Cast a short range explosion of light at a target location within a certain range, dealing aoe damage.
        /// 

        W_Ability = new AbilityClass();
        W_Ability.typeOfAbilityCast = AbilityClass.TypeOfAbilityCast.AOESkillShot;
        W_Ability.typeOfAbilityCC = AbilityClass.TypeOfAbilityCC.None;
        W_Ability.typeOfAbilityDamage = AbilityClass.TypeOfAbilityDamage.AbilityDamage;

        W_Ability.abilityName = "Light Explosion";
        W_Ability.abilityCooldown = 9f;
        W_Ability.HUDIcon = W_HudIcon;
        W_Ability.Indicator = W_Indicator;
        W_Ability.Range = W_Range;
        W_Ability.abilityKeyCode = W_Ability_Hotkey;

        W_Ability.abilityMaxLevel = 5;

        W_Ability.abilityRangeNum = 4.7f;

        W_Ability.abilityDuration = 0f;
        W_Ability.abilityBuffPercentage = 0f;

        W_Ability.abilitySpeed = 0f; // it doesn't move
        W_Ability.abilityCastTime = 0f;

        W_Ability.abilityBaseDamage = 90f;
        W_Ability.abilityScaling = .3f;

        W_Ability.abilityCost = 45f;
        W_Ability.abilityDescription = string.Format("{0} creates an explosion of light at a target location, dealing {1} (+{2}) AOE ability damage. Can be cast while moving.", HeroName, W_Ability.abilityBaseDamage, heroAbilityDmg * W_Ability.abilityScaling);
        W_Ability.abilityPerLevel = string.Format("Damage: 90 / 110 / 130 / 140 / 150\nCost: 45 / 50 / 55 / 60 / 60\nCooldown: 9 / 8.75 / 8.25 / 7.75 / 6");

        W_Ability.abilityDamagePerLevel = new float[5];
        W_Ability.abilityDamagePerLevel[0] = 90;
        W_Ability.abilityDamagePerLevel[1] = 110;
        W_Ability.abilityDamagePerLevel[2] = 130;
        W_Ability.abilityDamagePerLevel[3] = 140;
        W_Ability.abilityDamagePerLevel[4] = 150;

        W_Ability.abilityCostPerLevel = new float[5];
        W_Ability.abilityCostPerLevel[0] = 45;
        W_Ability.abilityCostPerLevel[1] = 50;
        W_Ability.abilityCostPerLevel[2] = 55;
        W_Ability.abilityCostPerLevel[3] = 60;
        W_Ability.abilityCostPerLevel[4] = 60;

        W_Ability.abilityCooldownPerLevel = new float[5];
        W_Ability.abilityCooldownPerLevel[0] = 9;
        W_Ability.abilityCooldownPerLevel[1] = 8.75f;
        W_Ability.abilityCooldownPerLevel[2] = 8.25f;
        W_Ability.abilityCooldownPerLevel[3] = 7.75f;
        W_Ability.abilityCooldownPerLevel[4] = 6;

        ///
        /// E Abililty Init
        ///

        /// Ability Summary:
        /// Cast to gain a burst of movement speed and attack speed for a few seconds.
        /// 

        E_Ability = new AbilityClass();
        E_Ability.typeOfAbilityCast = AbilityClass.TypeOfAbilityCast.SelfBuff;
        E_Ability.typeOfAbilityCC = AbilityClass.TypeOfAbilityCC.None;
        E_Ability.typeOfAbilityDamage = AbilityClass.TypeOfAbilityDamage.NoDamage;

        E_Ability.abilityName = "Haste";
        E_Ability.abilityCooldown = 11f;
        E_Ability.HUDIcon = E_HudIcon;
        E_Ability.Indicator = E_Indicator;
        E_Ability.Range = E_Range;
        E_Ability.abilityKeyCode = E_Ability_Hotkey;

        E_Ability.abilityMaxLevel = 5;

        E_Ability.abilityRangeNum = 1f;

        E_Ability.abilityDuration = 2f;
        E_Ability.abilityBuffPercentage = .45f;

        E_Ability.abilityBaseDamage = 0f; // it doesn't do damage

        E_Ability.abilityCost = 60f;
        E_Ability.abilityDescription = string.Format("{0} makes haste, gaining a burst of {1}% movement speed and attack speed for {2} seconds. Acts as an auto-attack reset.", HeroName, E_Ability.abilityBuffPercentage * 100, E_Ability.abilityDuration);
        E_Ability.abilityPerLevel = string.Format("Movement Speed & Attack Speed %: 45 / 50 / 55 / 60 / 60\nDuration: 2 / 2.2 / 2.6 / 2.8 / 3");

        E_Ability.abilityBuffPercentagePerLevel = new float[5];
        E_Ability.abilityBuffPercentagePerLevel[0] = .45f;
        E_Ability.abilityBuffPercentagePerLevel[1] = .50f;
        E_Ability.abilityBuffPercentagePerLevel[2] = .55f;
        E_Ability.abilityBuffPercentagePerLevel[3] = .60f;
        E_Ability.abilityBuffPercentagePerLevel[4] = .60f;

        E_Ability.abilityDurationPerLevel = new float[5];
        E_Ability.abilityDurationPerLevel[0] = 2;
        E_Ability.abilityDurationPerLevel[1] = 2.2f;
        E_Ability.abilityDurationPerLevel[2] = 2.6f;
        E_Ability.abilityDurationPerLevel[3] = 2.8f;
        E_Ability.abilityDurationPerLevel[4] = 3f;

        ///
        /// R Abililty Init
        ///

        /// Ability Summary:
        /// Cast a targeted fire bomb at an enemy.
        /// 

        R_Ability = new AbilityClass();
        R_Ability.typeOfAbilityCast = AbilityClass.TypeOfAbilityCast.Targeted;
        R_Ability.typeOfAbilityCC = AbilityClass.TypeOfAbilityCC.Slow;
        R_Ability.typeOfAbilityDamage = AbilityClass.TypeOfAbilityDamage.AbilityDamage;

        R_Ability.abilityName = "Fire Bomb";
        R_Ability.abilityCooldown = 55f;
        R_Ability.HUDIcon = R_HudIcon;
        R_Ability.Indicator = R_Indicator;
        R_Ability.Range = R_Range;
        R_Ability.abilityKeyCode = R_Ability_Hotkey;

        R_Ability.abilityMaxLevel = 3;

        R_Ability.abilityRangeNum = 9f;

        R_Ability.abilityDuration = 0f;
        R_Ability.abilityBuffPercentage = 0f;

        R_Ability.abilitySpeed = 25f; // moderately fast (targeted abilities scale higher ~ 40 being gigaspeed
        R_Ability.abilityCastTime = 0f;

        R_Ability.abilityBaseDamage = 150f;
        R_Ability.abilityScaling = .75f;

        R_Ability.abilityCost = 100f;
        R_Ability.abilityDescription = string.Format("{0} conjures a massive fire bomb at an enemy, dealing {1} (+{2}) ability damage. ", HeroName, R_Ability.abilityBaseDamage, heroAbilityDmg * R_Ability.abilityScaling);
        R_Ability.abilityPerLevel = string.Format("Damage: 150 / 195 / 245\nCooldown: 55 / 45 / 35");

        R_Ability.abilityDamagePerLevel = new float[3];
        R_Ability.abilityDamagePerLevel[0] = 150;
        R_Ability.abilityDamagePerLevel[1] = 195;
        R_Ability.abilityDamagePerLevel[2] = 245;

        R_Ability.abilityCooldownPerLevel = new float[3];
        R_Ability.abilityCooldownPerLevel[0] = 55;
        R_Ability.abilityCooldownPerLevel[1] = 45;
        R_Ability.abilityCooldownPerLevel[2] = 35;

    }

    private void OnApplicationQuit()
    {
        Q_Ability.abilityDamagePerLevel = null;
        Q_Ability.abilityCostPerLevel = null;
        Q_Ability.abilityCooldownPerLevel = null;

        W_Ability.abilityDamagePerLevel = null;
        W_Ability.abilityCostPerLevel = null;
        W_Ability.abilityCooldownPerLevel = null;

        E_Ability.abilityBuffPercentagePerLevel = null;
        E_Ability.abilityDurationPerLevel = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
    }
}
