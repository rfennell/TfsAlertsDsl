//------------------------------------------------------------------------------------------------- 
// <copyright file="DslLibrary.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using NLog;
using System;

namespace TFSEventsProcessor.Dsl
{
    using System.ComponentModel.Composition;
    using System.Globalization;

    using TFSEventsProcessor.Helpers;
    using TFSEventsProcessor.Providers;

    /// <summary>
    /// Contains the DSL API
    /// </summary>
    [Export(typeof(IDslLibrary))]
    public class DslLibrary : IDslLibrary
    {
        /// <summary>
        /// Instance of nLog interface
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The raw event XML
        /// </summary>
        private string eventXml;

        /// <summary>
        /// The raw event XML
        /// </summary>
        public string EventXml
        {
            get
            {
                return this.eventXml;
            }
            set
            {
                this.eventXml = value;
            }
        }

        /// <summary>
        /// The base script path
        /// </summary>
        private string scriptFolder;

        /// <summary>
        /// The script folder
        /// </summary>
        public string ScriptFolder
        {
            get
            {
                return this.scriptFolder;
            }
            set
            {
                this.scriptFolder = value;
            }
        }


        /// <summary>
        /// Instance of TFS provider
        /// </summary>
        private Providers.ITfsProvider iTfsProvider;

        /// <summary>
        /// Instance of TFS provider
        /// </summary>
        public Providers.ITfsProvider TfsProvider
        {
            get
            {
                return this.iTfsProvider;
            }
            set
            {
                this.iTfsProvider = value;
            }
        }

        /// <summary>
        /// Instance of the Email provider
        /// </summary>
        public Providers.IEmailProvider EmailProvider
        {
            get
            {
                return this.iEmailProvider;
            }
            set
            {
                this.iEmailProvider = value;
            }
        }

        /// <summary>
        /// Instance of the Email provider
        /// </summary>
        private Providers.IEmailProvider iEmailProvider;

        /// <summary>
        /// Constructor for DSL library
        /// </summary>
        /// <param name="iTfsProvider">The TFS provider</param>
        /// <param name="iEmailProvider">The Email provider</param>
        public DslLibrary(Providers.ITfsProvider iTfsProvider, Providers.IEmailProvider iEmailProvider)
        {
            this.iTfsProvider = iTfsProvider;
            this.iEmailProvider = iEmailProvider;
        }

        /// <summary>
        /// Constructor for DSL library, used when loading from MEF
        /// </summary>
        public DslLibrary()
        {

        }

        /// <summary>
        /// Sends a message to the info level logger
        /// </summary>
        /// <param name="msg">The message</param>
        public static void LogInfoMessage(object msg)
        {
            logger.Info(BuildMessageText(msg));
        }

        /// <summary>
        /// Sends a message to the debug level logger
        /// </summary>
        /// <param name="msg">The message</param>
        public static void LogDebugMessage(object msg)
        {
            logger.Debug(BuildMessageText(msg));
        }

        /// <summary>
        /// Sends a message to the error level logger
        /// </summary>
        /// <param name="msg">The message</param>
        public static void LogErrorMessage(object msg)
        {
            logger.Error(BuildMessageText(msg));
        }

        /// <summary>
        /// Works out the value passed and tries to convert it into something that can go into the text logger
        /// </summary>
        /// <param name="msg">The message</param>
        /// <returns>A string</returns>
        private static string BuildMessageText(object msg)
        {
            var retValue = new StringBuilder();
            if (msg.GetType() == typeof(IronPython.Runtime.List))
            {
                retValue.Append("List: ");
                foreach (var item in (IronPython.Runtime.List)msg)
                {
                    retValue.Append(string.Format("[{0}] ", item.ToString()));
                }
            }
            else
            {
                retValue.Append(msg.ToString());
            }
            return retValue.ToString();
        }

        /// <summary>
        /// Gets a work item from TFS
        /// </summary>
        /// <param name="id">The work item ID</param>
        /// <returns>A workitem</returns>
        public WorkItem GetWorkItem(int id)
        {
            return this.iTfsProvider.GetWorkItem(id);
        }

        /// <summary>
        /// Gets a parent work item from TFS
        /// </summary>
        /// <param name="wi">The work item to find parent for</param>
        /// <returns>A workitem</returns>
        public WorkItem GetParentWorkItem(WorkItem wi)
        {
            return this.iTfsProvider.GetParentWorkItem(wi);
        }

        /// <summary>
        /// Gets the child work items for a work item from TFS
        /// </summary>
        /// <param name="wi">The work item to find parent for</param>
        /// <returns>A workitem</returns>
        public WorkItem[] GetChildWorkItems(WorkItem wi)
        {
            return this.iTfsProvider.GetChildWorkItems(wi);
        }

        /// <summary>
        /// Gets the details of a build
        /// </summary>
        /// <param name="buildUri">The build url</param>
        /// <returns>The build details</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "We are doing the conversion to ease life in the dsl")]
        public IBuildDetail GetBuildDetails(string buildUri)
        {
            return this.iTfsProvider.GetBuildDetails(new Uri(buildUri));
        }

        /// <summary>
        /// Gets the details of a build
        /// </summary>
        /// <param name="buildUri">The build url</param>
        /// <param name="keepForever">True of should keep build forever</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "We are doing the conversion to ease life in the dsl")]
        public void SetBuildRetension(string buildUri, bool keepForever)
        {
            this.iTfsProvider.SetBuildRetension(new Uri(buildUri), keepForever);
        }

        /// <summary>
        /// Gets a changeset
        /// </summary>
        /// <param name="id">The changeset id</param>
        /// <returns>The changeset</returns>
        public Changeset GetChangeset(int id)
        {
            return this.iTfsProvider.GetChangeSet(id);
        }

