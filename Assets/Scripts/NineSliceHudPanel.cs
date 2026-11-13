using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NineSliceHudPanel : MonoBehaviour
{
    [SerializeField] private Sprite nineSliceFrameSprite;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.pickingMode = PickingMode.Ignore;

        var panel = new VisualElement();
        panel.style.position = Position.Absolute;
        panel.style.bottom = 24;
        panel.style.left = 24;
        panel.style.width = 480; // Stretches cleanly to any width
        panel.style.height = 96;
        panel.style.backgroundImage = new StyleBackground(nineSliceFrameSprite);

        root.Add(panel);
    }
}