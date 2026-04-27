using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public LevelChunk[] chunkPrefabs;
    public LevelChunk startChunk;

    [Header("Configurações")]
    public float spawnDistanceAhead = 20f;
    public float despawnDistanceBehind = 15f;

    private Vector3 _nextSpawnPosition;
    private Queue<LevelChunk> _activeChunks = new Queue<LevelChunk>();

    private void Start()
    {
        _nextSpawnPosition = startChunk.endPoint.position;
        _activeChunks.Enqueue(startChunk);

        for (int i = 0; i < 3; i++)
        {
            SpawnChunk();
        }
    }

    private void Update()
    {

        if (player.position.x + spawnDistanceAhead > _nextSpawnPosition.x)
        {
            SpawnChunk();
        }

        LevelChunk oldestChunk = _activeChunks.Peek();
        if (oldestChunk.endPoint.position.x < player.position.x - despawnDistanceBehind)
        {
            RecycleChunk();
        }
    }

    private void SpawnChunk()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);

        LevelChunk newChunk = Instantiate(chunkPrefabs[randomIndex], _nextSpawnPosition, Quaternion.identity);

        _nextSpawnPosition = newChunk.endPoint.position;

        // Adiciona à fila de chunks ativos
        _activeChunks.Enqueue(newChunk);
    }

    private void RecycleChunk()
    {
        LevelChunk chunkToRemove = _activeChunks.Dequeue();
        Destroy(chunkToRemove.gameObject);
    }
}