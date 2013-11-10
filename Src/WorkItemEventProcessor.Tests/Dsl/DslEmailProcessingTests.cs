using NUnit.Framework;
using TFSEventsProcessor.Providers;

namespace TFSEventsProcessor.Tests.Dsl
{
    [TestFixture]
    public class DslEmailProcessingTests
    {
   
        [Test]
        public void Can_use_Dsl_to_send_an_email()
        {

            // arrange
            // redirect the console
            var consoleOut = Helpers.Logging.RedirectConsoleOut();

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\email\sendemail.py", tfsProvider.Object, emailProvider.Object);

            // assert
            emailProvider.Verify( e => e.SendEmailAlert("fred@test.com", "The subject", "The body of the email"));
        }


     
    }
}
