using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner Variables")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private ObjectPooler pooler;
    [SerializeField] private int maxQueuedUnits = 6;
    [SerializeField] private Transform spawnPoint;
    private UnitScriptableObject selectedData;
    private Queue<UnitScriptableObject> dataQueue = new Queue<UnitScriptableObject>();

    void Start()
    {
        StartCoroutine(SpawnUnits());
    }

    public void EnqueueSummon(UnitScriptableObject data)
    {
        if (dataQueue.Count < maxQueuedUnits && ResourceManager.Instance.RemoveCoins(data.unitCost)) 
        {
            dataQueue.Enqueue(data);
            UIManager.Instance.UpdateQueueUI(dataQueue.ToArray());
        }
        else Debug.Log("Spawner is full!");
    }

    private IEnumerator SpawnUnits()
    {
        while (true)
        {
            if (dataQueue.Count > 0)
            {
                selectedData = dataQueue.Dequeue();
                UIManager.Instance.UpdateQueueUI(dataQueue.ToArray());
                SpawnUnitCommand spawn = new SpawnUnitCommand(selectedData, spawnPoint.position, true);
                spawn.Execute();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public UnitScriptableObject[] GetQueueArray()
    {
        return dataQueue.ToArray();
    }
}
