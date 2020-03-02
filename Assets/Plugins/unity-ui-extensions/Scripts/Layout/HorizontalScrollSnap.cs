/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions {

	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Layout/Extensions/Horizontal Scroll Snap")]
	public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {
		private Transform _container;

		private int _screens {
			get {
				return _container.transform.childCount;
			}
		}

		private bool _fastSwipeTimer = false;
		private int _fastSwipeCounter = 0;
		private int _fastSwipeTarget = 30;

		private List<Vector3> _positions {
			get {
				List<Vector3> positions = new List<Vector3>();
				for (int i = 0; i < _container.transform.childCount; i++) {
					RectTransform child = _container.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
					positions.Add(-child.localPosition);
				}
				return positions;
			}
		}
		private ScrollRect _scroll_rect;
		private Vector3 _lerp_target;
		private bool _lerp;

		[Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
		public GameObject Pagination;

		[Tooltip("Transition speed between pages. (optional)")]
		public float transitionSpeed = 7.5f;

		public Boolean UseFastSwipe = true;
		public int FastSwipeThreshold = 100;

		private bool _startDrag = true;
		private Vector3 _startPosition = new Vector3();

		[Tooltip("The currently active page")]
		[SerializeField]
		private int _currentScreen;

		[Tooltip("The screen / page to start the control on")]
		public int StartingScreen = 1;

		private bool fastSwipe = false; //to determine if a fast swipe was performed

		// Use this for initialization
		void Start() {
			_scroll_rect = gameObject.GetComponent<ScrollRect>();
			if (_scroll_rect.horizontalScrollbar || _scroll_rect.verticalScrollbar) {
				throw new Exception("Warning, using scrollbors with the Scroll Snap controls is not advised as it causes unpredictable results");
			}

			_container = _scroll_rect.content;
			DistributePages();
			_lerp = false;
			_currentScreen = StartingScreen;
			_container.localPosition = _positions[_currentScreen-1];
			ChangeBulletsInfo(_currentScreen);
		}

		void Update() {
			if (_lerp) {
				_container.localPosition = Vector3.Lerp(_container.localPosition, _lerp_target, transitionSpeed * Time.deltaTime);
				if (Vector3.Distance(_container.localPosition, _lerp_target) < 0.005f) {
					_lerp = false;
				}

				//change the info bullets at the bottom of the screen. Just for visual effect
				if (Vector3.Distance(_container.localPosition, _lerp_target) < 10f) {
					ChangeBulletsInfo(CurrentScreen());
				}
			}

			if (_fastSwipeTimer) {
				_fastSwipeCounter++;
			}
		}

		//Function for switching to a specific screen
		public void GoToScreen(int screenIndex) {
			if (screenIndex <= _screens && screenIndex >= 0) {
				_lerp = true;
				_lerp_target = _positions[screenIndex];

				ChangeBulletsInfo(screenIndex);
			}
		}

		private void NextScreenCommand() {
			if (_currentScreen < _screens - 1) {
				_lerp = true;
				_lerp_target = _positions[_currentScreen + 1];

				ChangeBulletsInfo(_currentScreen + 1);
			}
		}

		private void PrevScreenCommand() {
			if (_currentScreen > 0) {
				_lerp = true;
				_lerp_target = _positions[_currentScreen - 1];

				ChangeBulletsInfo(_currentScreen - 1);
			}
		}

		//find the closest registered point to the releasing point
		private Vector3 FindClosestFrom(Vector3 start) {
			Vector3 closest = Vector3.zero;
			float distance = Mathf.Infinity;

			foreach (Vector3 position in _positions) {
				if (Vector3.Distance(start, position) < distance) {
					distance = Vector3.Distance(start, position);
					closest = position;
				}
			}
			return closest;
		}

		//returns the current screen that the is seeing
		public int CurrentScreen() {
			Vector3 pos = FindClosestFrom(_container.localPosition);
			return _currentScreen = GetPageforPosition(pos);
		}

		//changes the bullets on the bottom of the page - pagination
		private void ChangeBulletsInfo(int currentScreen) {
			if (Pagination)
				for (int i = 0; i < Pagination.transform.childCount; i++) {
					Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (currentScreen == i)
						? true
						: false;
				}
		}

		//used for changing between screen resolutions
		private void DistributePages() {
			Vector2 size = _container.transform.GetChild(0).GetComponent<RectTransform>().GetSize();
			_container.GetComponent<RectTransform>().SetSize(Vector2.Scale(_container.parent.GetComponent<RectTransform>().GetSize(), new Vector2(_container.transform.childCount, 1)));
			int pageStep = (int)_scroll_rect.GetComponent<RectTransform>().rect.width;

			for (int i = 0; i < _container.transform.childCount; i++) {
				RectTransform child = _container.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
				child.SetSize(size);
				child.localPosition = new Vector3(i * pageStep, 0);
			}
		}

		int GetPageforPosition(Vector3 pos) {
			int page = 0;
			for (int i = 0; i < _positions.Count; i++) {
				if (_positions[i] == pos) {
					page = i;
				}
			}
			return page;
		}
		void OnValidate() {
			var childCount = gameObject.GetComponent<ScrollRect>().content.childCount;
			if (StartingScreen > childCount) {
				StartingScreen = childCount;
			}
			if (StartingScreen < 1) {
				StartingScreen = 1;
			}
		}

		#region Interfaces
		public void OnBeginDrag(PointerEventData eventData) {
			_startPosition = _container.localPosition;
			_fastSwipeCounter = 0;
			_fastSwipeTimer = true;
			_currentScreen = CurrentScreen();
		}

		public void OnEndDrag(PointerEventData eventData) {
			_startDrag = true;
			if (_scroll_rect.horizontal) {
				if (UseFastSwipe) {
					fastSwipe = false;
					_fastSwipeTimer = false;
					if (_fastSwipeCounter <= _fastSwipeTarget) {
						if (Math.Abs(_startPosition.x - _container.localPosition.x) > FastSwipeThreshold) {
							fastSwipe = true;
						}
					}
					if (fastSwipe) {
						if (_container.localPosition.x < _startPosition.x) {
							NextScreenCommand();
						} else {
							PrevScreenCommand();
						}
					} else {
						_lerp = true;
						_lerp_target = FindClosestFrom(_container.localPosition);
						_currentScreen = GetPageforPosition(_lerp_target);
					}
				} else {
					_lerp = true;
					_lerp_target = FindClosestFrom(_container.localPosition);
					_currentScreen = GetPageforPosition(_lerp_target);
				}
			}
		}

		public void OnDrag(PointerEventData eventData) {
			_lerp = false;
			if (_startDrag) {
				OnBeginDrag(eventData);
				_startDrag = false;
			}
		}
		#endregion


	}
}