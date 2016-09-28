﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Parameter {
	
	public readonly string Id;
	private float StartValue;
	public readonly string Text;
	public readonly bool ZeroEndsGame;

	public Calculation MaxValue;
	public float ActualMaxValueMultiplier { get; private set; }

	public float ActualValue;
	public List<Parameter> DragDownIfZero;

	public bool IsDraggingDown;
	public bool IsUsedAndIsZero;
	public List<Parameter> IsDraggedDownBy = new List<Parameter>();
	private readonly float DragDownIfZeroPenalty;

	public Parameter(string id, float startValue, string text, bool zeroEndsGame, float dragDownIfZeroPenalty) {
		Id = id;
		StartValue = startValue;
		Text = text;
		ActualValue = startValue;
		ActualMaxValueMultiplier = 1;
		ZeroEndsGame = zeroEndsGame;
		DragDownIfZeroPenalty = dragDownIfZeroPenalty;
	}

	public override string ToString() {
		return Id + " " + ActualValue + "/" + (MaxValue.Calculate() * ActualMaxValueMultiplier) + ", startValue: " + StartValue;
	}

	internal bool HasMaxValue() {
		return MaxValue != null;
	}

	internal bool CanUpdateWithoutOverflow(Change c, float? timeDelta = null) {
		bool canIt = true;
		if (c.ValueCalculation != null) {
			canIt = ActualValue >= c.ValueCalculation.Calculate(timeDelta);
		}
		return canIt;
	}

	public void UpdateWithChange(Change c, float? timeDelta = null) {
		if (c.MaxValueCalculation != null) {
			float deltaMaxValue = c.MaxValueCalculation.Calculate(timeDelta);
			ActualMaxValueMultiplier += deltaMaxValue;
		}

		if (c.ValueCalculation != null) {
			float deltaValue = c.ValueCalculation.Calculate(timeDelta);
			ActualValue += deltaValue;
		}

		if (MaxValue != null && ActualValue > MaxValue.Calculate() * ActualMaxValueMultiplier) {
			ActualValue = MaxValue.Calculate() * ActualMaxValueMultiplier;
		}

		DragDown();
	}

	public void DragDown() {
		if (ActualValue < 0f && ZeroEndsGame) {
			throw new Exception("End game.");
		}

		if (ActualValue < 0f) {
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
		DragDown();
	}

	internal void AddDragIfZeroAndMaxValue(List<Parameter> dragDownIfZero, Calculation maxValue) {
		DragDownIfZero = dragDownIfZero;
		MaxValue = maxValue;
	}

}