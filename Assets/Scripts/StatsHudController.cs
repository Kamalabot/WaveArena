using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StatsHudController : MonoBehaviour
{
    [SerializeField] private Texture2D hudFramePng; // Exported from Inkscape

    private Label energyLabel;
    private Label stepsLabel;
    private Label scoreLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.pickingMode = PickingMode.Ignore;

        // 1. Container displaying your Inkscape PNG frame
        var hudBox = new VisualElement();
        hudBox.style.position = Position.Absolute;
        hudBox.style.top = Length.Percent(50);
        hudBox.style.translate = new Translate(0, Length.Percent(-50)); // Centers vertically on exact screen midpoint
        hudBox.style.left = 24;   // Left margin (use 0 for edge-to-edge)
        hudBox.style.right = 24;  // Right margin (use 0 for edge-to-edge)
        hudBox.style.height = 120; // Adjust height as needed (or use Length.Percent(40))
        hudBox.style.flexDirection = FlexDirection.Row;
        hudBox.style.justifyContent = Justify.SpaceAround;
        hudBox.style.alignItems = Align.Center;

        if (hudFramePng != null)
            hudBox.style.backgroundImage = new StyleBackground(hudFramePng);

        root.Add(hudBox);

        // 2. Three dynamic text numbers inside the PNG frame
        energyLabel = CreateNumberLabel(hudBox, "100");
        stepsLabel  = CreateNumberLabel(hudBox, "0");
        scoreLabel  = CreateNumberLabel(hudBox, "0");
    }

    private Label CreateNumberLabel(VisualElement parent, string initialText)
    {
        var label = new Label(initialText);
        label.style.color = Color.white;
        label.style.fontSize = 20;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        parent.Add(label);
        return label;
    }

    // Call this whenever gameplay values change
    public void UpdateNumbers(int energy, int steps, int score)
    {
        energyLabel.text = energy.ToString();
        stepsLabel.text  = steps.ToString();
        scoreLabel.text  = score.ToString();
    }
}