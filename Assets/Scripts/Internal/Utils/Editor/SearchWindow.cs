using UnityEditor;
using System;
using UnityEngine;

public class SearchWindow : EditorWindow
{
	private Action<int> _onSelectionCallback;
	private object[] _selectionItems;
	private string _searchString = "";
	private Vector2 _scrollPosition = Vector2.zero;

	public static SearchWindow OpenWindow(Action<int> onSelectionCallback, params object[] itemsToSelectFrom)
	{
		SearchWindow window = GetWindow<SearchWindow>();
		window.Setup(onSelectionCallback, itemsToSelectFrom);
		window.Show(true);
		window.Focus();
		window.Repaint();
		return window;
	}

	public void Setup(Action<int> onSelectionCallback, params object[] itemsToSelectFrom)
	{
		_scrollPosition = Vector2.zero;
		_onSelectionCallback = onSelectionCallback;
		_selectionItems = itemsToSelectFrom;
	}

	private void OnGUI()
	{
		// SearchBar
		GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
		GUI.SetNextControlName("SearchBar");
		_searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
		GUILayout.EndHorizontal();

		if(focusedWindow)
			GUI.FocusControl("SearchBar");

		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
		// Selection
		int selectedIndex = -1;
		int showCount = 0;
		for(int i = 0; i < _selectionItems.Length; i++)
		{
			if(MatchesSearch(_selectionItems[i], i))
			{
				GUIStyle s = new GUIStyle(GUI.skin.button);
				s.stretchWidth = true;
				s.alignment = TextAnchor.MiddleLeft;
				if(GUILayout.Button(string.Concat(i, ": ", _selectionItems[i].ToString()), s) || KeyboardSelection(showCount))
				{
					selectedIndex = i;
				}
				showCount++;
			}
		}

		GUILayout.EndScrollView();

		if(selectedIndex >= 0)
		{
			Action<int> callback = _onSelectionCallback;
			_onSelectionCallback = null;
			callback(selectedIndex);
		}
	}

	private bool KeyboardSelection(int showIndex)
	{
		return Event.current.keyCode == KeyCode.Return && showIndex == 0 && focusedWindow;
	}

	private bool MatchesSearch(object obj, int index)
	{
		if(string.IsNullOrEmpty(_searchString))
			return true;

		if(index.ToString().IndexOf(_searchString) >= 0)
			return true;

		string[] parts = _searchString.Split(' ');

		for(int i = 0; i < parts.Length; i++)
		{
			if(!obj.ToString().ToLower().Contains(parts[i].ToLower()))
			{
				return false;
			}
		}

		return true;

	}

	private void OnDestroy()
	{
		if(_onSelectionCallback != null)
		{
			_onSelectionCallback(-1);
			_onSelectionCallback = null;
		}

		_selectionItems = null;
	}
}
