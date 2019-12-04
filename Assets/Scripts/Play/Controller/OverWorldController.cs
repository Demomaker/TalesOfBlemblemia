using System.Collections;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Controls the overworld
    /// Authors : Jérémie Bertrand
    /// </summary>
    [Findable(Game.Tags.OVER_WORLD_CONTROLLER_TAG)]
    public class OverWorldController : MonoBehaviour
    {
        [SerializeField] private Transform characterTransform;
        [SerializeField] private GameObject overWorldPath;
        
        [SerializeField] private bool isDebugging;
        
        [SerializeField] private UnitAnimator unitAnimator;

        private OnOverWorldEnter onOverWorldEnter;
        private LevelLoader levelLoader;
        private GameSettings gameSettings;
        private GameController gameController;
        

        public bool CharacterIsMoving { get; private set; }

        public Transform CharacterTransform => characterTransform;
        public bool IsDebugging => isDebugging;

        public bool CanLoadANewLevel => levelLoader.CanLoadNewLevel;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            onOverWorldEnter = Harmony.Finder.OnOverWorldEnter;
            levelLoader = Harmony.Finder.LevelLoader;
            gameController = Harmony.Finder.GameController;
        }
        
        private void Start()
        {
            onOverWorldEnter.Publish(this);
            if (gameController.PreviousLevelName == gameSettings.MorktressSceneName)
                levelLoader.FadeToLevel(gameSettings.EndSceneName, LoadSceneMode.Additive);
        }

        private IEnumerator MoveCharacterToLevelEntry(Vector3 to)
        {
            if(characterTransform == null || overWorldPath == null) yield break;
            while (characterTransform.position != to)
            {
                var startPosition = characterTransform.position;
                var endPosition = to;
                foreach (var node in overWorldPath.Children())
                {
                    while (node.transform.position.x == startPosition.x && startPosition.y != endPosition.y)
                    {
                        if (endPosition.y > startPosition.y)
                        {
                            yield return MoveCharacterToPosition(startPosition + new Vector3(0, 1, 0));
                        }
                        else
                        {
                            yield return MoveCharacterToPosition(startPosition + new Vector3(0, -1, 0));
                        }
                        startPosition = characterTransform.position;
                    }
                }
                if (endPosition.x > startPosition.x)
                {
                    yield return MoveCharacterToPosition(startPosition + new Vector3(1, 0, 0));
                }
                else
                {
                    yield return MoveCharacterToPosition(startPosition + new Vector3(-1, 0, 0));
                }
            }
        }

        private IEnumerator MoveCharacterToPosition(Vector3 endPosition)
        {
            
            var startPosition = characterTransform.position;
            var movementDuration = gameSettings.MovementDuration;
            for (float elapsedTime = 0; elapsedTime < movementDuration; elapsedTime += Time.deltaTime)
            {
                characterTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / movementDuration);
                yield return null;
            }
            characterTransform.position = endPosition;
        }
        
        public IEnumerator LoadLevel(string levelName, Vector3 position)
        {
            if(!CanLoadANewLevel || CharacterIsMoving) yield break;
            CharacterIsMoving = true;
            levelLoader.FadeToLevel(levelName, LoadSceneMode.Additive);
            unitAnimator.PlayMoveAnimation();
            yield return MoveCharacterToLevelEntry(position);
            unitAnimator.StopMoveAnimation();
            CharacterIsMoving = false;
        }

        [UsedImplicitly]
        public void ReturnToMainMenu()
        {
            levelLoader.FadeToLevel(gameSettings.MainmenuSceneName, LoadSceneMode.Additive);
        }
    }
}
