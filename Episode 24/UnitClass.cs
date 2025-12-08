using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "ScriptableObjects/New Class")]
public class UnitClass : ScriptableObject
{
    public string className;
    public string classDescription;

    public UnitStats baseStats;
    public UnitStats classGrowths;
}
