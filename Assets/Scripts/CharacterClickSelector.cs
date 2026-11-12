using System;
using UnityEngine;
using UnityEngine.EventSystems;
using PrimeTween;

public class CharacterClickSelector : MonoBehaviour, IPointerClickHandler
{
    [Header("Character Data")]
    [SerializeField] private string characterName = "Scout Bot";
    [SerializeField] private string[] abilities = { "Move Forward", "Turn Left", "Turn Right", "Scan Tile" };

    // Decoupled static event so any HUD controller can listen without tight references
    public static event Action<CharacterClickSelector> OnCharacterClicked;

    public string CharacterName => characterName;
    public string[] Abilities => abilities;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Only respond to left-click
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Tactile click feedback on the sprite
        Tween.PunchScale(transform, new Vector3(0.15f, 0.15f, 0f), duration: 0.2f);

        OnCharacterClicked?.Invoke(this);
    }
}