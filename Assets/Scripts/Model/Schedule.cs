using System;
using System.Collections.Generic;

public class Schedule {

	public List<ScheduledSituation> Situations = new List<ScheduledSituation>();
	public List<ScheduleUpdateListener> ScheduleUpdateListeners = new List<ScheduleUpdateListener>();
	public int? NightTimeFrom;
	public int? NightTimeDuration;

	public Schedule(int? nightTimeFrom, int? nightTimeDuration) {
		NightTimeFrom = nightTimeFrom;
		NightTimeDuration = nightTimeDuration;
	}

	internal void ReplaceSituation(int from, int duration, Situation situation, bool permament) {

		//if duration bigger than 1 then cut it to pieces;
		if (duration < 1) {
			throw new Exception("You can not add situation with duration 0");
		}
		if (duration > 1) {
			for (int i = 0; i < duration; i++) {
				ReplaceSituation(from + i, 1, situation, permament);
			}
			return;
		}

		ScheduledSituation ss = new ScheduledSituation(from, duration, situation, permament);
		ScheduledSituation[] s = Situations.ToArray();
		foreach (ScheduledSituation ssTmp in s) {
			if (AreOverlapping(ssTmp, ss)) {
				Situations.Remove(ssTmp);
				break;
			}
		}
		Situations.Add(ss);
		Situations.Sort((t, y) => t.From.CompareTo(y.From));
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

	internal ScheduledSituation GetSituationForHour(int hour) {
		if (hour > 23) {
			throw new Exception("There is no more hours in day, but you gave: " + hour);
		}

		foreach(ScheduledSituation ss in Situations) {
			if (hour == ss.From || (hour > ss.From && hour < ss.From + ss.Duration)) {
				return ss;
			}
		}
		throw new Exception("There has to be always a situation everywhere, check hour: " + hour);
	}

	internal void AddScheduleUpdateSituation(ScheduleUpdateListener listener) {
		ScheduleUpdateListeners.Add(listener);
	}

	internal void OverrideSchedule(Schedule scheduleOverride) {
		if (scheduleOverride != null) {
			foreach (ScheduledSituation ss in scheduleOverride.Situations) {
				ReplaceSituation(ss.From, ss.Duration, ss.Situation, ss.Permament);
			}
			if (scheduleOverride.NightTimeFrom != null) {
				NightTimeFrom = scheduleOverride.NightTimeFrom;
			}
			if (scheduleOverride.NightTimeDuration != null) {
				NightTimeDuration = scheduleOverride.NightTimeDuration;
			}
		}
	}

	public void ScheduleUpdated() {
		foreach(ScheduleUpdateListener listener in ScheduleUpdateListeners) {
			listener.ScheduleUpdated(this);
		}
	}

	internal Situation.Type GetActualDayNightType(int hourOfDay) {
		return (hourOfDay >= NightTimeFrom || hourOfDay < (NightTimeFrom + NightTimeDuration) % 24) ? Situation.Type.Night : Situation.Type.Day;
	}

	internal void Update(int hour, Situation ss) {
		ReplaceSituation(hour, 1, ss, false);
	}

	public interface ScheduleUpdateListener {
		void ScheduleUpdated(Schedule s);
	}
}