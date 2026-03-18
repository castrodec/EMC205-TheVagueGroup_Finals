using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Data/UnitData")]
public class UnitScriptableObject : ScriptableObject
{
    public enum AttackType { Melee, Ranged, Flying }
    
    [Header("Unit Attributes")]
    public string unitName;
    public int maxHealth;
    public float moveSpeed;
    public ProjectileScriptableObject projectileData;
    public LayerMask targetLayer;

    [Header("Attack Attributes")]
    public AttackType attackType;
    public int attackDamage;
    public float attackInterval;
    public float attackRange;
    public int ammoCapacity;
    public float reloadTime;

    [Header("Flying Attributes")]
    public float flightHeight = 3f;
    public float hoverSpeed = 2f;
    public float hoverAmount = 0.5f;
}