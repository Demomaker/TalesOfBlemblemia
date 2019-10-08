using UnityEngine;

namespace Game
{
    public class ComputerPlayer : UnitOwner
    {
        public override void Play()
        {
            for (int i = 0; i < playableUnits.Count; i++)
            {
                AiController.PlayTurn(playableUnits[i], ennemyUnits);
            }
            base.Play();
        }
    }
}