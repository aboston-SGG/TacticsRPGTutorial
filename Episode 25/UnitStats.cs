using NUnit.Framework.Constraints;
using Unity.Hierarchy;
using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public float speed;
    public float perception;
    public float endurance;
    public float strength;
    public float luck;
    public float intellect;
    public float spirit;
    public float mastery;

    public const float STAT_VARIANCE = 5f;

    // Constructor
    public UnitStats(float newSpeed, float newPerception, float newEndurance, float newStrength, float newLuck, float newIntellect, float newSpirit, float newMastery)
    {
        speed = newSpeed;
        perception = newPerception;
        endurance = newEndurance;
        strength = newStrength;
        luck = newLuck;
        intellect = newIntellect;
        spirit = newSpirit;
        mastery = newMastery;
    }

    // Levels up the stats by adding a random value based on growth rates
    public void LevelUpStats(UnitStats classGrowths, UnitStats unitGrowths)
    {
        float rand = Random.value;

        ApplyGrowth(ref speed, classGrowths.speed + unitGrowths.speed, rand);
        ApplyGrowth(ref perception, classGrowths.perception + unitGrowths.perception, rand);
        ApplyGrowth(ref endurance, classGrowths.endurance + unitGrowths.endurance, rand);
        ApplyGrowth(ref strength, classGrowths.strength + unitGrowths.strength, rand);
        ApplyGrowth(ref luck, classGrowths.luck + unitGrowths.luck, rand);
        ApplyGrowth(ref intellect, classGrowths.intellect + unitGrowths.intellect, rand);
        ApplyGrowth(ref spirit, classGrowths.spirit + unitGrowths.spirit, rand);
        ApplyGrowth(ref mastery, classGrowths.mastery + unitGrowths.mastery, rand);
    }

    // Initializes a stat value based on base value and growth rate with some variance
    public static float InitializeStatValue(float baseValue, float growth)
    {
        float min = Mathf.Clamp(baseValue - (growth * STAT_VARIANCE), 0, float.MaxValue);
        float max = baseValue + (growth * STAT_VARIANCE);

        return Mathf.Round(Random.Range(min, max));
    }

    private void ApplyGrowth(ref float stat, float growth, float rand)
    {
        if(growth > 1f)
        {
            int guaranteedIncrease = Mathf.FloorToInt(growth);
            stat += guaranteedIncrease;
            growth -= guaranteedIncrease;
        }

        if(growth > rand)
        {
            stat++;
        }
    }
}
