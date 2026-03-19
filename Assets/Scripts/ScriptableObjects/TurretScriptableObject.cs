using UnityEngine;

[CreateAssetMenu(fileName = "NewTurretData", menuName = "Game/Data/TurretData")]
public class TurretScriptableObject : ScriptableObject
{
    public enum TurretType { Bolt, Silo, Trap };

    [Header("Turret Data")]
    public string turretName;
    public TurretType turretType;
    public int maxHealth;
    public int cost;

    [Header("Combat Data")]
    public float fireRate;
    public float range;
    public int damage;
}
