# Add  key-value tuples to the dictionary
fields = {"Title": "The Title","Estimate": 2}  

wi = CreateWorkItem("tp","Bug",fields)

print("Work item '" + str(wi.Id) + "' has been created with the title '" + wi.Title +"'")