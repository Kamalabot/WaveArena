using UnityEngine;
using UnityEngine.UIElements;
using PrimeTween;

[RequireComponent(typeof(UIDocument))]
public class ClickHudController : MonoBehaviour
{
    [Header("9-Slice Frame Sprite (Optional)")]
    [SerializeField] private Sprite nineSliceFrameSprite;

    private VisualElement hudBox;
    private Label titleLabel;
    private VisualElement buttonContainer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        // Let clicks on empty screen space pass through to 2D sprites
        root.pickingMode = PickingMode.Ignore;

        // Centered modal panel (70% width x 50% height)
        hudBox = new VisualElement();
        hudBox.style.position = Position.Absolute;
        hudBox.style.top = Length.Percent(50);
        hudBox.style.left = Length.Percent(50);
        hudBox.style.translate = new Translate(Length.Percent(-50), Length.Percent(-50));
        hudBox.style.width = Length.Percent(70);
        hudBox.style.height = Length.Percent(50);

        // Layout children vertically: Header row on top, Ability buttons centered below
        hudBox.style.flexDirection = FlexDirection.Column;
        hudBox.style.justifyContent = Justify.SpaceBetween;
        hudBox.style.alignItems = Align.Center;
        hudBox.style.paddingTop = 24;
        hudBox.style.paddingBottom = 24;
        hudBox.style.paddingLeft = 28;
        hudBox.style.paddingRight = 28;

        // Use 9-slice Inkscape Sprite if assigned; otherwise fall back to dark slate box
        if (nineSliceFrameSprite != null)
        {
            hudBox.style.backgroundImage = new StyleBackground(nineSliceFrameSprite);
        }
        else
        {
            hudBox.style.backgroundColor = new Color(0.06f, 0.09f, 0.16f, 0.96f);
            hudBox.style.borderTopLeftRadius = 12;
            hudBox.style.borderTopRightRadius = 12;
            hudBox.style.borderBottomLeftRadius = 12;
            hudBox.style.borderBottomRightRadius = 12;
        }

        hudBox.style.display = DisplayStyle.None; // Start hidden until sprite is clicked
        root.Add(hudBox);

        // 1. Header Row (Unit Name + Close Button)
        var header = new VisualElement();
        header.style.width = Length.Percent(100);
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.alignItems = Align.Center;
        hudBox.Add(header);

        titleLabel = new Label("Unit");
        titleLabel.style.color = Color.white;
        titleLabel.style.fontSize = 24;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.Add(titleLabel);

        var closeBtn = new Button(HideHud) { text = "X" };
        closeBtn.style.width = 36;
        closeBtn.style.height = 36;
        closeBtn.style.fontSize = 18;
        header.Add(closeBtn);

        // 2. Center Row for Ability Buttons
        buttonContainer = new VisualElement();
        buttonContainer.style.width = Length.Percent(100);
        buttonContainer.style.flexGrow = 1;
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.SpaceAround;
        buttonContainer.style.alignItems = Align.Center;
        hudBox.Add(buttonContainer);

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
            btn.style.paddingTop = 12;
            btn.style.paddingBottom = 12;
            btn.style.paddingLeft = 18;
            btn.style.paddingRight = 18;
            btn.style.fontSize = 18;
            buttonContainer.Add(btn);
        }

        hudBox.style.display = DisplayStyle.Flex;

        // Smooth fade-in via PrimeTween
        hudBox.style.opacity = 0f;
        Tween.Custom(0f, 1f, new TweenSettings(0.18f, Ease.OutQuad), alpha =>
        {
            hudBox.style.opacity = alpha;
        });
    }

    private void HideHud()
    {
        hudBox.style.display = DisplayStyle.None;
    }

    private void OnAbilityPressed(CharacterClickSelector character, string abilityName)
    {
        Debug.Log($"[{character.CharacterName}] Selected ability: {abilityName}");
        // Optional: close the center modal automatically after choosing an ability
        HideHud();
    }
}