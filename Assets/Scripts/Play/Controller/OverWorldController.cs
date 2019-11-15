using System.Collections;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Controls the overworld
    /// Authors : Mike Bédard, Jérémie Bertrand
    /// </summary>
    [Findable(Game.Tags.OVER_WORLD_CONTROLLER_TAG)]
    public class OverWorldController : MonoBehaviour
    {
        [SerializeField] private bool isDebugging;
        private OnOverWorldEnter onOverWorldEnter;
        private Transform characterTransform;
        private GameObject overWorldPath;
        private LevelLoader levelLoader;
        private GameSettings gameSettings;

        private bool canLoadANewLevel = true;

        public Transform CharacterTransform => characterTransform;
        public bool IsDebugging => isDebugging;

        public bool CanLoadANewLevel => canLoadANewLevel;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            onOverWorldEnter = Harmony.Finder.OnOverWorldEnter;
            levelLoader = Harmony.Finder.LevelLoader;
            characterTransform = GameObject.FindWithTag(Tags.PLAYER_TAG)?.transform;
            overWorldPath = GameObject.Find("Path");
            if(characterTransform == null) Debug.LogError("Missing GameObject with tag 'Player' in OverWorld Scene.");
        }
        
        private void Start()
        {
            onOverWorldEnter.Publish(this);
        }

        private IEnumerator MoveCharacterToLevelEntry(Vector3 to)
        {
            if(characterTransform == null) yield break;
            while (characterTransform.position != to)
            {
                var startPosition = characterTransform.position;
                var endPosition = to;
                if(overWorldPath != null)
                {
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
            canLoadANewLevel = false;
            levelLoader.FadeToLevel(levelName, LoadSceneMode.Additive);
            yield return MoveCharacterToLevelEntry(position);
            canLoadANewLevel = true;
        }
    }
}
