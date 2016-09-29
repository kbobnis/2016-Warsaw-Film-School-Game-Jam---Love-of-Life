﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class UGuiTools : MonoBehaviour {

	[MenuItem("uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners() {
		var selectedTransforms = Selection.GetTransforms(SelectionMode.TopLevel);
		foreach (Transform activeTransform in selectedTransforms) {
			RectTransform t = activeTransform as RectTransform;
			RectTransform pt = activeTransform.parent as RectTransform;

			if (t == null || pt == null) return;

			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
												t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
												t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}

	[MenuItem("uGUI/AnchorsToCenter")]
	static void anchorsToCenter() {
		RectTransform t = Selection.activeTransform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null) return;

		Rect pRect = pt.rect;
		Rect rect = t.rect;
		var pos = (Vector2)t.localPosition - pRect.position;
		var newPos = new Vector2(pos.x / pRect.width, pos.y / pRect.height);

		t.anchorMax = t.anchorMin = newPos;
		t.pivot = new Vector2(0.5f, 0.5f);
		t.SetSize(rect.size);
		t.anchoredPosition = Vector2.zero;
	}

	[MenuItem("uGUI/Fill")]
	static void Fill() {
		RectTransform t = Selection.activeTransform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null) return;

		t.anchorMin = Vector2.zero;
		t.anchorMax = Vector2.one;
		t.offsetMin = t.offsetMax = Vector2.zero;
	}

	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors() {
		RectTransform t = Selection.activeTransform as RectTransform;

		if (t == null) return;

		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}
}