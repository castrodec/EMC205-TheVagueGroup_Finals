using UnityEngine;

public class TurretScriptableObject : ScriptableObject
{
    public string turretName;
    public int damage;
    public float range;
    public float fireRate;
    public bool isAOE;
    public bool isTracking;
    public GameObject turretPrefab;
    public int cost;
}
