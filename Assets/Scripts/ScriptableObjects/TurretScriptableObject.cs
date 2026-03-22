using UnityEngine;

[CreateAssetMenu(fileName = "NewTurretData", menuName = "Game/Data/TurretData")]
public class TurretScriptableObject : ScriptableObject
{
    public enum TurretType { Bolt, Silo, Trap };

    [Header("Turret Data")]
    public string turretName;
    public TurretType turretType;
    public int maxHealth;
    public int turretCost;
    public LayerMask targetLayer;
    public ProjectileScriptableObject projectileData;
    public Sprite turretSprite;

    [Header("Combat Data")]
    public float fireRate;
    public float detectionRange;
    public int damage;
    public int aoeRadius;
}
