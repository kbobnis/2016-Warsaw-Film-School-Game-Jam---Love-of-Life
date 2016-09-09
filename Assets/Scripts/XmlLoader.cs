using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;

internal class XmlLoader {

	internal static List<Parameter> LoadParameters(XmlDocument model) {

		List<Parameter> parameters = new List<Parameter>();
		XmlNode parametersXml = model.GetElementsByTagName("parameters")[0];
		float dragDownIfZeroPenalty = float.Parse( parametersXml.Attributes["dragDownIfZeroPenalty"].Value );
		foreach (XmlNode parameterXml in parametersXml.ChildNodes) {
			string id = parameterXml.Attributes["id"].Value;
			float? maxValue = parameterXml.Check("maxValue") ? float.Parse(parameterXml.Attributes["maxValue"].Value) : default(float?);
			float startValue = parameterXml.Check("startValue") ? float.Parse(parameterXml.Attributes["startValue"].Value) : 0;
			bool zeroEndsGame = parameterXml.Check("zeroEndsGame") ? parameterXml.Attributes["zeroEndsGame"].Value == "true" : false;
			if (!parameterXml.Check("text")) {
				throw new Exception("There is no text in parameter " + id);
			}
			string text = parameterXml.Attributes["text"].Value;
			parameters.Add(new Parameter(id, maxValue, startValue, text, zeroEndsGame, dragDownIfZeroPenalty));
		}

		//drag down if zero loading
		foreach(XmlNode parameterXml in parametersXml) {
			Parameter actualParam = parameters.FirstOrDefault(t => t.Id == parameterXml.Attributes["id"].Value);
			List<Parameter> dragDownIfZero = new List<Parameter>();
			if (parameterXml.Check("dragDownIfZero")) {
				string[] parameterIds = parameterXml.Attributes["dragDownIfZero"].Value.Split(',');

				foreach (string pTmp in parameterIds) {
					bool found = false;
					foreach (Parameter p in parameters) {
						if (p.Id == pTmp) {
							dragDownIfZero.Add(p);
							found = true;
						}
					}
					if (!found) {
						throw new Exception("DragDownIfZero has parameter (" + pTmp + ") but there is no such parameter.");
					}
				}
			}
			actualParam.AddDragDownIfZero(dragDownIfZero);
		}
		return parameters;
	}

	internal static Schedule LoadSchedule(XmlDocument model, List<Situation> situations) {
		XmlNode scheduleXml = model.GetElementsByTagName("schedule")[0];
		Situation defaultSituation = situations.FirstOrDefault(t => t.Id == scheduleXml.Attributes["default"].Value);
		if (defaultSituation == null) {
			throw new Exception("When loading default situation. There is no situaion with id " + scheduleXml.Attributes["default"].Value + ".");
		}
		Schedule schedule = new Schedule(defaultSituation);
		foreach (XmlNode scheduledSituation in model.GetElementsByTagName("schedule")[0].ChildNodes) {
			int from = int.Parse(scheduledSituation.Attributes["from"].Value);
			int duration = int.Parse(scheduledSituation.Attributes["duration"].Value);
			bool isPermament = scheduledSituation.Check("isPermament") ? scheduledSituation.Attributes["isPermament"].Value == "true" : false;
			Situation situation = situations.First(t => t.Id == scheduledSituation.Attributes["id"].Value);
			schedule.AddSituation(from, duration, situation, isPermament);
			
		}
		return schedule;
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

		} catch(Exception e) {
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
				List<Event> events = LoadEvents(situationXml, parameters);
				situations.Add(new Situation(id, text, changes, events, selectable));
			} catch (Exception e) {
				throw new Exception("Exception in situation " + id + ", " + e.Message);
			}
		}
		return situations;
	}

	private static List<Event> LoadEvents(XmlNode situationXml, List<Parameter> parameters) {
		List<Event> events = new List<Event>();
		foreach (XmlNode eventXml in situationXml.ChildNodes) {
			if (eventXml.Name == "event") {
				try {
					string id = eventXml.Attributes["id"].Value;
					string text = eventXml.Attributes["text"].Value;
					string maxTime = eventXml.Attributes["maxTime"].Value;
					string depleteText = eventXml.Check("depleteText") ? eventXml.Attributes["depleteText"].Value : null;
					bool canBeInterrupted = eventXml.Check("canBeInterrupted") ? eventXml.Attributes["canBeInterrupted"].Value == "true" : true;
					if (canBeInterrupted && !eventXml.Check("interruptText")) {
						throw new Exception("Event can be interrupted, but there is no interruptText");
					}
					string interruptText = canBeInterrupted ? eventXml.Attributes["interruptText"].Value : null;
					List<Change> changes = LoadChanges(eventXml, parameters);
					events.Add(new Event(id, text, maxTime, depleteText, interruptText, changes, canBeInterrupted));
				} catch (Exception e) {
					throw new Exception("Exception during loading event " + eventXml.Attributes["id"].Value + ", " + e.Message);
				}
			}
		}
		return events;
	}

	private static List<Change> LoadChanges(XmlNode situationXml, List<Parameter> parameters) {
		List<Change> changes = new List<Change>();
		try {
			foreach (XmlNode changeXml in situationXml.ChildNodes) {
				if (changeXml.Name == "change") {
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

					changes.Add(new Change(what, valueCalculation, maxValueCalculation, perTime));
				}
			}
		} catch (Exception e) {
			throw new Exception("Exception when loading change. " + e.Message);
		}
		return changes;
	}
}


public static class XmlNodeExtensions {

	public static bool Check(this XmlNode s, string attribute) {
		return s.Attributes[attribute] != null;
	}

}