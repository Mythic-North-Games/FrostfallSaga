using FrostfallSaga.EditModeTests.FightTests;
using FrostfallSaga.EditModeTests.FightTests.FighterTests;
using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.Assets.Code.Tests.EditModeTests.FighterTest
{
    public class FighterActiveAbilityTests
    {
        [Test]
        public void AP_Positive_Test()
        {
            int ap = 5;
            int initialAp = 10;
            int expectedFighterAp = initialAp + ap;
            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().actionPoints = initialAp;

        }
    }
}
