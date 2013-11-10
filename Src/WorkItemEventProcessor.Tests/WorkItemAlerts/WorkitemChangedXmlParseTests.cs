
namespace TFSEventsProcessor.Tests
{
    using NUnit.Framework;
    using TFSEventsProcessor.Tests.Helpers;
    using TFSEventsProcessor.Helpers;


    [TestFixture]
    public class WorkitemChangedXmlParseTests
    {
       [Test]
        public void Can_read_the_changed_fields_from_alert_xml_block()
        {
           // Arrange
           var alertMessage = TestData.DummyWorkItemChangedAlertXml();

           // act
           var actual = EventXmlHelper.GetWorkItemChangedAlertFields(alertMessage);

           // assert
           Assert.AreEqual(4 , actual.Count);
           Assert.AreEqual("Microsoft.VSTS.Common.StackRank", actual[1].ReferenceName);
           Assert.AreEqual(string.Empty, actual[1].OldValue);
           Assert.AreEqual("1", actual[1].NewValue);

        }

       [Test]
        public void Can_read_who_changed_the_workitem_from_alert_xml_block()
        {
            // Arrange
            var alertMessage = TestData.DummyWorkItemChangedAlertXml();

            // act
            var actual = EventXmlHelper.GetChangedBy(alertMessage);

            // assert
            Assert.AreEqual("Administrator", actual);

        }

       [Test]
        public void Can_read_changed_fields_when_value_set_to_null()
        {
            // Arrange
            var alertMessage = TestData.DummyAlertXmlWithNullFieldValue();

            // act
            var actual = EventXmlHelper.GetWorkItemChangedAlertFields(alertMessage);

            // assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Bm.CustomField1", actual[0].ReferenceName);
            Assert.AreEqual("s1", actual[0].OldValue);
            Assert.AreEqual(string.Empty, actual[0].NewValue);


        }
               
    
    }
}
