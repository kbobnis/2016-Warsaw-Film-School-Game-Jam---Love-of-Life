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