namespace Game
{
    /// <summary>
    /// The human player that controls its units
    /// Authors: Jérémie Bertrand
    /// </summary>
    public class HumanPlayer : UnitOwner
    {
        private const string HUMAN_PLAYER_NAME = "Player";

        public HumanPlayer() : base(HUMAN_PLAYER_NAME)
        {
            //Empty on purpose
        }
    }
}