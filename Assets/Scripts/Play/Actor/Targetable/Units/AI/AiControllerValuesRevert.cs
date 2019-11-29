namespace Game
{
    /// Author: Zacharie Lavigne
    public class AiControllerValuesRevert : AiControllerValues
    {
        public AiControllerValuesRevert()
        {
            attackingAxeWithAdvantageChoiceMod = 3.5f;
            attackingSpearWithAdvantageChoiceMod = 2.5f;
            attackingSwordWithAdvantageChoiceMod = 2.5f;
            attackingAxeWithoutAdvantageChoiceMod = -1.5f;
            attackingSpearWithoutAdvantageChoiceMod = -2.5f;
            attackingSwordWithoutAdvantageChoiceMod = -2f;
        }   
    }
}