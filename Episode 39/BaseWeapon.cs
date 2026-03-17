using System;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Items/Weapon")]
public class BaseWeapon : BaseItem
{
    public int power;
    public int minRange;
    public int maxRange;
    public float critChance;
    public float accuracyModifier;
    public WeaponType weaponType;

    // Override the UseItem method to equip the weapon to the unit
    public override void UseItem(Unit unit)
    {
        if (unit.unitClass.weaponProficiencies.HasFlag(weaponType))
        {
            unit.ChangeEquippedWeapon(this);
        }
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
