using UnityEngine;
using UnityEngine.UIElements;
using PrimeTween;

[RequireComponent(typeof(UIDocument))]
public class VectorHudController : MonoBehaviour
{
    private VectorRingElement ringElement;
    private float currentFill = 0.75f;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        ringElement = new VectorRingElement();
        ringElement.style.position = Position.Absolute;
        ringElement.style.left = 40;
        ringElement.style.top = 40;
        ringElement.style.width = 120;
        ringElement.style.height = 120;

        root.Add(ringElement);

        // Animate the vector arc smoothly with PrimeTween
        Tween.Custom(0f, 1f, duration: 1.2f, ease: Ease.OutCubic, onValueChange: val =>
        {
            ringElement.Progress = val;
        });
    }
}

public class VectorRingElement : VisualElement
{
    private float progress = 1f;
    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Clamp01(value);
            MarkDirtyRepaint(); // Triggers generateVisualContent only when changed
        }
    }

    public VectorRingElement()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Painter2D painter = mgc.painter2D;
        Vector2 center = contentRect.center;
        float radius = Mathf.Min(contentRect.width, contentRect.height) * 0.42f;

        // 1. Background track ring
        painter.strokeColor = new Color(1f, 1f, 1f, 0.15f);
        painter.lineWidth = 6f;
        painter.BeginPath();
        painter.Arc(center, radius, 0f, 360f);
        painter.Stroke();

        // 2. Active progress arc
        painter.strokeColor = Color.cyan;
        painter.lineWidth = 6f;
        painter.lineCap = LineCap.Round;
        painter.BeginPath();
        painter.Arc(center, radius, -90f, -90f + (360f * progress));
        painter.Stroke();
    }
}