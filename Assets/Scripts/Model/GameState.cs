using System.Collections.Generic;

public class GameState {

	internal Model Model;
	internal List<Parameter> Parameters;
	public float ActualGameSpeed = 1f;
	public float GameTime;
	public Schedule ScheduledSituations;
	public ScheduledSituation ActualSituation;

	public GameState(List<Parameter> parameters, Schedule scheduledSituations, Model model) {
		ActualGameSpeed = model.TimeChanges.NormalSpeed;
		Model = model;
		ScheduledSituations = scheduledSituations;
		ActualSituation = scheduledSituations.getSituationForHour(0, true);
		Parameters = parameters;
	}

	public int HourOfDay {
		get {
			return (int)GameTime % 24;
		}
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
