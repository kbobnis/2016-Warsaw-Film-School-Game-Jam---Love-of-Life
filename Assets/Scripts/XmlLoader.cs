using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;
using LOL;
using System.Xml.Linq;

internal class XmlLoader {

	internal static List<Parameter> LoadParameters(XmlDocument model) {

		XmlNode parametersXml = model.GetElementsByTagName("parameters")[0];

		//loading parameter ids
		List<Parameter> mainParameters = new List<Parameter>();
		List<string> parameterIds = new List<string>();
		float dragDownIfZeroPenalty = float.Parse(parametersXml.Attributes["dragDownIfZeroPenalty"].Value);
		foreach (XmlNode parameterXml in parametersXml.ChildNodes) {
			string id = parameterXml.Attributes["id"].Value;
			parameterIds.Add(id);
		}

		List<Parameter> parameters = new List<Parameter>();
		foreach (XmlNode parameterXml in parametersXml.ChildNodes) {
			string id = parameterXml.Attributes["id"].Value;

			if (!parameterXml.Check("maxValue")) {
				throw new Exception("You have to pass max value to parameter: " + id);
			}
			//Calculation maxValue = Calculation.from parameterXml.Attributes["maxValue"].Value
			float startValue = parameterXml.Check("startValue") ? float.Parse(parameterXml.Attributes["startValue"].Value) : 0;
			if (!parameterXml.Check("text")) {
				throw new Exception("There is no text in parameter " + id);
			}
			bool isMain = parameterXml.Check("isMain") ? parameterXml.Attributes["isMain"].Value == "true" : false;
			string text = parameterXml.Attributes["text"].Value;

			Parameter p = new Parameter(id, startValue, text, dragDownIfZeroPenalty, isMain);
			parameters.Add(p);
			if (p.IsMain) {
				mainParameters.Add(p);
			}
		}

		foreach (XmlNode parameterXml in parametersXml.ChildNodes) {
			string id = parameterXml.Attributes["id"].Value;
			Calculation maxValue = new Calculation(parameterXml.Attributes["maxValue"].Value, parameters);

			List<Parameter> dragDownIfZero = new List<Parameter>();
			dragDownIfZero.AddRange(mainParameters);
			if (parameterXml.Check("dragDownIfZero")) {
				string[] thisParameterIds = parameterXml.Attributes["dragDownIfZero"].Value.Split(',');

				foreach (string pTmp in thisParameterIds) {
					bool found = false;
					foreach (Parameter p in parameters) {
						if (p.Id == pTmp) {
							if (p.IsMain) {
								throw new Exception(p.Text + " is main parameter already. It will be dragged down if under zero.");
							}
							dragDownIfZero.Add(p);
							found = true;
						}
					}
					if (!found) {
						throw new Exception("DragDownIfZero has parameter (" + pTmp + ") but there is no such parameter.");
					}
				}
			}
			parameters.Find(t => t.Id == id).AddDragIfZeroAndMaxValue(dragDownIfZero, maxValue);
		}
		return parameters;
	}

	internal static List<Plot.Element> LoadPlot(XElement plotXml, List<Parameter> parameters, List<Situation> situations) {
		if (plotXml == null) {
			throw new Exception("A story must have at least one plot element.");
		}
		List<Plot.Element> plotElements = new List<Plot.Element>();
		foreach (XElement plotElementXml in plotXml.Elements()) {
			string text = plotElementXml.Attribute("text").Value;
			List<Plot.Element.Goal> plotGoals = LoadPlotGoals(plotElementXml.Elements("goal"), parameters);
			Schedule scheduleOverride = plotElementXml.Element("schedule")!=null?LoadSchedule(ToXmlElement( plotElementXml.Element("schedule") ), situations):null;
			plotElements.Add(new Plot.Element(text, plotGoals, scheduleOverride));
		}
		if (plotElements[plotElements.Count-1].Goals.Any(t => t is Plot.Element.DayNumberGoal)) {
			throw new Exception("You can not have any day number goal (only ParameterValueGoal)  in the last plot element. Because of high scores.");
		}
		return plotElements;
	}

	private static XmlNode ToXmlElement(XElement element) {
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(element.ToString());
		return doc.FirstChild;
	}

	private static List<Plot.Element.Goal> LoadPlotGoals(IEnumerable<XElement> plotGoals, List<Parameter> parameters) {
		List<Plot.Element.Goal> goals = new List<Plot.Element.Goal>();
		foreach (XElement goalXml in plotGoals) {
			if (goalXml.Name != "goal") {
				throw new Exception("There is something wrong with plot goal, it has wrong name: " + goalXml.Name);
			}
			//its paramId or dayNumber
			if (goalXml.Attribute("dayNumber") != null) {
				int dayNumber = int.Parse(goalXml.Attribute("dayNumber").Value);
				goals.Add(new Plot.Element.DayNumberGoal(dayNumber));
			} else if (goalXml.Attribute("paramId") != null) {
				string id = goalXml.Attribute("paramId").Value;
				Parameter p = parameters.FirstOrDefault(t => t.Id == id);
				if (p == null) {
					throw new Exception("There is no parameter with id: " + id);
				}
				float desiredValue = float.Parse(goalXml.Attribute("value").Value);
				goals.Add(new Plot.Element.ParameterValueGoal(p, desiredValue));
			} else {
				throw new Exception("You must define dayNumber or paramId.");
			}
		}
		return goals;
	}

