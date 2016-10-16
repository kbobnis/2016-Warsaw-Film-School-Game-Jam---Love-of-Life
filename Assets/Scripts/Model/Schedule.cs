using System;
using System.Collections.Generic;

public class Schedule {

	public readonly Situation DefaultSituation;
	public List<ScheduledSituation> Situations = new List<ScheduledSituation>();
	public List<ScheduleUpdateListener> ScheduleUpdateListeners = new List<ScheduleUpdateListener>();

	public Schedule(Situation defaultSituation) {
		DefaultSituation = defaultSituation;
	}

	internal void AddSituation(int from, int duration, Situation situation, bool permament, bool @override=false) {
		if (situation == null) {
			RemoveSituation(from);
			return;
		}

		//if duration bigger than 1 then cut it to pieces;
		if (duration < 1) {
			throw new Exception("You can not add situation with duration 0");
		}
		if (duration > 1) {
			for (int i = 0; i < duration; i++) {
				AddSituation(from + i, 1, situation, permament, @override);
			}
			return;
		}

		ScheduledSituation ss = new ScheduledSituation(from, duration, situation, permament);
		ScheduledSituation[] s = Situations.ToArray();
		foreach (ScheduledSituation ssTmp in s) {
			if (AreOverlapping(ssTmp, ss)) {
				if (@override) {
					Situations.Remove(ssTmp);
					break;
				} else {
					throw new Exception("Schedules are overlapping: " + ss + ", and: " + ssTmp);
				}
			}
		}
		Situations.Add(ss);
		Situations.Sort((t, y) => t.From.CompareTo(y.From));
		ScheduleUpdated();
	}

	private void RemoveSituation(int from) {
		ScheduledSituation tmp = null;
		foreach(ScheduledSituation ss in Situations) {
			if (ss.From == from) {
				tmp = ss;
			}
		}

		if (tmp != null) {
			Situations.Remove(tmp);
		}
		ScheduleUpdated();
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

	internal ScheduledSituation getSituationForHour(int hour, bool includeDefault=false) {
		if (hour > 23) {
			throw new Exception("There is no more hours in doba, but you gave: " + hour);
		}

		foreach(ScheduledSituation ss in Situations) {
			if (hour == ss.From || (hour > ss.From && hour < ss.From + ss.Duration)) {
				return ss;
			}
		}
		return includeDefault?new ScheduledSituation(hour, 1, DefaultSituation, false):null;
	}

	internal void AddScheduleUpdateSituation(ScheduleUpdateListener listener) {
		ScheduleUpdateListeners.Add(listener);
	}

	internal void OverrideSchedule(Schedule scheduleOverride) {
		if (scheduleOverride != null) {
			foreach (ScheduledSituation ss in scheduleOverride.Situations) {
				AddSituation(ss.From, ss.Duration, ss.Situation, ss.Permament, true);
			}
		}
	}

	public void ScheduleUpdated() {
		foreach(ScheduleUpdateListener listener in ScheduleUpdateListeners) {
			listener.ScheduleUpdated(Situations);
		}
	}

	internal void Update(int hour, Situation ss) {
		AddSituation(hour, 1, ss, false);
	}

	public interface ScheduleUpdateListener {
		void ScheduleUpdated(List<ScheduledSituation> situations);
	}
}