using System.Collections.Generic;

public class Parameter {

	public readonly string Id;
	private float StartValue;
	public readonly string Text;
	public readonly bool IsMain;

	public Calculation MaxValue;
	public float ActualMaxValueMultiplier { get; private set; }

	public float ActualValue;
	public List<Parameter> DragDownIfZero;
	

	public bool IsDraggingDown;
	public bool IsUsedAndIsZero;
	public List<Parameter> IsDraggedDownBy = new List<Parameter>();
	private readonly float DragDownIfZeroPenalty;
	public float PreviousLoopMaxValueDelta;
	public float PreviousLoopValueDelta;

	public Parameter(string id, float startValue, string text, float dragDownIfZeroPenalty, bool isMain) {
		Id = id;
		StartValue = startValue;
		Text = text;
		ActualValue = startValue;
		ActualMaxValueMultiplier = 1;
		IsMain = isMain;
		DragDownIfZeroPenalty = dragDownIfZeroPenalty;
	}

	public override string ToString() {
		return Id + " " + ActualValue + "/" + (MaxValue.Calculate() * ActualMaxValueMultiplier) + ", startValue: " + StartValue;
	}

	internal bool HasMaxValue() {
		return MaxValue != null;
	}

	public void UpdateValuesFromPreviousLoop() {
		ActualMaxValueMultiplier += PreviousLoopMaxValueDelta;
		PreviousLoopMaxValueDelta = 0;
		ActualValue += PreviousLoopValueDelta;
		PreviousLoopValueDelta = 0;
	}

	public void DragDown() {
		if (MaxValue != null && ActualValue > MaxValue.Calculate() * ActualMaxValueMultiplier) {
			ActualValue = MaxValue.Calculate() * ActualMaxValueMultiplier;
		}

		if (ActualValue < 0f && !IsMain) { //if this is main, then this is already over
			if (DragDownIfZero != null) {
				float howMuch = -ActualValue / DragDownIfZero.Count;
				IsDraggingDown = true;
				foreach (Parameter p in DragDownIfZero) {
					p.DropValue(howMuch, this);
				}
			}
			ActualValue = 0f;
		}

		if (IsMain) {
			if (ActualValue <= 0f) {
				Game.Me.EndGame(new EndCondition.Lose(this, Game.Me.GameState));
			}
		}
	}

	private void DropValue(float howMuch, Parameter parameter) {
		ActualValue -= howMuch * parameter.DragDownIfZeroPenalty;
		IsDraggedDownBy.Add(parameter);
		DragDown();
	}

	internal void AddDragIfZeroAndMaxValue(List<Parameter> dragDownIfZero, Calculation maxValue) {
		DragDownIfZero = dragDownIfZero;
		MaxValue = maxValue;
	}

}