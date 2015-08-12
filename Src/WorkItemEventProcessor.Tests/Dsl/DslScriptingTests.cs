using System;
using System.Collections.Generic;
using System.IO;
using IronPython.Runtime;
using Microsoft.Scripting;
using NUnit.Framework;

namespace TFSEventsProcessor.Tests.Dsl
{
    using TFSEventsProcessor.Providers;
    using NLog;

    [TestFixture]
    public class DslScriptingTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void A_null_dsl_script_throws_exception()
        {
            // arrange
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(null, null, null);

            // assert
            // checked in attribute
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void A_missing_dsl_script_throws_exception()
        {
            // arrange
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\dummy.py", null, null);

            // assert
            // checked in attribute
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void A_missing_TFS_provider_throws_exception()
        {
            // arrange
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\badscript1.py", null, null);

            // assert
            // checked in attribute
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void A_missing_Email_provider_throws_exception()
        {
            // arrange
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\badscript1.py", tfsProvider.Object, null);

            // assert
            // checked in attribute
        }

        [Test]
        [ExpectedException(typeof(SyntaxErrorException))]
        public void A_script_with_syntax_error_throws_exception()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // act
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\badscript1.py", tfsProvider.Object, emailProvider.Object);

            // assert
            // checked in attribute
        }

        [Test]
        [ExpectedException(typeof(UnboundNameException))]
        public void A_script_with_an_invalid_DSL_call_throws_exception()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\badscript2.py", tfsProvider.Object, emailProvider.Object);

            // assert
            // checked in attribute
        }


        [Test]
        public void Can_pass_argument_to_named_script()
        {
            // arrange
            // redirect the console
            var consoleOut = new StringWriter();
            Console.SetOut(consoleOut);

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "foo", "bar", "biz baz" } },
            };
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\args.py", args, tfsProvider.Object, emailProvider.Object);

            // assert

            Assert.AreEqual("['foo', 'bar', 'biz baz']" + Environment.NewLine, consoleOut.ToString());

        }

        [Test]
        public void Can_pass_argument_to_script_when_scripts_found_by_folder()
        {
            // arrange
            // redirect the console
            var consoleOut = new StringWriter();
            Console.SetOut(consoleOut);

            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            var args = new Dictionary<string, object>
            {
                { "Arguments", new[] { "foo", "bar", "biz baz" } },
            };
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@".\dsl", @"dsl\scripting","args.py", args, tfsProvider.Object, emailProvider.Object,string.Empty);

            // assert

            Assert.AreEqual("['foo', 'bar', 'biz baz']" + Environment.NewLine, consoleOut.ToString());

        }


        [Test]
        public void Can_use_methods_in_two_dsl_libraries_script()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Info);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting\twolibraries.py", tfsProvider.Object, emailProvider.Object);

            // assert
            Assert.AreEqual(3, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
             Assert.AreEqual("INFO | TFSEventsProcessor.Dsl.DslLibrary | When you add 1 and 2 you get 3", memLogger.Logs[2]);

             emailProvider.Verify(e => e.SendEmailAlert("fred@test.com", "The subject", "When you add 1 and 2 you get 3"));
        }


        [Test]
        public void Error_logged_if_no_Dsl_library_folder_found()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Info);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"c:\dummy", @"dsl\scripting\args.py", string.Empty, null, tfsProvider.Object, emailProvider.Object, string.Empty);

            // assert
            Assert.AreEqual(2, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            Assert.AreEqual(@"ERROR | TFSEventsProcessor.Dsl.DslProcessor | TFSEventsProcessor: DslProcessor cannot find DSL folder c:\dummy", memLogger.Logs[1]);

        }

        [Test]
        public void Error_logged_if_no_Dsl_library_in_dsl_folder()
        {
            // arrange
            var emailProvider = new Moq.Mock<IEmailProvider>();
            var tfsProvider = new Moq.Mock<ITfsProvider>();

            // create a memory logger
            var memLogger = Helpers.Logging.CreateMemoryTargetLogger(LogLevel.Info);
            var engine = new TFSEventsProcessor.Dsl.DslProcessor();

            // act
            engine.RunScript(@"dsl\scripting", @"dsl\scripting\args.py", string.Empty, null, tfsProvider.Object, emailProvider.Object, string.Empty);

            // assert
            Assert.AreEqual(3, memLogger.Logs.Count);
            // memLogger.Logs[0] is the log message from the runscript method
            // memLogger.Logs[1] is the log message from the runscript method
            Assert.IsTrue(memLogger.Logs[2].StartsWith(@"ERROR | TFSEventsProcessor.Dsl.DslProcessor | TFSEventsProcessor: DslProcessor cannot find DSL libraries in folder " ));

        }

    }
}
