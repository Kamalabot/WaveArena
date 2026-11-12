using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float horizontalLimit = 8f;

    [Header("Shooting")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime;
    private IObjectPool<Bullet> bulletPool;
    private Vector3 originalScale;
    private OverheadText overheadtext;

    private void Awake()
    {
        originalScale = transform.localScale;
        
        if (overheadtext == null)
        {
            overheadtext = GetComponentInChildren<OverheadText>();
        }   
        
        bulletPool = new ObjectPool<Bullet>(
            createFunc: () =>
            {
                Bullet b = Instantiate(bulletPrefab);
                b.SetPool(bulletPool);
                return b;
            },
            actionOnGet: b => b.gameObject.SetActive(true),
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            collectionCheck: false,
            defaultCapacity: 25,
            maxSize: 100
        );
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += h * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        transform.position = pos;

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FireBullet()
    {
        Bullet b = bulletPool.Get();
        b.transform.position = firePoint != null ? firePoint.position : transform.position;
        overheadtext.ShowMessage("Firing Bullets...", Color.yellow, 1.5f);
        Tween.Scale(transform, endValue: new Vector3(originalScale.x * 0.85f, originalScale.y * 1.25f, originalScale.z), duration: 0.06f, ease: Ease.OutQuad)
            .OnComplete(() => Tween.Scale(transform, endValue: originalScale, duration: 0.1f, ease: Ease.OutBounce));
    }
}