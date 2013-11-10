import sys
# Expect 2 args the event type and a value unique ID
if sys.argv[0] == "BuildEvent" : 
    print ("A build event " + sys.argv[1])
elif sys.argv[0] == "WorkItemEvent" :
	print ("A wi event " + sys.argv[1])
elif sys.argv[0] == "CheckInEvent" :
	print ("A checkin event " + sys.argv[1])
else:
	print ("Was not expecting to get here")
	print sys.argv

	