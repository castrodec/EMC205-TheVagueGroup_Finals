using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class UnitPoolDefinition {
        public string label; // This is what you'll see in the Inspector list
        public UnitScriptableObject data;
        public UnitController prefab;
    }

    [System.Serializable]
    public class ProjectilePoolDefinition {
        public string label; // This is what you'll see in the Inspector list
        public ProjectileScriptableObject data;
        public ProjectileController prefab;
    }

    [System.Serializable]
    public class TurretPoolDefinition {
        public string label; // This is what you'll see in the Inspector list
        public TurretScriptableObject data;
        public TurretController prefab;
    }

    public static ObjectPooler Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private const int DEFAULT = 50;
    [SerializeField] private const int MAX_POOL_SIZE = 200;

    [Header("Dictionaries and Definitions")]
    public List<UnitPoolDefinition> unitDefinitions;
    public List<ProjectilePoolDefinition> projectileDefinitions;
    public List<TurretPoolDefinition> turretDefinitions;

    private Dictionary<UnitScriptableObject, ObjectPool<UnitController>> unitPools = 
    new Dictionary<UnitScriptableObject, ObjectPool<UnitController>>();
    private Dictionary<ProjectileScriptableObject, ObjectPool<ProjectileController>> projectilePools = 
    new Dictionary<ProjectileScriptableObject, ObjectPool<ProjectileController>>();
    private Dictionary<TurretScriptableObject, ObjectPool<TurretController>> turretPools =
    new Dictionary<TurretScriptableObject, ObjectPool<TurretController>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        InitializeProjectilePool();
        InitializeUnitPool();
        InitializeTurretPool();
    }

    private void InitializeProjectilePool()
    {
        foreach (var projectile in projectileDefinitions)
        {
            var pool = new ObjectPool<ProjectileController>(
                createFunc: () => Instantiate(projectile.prefab),
                actionOnGet: projectile => projectile.gameObject.SetActive(true),
                actionOnRelease: projectile => projectile.gameObject.SetActive(false),
                actionOnDestroy: projectile => Destroy(projectile.gameObject),
                collectionCheck: true,
                defaultCapacity: DEFAULT,
                maxSize: MAX_POOL_SIZE
                );
            projectilePools.Add(projectile.data, pool);
        }
    }

    private void InitializeUnitPool()
    {
        foreach (var unit in unitDefinitions)
        {
            var pool = new ObjectPool<UnitController>(
                createFunc: () => Instantiate(unit.prefab),
                actionOnGet: unit => unit.gameObject.SetActive(true),
                actionOnRelease: unit => unit.gameObject.SetActive(false),
                actionOnDestroy: unit => Destroy(unit.gameObject),
                collectionCheck: true,
                defaultCapacity: DEFAULT,
                maxSize: MAX_POOL_SIZE
                );
            unitPools.Add(unit.data, pool);
        }
    }

    private void InitializeTurretPool()
    {
        foreach (var turret in turretDefinitions)
        {
            var pool = new ObjectPool<TurretController>(
                createFunc: () => Instantiate(turret.prefab),
                actionOnGet: turret => turret.gameObject.SetActive(true),
                actionOnRelease: turret => turret.gameObject.SetActive(false),
                actionOnDestroy: turret => Destroy(turret.gameObject),
                collectionCheck: true,
                defaultCapacity: DEFAULT,
                maxSize: MAX_POOL_SIZE
                );
            turretPools.Add(turret.data, pool);
        }
    }

    public UnitController SpawnUnit(UnitScriptableObject data, Vector2 position, bool isAlly)
    {
        if (unitPools.TryGetValue(data, out var pool))
        {
            UnitController unit = pool.Get();
            unit.SetPool(pool);
            unit.ResetUnit(position, isAlly ? Quaternion.identity : Quaternion.Euler(0, 180, 0), isAlly, data);
            return unit;
        }
        return null;
    }

    public ProjectileController SpawnProjectile(ProjectileScriptableObject data, Vector2 position, Transform target, Vector2 direction, bool isAllyProjectile) 
    {
        if (projectilePools.TryGetValue(data, out var pool))
        {
            ProjectileController projectile = pool.Get();
            projectile.SetPool(pool);
            
            // Set position first
            projectile.transform.position = position;
            
            // Initialize with direction instead of a target Transform
            projectile.Initialize(target, direction, isAllyProjectile, data);
            
            return projectile;
        }
        return null;
    }

    public TurretController SpawnTurret(TurretScriptableObject data, Vector2 position) 
    {
        if (turretPools.TryGetValue(data, out var pool))
        {
            TurretController turret = pool.Get();
            // turret.SetPool(pool);
            // turret.ResetTurret(position, data);
            return turret;
        }
        return null;
    }

    public UnitController SpawnRandomUnit(Vector2 position, bool isAlly) 
    => SpawnUnit(unitDefinitions[Random.Range(0, unitDefinitions.Count)].data, position, isAlly);
}
