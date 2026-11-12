using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class AbilityHudController : MonoBehaviour
{
    private VisualElement panel;
    private Label titleLabel;
    private VisualElement buttonContainer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        // Critical: allow clicks on empty screen space to pass through to 2D sprites
        root.pickingMode = PickingMode.Ignore;

        // Build the HUD panel in code (or query via root.Q<VisualElement>("AbilityPanel") if using UXML)
        panel = new VisualElement();
        panel.style.position = Position.Absolute;
        panel.style.bottom = 24;
        panel.style.left = Length.Percent(50);
        panel.style.translate = new Translate(Length.Percent(-50), 0);
        panel.style.backgroundColor = new Color(0.06f, 0.09f, 0.16f, 0.95f);
        panel.style.paddingTop = 12;
        panel.style.paddingBottom = 12;
        panel.style.paddingLeft = 16;
        panel.style.paddingRight = 16;
        panel.style.borderTopLeftRadius = 8;
        panel.style.borderTopRightRadius = 8;
        panel.style.borderBottomLeftRadius = 8;
        panel.style.borderBottomRightRadius = 8;
        panel.style.display = DisplayStyle.None; // Start hidden
        root.Add(panel);

        // Header row with character title + close button
        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.marginBottom = 8;
        panel.Add(header);

        titleLabel = new Label("Unit");
        titleLabel.style.color = Color.white;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.Add(titleLabel);

        var closeBtn = new Button(HideHud) { text = "X" };
        closeBtn.style.marginLeft = 16;
        header.Add(closeBtn);

        // Horizontal row for ability buttons
        buttonContainer = new VisualElement();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        panel.Add(buttonContainer);

        CharacterClickSelector.OnCharacterClicked += ShowAbilitiesFor;
    }

    private void OnDisable()
    {
        CharacterClickSelector.OnCharacterClicked -= ShowAbilitiesFor;
    }

    private void ShowAbilitiesFor(CharacterClickSelector character)
    {
        titleLabel.text = character.CharacterName;
        buttonContainer.Clear();

        foreach (string abilityName in character.Abilities)
        {
            var btn = new Button(() => OnAbilityPressed(character, abilityName))
            {
                text = abilityName
            };
            btn.style.marginRight = 6;
            buttonContainer.Add(btn);
        }

        panel.style.display = DisplayStyle.Flex;
    }

    private void HideHud()
    {
        panel.style.display = DisplayStyle.None;
    }

    private void OnAbilityPressed(CharacterClickSelector character, string abilityName)
    {
        Debug.Log($"[{character.CharacterName}] Queued ability: {abilityName}");
        // Hook into your IsoGridHopper or command queue here
    }
}