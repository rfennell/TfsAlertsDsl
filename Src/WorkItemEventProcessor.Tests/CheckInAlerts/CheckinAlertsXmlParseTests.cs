
namespace TFSEventsProcessor.Tests
{
    using NUnit.Framework;
    using TFSEventsProcessor.Tests.Helpers;
    using TFSEventsProcessor.Helpers;


    [TestFixture]
    public class CheckinAlertsXmlParseTests
    {
       [Test]
        public void Can_read_the_changed_fields_from_alert_xml_block()
        {
           // Arrange
           var alertMessage = TestData.DummyCheckInAlertXml();

           // act
           var actual = EventXmlHelper.GetCheckInDetails(alertMessage);

           // assert
           Assert.AreEqual("Scrum (TFVC) Changeset 62: The comment", actual.Summary);
           Assert.AreEqual(@"TYPHOONTFS\Richard", actual.Committer);
           Assert.AreEqual(@"Scrum (TFVC)", actual.TeamProject);
           Assert.AreEqual(@"The comment", actual.Comment);
           Assert.AreEqual(1, actual.FilesAdded.Count);
           Assert.AreEqual("NextClass.cs", actual.FilesAdded[0]);
           Assert.AreEqual(2, actual.FilesEdited.Count);
           Assert.AreEqual("ClassLibrary1.csproj", actual.FilesEdited[0]);
           Assert.AreEqual(1, actual.FilesDeleted.Count);
           Assert.AreEqual("Class1.cs", actual.FilesDeleted[0]);
           Assert.AreEqual(62, actual.Changeset);
        }
 
    
    }
}
