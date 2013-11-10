

namespace TFSEventsProcessor.Tests
{
    using NUnit.Framework;
    using TFSEventsProcessor.Helpers;
    using TFSEventsProcessor.Tests.Helpers;

    [TestFixture]
    public class BuildStatusChangedXmlParseTests
    {
       [Test]
        public void Can_read_the_changed_fields_from_alert_xml_block()
        {
           // Arrange
           var alertMessage = TestData.DummyBuildStatusChangedAlertXmlWithQualityChange();

           // act
           var actual = EventXmlHelper.GetBuildStatusChangedAlertFields(alertMessage);

           // assert
           Assert.AreEqual("vstfs:///Build/Build/49", actual.BuildUri.ToString());
           Assert.AreEqual("Helpdesk Build CallTracker Dev_20100317.2 Quality Changed To Initial Test Passed", actual.Summary);
           Assert.AreEqual("Initial Test Passed", actual.NewQuality);

        }
             
    }
}
