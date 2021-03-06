﻿//------------------------------------------------------------------------------------------------- 
// <copyright file="DslProcessor.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace TFSEventsProcessor.Dsl
{
    using System.Collections;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    /// <summary>
    /// Contains the DSL API
    /// </summary>
    public class DslProcessor
    {
        /// <summary>
        /// Instance of nLog interface
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The variable to bind the MEF loaded DSL object
        /// </summary>
        [ImportMany(typeof(IDslLibrary))]
        private IEnumerable<IDslLibrary> dslLibraries = null;

        /// <summary>
        /// Runs a named Pyphon script that uses the DSL
        /// </summary>
        /// <param name="scriptname">The python script file</param>
        /// <param name="iTfsProvider">The TFS provider</param>
        /// <param name="iEmailProvider">The email provider</param>
        public void RunScript(string scriptname, Providers.ITfsProvider iTfsProvider, Providers.IEmailProvider iEmailProvider)
        {
            this.RunScript(@".\dsl", ".", scriptname, null, iTfsProvider, iEmailProvider, string.Empty);
        }

        /// <summary>
        /// Runs a named Pyphon script that uses the DSL
        /// </summary>
        /// <param name="scriptname">The python script file</param>
        /// <param name="args">The parameters to pass to the script</param>
        /// <param name="iTfsProvider">The TFS provider</param>
        /// <param name="iEmailProvider">The email provider</param>
        public void RunScript(string scriptname, Dictionary<string, object> args, Providers.ITfsProvider iTfsProvider, Providers.IEmailProvider iEmailProvider)
        {
            this.RunScript(@".\dsl", ".", scriptname, args, iTfsProvider, iEmailProvider, string.Empty);
        }

        /// <summary>
        /// Runs a named Pyphon script that uses the DSL
        /// </summary>
        /// <param name="dslFolder">The folder to scan for MEF DSL files instead of the current folder</param>
        /// <param name="scriptFolder">The base folder to load scripts file</param>
        /// <param name="scriptname">The python script file, this can be a full path or a file in the base folder</param>
        /// <param name="args">The parameters to pass to the script</param>
        /// <param name="iTfsProvider">The TFS provider</param>
        /// <param name="iEmailProvider">The email provider</param>
        /// <param name="eventXml">The raw XML data from the event</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Allowing complexity as this method pulling in the whole DSL")]
        public void RunScript(string dslFolder, string scriptFolder, string scriptname, Dictionary<string, object> args, Providers.ITfsProvider iTfsProvider, Providers.IEmailProvider iEmailProvider, string eventXml)
        {
            if (scriptname == null)
            {
                throw new ArgumentNullException("scriptname");
            }
            else
            {
                if (string.IsNullOrEmpty(scriptFolder))
                {
                    scriptFolder = ".";
                }

                if (File.Exists(scriptname) == false)
                {
                    this.logger.Info(string.Format("TFSEventsProcessor: DslProcessor cannot find script:{0}", scriptname));

                    // we have not been given a resolvable file name, so try appending the base path
                    scriptname = Path.Combine(scriptFolder, scriptname);
                    if (File.Exists(scriptname) == false)
                    {
                        this.logger.Error(string.Format("TFSEventsProcessor: DslProcessor cannot find script:{0}", scriptname));
                        throw new FileNotFoundException(string.Format("The script file '{0}' could not be found", scriptname));
                    }
                }
            }

            if (eventXml == null)
            {
                throw new ArgumentNullException("eventXml");
            }

            if (iTfsProvider == null)
            {
                throw new ArgumentNullException("iTfsProvider");
            }

            if (iEmailProvider == null)
            {
                throw new ArgumentNullException("iEmailProvider");
            }

            if (string.IsNullOrEmpty(dslFolder))
            {
                dslFolder = ".";
            }

            // load the DSL wrapper class via MEF
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            if (Directory.Exists(dslFolder))
            {
                this.logger.Info(
                    string.Format("TFSEventsProcessor: DslProcessor loading DSL from {0}", Path.GetFullPath(dslFolder)));
                catalog.Catalogs.Add(new DirectoryCatalog(dslFolder));
            }
            else
            {
                this.logger.Error(
                    string.Format("TFSEventsProcessor: DslProcessor cannot find DSL folder {0}", Path.GetFullPath(dslFolder)));
                return;
            }

            //Create the CompositionContainer with the parts in the catalog
            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                this.logger.Error(compositionException.ToString());
                return;
            }

            if (this.dslLibraries.Any())
            {
                // create the engine
                var engine = IronPython.Hosting.Python.CreateEngine(args);
                var objOps = engine.Operations;
                var scope = engine.CreateScope();

                // inject the providers
                foreach (var item in this.dslLibraries)
                {
                    item.EmailProvider = iEmailProvider;
                    item.TfsProvider = iTfsProvider;
                    item.EventXml = eventXml;
                    item.ScriptFolder = scriptFolder;

                    // Read in the methods
                    foreach (string memberName in objOps.GetMemberNames(item))
                    {
                        scope.SetVariable(memberName, objOps.GetMember(item, memberName));
                    }
                }

                // run the script
                this.logger.Info(string.Format(
                      "TFSEventsProcessor: DslProcessor running script:{0}",
                      scriptname));
                var script = engine.CreateScriptSourceFromFile(scriptname);
                script.Execute(scope);
            }
            else
            {
                this.logger.Error(
                    string.Format("TFSEventsProcessor: DslProcessor cannot find DSL libraries in folder {0}", Path.GetFullPath(dslFolder)));
                return;
            }

        }
    }
}
