//------------------------------------------------------------------------------------------------- 
// <copyright file="IDslScriptService.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TFSEventsProcessor
{
    /// <summary>
    /// The interface that defines the WCF service
    /// </summary>
    [ServiceContract(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03")]
    public interface IDslScriptService
    {
        /// <summary>
        /// The main method that catches the alert
        /// </summary>
        /// <param name="eventXml">The XML block of the work item changes</param>
        /// <param name="tfsIdentityXml">The TFS security details</param>
        [OperationContract(Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03/Notify")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document)]
        void Notify(string eventXml, string tfsIdentityXml);
    }

}
