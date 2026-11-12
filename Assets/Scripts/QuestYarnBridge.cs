using UnityEngine;
using Yarn.Unity;

public class QuestYarnBridge : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;

    private VariableStorageBehaviour VariableStorage => dialogueRunner.VariableStorage;

    // Call this from your Inventory / Pickup script
    public void OnItemCollected(string itemId)
    {
        if (itemId == "SecurityKeycard")
        {
            VariableStorage.SetValue("$hasKeycard", true);
        }
    }

    // Call this from your Enemy / Objective script
    public void OnDroneDestroyed()
    {
        VariableStorage.TryGetValue("$dronesCleared", out float currentCount);
        VariableStorage.SetValue("$dronesCleared", currentCount + 1f);
    }
}