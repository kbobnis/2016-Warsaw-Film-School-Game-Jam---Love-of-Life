using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;

internal class XmlLoader {
	
internal static List<Parameter> LoadParameters(XmlDocument model) {

		List<Parameter> parameters = new List<Parameter>();
		XmlNodeList parametersXml = model.GetElementsByTagName("parameters")[0].ChildNodes;
		foreach(XmlNode parameterXml in parametersXml) {
			string id = parameterXml.Attributes["id"].Value;
			float? maxValue = parameterXml.Check("maxValue") ? float.Parse(parameterXml.Attributes["maxValue"].Value) : default(float?);
			float startValue = parameterXml.Check("startValue") ? float.Parse(parameterXml.Attributes["startValue"].Value) : 0;
			parameters.Add(new Parameter(id, maxValue, startValue));
		}
		return parameters;
	}

	internal static Schedule LoadSchedule(XmlDocument model, List<Situation> situations) {
		Schedule schedule = new Schedule();
		foreach(XmlNode scheduledSituation in model.GetElementsByTagName("schedule")[0].ChildNodes) {
			int from = int.Parse( scheduledSituation.Attributes["from"].Value) ;
			int duration = int.Parse( scheduledSituation.Attributes["duration"].Value);
			Situation situation = situations.First(t => t.Id == scheduledSituation.Attributes["id"].Value);
			schedule.AddSituation(from, duration, situation, true);
		}
		return schedule;
	}

	internal static List<Situation> LoadSituations(XmlDocument model, List<Parameter> parameters) {

		List<Situation> situations = new List<Situation>();
		foreach(XmlNode situationXml in model.GetElementsByTagName("situations")[0].ChildNodes) {
			string id = situationXml.Attributes["id"].Value;
			string text = situationXml.Attributes["text"].Value;
			bool selectable = situationXml.Check("selectable") ? (situationXml.Attributes["selectable"].Value == "false" ? false : true) : true;
			List<Change> changes = LoadChanges(situationXml, parameters);
			List<Event> events = LoadEvents(situationXml, parameters);
			situations.Add(new Situation(id, text, changes, events, selectable));
		}
		return situations;
	}

	private static List<Event> LoadEvents(XmlNode situationXml, List<Parameter> parameters) {
		List<Event> events = new List<Event>();
		foreach(XmlNode eventXml in situationXml.ChildNodes) {
			if (eventXml.Name == "event") {
				string id = eventXml.Attributes["id"].Value;
				string text = eventXml.Attributes["text"].Value;
				string maxTime = eventXml.Attributes["maxTime"].Value;
				string depleteText = eventXml.Check("depleteText")? eventXml.Attributes["depleteText"].Value:null;
				bool canBeInterrupted = eventXml.Check("canBeInterrupted") ? eventXml.Attributes["canBeInterrupted"].Value=="true": true;
				string interruptText = canBeInterrupted?eventXml.Attributes["interruptText"].Value:null;
				List<Change> changes = LoadChanges(eventXml, parameters);
				events.Add(new Event(id, text, maxTime, depleteText, interruptText, changes, canBeInterrupted));
			}
		}
		return events;
	}

	private static List<Change> LoadChanges(XmlNode situationXml, List<Parameter> parameters) {
		List<Change> changes = new List<Change>();
		try {
			foreach (XmlNode changeXml in situationXml.ChildNodes) {
				if (changeXml.Name == "change") {
					Parameter what = parameters.First(t => t.Id == changeXml.Attributes["what"].Value);
					Calculation valueCalculation = changeXml.Check("value")?new Calculation(changeXml.Attributes["value"].Value):null;
					Calculation maxValueCalculation = changeXml.Check("maxValue") ? new Calculation(changeXml.Attributes["maxValue"].Value) : null;
					if (valueCalculation != null && maxValueCalculation != null) {
						throw new Exception("You can only set one calculation, but you have set two. value and maxValue.");
					}
					changes.Add(new Change(what, valueCalculation, maxValueCalculation));
				}
			}
		} catch(NullReferenceException e) {
			throw new Exception("Exception when loading change in situation " + situationXml.Name + ", " + situationXml.Attributes["id"].Value + ". " + e.Message);
		}
		return changes;
	}
}


public static class XmlNodeExtensions {

	public static bool Check(this XmlNode s, string attribute) {
		return s.Attributes[attribute] != null;
	}

}