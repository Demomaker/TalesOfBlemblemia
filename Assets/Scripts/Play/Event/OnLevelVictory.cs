using Harmony;

namespace Game
{
    public class OnLevelVictory : EventChannel<LevelController>
    {
        //BC : AHHHHH!!!!!! Un event statique!!!!!!! NOPE NOPE NOPE ABORT ABORT ABORT.....
        //     NE FAITES JAMAIS ÇA!!!!!! Corrigez moi ça pour la beta, ça presse!
        public static event EventHandler<LevelController> Notify; 
        public override void Publish(LevelController eventParam)
        {
            Notify?.Invoke(eventParam);
        }
    }
}