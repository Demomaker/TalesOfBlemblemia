using Boo.Lang;
using UnityEngine;

namespace Game
{
    public class ComputerPlayer : UnitOwner
    {
        private static ComputerPlayer instance = null;
        
        public static ComputerPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComputerPlayer();
                }
                return instance;
            }
        }

        private ComputerPlayer()
        {
        }

        public override void Play()
        {
            Debug.Log("Beginning of ai turn");
            for (int i = 0; i < playableUnits.Count; i++)
            {
                AiController.PlayTurn(playableUnits[i], enemyUnits);
            }
            base.Play();
        }
    }
}