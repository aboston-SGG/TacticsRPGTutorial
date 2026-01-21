using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Items/Weapon")]
public class BaseWeapon : BaseItem
{
    public int power;
    public int minRange;
    public int maxRange;
    public float critChance;

    // Override the UseItem method to equip the weapon to the unit
    public override void UseItem(Unit unit)
    {
        unit.ChangeEquippedWeapon(this);
    }
}
