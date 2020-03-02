using System;
using System.Collections.Generic;
using UnityEngine;

public class Situation {
	public readonly string Id;
	public readonly string Text;
	public readonly List<Change> Changes;
	public readonly bool Selectable;
	public readonly List<LOL.Button> Buttons;
	public readonly Type DayNightType;

	public Situation(string id, string text, List<Change> changes, bool selectable, List<LOL.Button> buttons, Situation.Type type) {
		Id = id;
		Changes = changes;
		Selectable = selectable;
		Text = text;
		Buttons = buttons;
		DayNightType = type;
	}

	public override string ToString() {
		return Id + " with " + Changes.Count + " changes. buttons: " + Buttons.Count;
	}

	public class Type {
		public static readonly Type Day = new Type("day", new Color(255/255f, 219/255f, 68/255f));
		public static readonly Type Night = new Type("night", new Color(52/255f, 0, 216/255f));

		public static readonly Type[] AllTypes = new Type[] { Day, Night };
		
		private string String;
		public readonly Color Color;

		public Type(string @string, Color color) {
			String = @string;
			Color = color;
		}

		internal static Type FromString(string value) {
			foreach(Type t in AllTypes) {
				if (t.String == value) {
					return t;
				}
			}
			throw new Exception("There is no type of Situation with string: " + value);
		}

		internal bool IsRightType(Type type) {
			return this == type;
		}
	}
}