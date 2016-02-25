using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TFSEventsProcessor.Tests.ScriptLoader
{
    [TestClass]
    public class ScriptSelectionTests
    {
        [TestMethod]
        public void Can_set_a_default_script()
        {
            // arrange
            var script = "default.py";

            // act
            var actual = DslScriptService.GetScriptName("no used", script);

            // assert
            Assert.AreEqual(script, actual);
        }
        
        [TestMethod]
        public void Uses_eventname_if_no_default_script()
        {
            // arrange
            var eventName = DslScriptService.EventTypes.BuildEvent;

            // act
            var actual = DslScriptService.GetScriptName(eventName.ToString(), "");

            // assert
            Assert.AreEqual(eventName + ".py", actual);
        }
    }
}
