using UnityEngine;

namespace Game
{
    public class ComputerPlayer : UnitOwner
    {
        public override void Play()
        {
            for (int i = 0; i < playableUnits.Count; i++)
            {
                                                                    //TODO changer pour la grid de jeu
                AiController.PlayTurn(playableUnits[i], ennemyUnits, new Grid());
            }
            base.Play();
        }
    }
}