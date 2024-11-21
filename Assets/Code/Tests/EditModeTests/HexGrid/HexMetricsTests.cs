using FrostfallSaga.Grid;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.Grid
{
    public class HexMetricsTests
    {
        [Test]
        public void OuterRadius_Test()
        {
            // Arrange
            float hexagoneSize = 10f;
            float expectedOuterRadius = 10f;

            // Act
            float outerRadius = HexMetrics.OuterRadius(hexagoneSize);

            // Assert
            Assert.AreEqual(expectedOuterRadius, outerRadius);
        }

        [Test]
        public void InnerRadius_Test()
        {
            // Arrange
            float hexagoneSize = 10f;
            float expectedInnerRadius = 8.66025352f;

            // Act
            float innerRadius = HexMetrics.InnerRadius(hexagoneSize);

            // Assert
            Assert.AreEqual(expectedInnerRadius, innerRadius);
        }

        [Test]
        public void OuterRadius_GreaterThen_InnerRadius_Test()
        {
            // Arrange
            float hexagoneSize = 10f;

            // Act
            float outerRadius = HexMetrics.OuterRadius(hexagoneSize);
            float innerRadius = HexMetrics.InnerRadius(hexagoneSize);

            // Assert
            Assert.Greater(outerRadius, innerRadius);
        }
    }
}
