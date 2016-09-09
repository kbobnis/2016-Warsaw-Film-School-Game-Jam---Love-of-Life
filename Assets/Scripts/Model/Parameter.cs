using System;
using System.Collections.Generic;

public class Parameter {
	
	public readonly string Id;
	public float? MaxValue;
	private float StartValue;
	public readonly string Text;
	public readonly bool ZeroEndsGame;

	private float ActualMaxValue;
	public float ActualValue;
	public List<Parameter> DragDownIfZero;

	public bool IsDraggingDown;
	public bool IsUsedAndIsZero;
	public List<Parameter> IsDraggedDownBy = new List<Parameter>();
	private readonly float DragDownIfZeroPenalty;

	public Parameter(string id, float? maxValue, float startValue, string text, bool zeroEndsGame, float dragDownIfZeroPenalty) {
		Id = id;
		MaxValue = maxValue;
		StartValue = startValue;
		Text = text;
		ActualValue = startValue;
		if (MaxValue != null) {
			ActualMaxValue = MaxValue.Value;
		}
		ZeroEndsGame = zeroEndsGame;
		DragDownIfZeroPenalty = dragDownIfZeroPenalty;
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

		ActualizeValue();
	}

	public void ActualizeValue() {
		if (ActualValue < 0f) {
			if (ZeroEndsGame) {
				throw new Exception("End game.");
			}
			if (DragDownIfZero != null) {
				float howMuch = -ActualValue / DragDownIfZero.Count;
				IsDraggingDown = true;
				foreach (Parameter p in DragDownIfZero) {
					p.DropValue(howMuch, this);
				}
			}
			ActualValue = 0f;
		}
	}

	private void DropValue(float howMuch, Parameter parameter) {
		ActualValue -= howMuch * parameter.DragDownIfZeroPenalty;
		IsDraggedDownBy.Add(parameter);
		ActualizeValue();
	}

	internal void AddDragDownIfZero(List<Parameter> dragDownIfZero) {
		DragDownIfZero = dragDownIfZero;
	}
}