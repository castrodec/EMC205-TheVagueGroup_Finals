using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public UnitScriptableObject unitData;
        public int count;
    }

    [System.Serializable]
    public class WaveDefinition
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;

        // Logic to create a randomized list of all enemies in this wave
        public List<UnitScriptableObject> GetRandomizedSpawnList()
        {
            List<UnitScriptableObject> allEnemies = new List<UnitScriptableObject>();
            foreach (var group in enemyGroups)
            {
                for (int i = 0; i < group.count; i++)
                    allEnemies.Add(group.unitData);
            }

            // Fisher-Yates Shuffle
            for (int i = 0; i < allEnemies.Count; i++)
            {
                UnitScriptableObject temp = allEnemies[i];
                int randomIndex = Random.Range(i, allEnemies.Count);
                allEnemies[i] = allEnemies[randomIndex];
                allEnemies[randomIndex] = temp;
            }
            return allEnemies;
        }
    }

    public static WaveManager Instance { get; private set; }
    [Header("Wave Management")]
    [SerializeField] private List<WaveDefinition> waveDefinitions = new List<WaveDefinition>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval;


    private Queue<WaveDefinition> waveQueue = new Queue<WaveDefinition>();
    public bool isSpawning = false;
    public int activeEnemyCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(this);
    }

    void Start() {
        foreach (WaveDefinition wave in waveDefinitions) {
            waveQueue.Enqueue(wave);
        }

        StartNextWave();
    }

    public void StartNextWave()
    {
        if (waveQueue.Count > 0 && !isSpawning && activeEnemyCount == 0)
        {
            UIManager.Instance.UpdateStartWaveButton(false);
            StartCoroutine(SpawnWaveRoutine(waveQueue.Dequeue()));
        }
    }

    private IEnumerator SpawnWaveRoutine(WaveDefinition wave)
    {
        isSpawning = true;
        List<UnitScriptableObject> spawnList = wave.GetRandomizedSpawnList();

        foreach (var enemyData in spawnList)
        {
            activeEnemyCount++; // Increment count BEFORE spawning
            SpawnUnitCommand spawn = new SpawnUnitCommand(enemyData, spawnPoint.position, false);
            spawn.Execute();
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    public void EnemyDied() {
        activeEnemyCount--;

        if (activeEnemyCount <= 0 && !isSpawning) {
            if (waveQueue.Count > 0) UIManager.Instance.UpdateStartWaveButton(true);
            else Debug.Log("VICTORY! All waves cleared.");
        }
    }
}
