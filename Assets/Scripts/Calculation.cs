using System;
using System.Collections.Generic;
using System.Linq;

public class Calculation {
	private string Value;
	private List<Element> Elements;

	public override string ToString() {
		return Value;
	}

	public Calculation(string value, List<Parameter> parameters) {
		Value = value;

		try {
			string[] stringElements = Value.Trim().Split(' ');
			List<Element> elements = stringElements.Select(t => Element.FromString(t, parameters)).ToList();

			//there has to be 1, 3, 5, 7, 9, ... elements
			if (elements.Count % 2 != 1) {
				throw new Exception("Wrong number of elements, there has to be 3, 5, 7, 9, ...");
			}
			//1, 3, 5, 7, 9 element has to be number of parameter
			for (int i = 0; i < elements.Count; i += 2) {
				if (!(elements[i] is ElementNumber) && !(elements[i] is ElementParameter) && !(elements[i] is ElementFunction)) {
					throw new Exception("Element number " + (i + 1) + " has to be a number or parameter and is " + elements[i].GetType().Name);
				}
			}
			//2, 4, 6, 8, ... element has to be a sign
			for (int i = 1; i < elements.Count; i += 2) {
				if (!(elements[i] is ElementSign)) {
					throw new Exception("Element number " + (i + 1) + " has to be a sign and is " + elements[i].GetType().Name);
				}
			}
			Elements = elements;
		} catch (Exception e) {
			throw new Exception("Exception thrown when parsing " + value + " " + e.Message);
		}
	}

	internal float Calculate(float? timeDelta = null) {

		int i = 0;
		float res = Elements[i].GetValue();
		while (i + 2 < Elements.Count) {
			if ((Elements[i + 1] as ElementSign).SignType == ElementSign.SignTypeEnum.Add) {
				res += Elements[i + 2].GetValue();
			}
			if ((Elements[i + 1] as ElementSign).SignType == ElementSign.SignTypeEnum.Mult) {
				res *= Elements[i + 2].GetValue();
			}
			i += 2;
		}
		//add pairs
		return res * (timeDelta != null ? timeDelta.Value : 1);
	}

	class ElementNumber : Element {
		private float Value;

		public ElementNumber(float n) {
			Value = n;
		}

		public override float GetValue() {
			return Value;
		}
	}

	class ElementSign : Element {

		public SignTypeEnum SignType = SignTypeEnum.Add;

		public enum SignTypeEnum {
			Mult, Add
		}

		public ElementSign(string t) {
			switch (t) {
				case "*": SignType = SignTypeEnum.Mult; break;
				case "+": SignType = SignTypeEnum.Add; break;
				default:
					throw new Exception("There is no sign type for string " + t);
			}
		}

		public override float GetValue() {
			throw new Exception("You don't get value from sign.");
		}
	}

	class ElementParameter : Element {
		public readonly Parameter P;

		public ElementParameter(Parameter p) {
			P = p;
		}

		public override float GetValue() {
			if (P.ActualValue <= 0) {
				P.IsUsedAndIsZero = true;
			}

			return P.ActualValue;
		}
	}

	class ElementFunction : Element {

		private FunctionClass Function;
		private Parameter Parameter;

		public ElementFunction(string function, Parameter p) : base() {
			switch (function) {
				case "getOne":
					Function = FunctionClass.GetOneFunction;
					break;
				case "maxValue":
					Function = FunctionClass.MaxValueFunction;
					break;
				default:
					throw new Exception("There is no function with id: " + function);
			}
			Parameter = p;
		}

		public override float GetValue() {
			return Function.GetValue(Parameter);
		}


		abstract class FunctionClass {
			public static FunctionClass GetOneFunction = new GetOneFunction();
			public static FunctionClass MaxValueFunction = new MaxValueFunction();

			public abstract float GetValue(Parameter p);
		}

		class GetOneFunction : FunctionClass {
			public override float GetValue(Parameter p) {
				return p.ActualValue > 0 ? 1 : 0;
			}
		}

		class MaxValueFunction : FunctionClass {
			public override float GetValue(Parameter p) {
				return p.MaxValue.Calculate() * p.ActualMaxValueMultiplier;
			}
		}
	}

	abstract class Element {

		public abstract float GetValue();

		internal static Element FromString(string t, List<Parameter> parameters) {

			float n;
			bool isNumeric = float.TryParse(t, out n);
			if (isNumeric) {
				return new ElementNumber(float.Parse(t));
			}

			//check if element is a parameter
			foreach (Parameter p in parameters) {
				if (t == p.Id) {
					return new ElementParameter(p);
				}
			}

			if (t == "*" || t == "+") {
				return new ElementSign(t);
			}

			//check if element is a function over parameter
			string[] elements = t.Split('-');
			if (elements.Length == 2) {
				Parameter p = parameters.FirstOrDefault(k => k.Id == elements[1]);
				if (p == null) {
					throw new Exception("You have defined a function " + t + ", but there is no parameter with id " + elements[1]);
				}
				return new ElementFunction(elements[0], p);
			}

			throw new Exception("Unrecognized element: " + t);

		}
	}
}