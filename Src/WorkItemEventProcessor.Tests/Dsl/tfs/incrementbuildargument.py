import sys
# Expect 2 args the event type and a value unique ID for the build
if sys.argv[0] == "BuildEvent" : 
	uri = sys.argv[1]
	build = GetBuildDetails(uri)
	if build.Quality == "Released" : 
		IncrementBuildNumber(uri)
		msg = "'" + build.BuildDefinition.Name + "' version incremented to " + GetVersionNumber(uri) + " as last build quality set to '" + build.Quality +"'"
		SendEmail("richard@typhoontfs",build.BuildDefinition.Name + " version incremented", msg)
		LogInfoMessage(msg)
else:
	LogErrorMessage("Was not expecting to get here")
	LogErrorMessage(sys.argv)
