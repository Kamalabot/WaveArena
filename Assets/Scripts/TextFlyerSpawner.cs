using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using PrimeTween;

public class TextFlyerSpawner : MonoBehaviour
{
    public static TextFlyerSpawner Instance { get; private set; }

    [SerializeField] private TextMeshPro flyerPrefab;
    private IObjectPool<TextMeshPro> pool;

    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<TextMeshPro>(
            createFunc: () => Instantiate(flyerPrefab, transform),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: 15,
            maxSize: 50
        );
    }

    public void SpawnFlyer(Vector3 worldPos, string text, Color color)
    {
        TextMeshPro flyer = pool.Get();
        flyer.text = text;
        flyer.color = color;
        flyer.alpha = 1f;
        flyer.transform.position = worldPos;
        flyer.transform.localScale = Vector3.one * 0.6f;

        // Slight horizontal scatter so rapid flyers don't overlap identically
        Vector3 endPos = worldPos + new Vector3(Random.Range(-0.4f, 0.4f), 1.2f, 0f);

        Sequence.Create()
            .Group(Tween.Position(flyer.transform, endPos, 0.75f, Ease.OutCubic))
            .Group(Tween.Scale(flyer.transform, Vector3.one, 0.2f, Ease.OutBack))
            .Chain(Tween.Alpha(flyer, 0f, 0.25f))
            .OnComplete(() => pool.Release(flyer));
    }
}