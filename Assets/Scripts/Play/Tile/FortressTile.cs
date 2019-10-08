namespace Game
{
    //Author: Jérémie Bertrand
    public class FortressTile : Tile
    {
        public FortressTile() : base(TileType.Fortress, TileValues.DEFAULT_COST_TO_MOVE, TileValues.FORTRESS_DEFENSE_RATE)
        {
        }
    }
}