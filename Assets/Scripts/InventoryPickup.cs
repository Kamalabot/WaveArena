using UnityEngine;
using Yarn.Unity;

public class InventoryPickup : MonoBehaviour
{
    [SerializeField] private string itemId = "SecurityKeycard";
    [SerializeField] private string yarnNodeOnPickup = "Event_KeycardFound";
    [SerializeField] private DialogueRunner dialogueRunner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1. Update persistent Yarn state
        dialogueRunner.VariableStorage.SetValue("$hasKeycard", true);

        // 2. Immediately trigger the specific reaction node
        TriggerNodeSafe(yarnNodeOnPickup);

        Destroy(gameObject);
    }

    private void TriggerNodeSafe(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName) || dialogueRunner == null) return;

        if (!dialogueRunner.NodeExists(nodeName))
        {
            Debug.LogWarning($"[Yarn] Node '{nodeName}' not found in YarnProject.");
            return;
        }

        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop(); // Or queue it until onDialogueComplete fires
        }

        dialogueRunner.StartDialogue(nodeName);
    }
}