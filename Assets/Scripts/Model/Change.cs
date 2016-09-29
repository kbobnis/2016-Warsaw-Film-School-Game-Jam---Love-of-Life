public class Change {

	public readonly Calculation ValueCalculation;
	public readonly Calculation MaxValueCalculation;
	public readonly Parameter What;
	public readonly float PerTime;

	public Change(Parameter what, Calculation valueCalculation, Calculation maxValueCalculation, float perTime) {
		What = what;
		ValueCalculation = valueCalculation;
		PerTime = perTime;
		MaxValueCalculation = maxValueCalculation;
	}

	internal bool CanUpdateWithoutOverflow(Change c, float? timeDelta = null) {
		bool canIt = true;
		if (c.ValueCalculation != null) {
			canIt = c.What.ActualValue + c.ValueCalculation.Calculate(timeDelta) >= 0;
		}
		return canIt;
	}

	public void UpdateParams(float? timeDelta = null) {

		What.UpdateValuesFromPreviousLoop();
		if (MaxValueCalculation != null) {
			What.PreviousLoopMaxValueDelta += MaxValueCalculation.Calculate(timeDelta);
		}

		if (ValueCalculation != null) {
			What.PreviousLoopValueDelta += ValueCalculation.Calculate(timeDelta);
		}

		What.DragDown();
	}

	public override string ToString() {
		string valueCalc = ", value: " + ValueCalculation;
		string maxValueCalc = ", max value: " + MaxValueCalculation;
		return "change: " + What.Id + (ValueCalculation!=null?valueCalc:"") + (MaxValueCalculation!=null?maxValueCalc:"") ;
	}

	public string ToHumanString() {
		string valueCalc = "" + ValueCalculation;
		string maxValueCalc = " maks: " + MaxValueCalculation;
		return (ValueCalculation != null ? valueCalc : "") + (MaxValueCalculation != null ? maxValueCalc : "") + " = " + What.Text;
	}
}