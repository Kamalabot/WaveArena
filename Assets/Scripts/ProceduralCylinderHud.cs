using UnityEngine;
using UnityEngine.UIElements;
using PrimeTween;

[RequireComponent(typeof(UIDocument))]
public class ProceduralCylinderHud : MonoBehaviour
{
    [Header("Layout & Size (px)")]
    [SerializeField] private float leftMargin = 40f;
    [SerializeField] private float bottomMargin = 40f;
    [SerializeField] private float cylinderWidth = 90f;
    [SerializeField] private float cylinderHeight = 220f;

    [Header("Colors")]
    [SerializeField] private Color liquidBodyColor = new Color(0.0f, 0.78f, 0.92f, 0.85f);
    [SerializeField] private Color meniscusTopColor = new Color(0.45f, 0.95f, 1.0f, 0.95f);
    [SerializeField] private Color glassOutlineColor = new Color(1f, 1f, 1f, 0.65f);

    private CylinderGaugeElement gaugeElement;
    private Tween fillTween;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gaugeElement = new CylinderGaugeElement
        {
            LiquidColor = liquidBodyColor,
            MeniscusColor = meniscusTopColor,
            OutlineColor = glassOutlineColor
        };

        gaugeElement.style.position = Position.Absolute;
        gaugeElement.style.left = leftMargin;
        gaugeElement.style.bottom = bottomMargin;
        gaugeElement.style.width = cylinderWidth;
        gaugeElement.style.height = cylinderHeight;
        gaugeElement.pickingMode = PickingMode.Ignore;

        root.Add(gaugeElement);

        // Demo startup animation using PrimeTween
        SetFill(0.68f, 1.0f);
    }

    /// <summary>
    /// Updates the cylinder liquid level smoothly (normalized 0.0 to 1.0).
    /// </summary>
    public void SetFill(float normalizedValue, float duration = 0.3f)
    {
        if (gaugeElement == null) return;

        float target = Mathf.Clamp01(normalizedValue);
        fillTween.Stop();

        fillTween = Tween.Custom(
            startValue: gaugeElement.FillAmount,
            endValue: target,
            duration: duration,
            ease: Ease.OutCubic,
            onValueChange: val => gaugeElement.FillAmount = val
        );
    }
}

public class CylinderGaugeElement : VisualElement
{
    private const float Kappa = 0.55228475f; // Cubic Bezier circular approximation constant

    private float fillAmount = 0f;
    public float FillAmount
    {
        get => fillAmount;
        set
        {
            float clamped = Mathf.Clamp01(value);
            if (Mathf.Approximately(fillAmount, clamped)) return;
            fillAmount = clamped;
            MarkDirtyRepaint();
        }
    }

    public Color LiquidColor { get; set; } = new Color(0f, 0.78f, 0.92f, 0.85f);
    public Color MeniscusColor { get; set; } = new Color(0.45f, 0.95f, 1f, 0.95f);
    public Color OutlineColor { get; set; } = new Color(1f, 1f, 1f, 0.65f);

    public CylinderGaugeElement()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Painter2D painter = mgc.painter2D;

        float padding = 4f;
        float w = contentRect.width - padding * 2f;
        float h = contentRect.height - padding * 2f;
        if (w <= 2f || h <= 2f) return;

        float rx = w * 0.5f;              // Horizontal ellipse radius
        float ry = Mathf.Min(rx * 0.32f, h * 0.15f); // Vertical ellipse perspective radius
        float cx = contentRect.center.x;
        float topY = padding + ry;
        float bottomY = padding + h - ry;
        float usableHeight = bottomY - topY;

        // 1. Back Cylinder Shell (subtle dark interior)
        painter.fillColor = new Color(0.05f, 0.08f, 0.12f, 0.45f);
        DrawCylinderBodyPath(painter, cx, topY, bottomY, rx, ry);
        painter.Fill();

