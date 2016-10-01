using System;
using System.Collections.Generic;

public class GameState {

	internal Model Model;
	internal List<Parameter> Parameters;
	public float ActualGameSpeed = 1f;
	public float GameTime;
	public Schedule Schedule;
	public List<Situation> Situations;
	public ScheduledSituation ActualSituation;
	public bool GameHasEnded;

	public GameState(List<Parameter> parameters, List<Situation> situations, Schedule scheduledSituations, Model model) {
		Situations = situations;
		ActualGameSpeed = model.TimeChanges.NormalSpeed;
		Model = model;
		Schedule = scheduledSituations;
		ActualSituation = scheduledSituations.getSituationForHour(0, true);
		Parameters = parameters;
	}

	public int HourOfDay {
		get {
			return (int)GameTime % 24;
		}
	}

	internal void Update(float deltaTime) {
		if (!GameHasEnded) {
			int hourOfDay = HourOfDay;
			float timeDelta = deltaTime / 60f * ActualGameSpeed;
			GameTime += timeDelta;
			UpdateParameters(timeDelta);
		}
	}

	public int DayNumber {
		get {
			return (int)GameTime / 24 + 1;
		}
	}

	public void UpdateParameters(float timeDelta) {
		//clear all parameter negative values
		foreach (Parameter p in Parameters) {
			p.IsUsedAndIsZero = false;
			p.IsDraggedDownBy.Clear();
			p.IsDraggingDown = false;
		}

		//actual situation changes
		foreach (Change change in ActualSituation.Situation.Changes) {
			change.UpdateParams(timeDelta);
		}

		//all time changes
		foreach (Change change in Model.TimeChanges.Changes) {
			change.UpdateParams(timeDelta);
		}

		//update all params 
		foreach (Parameter p in Parameters) {
			p.UpdateValuesFromPreviousLoop();
		}
	}
}
