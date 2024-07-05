using UnityEngine;

/* Contains all the stats for a character. */

public class CharacterStats : MonoBehaviour
{
    public Stat maxHealth;          // Maximum amount of health
    public int currentHealth { get; protected set; }   // Current amount of health

    public Stat damage;
    public Stat armor;
    public CharacterStats_SO characterDefinition_Template;
    public CharacterStats_SO characterDefinition;
    public CharacterInventory charInv;
    public GameObject characterWeaponSlot;

    #region Constructors
    public CharacterStats()
    {
        charInv = CharacterInventory.instance;
    }
    #endregion

    public event System.Action OnHealthReachedZero;

    public virtual void Awake()
    {
        currentHealth = maxHealth.GetValue();
    }

    // Start with max HP, Hunger and Thirsty
    public virtual void Start()
    {
        if (characterDefinition_Template != null)
            characterDefinition = Instantiate(characterDefinition_Template);

        if (!characterDefinition.setManually)
        {
            characterDefinition.maxHealth = 100;
            characterDefinition.currentHealth = 50;

            characterDefinition.maxMana = 25;
            characterDefinition.currentMana = 10;

            characterDefinition.maxWealth = 500;
            characterDefinition.currentWealth = 0;

            characterDefinition.baseDamage = 2;
            characterDefinition.currentDamage = characterDefinition.baseDamage;

            characterDefinition.baseResistance = 0;
            characterDefinition.currentResistance = 0;

            characterDefinition.maxEncumbrance = 50f;
            characterDefinition.currentEncumbrance = 0f;

            characterDefinition.charExperience = 0;
            characterDefinition.charLevel = 1;
        }
    }

    // Damage
    public void TakeDamage(int damage)
    {

        // Subtract the armor value - Make sure damage doesn't go below 0.
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // Subtract damage from health
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If we hit 0. Die.
        if (currentHealth <= 0)
        {
            if (OnHealthReachedZero != null)
            {
                OnHealthReachedZero();
            }
        }
    }

    // Heal the character.
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.GetValue());
    }
    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(int manaAmount)
    {
        characterDefinition.ApplyMana(manaAmount);
    }

    public void GiveWealth(int wealthAmount)
    {
        characterDefinition.GiveWealth(wealthAmount);
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage2(int amount)
    {
        characterDefinition.TakeDamage(amount);
    }

    public void TakeMana(int amount)
    {
        characterDefinition.TakeMana(amount);
    }
    #endregion

    #region Weapon and Armor Change
    public void ChangeWeapon(ItemPickUp weaponPickUp)
    {
        if (!characterDefinition.UnEquipWeapon(weaponPickUp, charInv, characterWeaponSlot))
        {
            characterDefinition.EquipWeapon(weaponPickUp, charInv, characterWeaponSlot);
        }
    }

    public void ChangeArmor(ItemPickUp armorPickUp)
    {
        if (!characterDefinition.UnEquipArmor(armorPickUp, charInv))
        {
            characterDefinition.EquipArmor(armorPickUp, charInv);
        }
    }
    #endregion
    #region Reporters
    public int GetHealth()
    {
        return characterDefinition.currentHealth;
    }

    public Weapon3 GetCurrentWeapon()
    {
        if (characterDefinition.weapon != null)
        {
            return characterDefinition.weapon.itemDefinition.weaponSlotObject;
        }
        else
        {
            return null;
        }
    }

    public int GetDamage()
    {
        return characterDefinition.currentDamage;
    }

    public float GetResistance()
    {
        return characterDefinition.currentResistance;
    }

    #endregion
}
