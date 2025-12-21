using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Consumable", menuName = "Scriptable Objects/Items/Consumable")]
public class BaseConsumable : BaseItem
{
    public int healAmount;
}
