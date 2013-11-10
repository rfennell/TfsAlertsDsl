﻿//------------------------------------------------------------------------------------------------- 
// <copyright file="TfsProvider.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.IO;
using Microsoft.TeamFoundation.Build.Client;
using NLog;
using TFSEventsProcessor.Dsl;

namespace TFSEventsProcessor.Providers
{
    /// <summary>
    /// Class to manage the connection to TFS
    /// </summary>
    public class TfsProvider : ITfsProvider
    {
        /// <summary>
        /// Instance of nLog interface
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The current TFS instance
        /// </summary>
        public TfsTeamProjectCollection TfsInstance { get; set; }

        /// <summary>
        /// Creates an instance of the class used to communicate with TFS
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Cannot build if use a more limited exception")]
        public TfsProvider()
        {
        }

        /// <summary>
        /// Creates an instance of the class used to communicate with TFS
        /// </summary>
        /// <param name="tfsIdentityXml">The TFS security details</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes",Justification="Want to avoid lost ecaptions")]
        public void UnpackIdentity(string tfsIdentityXml)
        {
            //Get the url from the tfsIdentity xml. 
            var doc = new XmlDocument();
            doc.LoadXml(tfsIdentityXml);

            if (doc.FirstChild == null)
            {
                throw new Exception("url not found");
            }
            var url = doc.FirstChild.Attributes["url"].Value;

            //the url is in the form of http://localhost:8080/tfs/DefaultCollection/Services/v3.0/LocationService.asmx
            //strip the /Services/v3.0/LocationService.asmx part
            url = url.Substring(0, url.Length - ("/Services/v3.0/LocationService.asmx").Length);

            // Instantiate a reference to the TFS Project Collection
            this.TfsInstance = new TfsTeamProjectCollection(new Uri(url));
        }

        /// <summary>
        /// Updates the build retension for a build
        /// </summary>
        /// <param name="buildUri">The path to the build</param>
        /// <param name="keepForever">True if the build should be kept</param>
        public void SetBuildRetension(Uri buildUri, bool keepForever)
        {
            logger.Info(string.Format("TfsHelper: Setting the build retension of [{0}]  to [{1}]", buildUri, keepForever));
            var service = (IBuildServer)this.TfsInstance.GetService(typeof(IBuildServer));
            var buildDetail = service.GetBuild(buildUri);
            buildDetail.KeepForever = keepForever;
            service.SaveBuilds(new IBuildDetail[] { buildDetail });
        }

        /// <summary>
        /// Returns the work item with the specified id
        /// </summary>
        /// <param name="id">The work item ID</param>
        /// <returns>The work item</returns>
        public WorkItem GetWorkItem(int id)
        {
            var store = (WorkItemStore)this.TfsInstance.GetService(typeof(WorkItemStore));
            return store.GetWorkItem(id);
        }

        /// <summary>
        /// Returns the parent work item of the work item with the specified id. When it has no parent, null is returned.
        /// </summary>
        /// <param name="id">The work item ID</param>
        /// <returns>The work item</returns>
        public WorkItem OpenParentWorkItem(int id)
        {
            // Get the work item with the specified id
            var workItem = this.GetWorkItem(id);

            // Get the link to the parent work item through the work item links
            var q = from l in workItem.WorkItemLinks.OfType<WorkItemLink>()
                    where l.LinkTypeEnd.LinkType.LinkTopology == WorkItemLinkType.Topology.Tree
                    && !l.LinkTypeEnd.IsForwardLink
                    select l.TargetId;

            // If there is a link with a parent work item
            if (q.Count() > 0)
            {
                // Return that one
                return this.GetWorkItem(q.ElementAt(0));
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        ///Creates a work item 
        /// </summary>
        /// <param name="teamproject">The team project to create the work item in</param>
        /// <param name="witName">The work item type to create</param>
        /// <param name="fields">The properties to set</param>
        /// <returns>The work item</returns>
        public WorkItem CreateWorkItem(string teamproject, string witName, Dictionary<string, object> fields)
        {
            if (string.IsNullOrEmpty(teamproject))
            {
                throw new ArgumentNullException("teamproject");
            }

            if (string.IsNullOrEmpty(witName))
            {
                throw new ArgumentNullException("witName");
            }

            if (fields==null)
            {
                throw new ArgumentNullException("fields");
            }

            var store = (WorkItemStore)this.TfsInstance.GetService(typeof(WorkItemStore));
            var wit = store.Projects[teamproject].WorkItemTypes[witName];
            var wi = new WorkItem(wit);
            foreach( var pair in fields)
            {
                wi.Fields[pair.Key].Value = pair.Value;
            }
            wi.Save();
            return wi;
        }

        /// <summary>
        /// Gets the details of a given build
        /// </summary>
        /// <param name="buildUri">The URI of the build</param>
        /// <returns>The build details</returns>
        public IBuildDetail GetBuildDetails(Uri buildUri)
        {
            logger.Info(string.Format("TfsHelper: Getting the build [{0}]", buildUri));
            var service = (IBuildServer)this.TfsInstance.GetService(typeof(IBuildServer));
            return service.GetBuild(buildUri);
    
        }

        /// <summary>
        /// Get the details of a given changeset
        /// </summary>
        /// <param name="id">Changeset ID</param>
        /// <returns>The changeset</returns>
        public Changeset GetChangeSet(int id)
        {
            logger.Info(string.Format("TfsHelper: Getting the changeset [{0}]", id));
  
            var versionControl = (VersionControlServer)this.TfsInstance.GetService(typeof(VersionControlServer));
            return versionControl.GetChangeset(id);
            
        }

       
    }
}