        /// <summary>
        /// Creates a work item
        /// </summary>
        /// <param name="tp">The team project to create wi in</param>
        /// <param name="witName">The work item type</param>
        /// <param name="fields">The fields to set</param>
        /// <returns>The created work item</returns>
        public WorkItem CreateWorkItem(string tp, string witName, IDictionary<string, object> fields)
        {
            return this.iTfsProvider.CreateWorkItem(tp, witName, new Dictionary<string, object>(fields));
        }

        /// <summary>
        /// Updates a work item
        /// </summary>
        /// <param name="wi">The work item to save</param>
        public void UpdateWorkItem(WorkItem wi)
        {
            this.iTfsProvider.UpdateWorkItem(wi);
        }

        /// <summary>
        /// Send an email 
        /// </summary>
        /// <param name="to">To address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        public void SendEmail(string to, string subject, string body)
        {
            this.iEmailProvider.SendEmailAlert(to, subject, body);
        }

        /// <summary>
        /// Sends an email based on a template
        /// </summary>
        /// <param name="workItemId">The work item ID</param>
        /// <param name="templatePath">Path to the email template</param>
        /// <param name="dumpAllWorkItemFields">If true appends all work item fields to the email</param>
        /// <param name="dumpAllAlertFields">If true appends all alert fields to the email</param>
        /// <param name="showMissingFieldNames">If true adds error messages for incorrect field names</param>
        public void SendEmail(int workItemId, string templatePath, bool dumpAllWorkItemFields, bool dumpAllAlertFields, bool showMissingFieldNames)
        {

            // Get this list of changes
            var alertItems = EventXmlHelper.GetWorkItemChangedAlertFields(this.eventXml);
            var changedBy = EventXmlHelper.GetChangedBy(this.eventXml);

            // Create a new Tfs helper
            var fieldLookupProvider = new TfsFieldLookupProvider(
                this.iTfsProvider.GetWorkItem(workItemId),
                alertItems,
                changedBy,
                showMissingFieldNames);

            // Process the email using a template
            this.iEmailProvider.SendEmailAlert(
                fieldLookupProvider,
                templatePath,
                dumpAllWorkItemFields,
                dumpAllAlertFields);
        }

        /// <summary>
        /// The folder the script engine is using to load scripts
        /// </summary>
        /// <returns>A folder path</returns>
        public string CurrentScriptFolder()
        {
            return this.scriptFolder;
        }

        /// <summary>
        /// Gets argument field in a build process definition.
        /// </summary>
        /// <param name="buildUri">The URI of the build instance</param>
        /// <param name="argumentName">The field to return</param>
        /// <returns>The argument value</returns>
        public object GetBuildArgument(string buildUri, string argumentName)
        {
            var build = this.GetBuildDetails(buildUri);
            return this.iTfsProvider.GetBuildArgument(build.BuildDefinitionUri, argumentName);
        }


        /// <summary>
        /// Gets version number a build process definition.
        /// </summary>
        /// <param name="buildUri">The URI of the build instance</param>
        /// <returns>The version value</returns>
        public string GetVersionNumber(string buildUri)
        {
            var build = this.GetBuildDetails(buildUri);
            return string.Format(
                "{0}.{1}.[days since {2}].[build count]",
                this.iTfsProvider.GetBuildArgument(build.BuildDefinitionUri, "MajorVersion"),
                this.iTfsProvider.GetBuildArgument(build.BuildDefinitionUri, "MinorVersion"),
                this.iTfsProvider.GetBuildArgument(build.BuildDefinitionUri, "VersionStartDate").ToString());
        }

        /// <summary>
        /// Sets argument field in a build process definition.
        /// </summary>
        /// <param name="buildUri">The URI of the build instance</param>
        /// <param name="argumentName">The field to return</param>
        /// <param name="value">The value to set</param>
        public void SetBuildArgument(string buildUri, string argumentName, object value)
        {
            var build = this.GetBuildDetails(buildUri);
            this.iTfsProvider.SetBuildArgument(build.BuildDefinitionUri, argumentName, value);
        }

        /// <summary>
        /// Increment a string argument field in a build process definition.
        /// This can be used to raise a version number when some event occurs
        /// </summary>
        /// <param name="buildUri">The URI of the build instance</param>
        public void IncrementBuildNumber(string buildUri)
       {
           var build = this.GetBuildDetails(buildUri);

           var argumentName = "MinorVersion";
           var value = this.TfsProvider.GetBuildArgument(build.BuildDefinitionUri, argumentName);

           if (value == null)
           {
               LogInfoMessage(string.Format("Argument {0} is not set. This could be because it is not used in this build template, or because it only has a default value not an explicitaly set one", argumentName));
           }
           else
           {
               LogInfoMessage(string.Format("Argument {0} old value is {1}", argumentName, value.ToString()));

               // as we are incrementing we need to treat this an Int
               // the most likey field to be updating is on for the TFSVersion activity which
               // stores the values as strings, so we need to parse it
               var number = -1;
               if (value != null)
               {
                   int.TryParse(value.ToString(), out number);
               }
               number++;

               LogInfoMessage(string.Format("Updated argument {0} value is {1}", argumentName, number));

               this.TfsProvider.SetBuildArgument(build.BuildDefinitionUri, argumentName, number.ToString(CultureInfo.InvariantCulture));

               // we also use the start date so this should be reset start date
               this.TfsProvider.SetBuildArgument(build.BuildDefinitionUri, "VersionStartDate", DateTime.Today);
           }
       }
    }
}
