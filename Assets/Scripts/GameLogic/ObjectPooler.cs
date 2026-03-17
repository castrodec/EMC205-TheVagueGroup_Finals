using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }
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

    [Header("Pool Settings")]
    [SerializeField] private const int DEFAULT = 50;
    [SerializeField] private const int MAX_POOL_SIZE = 200;

    [Header("Dictionaries and Definitions")]
    public List<UnitPoolDefinition> unitDefinitions;
    public List<ProjectilePoolDefinition> projectileDefinitions;
    private Dictionary<UnitScriptableObject, ObjectPool<UnitController>> unitPools = 
    new Dictionary<UnitScriptableObject, ObjectPool<UnitController>>();
    private Dictionary<ProjectileScriptableObject, ObjectPool<ProjectileController>> projectilePools = 
    new Dictionary<ProjectileScriptableObject, ObjectPool<ProjectileController>>();

    private void Awake()
    {
        Instance = this;

        InitializeProjectilePool();
        InitializeUnitPool();
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

    public UnitController SpawnUnit(UnitScriptableObject data, Vector2 position, bool isAlly)
    {
        if (unitPools.TryGetValue(data, out var pool))
        {
            UnitController unit = pool.Get();
            unit.SetPool(pool);
            unit.ResetUnit(position, isAlly ? Quaternion.identity : Quaternion.Euler(0, 180, 0), isAlly);
            unit.Initialize(data);
            return unit;
        }
        return null;
    }

    public ProjectileController SpawnProjectile(ProjectileScriptableObject data, Vector2 position, Quaternion rotation, bool isAllyProjectile, Transform targetPosition) {
        if (projectilePools.TryGetValue(data, out var pool))
        {
            ProjectileController projectile = pool.Get();
            projectile.SetPool(pool);
            projectile.ResetProjectile(position, rotation, isAllyProjectile, targetPosition);
            projectile.Initialize(data);
            return projectile;
        }
        return null;
    }
}
