//------------------------------------------------------------------------------------------------- 
// <copyright file="DslScriptService.svc.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using NLog;
using TFSEventsProcessor.Helpers;
using TFSEventsProcessor.Providers;

namespace TFSEventsProcessor
{
    /// <summary>
    /// The WCF service entry point for the EmailService
    /// </summary>
    public class DslScriptService : IDslScriptService
    {
        /// <summary>
        /// Tag to use for Build event
        /// </summary>
        private enum EventTypes { BuildEvent, CheckInEvent, WorkItemEvent };

        /// <summary>
        /// Instance of nLog interface
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Instance of email provider
        /// </summary>
        private Providers.IEmailProvider iEmailProvider;

        /// <summary>
        /// Instance of TFS provider
        /// </summary>
        private Providers.ITfsProvider iTfsProvider;

        /// <summary>
        /// The script to run
        /// </summary>
        private string scriptFile;

        /// <summary>
        /// Folder to scan for DSL assemblies
        /// </summary>
        private string dslFolder;

        /// <summary>
        /// Folder to look for scripts in
        /// </summary>
        private string scriptFolder;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DslScriptService()
        {
            this.iEmailProvider = new Providers.SmtpEmailProvider(
                System.Configuration.ConfigurationManager.AppSettings["SMTPServer"],
                System.Configuration.ConfigurationManager.AppSettings["FromEmail"],
                System.Configuration.ConfigurationManager.AppSettings["EmailDomain"]);

            this.scriptFile = System.Configuration.ConfigurationManager.AppSettings["ScriptFile"];
            this.dslFolder = System.Configuration.ConfigurationManager.AppSettings["DSLFolder"];
            this.scriptFolder = System.Configuration.ConfigurationManager.AppSettings["ScriptFolder"];

            this.iTfsProvider = new Providers.TfsProvider();
        }

        /// <summary>
        /// Test constructor
        /// </summary>
        /// <param name="iEmailProvider">Email provider</param>
        /// <param name="iTfsProvider">Smpt Provider</param>
        /// <param name="scriptFile">The script file to run</param>
        /// <param name="dslFolder">Folder to scan for DSL assemblies</param>
        public DslScriptService(Providers.IEmailProvider iEmailProvider, Providers.ITfsProvider iTfsProvider, string scriptFile, string dslFolder)
        {
            this.iEmailProvider = iEmailProvider;
            this.iTfsProvider = iTfsProvider;
            this.scriptFile = scriptFile;
            this.dslFolder = dslFolder;
        }

        /// <summary>
        /// The main method that catches the alert
        /// </summary>
        /// <param name="eventXml">The XML block of the work item changes</param>
        /// <param name="tfsIdentityXml">The TFS security details</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We are using a wide exception to make sure we don't block")]
        public void Notify(string eventXml, string tfsIdentityXml)
        {
            try
            {
                if (ConfigHelper.ParseOrDefault(System.Configuration.ConfigurationManager.AppSettings["LogEventsToFile"]) == true)
                {
                    var logPath = ConfigHelper.GetLoggingPath();
                    logger.Info(string.Format("TFSEventsProcessor: DslScriptService Event being logged to [{0}]", logPath));
                    LoggingHelper.DumpEventToDisk(eventXml, logPath);
                }

                // Create a new Tfs helper
                this.iTfsProvider.UnpackIdentity(tfsIdentityXml);

                // work out the event type
                string[] argItems = null;
                try
                {
                    var buildDetails = EventXmlHelper.GetBuildStatusChangedAlertFields(eventXml);
                    argItems = new[] { EventTypes.BuildEvent.ToString(), buildDetails.BuildUri.ToString() };
                    logger.Info(string.Format(
                        "TFSEventsProcessor: DslScriptService Event being processed for Build:{0}",
                        buildDetails.BuildUri));

                }
                catch (NullReferenceException)
                {
                    // if it not build must be work item
                    // Extract the required information out of the eventXml
                    var workItemId = EventXmlHelper.GetWorkItemValue<int>(
                        eventXml,
                        EventXmlHelper.FieldSection.CoreFields,
                        EventXmlHelper.FieldType.IntegerField,
                        EventXmlHelper.ValueType.NewValue,
                        "System.Id");
                    if (workItemId > 0)
                    {
                        argItems = new[] { EventTypes.WorkItemEvent.ToString(), workItemId.ToString() };
                        logger.Info(
                            string.Format("TFSEventsProcessor: DslScriptService Event being processed for WI:{0}", workItemId));
                    }
                    else
                    {
                        try
                        {
                            var checkInDetails = EventXmlHelper.GetCheckInDetails(eventXml);
                            argItems = new[] { EventTypes.CheckInEvent.ToString(), checkInDetails.Changeset.ToString() };
                            logger.Info(
                                string.Format(
                                    "TFSEventsProcessor: DslScriptService Event being processed for Checkin:{0}",
                                    checkInDetails.Changeset));
                        }
                        catch (NullReferenceException)
                        {
                            // other event type
                        }
                    }
                }
                var args = new Dictionary<string, object>
                {
                    { "Arguments", argItems },
                };

                var engine = new TFSEventsProcessor.Dsl.DslProcessor();
                engine.RunScript(
                    this.dslFolder,
                    this.scriptFolder,
                    GetScriptName(argItems[0], this.scriptFile),
                    args,
                    this.iTfsProvider,
                    this.iEmailProvider,
                    eventXml);

            }
            catch (Exception ex)
            {
                // using a global exception catch to make sure we don't block any threads
                this.DumpException(ex);
            }

        }

        /// <summary>
        /// Returns the name of the script to run
        /// </summary>
        /// <param name="type">The event type</param>
        /// <param name="defaultScript">Default script name</param>
        /// <returns></returns>
        private static string GetScriptName(string type, string defaultScript)
        {
            var retItem = defaultScript;
            if (string.IsNullOrEmpty(defaultScript))
            {
                retItem = string.Format("{0}.py", type.ToString());
            }
            logger.Info(
                        string.Format(
                            "TFSEventsProcessor: DslScriptService using script file {0}",
                            retItem));
            return retItem;
        }

        /// <summary>
        /// Outputs error exception
        /// </summary>
        /// <param name="ex">The exception to dump</param>
        private void DumpException(Exception ex)
        {
            logger.Error(string.Format("TFSEventsProcessor.DslScriptService: {0}", ex.Message));
            logger.Error(string.Format("TFSEventsProcessor.DslScriptService: {0}", ex.StackTrace));
            if (ex.InnerException != null)
            {
                this.DumpException(ex.InnerException);
            }
        }
    }
}
