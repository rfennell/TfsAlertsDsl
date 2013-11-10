using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using TFSEventsProcessor.Helpers;

namespace TFSEventsProcessor.Tests.Helpers
{
    class TestData
    {
       
        /// <summary>
        /// A simple template for test
        /// </summary>
        /// <returns>The template</returns>
        internal static string DummyTemplate()
        {
            return @"<subject>The Work Item @@System.ID@@ has been edited</subject>
              <body>
              The title is @@System.Title@@ &lt;u&gt;for&lt;/u&gt; the @@System.ID@@ by ##System.ChangedBy##
              </body>
              <wifieldheader>&lt;br /&gt;&lt;strong&gt;&lt;u&gt;All wi fields in the alert&lt;/strong&gt;&lt;/u&gt;</wifieldheader>
              <alertfieldheader>&lt;br /&gt;&lt;strong&gt;&lt;u&gt;All changed fields in the alert&lt;/strong&gt;&lt;/u&gt;</alertfieldheader>";
        }

        /// <summary>
        /// The XMl we get from the TFS server call
        /// </summary>
        /// <returns></returns>
        internal static string DummyWorkItemChangedAlertXml()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><WorkItemChangedEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><PortfolioProject>TechForge</PortfolioProject><ProjectNodeId>1bfb65fe-5039-44ec-a6a4-5251ddb7c788</ProjectNodeId><AreaPath>\TechForge</AreaPath><Title>TechForge Work Item Changed: User Story 416 - Tests user story</Title><WorkItemTitle>Tests user story</WorkItemTitle><Subscriber>TFS2010\Administrator</Subscriber><ChangerSid>S-1-5-21-2464940929-4154044706-4000345470-500</ChangerSid><DisplayUrl>http://tfs2010:8080/tfs/web/wi.aspx?pcguid=331c9c6a-5332-4f4d-9f13-8f35e212035f&amp;id=416</DisplayUrl><TimeZone>GMT Daylight Time</TimeZone><TimeZoneOffset>+01:00:00</TimeZoneOffset><ChangeType>Change</ChangeType><CoreFields><IntegerFields><Field><Name>ID</Name><ReferenceName>System.Id</ReferenceName><OldValue>416</OldValue><NewValue>416</NewValue></Field><Field><Name>Rev</Name><ReferenceName>System.Rev</ReferenceName><OldValue>44</OldValue><NewValue>45</NewValue></Field><Field><Name>AreaID</Name><ReferenceName>System.AreaId</ReferenceName><OldValue>105</OldValue><NewValue>105</NewValue></Field></IntegerFields><StringFields><Field><Name>Work Item Type</Name><ReferenceName>System.WorkItemType</ReferenceName><OldValue>User Story</OldValue><NewValue>User Story</NewValue></Field><Field><Name>Title</Name><ReferenceName>System.Title</ReferenceName><OldValue>Tests user story</OldValue><NewValue>Tests user story</NewValue></Field><Field><Name>Area Path</Name><ReferenceName>System.AreaPath</ReferenceName><OldValue>\TechForge</OldValue><NewValue>\TechForge</NewValue></Field><Field><Name>State</Name><ReferenceName>System.State</ReferenceName><OldValue>Active</OldValue><NewValue>Active</NewValue></Field><Field><Name>Reason</Name><ReferenceName>System.Reason</ReferenceName><OldValue>New</OldValue><NewValue>New</NewValue></Field><Field><Name>Assigned To</Name><ReferenceName>System.AssignedTo</ReferenceName><OldValue>richard</OldValue><NewValue>richard</NewValue></Field><Field><Name>Changed By</Name><ReferenceName>System.ChangedBy</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Created By</Name><ReferenceName>System.CreatedBy</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Changed Date</Name><ReferenceName>System.ChangedDate</ReferenceName><OldValue>10/10/2012 11:38:58</OldValue><NewValue>11/10/2012 16:29:24</NewValue></Field><Field><Name>Created Date</Name><ReferenceName>System.CreatedDate</ReferenceName><OldValue>08/10/2012 17:34:52</OldValue><NewValue>08/10/2012 17:34:52</NewValue></Field><Field><Name>Authorized As</Name><ReferenceName>System.AuthorizedAs</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Iteration Path</Name><ReferenceName>System.IterationPath</ReferenceName><OldValue>\TechForge</OldValue><NewValue>\TechForge</NewValue></Field></StringFields></CoreFields><TextFields><TextField><Name>Description</Name><ReferenceName>System.Description</ReferenceName><Value>As a &lt;type of user&gt; I want &lt;some goal&gt; so that &lt;some reason&gt;a1, b1, c1, d1, e1, f1, g1, h1, i1, j1, k1, l1; m1, n1, o1,p1, q1, r1, s1, t1, s1, u1, v1 w1, x1</Value></TextField></TextFields><ChangedFields><IntegerFields /><StringFields><Field><Name>Changed By</Name><ReferenceName>System.ChangedBy</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Stack Rank</Name><ReferenceName>Microsoft.VSTS.Common.StackRank</ReferenceName><OldValue /><NewValue>1</NewValue></Field><Field><Name>Story Points</Name><ReferenceName>Microsoft.VSTS.Scheduling.StoryPoints</ReferenceName><OldValue /><NewValue>2</NewValue></Field><Field><Name>Risk</Name><ReferenceName>Microsoft.VSTS.Common.Risk</ReferenceName><OldValue /><NewValue>1 - High</NewValue></Field></StringFields></ChangedFields></WorkItemChangedEvent>";
        }

