using Harmony;
using UnityEngine;

namespace Game
{
    [Findable(Game.Tags.GAME_EVENT_HANDLER_TAG)]
    public class GameEventHandler : MonoBehaviour
    {
        private void Awake()
        {
            AddEventComponents();
        }
        private void AddEventComponents()
        {
            gameObject.AddComponent<OnAttack>();
            gameObject.AddComponent<OnDodge>();
            gameObject.AddComponent<OnHurt>();
            gameObject.AddComponent<OnButtonClick>();
            gameObject.AddComponent<OnLevelChange>();
            gameObject.AddComponent<OnLevelVictory>();
            gameObject.AddComponent<OnLevelFailed>();
            gameObject.AddComponent<OnMissionFailed>();
            gameObject.AddComponent<OnMusicToggle>();
            gameObject.AddComponent<OnUnitDeath>();
            gameObject.AddComponent<OnUnitMove>();
            gameObject.AddComponent<OnMainMenuEnter>();
            gameObject.AddComponent<OnMainVolumeChange>();
            gameObject.AddComponent<OnMusicVolumeChange>();
            gameObject.AddComponent<OnOverWorldEnter>();
            gameObject.AddComponent<OnPlayerUnitLoss>();
            gameObject.AddComponent<OnSFXToggle>();
            gameObject.AddComponent<OnSFXVolumeChange>();
        }
    }
}

