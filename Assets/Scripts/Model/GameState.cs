using System.Collections.Generic;

public class GameState {

	internal Model Model;
	public Plot Plot;

	internal List<Parameter> Parameters;
	public float ActualGameSpeed = 1f;
	public int ActualPoints;
	public float GameTime;

	internal Plot.Element ActualPlotElement;
	public Schedule Schedule;
	public List<Situation> Situations;
	public ScheduledSituation ActualSituation;
	public bool GameHasEnded;
	public readonly string GameHash;
	public readonly int PointsEvery;

	public List<GameTimeChangeListener> GameTimeChangeListeners = new List<GameTimeChangeListener>();
	public List<ActualPointsChangeListener> ActualPointsChangeLisnters = new List<ActualPointsChangeListener>();

	public GameState(List<Parameter> parameters, List<Situation> situations, Schedule schedule, Model model, Plot plot, string gameHash, int pointsEvery) {
		PointsEvery = pointsEvery;
		Situations = situations;
		ActualGameSpeed = model.TimeChanges.NormalSpeed;
		Model = model;
		Schedule = schedule;
		ActualSituation = schedule.GetSituationForHour(0);
		Parameters = parameters;
		Plot = plot;
		SetPlotElement(plot.Elements[0]);
		GameHash = gameHash;
	}

	public float HourOfDay {
		get {
			return GameTime % 24f;
		}
	}

	internal void Update(float deltaTime) {
		if (!GameHasEnded) {
			float hourOfDay = HourOfDay;
			int oldDayNumber = DayNumber;

			float timeDelta = deltaTime / 60f * ActualGameSpeed;
			GameTime += timeDelta;

			if (hourOfDay != HourOfDay) {
				int newPoints = (int)GameTime % PointsEvery == 0 ? 1 : 0;
				if (newPoints > 0) {
					ActualPoints += newPoints;
					foreach(ActualPointsChangeListener apl in ActualPointsChangeLisnters) {
						apl.PointsChanged(ActualPoints);
					}
				}
			}
			foreach (GameTimeChangeListener l in GameTimeChangeListeners) {
				l.GameTimeUpdated(hourOfDay);
			}
			UpdateParameters(timeDelta);

			//if new day
			int newDayNumber = DayNumber;
			if (newDayNumber != oldDayNumber) {
				if (ActualPlotElement.DayUpdated(newDayNumber) == Plot.Element.Goal.Fullfilled.Yes) {
					NewPlotElement();
				}
			}
			//check parameters
			if (ActualPlotElement.ParametersUpdated(Parameters) == Plot.Element.Goal.Fullfilled.Yes) {
				NewPlotElement();
			}
		}
	}

	private void NewPlotElement() {
		int index = Plot.Elements.IndexOf(ActualPlotElement) + 1;
		if (index >= Plot.Elements.Count) {
			Game.Me.EndGame(new EndCondition.Win(ActualPlotElement, Game.Me.GameState));
		} else {
			SetPlotElement(Plot.Elements[index]);
		}
	}

	private void SetPlotElement(Plot.Element el) {
		ActualPlotElement = el;
		Schedule.OverrideSchedule(el.ScheduleOverride);
	}

	public int DayNumber {
		get {
			return (int)GameTime / 24;
		}
	}

	public void UpdateParameters(float timeDelta) {
		//clear all parameter negative values
		foreach (Parameter p in Parameters) {
			p.IsUsedAndIsZero = false;
			p.IsDraggedDownBy.Clear();
			p.IsDraggingDown = false;
		}
		bool isRightType = ActualSituation==null || ActualSituation.Situation.DayNightType.IsRightType(Schedule.GetActualDayNightType((int)Game.Me.GameState.HourOfDay));

		//actual situation changes
		if (ActualSituation != null) {
			foreach (Change change in ActualSituation.Situation.Changes) {
				change.UpdateParams(isRightType, timeDelta);
			}
		}

		//all time changes
		foreach (Change change in Model.TimeChanges.Changes) {
			change.UpdateParams(true, timeDelta); //time changes don't change during the day
		}

		//update all params 
		foreach (Parameter p in Parameters) {
			p.UpdateValuesFromPreviousLoop();
		}
	}
}

public interface GameTimeChangeListener {
	void GameTimeUpdated(float hourOfDay);
}