        // 2. Liquid Column + Moving Top Meniscus
        if (fillAmount > 0.001f)
        {
            float liquidTopY = Mathf.Lerp(bottomY, topY, fillAmount);

            // Liquid vertical body + bottom rounded curve
            painter.fillColor = LiquidColor;
            DrawCylinderBodyPath(painter, cx, liquidTopY, bottomY, rx, ry);
            painter.Fill();

            // Bright 3D top meniscus surface at liquidTopY
            painter.fillColor = MeniscusColor;
            DrawEllipsePath(painter, new Vector2(cx, liquidTopY), rx, ry);
            painter.Fill();

            // Crisp highlight rim around the meniscus
            painter.strokeColor = new Color(1f, 1f, 1f, 0.55f);
            painter.lineWidth = 1.5f;
            DrawEllipsePath(painter, new Vector2(cx, liquidTopY), rx, ry);
            painter.Stroke();
        }

        // 3. Outer Glass Tank Wireframe (Side walls + Bottom Front Arc + Top Rim)
        painter.strokeColor = OutlineColor;
        painter.lineWidth = 2f;
        painter.lineCap = LineCap.Round;

        // Top full rim ellipse
        DrawEllipsePath(painter, new Vector2(cx, topY), rx, ry);
        painter.Stroke();

        // Left & right walls + bottom front curve
        painter.BeginPath();
        painter.MoveTo(new Vector2(cx - rx, topY));
        painter.LineTo(new Vector2(cx - rx, bottomY));
        AddBottomHalfEllipse(painter, new Vector2(cx, bottomY), rx, ry);
        painter.LineTo(new Vector2(cx + rx, topY));
        painter.Stroke();

        // 4. Measurement Tick Marks (25%, 50%, 75%)
        painter.strokeColor = new Color(OutlineColor.r, OutlineColor.g, OutlineColor.b, 0.4f);
        painter.lineWidth = 1.25f;
        for (int i = 1; i <= 3; i++)
        {
            float t = i * 0.25f;
            float tickY = Mathf.Lerp(bottomY, topY, t);
            float tickLen = (i == 2) ? rx * 0.45f : rx * 0.25f;

            painter.BeginPath();
            painter.MoveTo(new Vector2(cx - rx, tickY));
            painter.LineTo(new Vector2(cx - rx + tickLen, tickY));
            painter.Stroke();
        }
    }

    private static void DrawCylinderBodyPath(Painter2D painter, float cx, float topY, float bottomY, float rx, float ry)
    {
        painter.BeginPath();
        painter.MoveTo(new Vector2(cx - rx, topY));
        painter.LineTo(new Vector2(cx - rx, bottomY));
        AddBottomHalfEllipse(painter, new Vector2(cx, bottomY), rx, ry);
        painter.LineTo(new Vector2(cx + rx, topY));
        painter.ClosePath();
    }

    private static void DrawEllipsePath(Painter2D painter, Vector2 center, float rx, float ry)
    {
        float ox = rx * Kappa;
        float oy = ry * Kappa;

        painter.BeginPath();
        painter.MoveTo(new Vector2(center.x - rx, center.y));
        // Top-left to Top-center
        painter.BezierCurveTo(
            new Vector2(center.x - rx, center.y - oy),
            new Vector2(center.x - ox, center.y - ry),
            new Vector2(center.x, center.y - ry));
        // Top-center to Right
        painter.BezierCurveTo(
            new Vector2(center.x + ox, center.y - ry),
            new Vector2(center.x + rx, center.y - oy),
            new Vector2(center.x + rx, center.y));
        // Right to Bottom-center
        painter.BezierCurveTo(
            new Vector2(center.x + rx, center.y + oy),
            new Vector2(center.x + ox, center.y + ry),
            new Vector2(center.x, center.y + ry));
        // Bottom-center to Left
        painter.BezierCurveTo(
            new Vector2(center.x - ox, center.y + ry),
            new Vector2(center.x - rx, center.y + oy),
            new Vector2(center.x - rx, center.y));
        painter.ClosePath();
    }

    private static void AddBottomHalfEllipse(Painter2D painter, Vector2 center, float rx, float ry)
    {
        float ox = rx * Kappa;
        float oy = ry * Kappa;

        // Left to Bottom-center
        painter.BezierCurveTo(
            new Vector2(center.x - rx, center.y + oy),
            new Vector2(center.x - ox, center.y + ry),
            new Vector2(center.x, center.y + ry));
        // Bottom-center to Right
        painter.BezierCurveTo(
            new Vector2(center.x + ox, center.y + ry),
            new Vector2(center.x + rx, center.y + oy),
            new Vector2(center.x + rx, center.y));
    }
}