	internal static Schedule LoadSchedule(XmlNode scheduleXml, List<Situation> situations) {
		try {
			Situation defaultSituation = null;
			if (scheduleXml.Check("default")) {
				defaultSituation = situations.FirstOrDefault(t => t.Id == scheduleXml.Attributes["default"].Value);
				if (defaultSituation == null) {
					throw new Exception("When loading default situation. There is no situaion with id " + scheduleXml.Attributes["default"].Value + ".");
				}
			}
			Schedule schedule = new Schedule(defaultSituation);
			foreach (XmlNode scheduledSituation in scheduleXml.ChildNodes) {
				int from = int.Parse(scheduledSituation.Attributes["from"].Value);
				int duration = int.Parse(scheduledSituation.Attributes["duration"].Value);
				bool isPermament = scheduledSituation.Check("isPermament") ? scheduledSituation.Attributes["isPermament"].Value == "true" : false;
				string situationId = scheduledSituation.Attributes["id"].Value;
				if (!situations.Any(t => t.Id == situationId)) {
					throw new Exception("There is no situation with id: " + situationId);
				}
				Situation situation = situations.First(t => t.Id == situationId);
				schedule.AddSituation(from, duration, situation, isPermament);

			}
			return schedule;
		} catch (Exception e) {
			throw new Exception("When loading schedule: " + e.Message);
		}
	}

	internal static TimeChanges LoadTime(XmlDocument model, List<Parameter> parameters) {
		XmlNode timeXml = model.GetElementsByTagName("time")[0];
		try {
			if (!timeXml.Check("normalSpeed")) {
				throw new Exception("There is no normalSpeed defined.");
			}
			float normalSpeed = float.Parse(timeXml.Attributes["normalSpeed"].Value);
			if (!timeXml.Check("fasterSpeed")) {
				throw new Exception("There is no fasterSpeed defined.");
			}
			float fasterSpeed = float.Parse(timeXml.Attributes["fasterSpeed"].Value);

			List<Change> timeChanges = LoadChanges(timeXml, parameters);

			return new TimeChanges(normalSpeed, fasterSpeed, timeChanges);

		} catch (Exception e) {
			throw new Exception("When loading time: " + e.Message);
		}

	}

	internal static List<Situation> LoadSituations(XmlDocument model, List<Parameter> parameters) {

		List<Situation> situations = new List<Situation>();
		foreach (XmlNode situationXml in model.GetElementsByTagName("situations")[0].ChildNodes) {
			string id = situationXml.Attributes["id"].Value;
			try {
				if (!situationXml.Check("text")) {
					throw new Exception("There is no text attribute.");
				}
				string text = situationXml.Attributes["text"].Value;
				bool selectable = situationXml.Check("selectable") ? (situationXml.Attributes["selectable"].Value == "false" ? false : true) : true;
				List<Change> changes = LoadChanges(situationXml, parameters);
				List<Button> buttons = LoadButtons(situationXml, parameters);
				situations.Add(new Situation(id, text, changes, selectable, buttons));
			} catch (Exception e) {
				throw new Exception("Exception in situation " + id + ", " + e.Message);
			}
		}
		return situations;
	}

	private static List<LOL.Button> LoadButtons(XmlNode situationXml, List<Parameter> parameters) {
		List<Button> buttons = new List<Button>();
		try {
			foreach (XmlNode buttonXml in situationXml.ChildNodes) {
				if (buttonXml.Name == "button") {
					List<Change> changes = new List<Change>();
					foreach (XmlNode changeXml in buttonXml.ChildNodes) {
						changes.Add(LoadChange(changeXml, parameters));
					}
					if (!buttonXml.Check("text")) {
						throw new Exception("Button must have text attribute");
					}
					string text = buttonXml.Attributes["text"].Value;

					buttons.Add(new Button(text, changes));
				}
			}
		} catch (Exception e) {
			throw new Exception("Exception when loading button. " + e.Message);
		}
		return buttons;
	}

	private static List<Change> LoadChanges(XmlNode situationXml, List<Parameter> parameters) {
		List<Change> changes = new List<Change>();
		try {
			foreach (XmlNode changeXml in situationXml.ChildNodes) {
				if (changeXml.Name == "change") {
					changes.Add(LoadChange(changeXml, parameters));
				}
			}
		} catch (Exception e) {
			throw new Exception("Exception when loading change. " + e.Message);
		}
		return changes;
	}

	private static Change LoadChange(XmlNode changeXml, List<Parameter> parameters) {
		if (changeXml.Name != "change") {
			throw new Exception("That is not a change: " + changeXml.Name + ". " + changeXml.InnerXml);
		}
		if (!changeXml.Check("what")) {
			throw new Exception("There is no 'what' attribute.");
		}
		Parameter what = parameters.FirstOrDefault(t => t.Id == changeXml.Attributes["what"].Value);
		if (what == null) {
			throw new Exception("There is no parameter with id: " + changeXml.Attributes["what"].Value);
		}
		Calculation valueCalculation = changeXml.Check("value") ? new Calculation(changeXml.Attributes["value"].Value, parameters) : null;
		Calculation maxValueCalculation = changeXml.Check("maxValue") ? new Calculation(changeXml.Attributes["maxValue"].Value, parameters) : null;
		if (valueCalculation != null && maxValueCalculation != null) {
			throw new Exception("You can only set one calculation, but you have set two. value and maxValue.");
		}
		float perTime = 1f;
		if (changeXml.Check("per")) {
			perTime = float.Parse(changeXml.Attributes["per"].Value);
		}

		return new Change(what, valueCalculation, maxValueCalculation, perTime);
	}

}

public static class XmlNodeExtensions {

	public static bool Check(this XmlNode s, string attribute) {
		return s.Attributes[attribute] != null;
	}

}