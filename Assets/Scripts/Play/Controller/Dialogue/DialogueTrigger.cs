using UnityEngine;

namespace Game
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue dialogue;
        private DialogueManager dialogueManager;

        private void Awake()
        {
            dialogueManager = Harmony.Finder.DialogueManager;
        }

        public void TriggerDialogue()
        {
            dialogueManager.StartDialogue(dialogue);
        }
    }
}

