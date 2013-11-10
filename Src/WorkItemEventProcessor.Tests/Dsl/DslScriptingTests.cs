using System;
using System.Collections.Generic;
using System.IO;
using IronPython.Runtime;
using Microsoft.Scripting;
using NUnit.Framework;

namespace TFSEventsProcessor.Tests.Dsl
{
    using TFSEventsProcessor.Providers;

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
        public void Can_pass_argument_to_script()
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

    }
}
