using System;
using System.Collections.Generic;

internal class Schedule {

	private List<ScheduledSituation> situations = new List<ScheduledSituation>();

	internal void AddSituation(float from, float duration, Situation situation) {

		ScheduledSituation ss = new ScheduledSituation(from, duration, situation);
		foreach(ScheduledSituation ssTmp in situations) {
			if (AreOverlapping(ssTmp, ss)) {
				throw new Exception("Schedules are overlapping: " + ss + ", and: " + ssTmp);
			}
		}
		situations.Add(ss);

	}

	private bool AreOverlapping(ScheduledSituation ssTmp, ScheduledSituation ss) {
		ScheduledSituation smaller = ssTmp.From < ss.From ? ssTmp : ss;
		float finish = smaller.From + smaller.Duration;
		if (finish >= 24) {
			finish -= 24;
		}
		return finish > ss.From;
	}

	internal class ScheduledSituation {
		public readonly float Duration;
		public readonly float From;
		public readonly Situation Situation;

		public ScheduledSituation(float from, float duration, Situation situation) {
			From = from;
			Duration = duration;
			Situation = situation;
		}

		public override string ToString() {
			return Situation.Id + " from " + From + " duration " + Duration;
		}
	}
}