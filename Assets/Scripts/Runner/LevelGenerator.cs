using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public LevelChunk[] chunkPrefabs;
    public LevelChunk startChunk;

    [Header("O Fim da Fase")]
    public LevelChunk finishLineChunk;
    public float distanciaParaVencer = 1200f;
    private bool _jaGerouChegada = false;

    [Header("Configurações")]
    public float spawnDistanceAhead = 30f;
    public float despawnDistanceBehind = 20f;
    public int initialPoolSizePerPrefab = 3;

    private Vector3 _nextSpawnPosition;
    private Queue<LevelChunk> _activeChunks = new Queue<LevelChunk>();
    private Dictionary<int, List<LevelChunk>> _chunkPools = new Dictionary<int, List<LevelChunk>>();

    private void Start()
    {
        InitializePool();

        _nextSpawnPosition = startChunk.endPoint.position;
        _activeChunks.Enqueue(startChunk);

        for (int i = 0; i < 5; i++)
        {
            SpawnChunk();
        }
    }

    private void Update()
    {
        if (!_jaGerouChegada && player.position.x + spawnDistanceAhead > _nextSpawnPosition.x)
        {
            if (_nextSpawnPosition.x >= distanciaParaVencer)
            {
                SpawnFinishLine();
            }
            else
            {
                SpawnChunk();
            }
        }

        if (_activeChunks.Count > 0)
        {
            LevelChunk oldestChunk = _activeChunks.Peek();
            if (oldestChunk.endPoint.position.x < player.position.x - despawnDistanceBehind)
            {
                RecycleChunk();
            }
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            _chunkPools[i] = new List<LevelChunk>();

            for (int j = 0; j < initialPoolSizePerPrefab; j++)
            {
                LevelChunk newChunk = Instantiate(chunkPrefabs[i]);
                newChunk.gameObject.SetActive(false);
                _chunkPools[i].Add(newChunk);
            }
        }
    }

    private void SpawnChunk()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        LevelChunk chunk = GetChunkFromPool(randomIndex);

        chunk.transform.position = _nextSpawnPosition;
        chunk.gameObject.SetActive(true);

        _nextSpawnPosition = chunk.endPoint.position;
        _activeChunks.Enqueue(chunk);
    }

    private LevelChunk GetChunkFromPool(int index)
    {
        List<LevelChunk> pool = _chunkPools[index];

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                return pool[i];
            }
        }

        LevelChunk newChunk = Instantiate(chunkPrefabs[index]);
        pool.Add(newChunk);
        return newChunk;
    }

    private void RecycleChunk()
    {
        LevelChunk chunkToRemove = _activeChunks.Dequeue();

        if (chunkToRemove != startChunk)
        {
            chunkToRemove.gameObject.SetActive(false);
        }
    }

    private void SpawnFinishLine()
    {
        _jaGerouChegada = true;
        
        LevelChunk chunkFinal = Instantiate(finishLineChunk, _nextSpawnPosition, Quaternion.identity);
        _activeChunks.Enqueue(chunkFinal);
    }
}