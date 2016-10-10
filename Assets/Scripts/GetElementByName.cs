using System;
using UnityEngine;

public static class GameObjectExtension  {

	public static T FindByName<T>(this GameObject go, string name) where T : Component{
		foreach (Transform t in go.GetComponentsInChildren<Transform>(true)) {
			if (t.gameObject.name == name) {
				return t.GetComponent<T>();
			}
		}
		throw new Exception("Child " + name + " not found in " + go.name);
	}

}
