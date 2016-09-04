using System;
using System.Collections.Generic;

public class Schedule {

	public readonly Situation DefaultSituation;
	private Situation ActualSituation;
	private List<ScheduledSituation> situations = new List<ScheduledSituation>();

	public Schedule(Situation defaultSituation) {
		DefaultSituation = defaultSituation;
	}

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
		ScheduledSituation bigger = smaller == ssTmp ? ss : ssTmp;
		float finish = smaller.From + smaller.Duration;
		if (finish >= 24) {
			finish -= 24;
		}
		return finish > bigger.From;
	}

	internal ScheduledSituation getSituationForHour(int hour) {
		foreach(ScheduledSituation ss in situations) {
			if (hour == ss.From || (hour > ss.From && hour < ss.From + ss.Duration)) {
				return ss;
			}
		}
		return null;
	}

	internal void Update(int hour, Situation ss) {
		AddSituation(hour, 1, ss, false);
	}
}