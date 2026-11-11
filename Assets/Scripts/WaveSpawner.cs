using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private EnemyCircle enemyPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int circlesPerWave = 7;
    [SerializeField] private float horizontalSpread = 7f;

    private IObjectPool<EnemyCircle> enemyPool;

    private void Awake()
    {
        enemyPool = new ObjectPool<EnemyCircle>(
            createFunc: () =>
            {
                EnemyCircle enemy = Instantiate(enemyPrefab);
                enemy.SetPool(enemyPool);
                return enemy;
            },
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            collectionCheck: false,
            defaultCapacity: 30,
            maxSize: 150
        );
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnCircleWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCircleWave()
    {
        float step = (horizontalSpread * 2f) / (circlesPerWave - 1);
        float topY = Camera.main.orthographicSize + 0.5f;

        for (int i = 0; i < circlesPerWave; i++)
        {
            float posX = -horizontalSpread + (i * step);
            EnemyCircle enemy = enemyPool.Get();
            enemy.Spawn(new Vector3(posX, topY, 0f));
        }
    }
}