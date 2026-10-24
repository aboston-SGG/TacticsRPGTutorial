using System;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Items/Weapon")]
public class BaseWeapon : BaseItem
{
    [Tooltip("The attack power of the weapon.")]
    public int power;
    [Min(0), Tooltip("Minimum Range of the weapon.")]
    public int minRange;
    [Min(0), Tooltip("Maximum Range of the weapon.")]
    public int maxRange;
    [Range(0f, 1f), Tooltip("Change for critical hit (0 = 0%, 1 = 100%).")]
    public float critChance;
    [Tooltip("Weight affects accuracy, movement speed, and attack speed.")]
    public float weight;
    [Tooltip("This weapon's type.")]
    public WeaponType weaponType;
    [Tooltip("Types of weapos this weapon is effective against.")]
    public WeaponType effectiveAgainst;

    // Override the UseItem method to equip the weapon to the unit
    public override void UseItem(ItemInstance instance, Unit unit)
    {
        if(unit == null)
        {
            Debug.LogWarning("UseItem called with null unit.");
            return;
        }

        if ((unit.unitClass.weaponProficiencies & weaponType) != 0)
        {
            unit.ChangeEquippedWeapon(instance);
        }
        else
        {
            Debug.Log($"{unit.name} is not profiecient with {name}.");
        }
    }

    // Called when the script loads or an inspector value is changed
    private void OnValidate()
    {
        if(maxRange < minRange)
        {
            Debug.LogWarning($"MaxRange ({maxRange}) is less than MinRange({minRange}) on {name}. Adjusting MaxRange to MinRange.");
            maxRange = minRange;
        }

        critChance = Mathf.Clamp01(critChance);
        minRange = Mathf.Max(0, minRange);
        maxRange = Mathf.Max(0, maxRange);
    }
}

[Flags]
public enum WeaponType
{
    None = 0,
    Sword = 1 << 0,
    Polearm = 1 << 1,
    Axe = 1 << 2,
    Blunt = 1 << 3,
    Bow = 1 << 4,
    Firearm = 1 << 5,
    Dagger = 1 << 6,
    Staff = 1 << 7,
    Wand = 1 << 8,
    Fist = 1 << 9,
    Primal = 1 << 10,
    Arcane = 1 << 11,
    Light = 1 << 12,
    Dark = 1 << 13
}
