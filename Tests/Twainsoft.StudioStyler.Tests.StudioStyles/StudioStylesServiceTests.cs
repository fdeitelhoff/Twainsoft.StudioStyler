using NUnit.Framework;
using Twainsoft.StudioStyler.Services.StudioStyles;

namespace Twainsoft.StudioStyler.Tests.StudioStyles
{
    [TestFixture]
    public class StudioStylesServiceTests
    {
        [Test]
        public void GetAllSchemesTest()
        {
            // Arrange
            var studioStylesService = new StudioStylesService();

            // Act
            var schemes = studioStylesService.Range(1, 5);

            // Assert
            Assert.That(schemes.Count, Is.EqualTo(5));
        }
    }
}
