using System.Collections.Generic;

public class Event {
	private string Id;
	private string Text;
	private string DepleteText;
	private string InterruptText;
	private string MaxTime;
	private bool CanBeInterrupted;
	private List<Change> Changes;

	public Event(string id, string text, string maxTime, string depleteText, string interruptText, List<Change> changes, bool canBeInterrupted) {
		Id = id;
		Text = text;
		MaxTime = maxTime;
		DepleteText = depleteText;
		InterruptText = interruptText;
		Changes = changes;
		CanBeInterrupted = canBeInterrupted;
	}
}