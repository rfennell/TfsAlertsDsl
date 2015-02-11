import sys
# Expect 2 args the event type and a value unique ID for the wi
if sys.argv[0] == "WorkItemEvent" : 
    wi = GetWorkItem(int(sys.argv[1]))
    parentwi = GetParentWorkItem(wi)
    if parentwi == None:
        LogInfoMessage("Work item '" + str(wi.Id) + "' has no parent")
    else:
        LogInfoMessage("Work item '" + str(wi.Id) + "' has parent '" + str(parentwi.Id) + "'")

        results = [c for c in GetChildWorkItems(parentwi) if c.State != "Done"]
        if  len(results) == 0 :
            LogInfoMessage("All child work items are 'Done'")
            parentwi.State = "Done"
            UpdateWorkItem(parentwi)
            msg = "Work item '" + str(parentwi.Id) + "' has been set as 'Done' as all its child work items are done"
            SendEmail("richard@typhoontfs","Work item '" + str(parentwi.Id) + "' has been updated", msg)
            LogInfoMessage(msg)
        else:
            LogInfoMessage("Not all child work items are 'Done'")
else:
	LogErrorMessage("Was not expecting to get here")
	LogErrorMessage(sys.argv)