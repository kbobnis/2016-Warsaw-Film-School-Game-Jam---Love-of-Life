internal class Change {
	private Calculation ValueCalculation;
	private Calculation MaxValueCalculation;
	private Parameter What;

	public Change(Parameter what, Calculation valueCalculation, Calculation maxValueCalculation) {
		What = what;
		ValueCalculation = valueCalculation;
	}
}