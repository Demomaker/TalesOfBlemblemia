using UnityEngine;

namespace Game
{
    public class HumanPlayer : UnitOwner
    {
        private static HumanPlayer instance = null;
        public static HumanPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HumanPlayer();
                }
                return instance;
            }
        }

        public override void CheckUnitDeaths()
        {
            base.CheckUnitDeaths();
            Debug.Log("Beginning of player turn");
        }

        private HumanPlayer()
        {
        }
    }
}