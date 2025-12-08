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

    public UnitStats(float newSpeed, float newPerception, float newEndurance, float newStrength)
    {
        speed = newSpeed;
        perception = newPerception;
        endurance = newEndurance;
        strength = newStrength;
    }

    // Levels up the stats by adding a random value based on growth rates
    public void LevelUpStats()
    {
        speed += Random.Range(0, 10);
        perception += Random.Range(0, 10);
        endurance += Random.Range(0, 10);
        strength += Random.Range(0, 10);
    }

    public static float InitializeStatValue(float baseValue, float growth)
    {
        float min = Mathf.Clamp(baseValue - (growth * STAT_VARIANCE), 0, float.MaxValue);
        float max = baseValue + (growth * STAT_VARIANCE);

        return Mathf.Round(Random.Range(min, max));
    }
}
