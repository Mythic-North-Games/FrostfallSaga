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
            var hexagoneSize = 10f;
            var expectedOuterRadius = 10f;

            // Act
            var outerRadius = HexMetrics.OuterRadius(hexagoneSize);

            // Assert
            Assert.AreEqual(expectedOuterRadius, outerRadius);
        }

        [Test]
        public void InnerRadius_Test()
        {
            // Arrange
            var hexagoneSize = 10f;
            var expectedInnerRadius = 8.66025352f;

            // Act
            var innerRadius = HexMetrics.InnerRadius(hexagoneSize);

            // Assert
            Assert.AreEqual(expectedInnerRadius, innerRadius);
        }

        [Test]
        public void OuterRadius_GreaterThen_InnerRadius_Test()
        {
            // Arrange
            var hexagoneSize = 10f;

            // Act
            var outerRadius = HexMetrics.OuterRadius(hexagoneSize);
            var innerRadius = HexMetrics.InnerRadius(hexagoneSize);

            // Assert
            Assert.Greater(outerRadius, innerRadius);
        }
    }
}