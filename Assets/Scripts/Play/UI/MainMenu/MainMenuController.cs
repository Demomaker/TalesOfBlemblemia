using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Controllers")] 
        [SerializeField] private SaveSlotSelectionController saveSlotSelectionController;
        [SerializeField] private LoadGameMenuController loadGameMenuController;
        [SerializeField] private OptionsMenuController optionsMenuController;
        [SerializeField] private CreditsMenuController creditsMenuController;

        [Header("Buttons")] 
        [SerializeField] private Button newGameButton = null;
        [SerializeField] private Button loadGameButton = null;
        [SerializeField] private Button optionsButton = null;
        [SerializeField] private Button creditsButton = null;
        [SerializeField] private Button exitGameButton = null;

        private Canvas mainMenuCanvas;
        private Navigator navigator;
        
        //BR : Je viens de voir que vous utilisez un canal événementiel pour envoyer un message au AudioManager de jouer
        //     un son.
        //
        //     En allant voir le Audio Manager, je me suis rendu compte que ce n'était pas le seul canal événementiel
        //     utilisé pour ça.
        //
        //     Pour éviter la multiplication des canaux, j'aurais créé un canal pour les événements de UI, un pour les
        //     événements d'unités et ainsi de suite. En utilisant une enum en paramêtre pour indiquer ce qu'il vient
        //     de se passer, le Audio Manager aurait pu à ce momment là choisir le bon son en conséquence.
        //
        //     Pour l'instant, laissez cela comme ça, mais si vous avez le courrage de le modifier, cela simplifierait
        //     grandement le code je pense.
        private OnMainMenuEnter onMainMenuEnter;

        private void Awake()
        {
            navigator = Finder.Navigator;
            mainMenuCanvas = GetComponent<Canvas>();
            onMainMenuEnter = new OnMainMenuEnter();
        }

        public void Enter()
        {
            onMainMenuEnter.Publish(this);
            navigator.Enter(mainMenuCanvas);
        }

        public void Leave()
        {
            navigator.Leave();
        }

        [UsedImplicitly]
        public void StartNewGame()
        {
            saveSlotSelectionController.Enter();
        }

        [UsedImplicitly]
        public void LoadGame()
        {
            loadGameMenuController.Enter();
        }

        [UsedImplicitly]
        public void Options()
        {
            optionsMenuController.Enter();
        }

        [UsedImplicitly]
        public void Credits()
        {
            creditsMenuController.Enter();
        }

        [UsedImplicitly]
        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}