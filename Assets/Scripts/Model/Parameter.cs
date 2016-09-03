internal class Parameter {
	
	public readonly string Id;
	private float? MaxValue;
	private float StartValue;

	public Parameter(string id, float? maxValue, float startValue) {
		Id = id;
		MaxValue = maxValue;
		StartValue = startValue;
	}

	public override string ToString() {
		return Id + ", maxValue: " + MaxValue + ", startValue: " + StartValue;
	}
}