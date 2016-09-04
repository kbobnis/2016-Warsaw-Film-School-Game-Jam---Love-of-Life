public class Change {

	public readonly Calculation ValueCalculation;
	private Calculation MaxValueCalculation;
	public readonly Parameter What;
	public readonly float PerTime;

	public Change(Parameter what, Calculation valueCalculation, Calculation maxValueCalculation, float perTime) {
		What = what;
		ValueCalculation = valueCalculation;
		PerTime = perTime;
	}
}