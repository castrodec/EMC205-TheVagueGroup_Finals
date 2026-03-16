using UnityEngine;

public class UnitScriptableObject : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Ranged,
        Flying
    }

    public string unitName;
    public int health;
    public float speed;
    public int damage;
    public AttackType attackType;
    public GameObject unitPrefab;
    public int cost;

}
