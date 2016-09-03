using System;
using System.Collections.Generic;

internal class Schedule {

	private List<ScheduledSituation> situations = new List<ScheduledSituation>();

	internal void AddSituation(float from, int duration, Situation situation, bool permament) {
		ScheduledSituation ss = new ScheduledSituation(from, duration, situation, permament);
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

	internal ScheduledSituation getSituationForHour(int hour) {
		foreach(ScheduledSituation ss in situations) {
			if (hour == ss.From || (hour > ss.From && hour < ss.From + ss.Duration)) {
				return ss;
			}
		}
		return null;
	}
}