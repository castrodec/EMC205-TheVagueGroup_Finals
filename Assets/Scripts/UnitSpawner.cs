using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour, ICommand
{
    [Header("Spawner Variables")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private bool isAllySpawner;
    public List<UnitScriptableObject> dataList;
    private UnitScriptableObject selectedData;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute()
    {

    }

    public void Undo()
    {

    }

    public void Queue()
    {

    }

    IEnumerator SpawnUnit()
    {
        yield return new WaitForSeconds(spawnInterval);
        ObjectPooler.Instance.SpawnUnit(selectedData, transform.position, isAllySpawner);
    }
}
