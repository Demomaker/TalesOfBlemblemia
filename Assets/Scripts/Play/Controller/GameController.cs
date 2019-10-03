
using UnityEngine;

namespace Game
 {
     public class GameController : MonoBehaviour
     {
         [SerializeField] private int choiceForEasy = 10;
         [SerializeField] private int choiceForMedium = 5;
         [SerializeField] private int choiceForHard = 3;
         
         private DifficultyLevel difficultyLevel;
         private readonly Dictionary<DifficultyLevel, int> choiceRangePerDifficulty = new Dictionary<DifficultyLevel, int>();
         private int choiceRange;
         private bool permaDeath;
         

         public GameController(DifficultyLevel difficultyLevel)
         {
             this.difficultyLevel = difficultyLevel;
             choiceRangePerDifficulty.Add(DifficultyLevel.Easy, choiceForEasy);
             choiceRangePerDifficulty.Add(DifficultyLevel.Medium, choiceForMedium);
             choiceRangePerDifficulty.Add(DifficultyLevel.Hard, choiceForHard);
         }

         public void Awake()
         {
             choiceRange = choiceRangePerDifficulty[difficultyLevel];
             permaDeath = difficultyLevel != DifficultyLevel.Easy;
         }
     }

     public enum DifficultyLevel
     {
         Easy,
         Medium,
         Hard
     }
 }
