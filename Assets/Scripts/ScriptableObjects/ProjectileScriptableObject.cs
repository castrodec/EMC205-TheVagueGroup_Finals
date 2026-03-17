using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileData", menuName = "Game/Data/ProjectileData")]
public class ProjectileScriptableObject : ScriptableObject
{
    [Header("Projectile Attributes")]
    public float projectileSpeed;
    public int projectileDamage;
    public bool isAOE, isTurretProjectile;
    public float lifeTime;
    public float aoeRadius;
}
