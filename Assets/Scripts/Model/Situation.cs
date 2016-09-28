using System.Collections.Generic;

public class Situation {
	public readonly string Id;
	public readonly string Text;
	public readonly List<Change> Changes;
	public readonly bool Selectable;
	public readonly List<LOL.Button> Buttons;

	public Situation(string id, string text, List<Change> changes, bool selectable, List<LOL.Button> buttons) {
		Id = id;
		Changes = changes;
		Selectable = selectable;
		Text = text;
		Buttons = buttons;
	}

	public override string ToString() {
		return Id + " with " + Changes.Count + " changes. buttons: " + Buttons.Count;
	}
}