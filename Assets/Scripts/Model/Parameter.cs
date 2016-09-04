public class Parameter {
	
	public readonly string Id;
	private float? MaxValue;
	private float StartValue;
	public readonly string Text;

	public Parameter(string id, float? maxValue, float startValue, string text) {
		Id = id;
		MaxValue = maxValue;
		StartValue = startValue;
		Text = text;
	}

	public override string ToString() {
		return Id + ", maxValue: " + MaxValue + ", startValue: " + StartValue;
	}
}