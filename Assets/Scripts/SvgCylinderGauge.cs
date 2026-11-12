using UnityEngine;
using UnityEngine.UIElements;
using PrimeTween;

[RequireComponent(typeof(UIDocument))]
public class SvgCylinderGauge : MonoBehaviour
{
    [Header("SVG Assets")]
    [SerializeField] private VectorImage cylinderFillSvg;
    [SerializeField] private VectorImage cylinderOutlineSvg; // Optional border overlay

    [Header("Dimensions (px)")]
    [SerializeField] private float width = 80f;
    [SerializeField] private float height = 220f;

    private VisualElement fillClipMask;
    private float currentFill = 0f;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 1. Main Gauge Container
        var container = new VisualElement();
        container.style.position = Position.Absolute;
        container.style.left = 40;
        container.style.bottom = 40;
        container.style.width = width;
        container.style.height = height;
        container.style.justifyContent = Justify.FlexEnd; // Anchor fill to bottom
        root.Add(container);

        // 2. Clipping Mask (shrinks/grows vertically to crop the SVG)
        fillClipMask = new VisualElement();
        fillClipMask.style.width = Length.Percent(100);
        fillClipMask.style.height = Length.Percent(0);
        fillClipMask.style.overflow = Overflow.Hidden;
        fillClipMask.style.justifyContent = Justify.FlexEnd;
        container.Add(fillClipMask);

        // 3. Full-Size SVG Graphic inside the mask
        var fillGraphic = new VisualElement();
        fillGraphic.style.width = width;
        fillGraphic.style.height = height;
        fillGraphic.style.flexShrink = 0; // Prevents SVG from squashing as mask shrinks
        fillGraphic.style.backgroundImage = new StyleBackground(cylinderFillSvg);
        fillClipMask.Add(fillGraphic);

        // 4. Optional Outline Overlay on top
        if (cylinderOutlineSvg != null)
        {
            var outlineGraphic = new VisualElement();
            outlineGraphic.style.position = Position.Absolute;
            outlineGraphic.style.width = width;
            outlineGraphic.style.height = height;
            outlineGraphic.style.backgroundImage = new StyleBackground(cylinderOutlineSvg);
            container.Add(outlineGraphic);
        }
    }

    // Call this whenever your gameplay variable changes (normalized 0.0 to 1.0)
    public void SetFillAmount(float normalizedValue, float tweenDuration = 0.25f)
    {
        float target = Mathf.Clamp01(normalizedValue);

        Tween.Custom(currentFill, target, new TweenSettings(tweenDuration, Ease.OutCubic), val =>
        {
            currentFill = val;
            fillClipMask.style.height = Length.Percent(val * 100f);
        });
    }
}