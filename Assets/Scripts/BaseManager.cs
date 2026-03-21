using UnityEngine;

public class BaseManager : MonoBehaviour
{

    [Header("Base Attributes")]
    [SerializeField] private int maxHealth;
    public int currentHealth;
    public int baseLevel;
    public bool isAllyBase;

    [Header("Base Component")]
    public UnitSpawner unitSpawner;
    public TurretBuilder turretBuilder;
    public AirStrikeHandler airStrike;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
