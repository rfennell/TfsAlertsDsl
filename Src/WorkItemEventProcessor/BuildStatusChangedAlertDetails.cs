//------------------------------------------------------------------------------------------------- 
// <copyright file="BuildStatusChangedAlertDetails.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFSEventsProcessor
{
    /// <summary>
    /// Container for the alert information stored in XML
    /// </summary>
    public class BuildStatusChangedAlertDetails
    {
        /// <summary>
        /// The build Uri
        /// </summary>
        public Uri BuildUri { get; set; }

        /// <summary>
        /// The text that describes the event as a whole
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The new build quality
        /// </summary>
        public string NewQuality { get; set; }
    }

}