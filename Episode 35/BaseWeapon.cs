using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Items/Weapon")]
public class BaseWeapon : BaseItem
{
    public int power;
    public int minRange;
    public int maxRange;

    public override void UseItem(Unit unit)
    {
        unit.equippedWeapon = this;
    }
}
