using System;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class InterestPointData
    {
        public AInterestPointConfigurationSO interestPointConfiguration;
        public int cellX;
        public int cellY;

        public InterestPointData(AInterestPointConfigurationSO config, int cellX, int cellY)
        {
            interestPointConfiguration = config;
            this.cellX = cellX;
            this.cellY = cellY;
        }
    }
}