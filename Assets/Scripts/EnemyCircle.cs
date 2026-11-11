using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyCircle : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 1.6f;
    private IObjectPool<EnemyCircle> pool;
    private Camera mainCam;
    private Collider2D circleCollider;
    private bool isDying;

    public void SetPool(IObjectPool<EnemyCircle> targetPool) => pool = targetPool;

    private void Awake()
    {
        mainCam = Camera.main;
        circleCollider = GetComponent<Collider2D>();
    }

    public void Spawn(Vector3 startPosition)
    {
        if (circleCollider != null)
        {
            circleCollider.enabled = true;
        }

        transform.position = startPosition;
        transform.localScale = Vector3.zero;
        isDying = false;

        Tween.Scale(transform, endValue: 0.7f, duration: 0.25f, ease: Ease.OutBack);
    }

    private void Update()
    {
        if (isDying) return;

        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (mainCam != null && transform.position.y < -mainCam.orthographicSize - 1f)
        {
            pool?.Release(this);
        }
    }

    public void Hit()
    {
        if (isDying) return;
        isDying = true;

        if (circleCollider != null)
        {
            circleCollider.enabled = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }

        Tween.StopAll(transform);

        Sequence.Create()
            .Chain(Tween.Scale(transform, endValue: 0.9f, duration: 0.05f, ease: Ease.OutQuad))
            .Chain(Tween.Scale(transform, endValue: 0f, duration: 0.12f, ease: Ease.InQuad))
            .OnComplete(() =>
            {
                pool?.Release(this);
            });
    }

    private void OnDisable()
    {
        Tween.StopAll(transform);
    }
}