using UnityEngine;
using UnityEngine.UI;

public class PanelSmallClock : MonoBehaviour, Schedule.ScheduleUpdateListener {

	internal void GameTimeUpdated(float hourOfDay) {
		float angle = hourOfDay / 24f * 360f;
		gameObject.FindByName<RectTransform>("Needle").localRotation = Quaternion.AngleAxis(angle, Vector3.back);
		//update highlighted situation
		HighlightActive((int)hourOfDay);
		UpdateDayPointer();
	}

	private void UpdateDayPointer() {
		float angle = (Game.Me.GameState.DayNumber + Game.Me.GameState.HourOfDay / 24f) / 30f * 360f * -1f;
		gameObject.FindByName<Transform>("RedNeedle").localRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
	}

	private void HighlightActive(int hourOfDay) {
		ScheduledSituation ss = Game.Me.GameState.Schedule.GetSituationForHour(hourOfDay);
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		bool isRightType = ss==null || ss.Situation.DayNightType == Game.Me.GameState.Schedule.GetActualDayNightType((int)Game.Me.GameState.HourOfDay);
		foreach (Transform childT in situationsTrans) {
			childT.gameObject.FindByName<Image>("Highlight").color = index == hourOfDay ? Color.white : Color.gray;
			index++;
		}
	}

	private void Refresh(Schedule schedule) {
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		//setup the situations
		foreach (Transform childT in situationsTrans) {
			ScheduledSituation ss = schedule.GetSituationForHour(index);
			Situation s = null;
			bool isPermament = false;
			bool isRightType = true;
			if (ss != null) {
				s = ss.Situation;
				isPermament = ss.Permament;
				isRightType = ss.Situation.DayNightType == schedule.GetActualDayNightType((int)Game.Me.GameState.HourOfDay);
				childT.gameObject.FindByName<Image>("RightWrongType").color = ss.Situation.DayNightType.Color;
			}
			childT.gameObject.GetComponent<ButtonSchedule>().Init(s, isPermament, index);
			index++;
		}

		//set the night arc
		Transform nightArc = gameObject.FindByName<Transform>("ImageNight");
		float startAngle = -schedule.NightTimeFrom.Value / 24f * 360;
		nightArc.localRotation = Quaternion.AngleAxis(startAngle, new Vector3(0, 0, 1));
		float fillAmount = schedule.NightTimeDuration.Value / 24f;
		nightArc.gameObject.GetComponent<Image>().fillAmount = fillAmount;

		//set the day arc
		Transform dayArc = gameObject.FindByName<Transform>("ImageDay");
		float dayStartAngle = -(schedule.NightTimeFrom.Value / 24f + fillAmount) * 360;
		dayArc.localRotation = Quaternion.AngleAxis(dayStartAngle, new Vector3(0, 0, 1));
		float dayFillAmount = 1 - fillAmount;
		dayArc.gameObject.GetComponent<Image>().fillAmount = dayFillAmount;
	}

	internal void Init(Schedule schedule) {
		schedule.AddScheduleUpdateSituation(this);
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		foreach (Transform childT in situationsTrans) {
			ScheduledSituation ss = schedule.GetSituationForHour(index);
			float angle = -index / 24f * 360 + 81.57f; //81.57 is the offset. this should be the first elements angle
			childT.localRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
			index++;
		}
		Refresh(schedule);
		HighlightActive(0);
	}

	public void ScheduleUpdated(Schedule s) {
		Refresh(s);
	}
}
