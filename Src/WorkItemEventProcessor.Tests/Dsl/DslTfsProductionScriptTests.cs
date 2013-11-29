namespace TFSEventsProcessor.Tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client.Fakes;
    using Microsoft.TeamFoundation.VersionControl.Client.Fakes;
    using Moq;
    using NLog;
    using TFSEventsProcessor.Providers;

    [TestFixture]
    public class DslTfsProductionScriptTests
    {

        [Test]
        public void Can_use_Dsl_to_set_build_retension_and_send_email()
        {

            // arrange
            var testUri = new Uri("vstfs:///Build/Build/123");

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(testUri);
            build.Setup(b => b.Quality).Returns("Test Quality");
            build.Setup(b => b.BuildNumber).Returns("123");

            var tfsProvider = new Moq.Mock<ITfsProvider>();
            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, true));
            emailProvider.Verify(e => e.SendEmailAlert("richard@typhoontfs", "123 quality changed", "'123' retension set to 'True' as quality was changed to 'Test Quality'"));
        }

        [Test]
        public void Can_use_Dsl_to_unset_build_retension_and_send_email()
        {

            // arrange
            var testUri = new Uri("vstfs:///Build/Build/123");

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(testUri);
            build.Setup(b => b.Quality).Returns("Test Failed");
            build.Setup(b => b.BuildNumber).Returns("123");

            var tfsProvider = new Moq.Mock<ITfsProvider>();
            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, false));
            emailProvider.Verify(e => e.SendEmailAlert("richard@typhoontfs", "123 quality changed", "'123' retension set to 'False' as quality was changed to 'Test Failed'"));
        }


        [Test]
        public void Can_use_Dsl_to_send_templated_email()
        {
            using (ShimsContext.Create())
            {
                // arrange
                var wi = new ShimWorkItem() { TitleGet = () => "The wi title", IdGet = () => 99 };
                var emailProvider = new Moq.Mock<IEmailProvider>();
                var tfsProvider = new Moq.Mock<ITfsProvider>();
                tfsProvider.Setup(t => t.GetWorkItem(99)).Returns(wi);
                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "WorkItemEvent", "99" } },
            };

                // act
                engine.RunScript(".", @"dsl\tfs", "sendtemplatedemail.py", args, tfsProvider.Object, emailProvider.Object, Helpers.TestData.DummyWorkItemChangedAlertXml());

                // assert
                emailProvider.Verify(e => e.SendEmailAlert(Moq.It.IsAny<IFieldLookupProvider>(), @"dsl\tfs\EmailTemplate.htm", true,true));
            }
        }

        [Test]
        public void Can_use_Dsl_to_increment_build_argument()
        {
            // arrange
            var buildUri = new Uri("vstfs:///Build/Build/123");
            var buildDefUri = new Uri("vstfs:///Build/Build/XYZ");

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var build = new Moq.Mock<IBuildDetail>();
            build.Setup(b => b.Uri).Returns(buildUri);
            build.Setup(b => b.Quality).Returns("Released");
            build.Setup(b => b.BuildDefinitionUri).Returns(buildDefUri);
            build.Setup(b => b.BuildDefinition.Name).Returns("Build Def");

            var tfsProvider = new Moq.Mock<ITfsProvider>();
            tfsProvider.Setup(t => t.GetBuildDetails(It.IsAny<Uri>())).Returns(build.Object);
            tfsProvider.Setup(t => t.GetBuildArgument(It.IsAny<Uri>(), "MajorVersion")).Returns(1);
            tfsProvider.Setup(t => t.GetBuildArgument(It.IsAny<Uri>(), "MinorVersion")).Returns(6);
            tfsProvider.Setup(t => t.GetBuildArgument(It.IsAny<Uri>(), "MinorVersion")).Returns(7); // used for the second call
      
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "BuildEvent", "vstfs:///Build/Build/123" } },
            };

            // act
            engine.RunScript(@"dsl\tfs\incrementbuildargument.py", args, tfsProvider.Object, emailProvider.Object);

            // assert

            emailProvider.Verify(e => e.SendEmailAlert("richard@typhoontfs", "Build Def version incremented", "'Build Def' version incremented to 1.7.0.x as last build quality set to 'Released'"));

        }

    }
}
