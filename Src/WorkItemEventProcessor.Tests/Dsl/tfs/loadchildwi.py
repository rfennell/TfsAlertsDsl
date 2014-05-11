wi = GetWorkItem(99)
for childwi in GetChildWorkItems(wi):
    print("Work item '" + str(wi.Id) + "' has a child '" + str(childwi.Id) + "' with the title '" + childwi.Title +"'")