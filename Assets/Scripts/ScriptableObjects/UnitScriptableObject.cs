using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Data/UnitData")]
public class UnitScriptableObject : ScriptableObject
{
    
    [Header("Unit Attributes")]
    public string unitName;
    public int maxHealth;
    public float moveSpeed;
    public int cost;
    public ProjectileScriptableObject projectileData;

    [Header("Attack Attributes")]
    public bool canFly;
    public bool isRanged;
    public int attackDamage;
    public float attackInterval;
    public float attackRange;
    public int ammoCapacity;


}
