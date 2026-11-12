using UnityEngine;
using TMPro;
using PrimeTween;

[RequireComponent(typeof(TextMeshPro))]
public class OverheadText : MonoBehaviour
{
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0.25f, 0f);
    private TextMeshPro tmp;
    private Camera mainCam;
    private Sequence activeSequence;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        mainCam = Camera.main;
        tmp.alpha = 0f;
    }

    private void LateUpdate()
    {
        // Lock world offset above parent regardless of parent rotation/scale
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + localOffset;
        }

        // Keep facing camera (essential for 3D; harmless in 2D)
        if (mainCam != null)
        {
            transform.rotation = mainCam.transform.rotation;
        }
    }

    public void ShowMessage(string message, Color color, float holdDuration = 1.2f)
    {
        activeSequence.Stop();

        tmp.text = message;
        tmp.color = color;
        tmp.alpha = 1f;
        transform.localScale = Vector3.one * 0.5f;

        activeSequence = Sequence.Create()
            .Group(Tween.Scale(transform, Vector3.one, 0.2f, Ease.OutBack))
            .ChainDelay(holdDuration)
            .Chain(Tween.Alpha(tmp, 0f, 0.25f));
    }
}