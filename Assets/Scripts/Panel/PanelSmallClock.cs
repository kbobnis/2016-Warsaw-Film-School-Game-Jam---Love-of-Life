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
		float angle = (Game.Me.GameState.DayNumber - 1 + Game.Me.GameState.HourOfDay / 24f)/ 30f * 360f * -1f; //-1 because we start from day 1 not 0.
		gameObject.FindByName<Transform>("RedNeedle").localRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
	}

	private void HighlightActive(int hourOfDay) {
		ScheduledSituation ss = Game.Me.GameState.Schedule.GetSituationForHour(hourOfDay);
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		foreach (Transform childT in situationsTrans) {
			childT.gameObject.FindByName<Image>("WhiteBackground").enabled = index == hourOfDay || childT.GetComponent<ButtonSchedule>().IsPermament;
			childT.GetComponentInChildren<Text>().color = index == hourOfDay ? Color.black : Color.white;
			index++;
		}
	}

	private void Refresh(Schedule schedule) {
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		foreach (Transform childT in situationsTrans) {
			ScheduledSituation ss = schedule.GetSituationForHour(index);
			childT.gameObject.GetComponent<ButtonSchedule>().Init(ss.Situation, ss.Permament, index);
			childT.gameObject.FindByName<Image>("WhiteBackground").color = ss.Permament ? Color.red : Color.white;
			childT.gameObject.FindByName<Image>("WhiteBackground").enabled = ss.Permament;
			childT.GetComponentInChildren<Text>().text = ss.Situation.Text;
			index++;
		}
	}

	internal void Init(Schedule schedule) {
		schedule.AddScheduleUpdateSituation(this);
		Transform situationsTrans = gameObject.FindByName<Transform>("Situations");
		int index = 0;
		foreach (Transform childT in situationsTrans) {
			ScheduledSituation ss = schedule.GetSituationForHour(index);
			float angle = -index / 24f * 360 + 81.57f; //81.57 is the offset. this should be the first elements angle
			childT.localRotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
			childT.gameObject.FindByName<Image>("WhiteBackground").enabled = false;
			index++;
		}
		Refresh(schedule);
		HighlightActive(0);
	}

	public void ScheduleUpdated(Schedule s) {
		Refresh(s);
	}
}
