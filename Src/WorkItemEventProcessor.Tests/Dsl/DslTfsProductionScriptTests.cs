namespace TFSEventsProcessor.Tests.Dsl
{
    using System;
    using System.Collections.Generic;

    using Microsoft.TeamFoundation.WorkItemTracking.Client;

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
                               {
                                   "Arguments",
                                   new[] { "BuildEvent", "vstfs:///Build/Build/123" }
                               },
                           };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, true));
            emailProvider.Verify(
                e =>
                e.SendEmailAlert(
                    "richard@typhoontfs",
                    "123 quality changed",
                    "'123' retension set to 'True' as quality was changed to 'Test Quality'"));
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
                               {
                                   "Arguments",
                                   new[] { "BuildEvent", "vstfs:///Build/Build/123" }
                               },
                           };

            // act
            engine.RunScript(@"dsl\tfs\setbuildretensionbyquality.py", args, tfsProvider.Object, emailProvider.Object);

            // assert
            tfsProvider.Verify(t => t.SetBuildRetension(testUri, false));
            emailProvider.Verify(
                e =>
                e.SendEmailAlert(
                    "richard@typhoontfs",
                    "123 quality changed",
                    "'123' retension set to 'False' as quality was changed to 'Test Failed'"));
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

                var args = new Dictionary<string, object> { { "Arguments", new[] { "WorkItemEvent", "99" } }, };

                // act
                engine.RunScript(
                    @".\dsl",
                    @"dsl\tfs",
                    "sendtemplatedemail.py",
                    args,
                    tfsProvider.Object,
                    emailProvider.Object,
                    Helpers.TestData.DummyWorkItemChangedAlertXml());

                // assert
                emailProvider.Verify(
                    e =>
                    e.SendEmailAlert(Moq.It.IsAny<IFieldLookupProvider>(), @"dsl\tfs\EmailTemplate.htm", true, true));
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
            tfsProvider.Setup(t => t.GetBuildArgument(It.IsAny<Uri>(), "MinorVersion")).Returns(7);
            // used for the second call
            tfsProvider.Setup(t => t.GetBuildArgument(It.IsAny<Uri>(), "VersionStartDate")).Returns("1 Jan 2012");

            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            var args = new Dictionary<string, object>
                           {
                               {
                                   "Arguments",
                                   new[] { "BuildEvent", "vstfs:///Build/Build/123" }
                               },
                           };

            // act
            engine.RunScript(@"dsl\tfs\incrementbuildargument.py", args, tfsProvider.Object, emailProvider.Object);

            // assert

            emailProvider.Verify(
                e =>
                e.SendEmailAlert(
                    "richard@typhoontfs",
                    "Build Def version incremented",
                    "'Build Def' version incremented to 1.7.[days since 1 Jan 2012].[build count] as last build quality set to 'Released'"));

        }

        [Test]
        [Ignore ("This fails fails due to the Python script call to parentwi.State = 'Done' failing. This seems to be an issue with MS Fakes, as OK in production or if Typemock is used to fake out the TFS work item.")]
        public void Can_use_Dsl_to_update_parent_work_item_when_all_children_done()
        {
            using (ShimsContext.Create())
            {
                // arrange
                var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Debug);

                var emailProvider = new Moq.Mock<IEmailProvider>();
                var wi100 = new ShimWorkItem()
                                {
                                    TitleGet = () => "The child title",
                                    IdGet = () => 100,
                                    StateGet = () => "Done"
                                };
                var wi101 = new ShimWorkItem()
                                {
                                    TitleGet = () => "The child title",
                                    IdGet = () => 101,
                                    StateGet = () => "Done"
                                };
                var parent99 = new ShimWorkItem()
                                   {
                                       TitleGet = () => "The parent title",
                                       IdGet = () => 99,
                                       StateGet = () => "Committed"
                                   };
                
                var tfsProvider = new Moq.Mock<ITfsProvider>();
                tfsProvider.Setup(t => t.GetWorkItem(It.IsAny<int>())).Returns(wi100);
                tfsProvider.Setup(t => t.GetParentWorkItem(It.IsAny<WorkItem>())).Returns(parent99);
                tfsProvider.Setup(t => t.GetChildWorkItems(It.IsAny<WorkItem>()))
                    .Returns(new WorkItem[] { wi100, wi101 });

                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                var args = new Dictionary<string, object> { { "Arguments", new[] { "WorkItemEvent", "100" } } };

                // act
                engine.RunScript(
                    @"dsl\tfs\changeparentworkitemstate.py",
                    args,
                    tfsProvider.Object,
                    emailProvider.Object);

                // assert
                foreach (var line in memLogger.Logs)
                {
                    Console.WriteLine(line);
                }

                tfsProvider.Verify(t => t.UpdateWorkItem(parent99));
                emailProvider.Verify(
                    e =>
                    e.SendEmailAlert(
                        "richard@typhoontfs",
                        "Work item '99' has been updated",
                        "Work item '99' has been set as 'Done' as all its child work items are done"));
            }
        }

        [Test]
        public void Cannot_use_Dsl_to_update_parent_work_item_if_children_not_complete()
        {
            using (ShimsContext.Create())
            {
                // arrange
                var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Debug);

                var emailProvider = new Moq.Mock<IEmailProvider>(MockBehavior.Strict);
                var wi100 = new ShimWorkItem()
                {
                    TitleGet = () => "The child title",
                    IdGet = () => 100,
                    StateGet = () => "Done"
                };
                var wi101 = new ShimWorkItem()
                {
                    TitleGet = () => "The child title",
                    IdGet = () => 101,
                    StateGet = () => "Committed"
                };
                var parent99 = new ShimWorkItem()
                {
                    TitleGet = () => "The parent title",
                    IdGet = () => 99,
                    StateGet = () => "Committed"
                };


                var tfsProvider = new Moq.Mock<ITfsProvider>(MockBehavior.Strict);
                tfsProvider.Setup(t => t.GetWorkItem(It.IsAny<int>())).Returns(wi100);
                tfsProvider.Setup(t => t.GetParentWorkItem(It.IsAny<WorkItem>())).Returns(parent99);
                tfsProvider.Setup(t => t.GetChildWorkItems(It.IsAny<WorkItem>()))
                    .Returns(new WorkItem[] { wi100, wi101 });

                var engine = new TFSEventsProcessor.Dsl.DslProcessor();

                var args = new Dictionary<string, object> { { "Arguments", new[] { "WorkItemEvent", "100" } } };

                // act
                engine.RunScript(
                    @"dsl\tfs\changeparentworkitemstate.py",
                    args,
                    tfsProvider.Object,
                    emailProvider.Object);

                // assert
                foreach (var line in memLogger.Logs)
                {
                    Console.WriteLine(line);
                }

                // we rely on the MockBehavior.Strict to throw an error if any methods are called we have not expected
                // i.e. a call to the emailer or update of the work item
            }
        }
    }
}

