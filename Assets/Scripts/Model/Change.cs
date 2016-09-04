public class Change {

	private Calculation ValueCalculation;
	private Calculation MaxValueCalculation;
	private Parameter What;
	public readonly float PerTime;

	public Change(Parameter what, Calculation valueCalculation, Calculation maxValueCalculation, float perTime) {
		What = what;
		ValueCalculation = valueCalculation;
		PerTime = perTime;
	}
}