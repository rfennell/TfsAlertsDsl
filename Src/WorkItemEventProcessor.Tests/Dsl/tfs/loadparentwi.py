wi = GetWorkItem(99)
parentwi = GetParentWorkItem(wi)
if parentwi == None:
    print("Work item '" + str(wi.Id) + "' has no parent")
else:
    print("Work item '" + str(wi.Id) + "' has a parent '" + str(parentwi.Id) + "' with the title '" + parentwi.Title +"'")