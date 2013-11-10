import sys
# Expect 2 args the event type and a value unique ID for the build
if sys.argv[0] == "BuildEvent" : 
	uri = sys.argv[1]
	build = GetBuildDetails(uri)
	if build.Quality == "Test Failed" or build.Quality == "Rejected" : 
		keepForever = False
	else:
		keepForever = True
	SetBuildRetension(uri, keepForever)
	msg = "'" + build.BuildNumber + "' retension set to '" + str(keepForever) + "' as quality was changed to '" + build.Quality +"'"
	SendEmail("richard@typhoontfs",build.BuildNumber + " quality changed", msg)
	LogInfoMessage(msg)
else:
	LogErrorMessage("Was not expecting to get here")
	LogErrorMessage(sys.argv)
