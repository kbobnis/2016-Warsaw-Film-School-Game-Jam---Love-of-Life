using System.Collections.Generic;

internal class Situation {
	public readonly string Id;
	private List<Change> Changes;
	private List<Event> Events;

	public Situation(string id, List<Change> changes, List<Event> events) {
		Id = id;
		Changes = changes;
		Events = events;
	}

	public override string ToString() {
		return Id + " with " + Changes.Count + " changes, and " + Events.Count + " events.";
	}
}