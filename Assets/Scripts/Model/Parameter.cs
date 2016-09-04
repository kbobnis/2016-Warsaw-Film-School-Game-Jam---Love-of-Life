using System;

public class Parameter {
	
	public readonly string Id;
	private float? MaxValue;
	private float StartValue;
	public readonly string Text;

	private float _ActualValue;
	private float PreviousValue;
	public float ActualValue {
		set {
			PreviousValue = _ActualValue;
			_ActualValue = value;
		}
		get {
			return _ActualValue;
		}
	}
	

	public Parameter(string id, float? maxValue, float startValue, string text) {
		Id = id;
		MaxValue = maxValue;
		StartValue = startValue;
		Text = text;
		ActualValue = startValue;
	}

	public override string ToString() {
		return Id + " " + ActualValue + "/" + MaxValue + ", startValue: " + StartValue;
	}

	internal bool IsRising() {
		return ActualValue > PreviousValue;
	}

	internal void UpdateValue(Situation s, float timeDelta) {
		bool changed = false;
		foreach(Change c in s.Changes) {
			if (c.What == this) {
				float deltaValue = c.ValueCalculation.Calculate(timeDelta);
				ActualValue += deltaValue;
				if (MaxValue != null && ActualValue > MaxValue.Value) {
					ActualValue = MaxValue.Value;
				}
				if (ActualValue <= 0) {
					Game.Me.EndGame(this);
				}
				changed = deltaValue!=0;
			}
		}
	}
}