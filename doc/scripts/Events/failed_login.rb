def failed_login(sender, e)
	Log.Notice "Failed login: #{e.Username} - #{e.IP}"
end