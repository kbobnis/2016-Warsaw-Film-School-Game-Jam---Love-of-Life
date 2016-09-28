
using System.Collections.Generic;

namespace LOL {

	public class Button {
		public readonly string Text;
		public readonly List<Change> Changes;

		public Button(string text, List<Change> changes) {
			Text = text;
			Changes = changes;
		}
	}

}
