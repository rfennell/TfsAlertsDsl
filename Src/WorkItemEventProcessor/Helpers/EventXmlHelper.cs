//------------------------------------------------------------------------------------------------- 
// <copyright file="EventXmlHelper.cs" company="Black Marble">
// Copyright (c) Black Marble. All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TFSEventsProcessor.Helpers
{
    /// <summary>
    /// Class to manage the contents of the TFS Alert message
    /// </summary>
    public sealed class EventXmlHelper
    {
        /// <summary>
        /// Intentionally empty as static class with no constructor
        /// </summary>
        private EventXmlHelper()
        {

        }

        /// <summary>
        /// Possible field sections
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Do not want to alter reference sample")]
        public enum FieldSection
        {
            CoreFields,
            ChangedFields
        }

        /// <summary>
        /// Possible field types
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Do not want to alter reference sample")]
        public enum FieldType
        {
            IntegerField,
            StringField
        }

        /// <summary>
        /// Possible value types
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Do not want to alter reference sample")]
        public enum ValueType
        {
            NewValue,
            OldValue
        }

        /// <summary>
        /// Adds all the change data from the alert xml
        /// </summary>
        /// <param name="eventXml">The xml data</param>
        /// <returns>List of changes</returns>
        public static List<WorkItemChangedAlertDetails> GetWorkItemChangedAlertFields(string eventXml)
        {
            var returnValue = new List<WorkItemChangedAlertDetails>();

            returnValue.AddRange(GetAlertFields(eventXml, EventXmlHelper.FieldSection.ChangedFields, EventXmlHelper.FieldType.StringField));
            returnValue.AddRange(GetAlertFields(eventXml, EventXmlHelper.FieldSection.ChangedFields, EventXmlHelper.FieldType.IntegerField));

            return returnValue;

        }

        /// <summary>
        /// Adds all the change data from the alert xml
        /// </summary>
        /// <param name="eventXml">The xml data</param>
        /// <returns>List of changes</returns>
        public static BuildStatusChangedAlertDetails GetBuildStatusChangedAlertFields(string eventXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(eventXml);

            var node = doc.SelectSingleNode("/BuildStatusChangeEvent");

            var returnValue = new BuildStatusChangedAlertDetails()
            {
                BuildUri = new Uri(node.SelectSingleNode("BuildUri").InnerText),
                Summary = node.SelectSingleNode("Title").InnerText,
            };

            foreach (XmlNode innernode in doc.SelectNodes("/BuildStatusChangeEvent/StatusChange"))
            {
                if (innernode.SelectSingleNode("FieldName").InnerText.Equals("Quality"))
                {
                    returnValue.NewQuality = innernode.SelectSingleNode("NewValue").InnerText;
                    break;
                }
            }

            return returnValue;

        }

        /// <summary>
        /// Gets who changed the wi and caused the alert
        /// </summary>
        /// <param name="eventXml">The xml data</param>
        /// <returns>The user ID</returns>
        public static string GetChangedBy(string eventXml)
        {
            var allCoreFields = GetAlertFields(eventXml, EventXmlHelper.FieldSection.CoreFields, EventXmlHelper.FieldType.StringField);
            var changedByField = allCoreFields.SingleOrDefault(f => f.ReferenceName.Equals("System.ChangedBy"));
            return changedByField.NewValue;
        }

        /// <summary>
        /// Helper function to get a block from the xml
        /// </summary>
        /// <param name="eventXml">Example data</param>
        /// <param name="section">Section to read</param>
        /// <param name="type">Sub type to read</param>
        /// <returns>List of actions in this section</returns>
        private static List<WorkItemChangedAlertDetails> GetAlertFields(string eventXml, EventXmlHelper.FieldSection section, EventXmlHelper.FieldType type)
        {
            var returnValue = new List<WorkItemChangedAlertDetails>();

            var path = string.Format("/WorkItemChangedEvent/{0}/{1}s/Field", section, type);

            var doc = new XmlDocument();
            doc.LoadXml(eventXml);

            foreach (XmlNode node in doc.SelectNodes(path))
            {
                returnValue.Add(new WorkItemChangedAlertDetails()
                {
                    ReferenceName = node.SelectSingleNode("ReferenceName").InnerText,
                    NewValue = RemoveFieldNulls(node.SelectSingleNode("NewValue")),
                    OldValue = RemoveFieldNulls(node.SelectSingleNode("OldValue"))
                });
            }

            return returnValue;
        }


        /// <summary>
        /// Safety method to make sure no nulls as passed to the email client
        /// </summary>
        /// <param name="fieldValue">The value to add to a message</param>
        /// <returns>A safe string</returns>
        private static string RemoveFieldNulls(XmlNode fieldValue)
        {
            try
            {
                return fieldValue.InnerText;
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the value of a field
        /// </summary>
        /// <typeparam name="T">Type of field</typeparam>
        /// <param name="eventXml">The source data</param>
        /// <param name="section">The section to search</param>
        /// <param name="type">The type of field</param>
        /// <param name="valueType">The return type of the value</param>
        /// <param name="refName">The unique field name</param>
        /// <returns>The resultant value</returns>
        public static T GetWorkItemValue<T>(string eventXml, FieldSection section, FieldType type, ValueType valueType, string refName)
        {
            var path = string.Format("/WorkItemChangedEvent/{0}/{1}s/Field[ReferenceName='{3}']/{2}", section, type, valueType, refName);

            var doc = new XmlDocument();
            doc.LoadXml(eventXml);

            var node = doc.SelectSingleNode(path);

            object text;
            if (node == null)
            {
                if (typeof(T) == typeof(int))
                {
                    text = 0;
                }
                else if (typeof(T) == typeof(string))
                {
                    text = string.Empty;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                text = node.InnerText;
            }

            return (T)Convert.ChangeType(text, typeof(T));
        }

        /// <summary>
        /// Converts the alerts XML to a object
        /// </summary>
        /// <param name="alertMessage">The xml message</param>
        /// <returns>The checkin details</returns>
        internal static CheckInAlertDetails GetCheckInDetails(string alertMessage)
        {
            var doc = new XmlDocument();
            doc.LoadXml(alertMessage);

            var node = doc.SelectSingleNode("/CheckinEvent");

            var returnValue = new CheckInAlertDetails()
            {
                Summary = node.SelectSingleNode("Title").InnerText,
                Committer = node.SelectSingleNode("Committer").InnerText,
                TeamProject = node.SelectSingleNode("TeamProject").InnerText,
                Comment = node.SelectSingleNode("Comment").InnerText,
            };

            foreach (XmlNode innernode in doc.SelectNodes("/CheckinEvent/Artifacts/Artifact[@ArtifactType='VersionedItem']"))
            {
                returnValue.Changeset = int.Parse(innernode.Attributes["ItemRevision"].InnerText);
                    
                switch (innernode.Attributes["ChangeType"].InnerText)
                {
                    case "edit":
                        returnValue.FilesEdited.Add(innernode.Attributes["Item"].InnerText);
                        break;
                    case "add":
                        returnValue.FilesAdded.Add(innernode.Attributes["Item"].InnerText);
                        break;
                    case "delete":
                        returnValue.FilesDeleted.Add(innernode.Attributes["Item"].InnerText);
                        break;
                }

            }

          return returnValue;
        }
    }
}