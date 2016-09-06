using System;

public class Parameter {
	
	public readonly string Id;
	public float? MaxValue;
	private float StartValue;
	public readonly string Text;

	private float ActualMaxValue;
	public bool IsUsedAndIsZero;
	public float ActualValue;
	

	public Parameter(string id, float? maxValue, float startValue, string text) {
		Id = id;
		MaxValue = maxValue;
		StartValue = startValue;
		Text = text;
		ActualValue = startValue;
		if (MaxValue != null) {
			ActualMaxValue = MaxValue.Value;
		}
	}

	public override string ToString() {
		return Id + " " + ActualValue + "/" + MaxValue + ", startValue: " + StartValue;
	}

	internal bool HasMaxValue() {
		return MaxValue != null;
	}

	public void UpdateWithChange(Change c, float timeDelta) {
		if (c.MaxValueCalculation != null) {
			float deltaMaxValue = c.MaxValueCalculation.Calculate(timeDelta);
			MaxValue += deltaMaxValue;
		}

		float deltaValue = c.ValueCalculation.Calculate(timeDelta);
		ActualValue += deltaValue;
		if (MaxValue != null && ActualValue > MaxValue.Value) {
			ActualValue = MaxValue.Value;
		}


		if (ActualValue < 0f) {
			ActualValue = 0f;
		}
	}

}