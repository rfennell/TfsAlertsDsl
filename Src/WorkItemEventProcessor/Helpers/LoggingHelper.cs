//------------------------------------------------------------------------------------------------- 
// <copyright file="LoggingHelper.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.IO;

namespace TFSEventsProcessor.Helpers
{
    /// <summary>
    /// Helper methods to aid logging
    /// </summary>
    public static class LoggingHelper
    {
        /// <summary>
        /// Dumps the event details as an XML for debug using a GUID for the file name
        /// </summary>
        /// <param name="eventXml">The details of the event</param>
        /// <param name="path">Base folder to dump file to</param>
        public static void DumpEventToDisk(string eventXml, string path)
        {
            File.WriteAllText(Path.Combine(path, string.Format("{0}.xml", Guid.NewGuid().ToString())), eventXml);
        }
    }
}