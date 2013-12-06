//------------------------------------------------------------------------------------------------- 
// <copyright file="ITfsProvider.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFSEventsProcessor.Dsl;

namespace TFSEventsProcessor.Providers
{
    /// <summary>
    /// TFS service provider
    /// </summary>
    public interface ITfsProvider
    {
           /// <summary>
        /// Creates an instance of the class used to communicate with TFS
        /// </summary>
        /// <param name="tfsIdentityXml">The TFS security details</param>
        void UnpackIdentity(string tfsIdentityXml);

             /// <summary>
        /// Updates the build retension for a build
        /// </summary>
        /// <param name="buildUri">The path to the build</param>
        /// <param name="keepForever">True if the build should be kept</param>
        void SetBuildRetension(Uri buildUri, bool keepForever);

        /// <summary>
        /// Returns the work item with the specified id
        /// </summary>
        /// <param name="id">The work item ID</param>
        /// <returns>The work item</returns>
        WorkItem GetWorkItem(int id);

        /// <summary>
        ///Creates a work item 
        /// </summary>
        /// <param name="teamproject">The team project to create the work item in</param>
        /// <param name="witName">The work item type to create</param>
        /// <param name="fields">The properties to set</param>
        /// <returns>The work item</returns>
        WorkItem CreateWorkItem(string teamproject, string witName, Dictionary<string, object> fields);

        /// <summary>
        /// Gets the details of a given build
        /// </summary>
        /// <param name="uri">The URI of the build</param>
        /// <returns>The build details</returns>
        IBuildDetail GetBuildDetails(Uri uri);

        /// <summary>
        /// Get the details of a given changeset
        /// </summary>
        /// <param name="id">Changeset ID</param>
        /// <returns>The changeset</returns>
        Changeset GetChangeSet(int id);

        /// <summary>
        /// Get a parameter argument from a TFS build
        /// </summary>
        /// <param name="buildDefUri">The Uri of the build definition</param>
        /// <param name="key">The arguement name</param>
        /// <returns>The value of the argument</returns>
        object GetBuildArgument(Uri buildDefUri, string key);

        /// <summary>
        /// Set a parameter argument from a TFS build
        /// </summary>
        /// <param name="buildDefUri">The Uri of the build definition</param>
        /// <param name="key">The arguement name</param>
        /// <param name="value">The value to set</param>
        void SetBuildArgument(Uri buildDefUri, string key, object value);

        /// <summary>
        /// Returns all the build results for a team project
        /// </summary>
        /// <param name="teamProjectName">The team project name</param>
        /// <returns>The build results</returns>
        IBuildDetail[] GetAllBuilds(string teamProjectName);

        /// <summary>
        /// Get all parameter argument from a TFS build
        /// </summary>
        /// <param name="buildDefUri">The Uri of the build definition</param>
        /// <returns>The value of the argument</returns>
        IDictionary<string, object> GetAllBuildArguments(Uri buildDefUri);

        /// <summary>
        /// Updates a work work item
        /// </summary>
        /// <param name="wi">The work item</param>
        void UpdateWorkItem(WorkItem wi);
    }
}
