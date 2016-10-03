
using System;
using UnityEngine;

public static class GameObjectExtension {

	public static GameObject GetElementByName(this GameObject go, string name) {
		foreach (Transform t in go.GetComponentsInChildren<Transform>()) {
			if (t.gameObject.name == name) {
				return t.gameObject;
			}
		}
		throw new Exception("Child " + name + " not found in " + go.name);
	}
	
	public static T GetElementByName<T>(this GameObject go, string name) {
		return go.GetElementByName(name).GetComponent<T>();
	}

}
