using System.Collections.Generic;
using System.Linq;

namespace LOL {

	public class Button {
		public readonly string Text;
		public readonly List<Change> Changes;

		public Button(string text, List<Change> changes) {
			Text = text;
			Changes = changes;
		}

		public override string ToString() {
			return Text + ", changes: " + Changes.Select(c => c.ToString()).Aggregate((t, y) => t + ", " + y);
		}
	}

}
