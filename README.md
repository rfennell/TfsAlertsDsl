###
This project has been migrated from CodePlex 
It is longer under development, it has been moved to GitHub as CodePlex is closing down. 
It has been replaced by [VSTSServiceHookDsl](https://github.com/rfennell/VSTSServiceHookDsl) which supports operations via Service Hooks for used with both TFS and VSTS ###


# Project Description #
Microsoft Team Foundation Server (TFS) provides an alerting model where given a certain condition, such as a&nbsp;check-in, work item edit or build completion,&nbsp;an email can be sent to an interest party or a call&nbsp;made to a SOAP based web service.
 Using this SOAP model it is possible to&nbsp;provide any bespoke operations you wish that are triggered by a&nbsp;change on the TFS server.

This framework is designed to ease the development of these bespoke SOAP wen services by providing helper methods for common XML processing steps and API operations such as calling back to the TFS server or accessing SMTP services.

They main differentiator of this project is that it also provides a Python based DSL that allows the actual operation performed when the endpoint is called to be edited without the need to&nbsp; to rebuild and redeploy the bespoke service.&nbsp;Operations
are defined by script such as show below

```
import sys
# Expect 2 args the event type and a value unique ID
LogInfoMessage( &quot;The following arguments were passed&quot; )
LogInfoMessage( sys.argv )<br>if sys.argv[0] == &quot;BuildEvent&quot; :
   LogInfoMessage (&quot;A build event &quot; &#43; sys.argv[1])
# a sample for using the DSL to create a work item is
   #fields = {&quot;Title&quot;: &quot;The Title&quot;,&quot;Effort&quot;: 2, &quot;Description&quot;: &quot;The desc of the bug&quot;}
   #teamproject = &quot;Scrum (TFVC)&quot;
   #wi = CreateWorkItem(teamproject ,&quot;bug&quot;,fields)
#LogInfoMessage (&quot;Work item '&quot; &#43; str(wi.Id) &#43; &quot;' has been created '&quot; &#43; wi.Title &#43;&quot;'&quot;)
elif sys.argv[0] == &quot;WorkItemEvent&quot; :
   LogInfoMessage (&quot;A wi event &quot; &#43; sys.argv[1])
elif sys.argv[0] == &quot;CheckInEvent&quot; :
LogInfoMessage (&quot;A checkin event &quot; &#43; sys.argv[1])
else:
   LogInfoMessage (&quot;Was not expecting to get here&quot;)
```