        /// <summary>
        /// The Xml we hey from the TFS server call
        /// </summary>
        /// <returns></returns>
        internal static string DummyCheckInAlertXml()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><CheckinEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><AllChangesIncluded>true</AllChangesIncluded><Subscriber>Richard</Subscriber><CheckinNotes /><CheckinInformation><CheckinInformation xsi:type=""CheckinWorkItemInfo"" Url=""http://typhoontfs:8080/tfs/web/wi.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;id=145"" Id=""145"" CheckinAction=""Associate"" Title=""Do the design"" Type=""Task"" State=""Done"" AssignedTo=""Ann"" /></CheckinInformation><Artifacts><Artifact xsi:type=""ClientArtifact"" ArtifactType=""Changeset"" ServerItem=""""><Url>http://typhoontfs:8080/tfs/web/cs.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;cs=62</Url></Artifact><Artifact xsi:type=""ClientArtifact"" ArtifactType=""VersionedItem"" Item=""Class1.cs"" Folder=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1"" TeamProject=""Scrum (TFVC)"" ItemRevision=""62"" ChangeType=""delete"" ServerItem=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1/Class1.cs""><Url>http://typhoontfs:8080/tfs/web/view.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;path=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fClass1.cs&amp;cs=62</Url></Artifact><Artifact xsi:type=""ClientArtifact"" ArtifactType=""VersionedItem"" Item=""ClassLibrary1.csproj"" Folder=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1"" TeamProject=""Scrum (TFVC)"" ItemRevision=""62"" ChangeType=""edit"" ServerItem=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1/ClassLibrary1.csproj""><Url>http://typhoontfs:8080/tfs/web/diff.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;opath=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fClassLibrary1.csproj&amp;ocs=61&amp;mpath=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fClassLibrary1.csproj&amp;mcs=62</Url></Artifact><Artifact xsi:type=""ClientArtifact"" ArtifactType=""VersionedItem"" Item=""Newcalls.cs"" Folder=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1"" TeamProject=""Scrum (TFVC)"" ItemRevision=""62"" ChangeType=""edit"" ServerItem=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1/Newcalls.cs""><Url>http://typhoontfs:8080/tfs/web/diff.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;opath=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fNewcalls.cs&amp;ocs=61&amp;mpath=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fNewcalls.cs&amp;mcs=62</Url></Artifact><Artifact xsi:type=""ClientArtifact"" ArtifactType=""VersionedItem"" Item=""NextClass.cs"" Folder=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1"" TeamProject=""Scrum (TFVC)"" ItemRevision=""62"" ChangeType=""add"" ServerItem=""$/Scrum (TFVC)/ClassLibrary1/ClassLibrary1/NextClass.cs""><Url>http://typhoontfs:8080/tfs/web/view.aspx?pcguid=093dd039-5385-4e81-b6c4-aa1c3aeacd70&amp;path=%24%2fScrum+(TFVC)%2fClassLibrary1%2fClassLibrary1%2fNextClass.cs&amp;cs=62</Url></Artifact></Artifacts><Owner>TYPHOONTFS\Richard</Owner><OwnerDisplay>Richard</OwnerDisplay><CreationDate>26/08/2013 17:12:36</CreationDate><Comment>The comment</Comment><TimeZone>GMT Daylight Time</TimeZone><TimeZoneOffset>+01:00:00</TimeZoneOffset><TeamProject>Scrum (TFVC)</TeamProject><PolicyOverrideComment /><PolicyFailures /><Title>Scrum (TFVC) Changeset 62: The comment</Title><ContentTitle>Changeset 62: The comment</ContentTitle><Committer>TYPHOONTFS\Richard</Committer><CommitterDisplay>Richard</CommitterDisplay><Number>62</Number></CheckinEvent>";
        }
                
        /// <summary>
        /// The XMl we get from the TFS server call
        /// </summary>
        /// <returns></returns>
        internal static string DummyBuildStatusChangedAlertXmlWithQualityChange()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><BuildStatusChangeEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><BuildUri>vstfs:///Build/Build/49</BuildUri><TeamFoundationServerUrl>http://tfs2010:8080/tfs/DefaultCollection</TeamFoundationServerUrl><TeamProject>Helpdesk</TeamProject><Title>Helpdesk Build CallTracker Dev_20100317.2 Quality Changed To Initial Test Passed</Title><Subscriber>TFS2010\Administrator</Subscriber><Id>CallTracker Dev_20100317.2</Id><Url>http://tfs2010:8080/tfs/web/build.aspx?pcguid=331c9c6a-5332-4f4d-9f13-8f35e212035f&amp;builduri=vstfs:///Build/Build/49</Url><TimeZone>GMT Standard Time</TimeZone><TimeZoneOffset>00:00:00</TimeZoneOffset><ChangedTime>07/11/2012 09:50:45</ChangedTime><StatusChange><FieldName>Quality</FieldName><NewValue>Initial Test Passed</NewValue></StatusChange><ChangedBy>TFS2010\Administrator</ChangedBy></BuildStatusChangeEvent>";
        }

        /// <summary>
        /// The XMl we get from the TFS server call
        /// </summary>
        /// <returns></returns>
        internal static string DummyBuildStatusChangedAlertXmlWithRejectedQualityChange()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><BuildStatusChangeEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><BuildUri>vstfs:///Build/Build/49</BuildUri><TeamFoundationServerUrl>http://tfs2010:8080/tfs/DefaultCollection</TeamFoundationServerUrl><TeamProject>Helpdesk</TeamProject><Title>Helpdesk Build CallTracker Dev_20100317.2 Quality Changed To Test Failed</Title><Subscriber>TFS2010\Administrator</Subscriber><Id>CallTracker Dev_20100317.2</Id><Url>http://tfs2010:8080/tfs/web/build.aspx?pcguid=331c9c6a-5332-4f4d-9f13-8f35e212035f&amp;builduri=vstfs:///Build/Build/49</Url><TimeZone>GMT Standard Time</TimeZone><TimeZoneOffset>00:00:00</TimeZoneOffset><ChangedTime>07/11/2012 09:50:45</ChangedTime><StatusChange><FieldName>Quality</FieldName><NewValue>Test Failed</NewValue></StatusChange><ChangedBy>TFS2010\Administrator</ChangedBy></BuildStatusChangeEvent>";
        }

        /// <summary>
        /// The XMl we get from the TFS server call
        /// </summary>
        /// <returns></returns>
        internal static string DummyBuildStatusChangedAlertXmlWithoutQualityChange()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><BuildStatusChangeEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><BuildUri>vstfs:///Build/Build/49</BuildUri><TeamFoundationServerUrl>http://tfs2010:8080/tfs/DefaultCollection</TeamFoundationServerUrl><TeamProject>Helpdesk</TeamProject><Title>Helpdesk Build CallTracker Dev_20100317.2 Quality Changed To Initial Test Passed</Title><Subscriber>TFS2010\Administrator</Subscriber><Id>CallTracker Dev_20100317.2</Id><Url>http://tfs2010:8080/tfs/web/build.aspx?pcguid=331c9c6a-5332-4f4d-9f13-8f35e212035f&amp;builduri=vstfs:///Build/Build/49</Url><TimeZone>GMT Standard Time</TimeZone><TimeZoneOffset>00:00:00</TimeZoneOffset><ChangedTime>07/11/2012 09:50:45</ChangedTime><StatusChange><FieldName>NotQuality</FieldName><NewValue>Initial Test Passed</NewValue></StatusChange><ChangedBy>TFS2010\Administrator</ChangedBy></BuildStatusChangeEvent>";
        }

        /// <summary>
        /// The alerts as they are internally stoed
        /// </summary>
        /// <returns></returns>
        internal static List<WorkItemChangedAlertDetails> DummyAlerts()
        {
          return  new List<WorkItemChangedAlertDetails>() {
                new WorkItemChangedAlertDetails() { ReferenceName="r1", OldValue="A", NewValue = "B"},
                new WorkItemChangedAlertDetails() { ReferenceName="r2", OldValue="C", NewValue = "D"}};

        }

        /// <summary>
        /// The alerts as they are internally stoed
        /// </summary>
        /// <param name="from">From UID</param>
        /// <param name="to">To UI</param>
        /// <returns></returns>
        internal static List<WorkItemChangedAlertDetails> AssignedToChangedAlerts(string from, string to)
        {
            return new List<WorkItemChangedAlertDetails>() {
                new WorkItemChangedAlertDetails() { ReferenceName="System.AssignedTo", OldValue=from, NewValue = to}
            };
        }

        internal static string DummyAlertXmlWithNullFieldValue()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?><WorkItemChangedEvent xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><PortfolioProject>TechForge</PortfolioProject><ProjectNodeId>1bfb65fe-5039-44ec-a6a4-5251ddb7c788</ProjectNodeId><AreaPath>\TechForge</AreaPath><Title>TechForge Work Item Changed: User Story 416 - Shazzam212b</Title><WorkItemTitle>Shazzam212b</WorkItemTitle><Subscriber>TFS2010\Administrator</Subscriber><ChangerSid>S-1-5-21-2464940929-4154044706-4000345470-500</ChangerSid><DisplayUrl>http://tfs2010:8080/tfs/web/wi.aspx?pcguid=331c9c6a-5332-4f4d-9f13-8f35e212035f&amp;id=416</DisplayUrl><TimeZone>GMT Daylight Time</TimeZone><TimeZoneOffset>+01:00:00</TimeZoneOffset><ChangeType>Change</ChangeType><CoreFields><IntegerFields><Field><Name>ID</Name><ReferenceName>System.Id</ReferenceName><OldValue>416</OldValue><NewValue>416</NewValue></Field><Field><Name>Rev</Name><ReferenceName>System.Rev</ReferenceName><OldValue>115</OldValue><NewValue>116</NewValue></Field><Field><Name>AreaID</Name><ReferenceName>System.AreaId</ReferenceName><OldValue>105</OldValue><NewValue>105</NewValue></Field></IntegerFields><StringFields><Field><Name>Work Item Type</Name><ReferenceName>System.WorkItemType</ReferenceName><OldValue>User Story</OldValue><NewValue>User Story</NewValue></Field><Field><Name>Title</Name><ReferenceName>System.Title</ReferenceName><OldValue>Shazzam212b</OldValue><NewValue>Shazzam212b</NewValue></Field><Field><Name>Area Path</Name><ReferenceName>System.AreaPath</ReferenceName><OldValue>\TechForge</OldValue><NewValue>\TechForge</NewValue></Field><Field><Name>State</Name><ReferenceName>System.State</ReferenceName><OldValue>Active</OldValue><NewValue>Active</NewValue></Field><Field><Name>Reason</Name><ReferenceName>System.Reason</ReferenceName><OldValue>New</OldValue><NewValue>New</NewValue></Field><Field><Name>Assigned To</Name><ReferenceName>System.AssignedTo</ReferenceName><OldValue>riccardo</OldValue><NewValue>riccardo</NewValue></Field><Field><Name>Changed By</Name><ReferenceName>System.ChangedBy</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Created By</Name><ReferenceName>System.CreatedBy</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Changed Date</Name><ReferenceName>System.ChangedDate</ReferenceName><OldValue>16/10/2012 13:04:05</OldValue><NewValue>16/10/2012 13:04:58</NewValue></Field><Field><Name>Created Date</Name><ReferenceName>System.CreatedDate</ReferenceName><OldValue>08/10/2012 17:34:52</OldValue><NewValue>08/10/2012 17:34:52</NewValue></Field><Field><Name>Authorized As</Name><ReferenceName>System.AuthorizedAs</ReferenceName><OldValue>Administrator</OldValue><NewValue>Administrator</NewValue></Field><Field><Name>Iteration Path</Name><ReferenceName>System.IterationPath</ReferenceName><OldValue>\TechForge</OldValue><NewValue>\TechForge</NewValue></Field></StringFields></CoreFields><ChangedFields><IntegerFields /><StringFields><Field><Name>CustomField1</Name><ReferenceName>Bm.CustomField1</ReferenceName><OldValue>s1</OldValue></Field></StringFields></ChangedFields></WorkItemChangedEvent>";
        }
    }
}
