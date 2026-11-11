using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    private IObjectPool<Bullet> pool;
    private Camera mainCam;

    public void SetPool(IObjectPool<Bullet> targetPool) => pool = targetPool;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);

        if (mainCam != null && transform.position.y > mainCam.orthographicSize + 1f)
        {
            pool?.Release(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyCircle>(out var enemy))
        {
            enemy.Hit();
            pool?.Release(this);
        }
    }
}