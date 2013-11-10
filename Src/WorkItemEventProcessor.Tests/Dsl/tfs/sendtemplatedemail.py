﻿import sys
# Expect 2 args the event type and a WI id
if sys.argv[0] == "WorkItemEvent" : 
    wi = int(sys.argv[1])
    dumpAllWorkItemFields = True
    dumpAllAlertFields = True
    showMissingFieldNames = True
    SendEmail(wi, CurrentScriptFolder() + "\EmailTemplate.htm", dumpAllWorkItemFields, dumpAllAlertFields, showMissingFieldNames)
    LogInfoMessage("Sending email for work item " + str(wi))
else:
	LogErrorMessage("Was not expecting to get here")
	LogErrorMessage(sys.argv)
