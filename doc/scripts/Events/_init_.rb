def _init_(events)
	events.OnFailedLogin { |sender, e| self.failed_login sender, e }
end