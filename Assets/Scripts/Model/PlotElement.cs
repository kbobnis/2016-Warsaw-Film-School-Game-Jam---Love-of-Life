using System.Collections.Generic;
using System.Linq;

public class Plot {

	public readonly List<Element> Elements;

	public Plot(List<Element> plotElements) {
		Elements = plotElements;
	}

	public class Element {
		public readonly List<Goal> Goals;
		public readonly Schedule ScheduleOverride;
		public readonly string Text;

		public Element(string text, List<Goal> plotGoals, Schedule scheduleOverride) {
			Text = text;
			Goals = plotGoals;
			ScheduleOverride = scheduleOverride;
		}

		internal Goal.Fullfilled DayUpdated(int newDayNumber) {
			foreach (Goal goal in Goals) {
				if (goal.IsFullfilledInDay(newDayNumber)) {
					return Goal.Fullfilled.Yes;
				}
			}
			return Goal.Fullfilled.No;
		}

		internal string GetInfo() {
			return Text + ". " + Goals.Select(t => t.GetInfo()).Aggregate((t, y) => t + " lub " + y);
		}

		internal Goal.Fullfilled ParametersUpdated(List<Parameter> parameters) {
			foreach(Goal goal in Goals) {
				if (goal.IsFullfilledWithParameters(parameters)) {
					return Goal.Fullfilled.Yes;
				}
			}
			return Goal.Fullfilled.No;
		}

		public abstract class Goal {
			public abstract bool IsFullfilledInDay(int newDayNumber);
			public abstract bool IsFullfilledWithParameters(List<Parameter> parameters);
			public abstract string GetInfo();

			public enum Fullfilled {
				Yes,
				No
			}
		}

		internal class DayNumberGoal : Goal {
			private int DayNumber;

			public DayNumberGoal(int dayNumber) {
				DayNumber = dayNumber;
			}

			public override string GetInfo() {
				return "Osiągnij dzień " + DayNumber;
			}

			public override bool IsFullfilledInDay(int newDayNumber) {
				return newDayNumber >= DayNumber;
			}

			public override bool IsFullfilledWithParameters(List<Parameter> parameters) {
				return false;
			}
		}
		internal class ParameterValueGoal : Goal {
			private float DesiredValue;
			private Parameter Parameter;

			public ParameterValueGoal(Parameter p, float desiredValue) {
				Parameter = p;
				DesiredValue = desiredValue;
			}

			public override string GetInfo() {
				return "Osiągnij wartość " + DesiredValue + " parametru " + Parameter.Text;
			}

			public override bool IsFullfilledInDay(int newDayNumber) {
				return false;
			}

			public override bool IsFullfilledWithParameters(List<Parameter> parameters) {
				return Parameter.ActualValue >= DesiredValue;
			}
		}
	}
}
