using UnityEngine;

public class PanelCenter : MonoBehaviour, GameTimeChangeListener {

	public PanelSituation PanelSituation;
	public CenterPanelParameters PanelParameters;
	public PanelSmallClock PanelSmallClock;

	private int? LastHourOfDay;
	public Situation ActualSituation;
	private GameState GameState;

	void Update() {
		if (GameState != null) {
			int hour = (int)Game.Me.GameState.HourOfDay;
			if (LastHourOfDay != hour) {
				HourHasChanged(hour);
				LastHourOfDay = hour;
			}
		}
	}

	internal void HourHasChanged(int newHour) {
		ScheduledSituation ss = GameState.Schedule.GetSituationForHour(newHour);
		GameState.ActualSituation = new ScheduledSituation((int)GameState.HourOfDay, 1, ss.Situation, false);
		PanelSituation.HourHasChanged(newHour, ss.Situation, GameState);
	}

	internal void Init(GameState gameState) {
		GameState = gameState;
		PanelParameters.Init(gameState.Parameters);
		PanelSmallClock.Init(gameState.Schedule);
	}

	public void GameTimeUpdated(float hourOfDay) {
		PanelSmallClock.GameTimeUpdated(hourOfDay);
	}
}
