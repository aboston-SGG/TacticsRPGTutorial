using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public float speed;
    public float perception;

    public UnitStats(float newSpeed, float newPerception)
    {
        speed = newSpeed;
        perception = newPerception;
    }
}
