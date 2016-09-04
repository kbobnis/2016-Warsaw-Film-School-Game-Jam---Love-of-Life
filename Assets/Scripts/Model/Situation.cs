using System.Collections.Generic;

public class Situation {
	public readonly string Id;
	public readonly string Text;
	public readonly List<Change> Changes;
	private List<Event> Events;
	public readonly bool Selectable;

	public Situation(string id, string text, List<Change> changes, List<Event> events, bool selectable) {
		Id = id;
		Changes = changes;
		Events = events;
		Selectable = selectable;
		Text = text;
	}

	public override string ToString() {
		return Id + " with " + Changes.Count + " changes, and " + Events.Count + " events.";
	}
}