using UnityEngine;
using System.Collections;
using UnityEditor;

public class DeletePlayerPrefs {
	[MenuItem("Tools/Clear PlayerPrefs")]
	private static void NewMenuOption() {
		PlayerPrefs.DeleteAll();
	}
}
