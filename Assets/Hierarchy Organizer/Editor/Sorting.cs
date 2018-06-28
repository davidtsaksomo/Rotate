using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Hierarchy Organizer v3.0
/// By Isaiah Kelly 
/// @IsaiahKelly on Twitter
/// isaiah@dangerofdeath.com
/// </summary>

public class Sorting : EditorWindow
{
	[SerializeField] enum ORDER {Ascending, Descending}		// Sorting order options.
	static ORDER sortOrder = ORDER.Ascending;				// The current sort order.
	static bool autoSort = false;							// Automatically sort the hierarchy.
	static bool justSorted = false;							// Way to check if we have already sorted to prevent endless sorting loops.


	[MenuItem("Window/Sorting")]
	public static void ShowWindow ()
	{
		// Show existing window instance or make one.
		EditorWindow.GetWindow (typeof(Sorting));
	}
	
	
	void OnGUI () 
	{
		GUILayout.Label ("Hierarchy Organizer", EditorStyles.boldLabel);

		if (GUILayout.Button ("Sort All Hierarchy"))
			EditorApplication.ExecuteMenuItem ("Tools/Sort Hierarchy");

		if (GUILayout.Button ("Sort Children of Selected"))
			EditorApplication.ExecuteMenuItem ("GameObject/Sort Children");

		GUILayout.Space (8);
		sortOrder = (ORDER)EditorGUILayout.EnumPopup("Sort Order", sortOrder);
		GUILayout.Space (8);
		autoSort = EditorGUILayout.Toggle ("Automatic Sorting", autoSort);
		GUILayout.Space (8);
		EditorGUILayout.HelpBox ("Automatic sorting may cause serious lag if you have a lot of objects in the scene.  Use with caution!", MessageType.Info);
	}


	[MenuItem ("GameObject/Move Up %[")] 
	static void MoveUp ()
	{
		// Get selected transform.
		Transform t = Selection.activeTransform;

		int index = t.GetSiblingIndex ();
		if (index != 0)
		    t.SetSiblingIndex (index -1);
	}


	// Used to validate the menu item defined by the function above. 
	[MenuItem ("GameObject/Move Up %[", true)] 
	static bool ValidateMoveUp ()
	{
		// Return false if auto sort mode is on.
		if (autoSort)
			return false;

		// Return false if no transform is selected. 
		return Selection.activeTransform != null; 
	}


	[MenuItem ("GameObject/Move Down %]")] 
	static void MoveDown ()
	{
		// Get selected transform.
		Transform t = Selection.activeTransform;
		
		int index = t.GetSiblingIndex ();
		t.SetSiblingIndex (index +1);
	}


	// Used to validate the menu item defined by the function above. 
	[MenuItem ("GameObject/Move Down %]", true)] 
	static bool ValidateMoveDown ()
	{
		// Return false if auto sort mode is on.
		if (autoSort)
			return false;

		// Return false if no transform is selected. 	
		return Selection.activeTransform != null; 
	}


	[MenuItem ("GameObject/Sort Children %'")] 
	static void SortSelected ()
	{
		// Get selected transforms.
		Transform[] transforms = Selection.GetTransforms (SelectionMode.Deep | SelectionMode.Editable);
		
		int newIndex = GetLowestIndex (transforms);
		
		// Sort the transforms by name.
		if (sortOrder == ORDER.Ascending)  // Sort in ascending order.
		{
			foreach (Transform t in transforms.Cast<Transform>().OrderBy (t=>t.name))
			{
				if (t != t.root)
				{
					t.SetSiblingIndex (newIndex);
					newIndex ++;
				}
			}
		}
		else // Sort in descending order.
		{
			foreach (Transform t in transforms.Cast<Transform>().OrderByDescending (t=>t.name))
			{
				if (t != t.root)
				{
					t.SetSiblingIndex (newIndex);
					newIndex ++;
				}
			}
		}

		Debug.Log ("Children Sorted.");
	}
	
	
	// Used to validate the menu item defined by the function above. 
	[MenuItem ("GameObject/Sort Children %'", true)] 
	static bool ValidateSortSelected ()
	{
		// Return false if auto sort mode is on.
		if (autoSort)
			return false;

		// Return false if no transform is selected. 
		return Selection.activeTransform != null; 
	}
	
	
	[MenuItem ("Tools/Sort Hierarchy")] 
	static void SortHierarchy ()
	{
		// Get all transforms.
		Transform[] transforms = FindObjectsOfType (typeof(Transform)) as Transform[];
		
		int newIndex = 0;
		
		// Sort the transforms by name.
		if (sortOrder == ORDER.Ascending)  // Sort in ascending order.
		{
			foreach (Transform t in transforms.Cast<Transform>().OrderBy (t=>t.name))
			{
				t.SetSiblingIndex (newIndex);
				newIndex ++;
			}
		}
		else // Sort in descending order.
		{
			foreach (Transform t in transforms.Cast<Transform>().OrderByDescending (t=>t.name))
			{
				t.SetSiblingIndex (newIndex);
				newIndex ++;
			}
		}

		Debug.Log ("Hierarchy Sorted.");
		justSorted = true;
	}

	
	static int GetLowestIndex (Transform[] t) 
	{
		int lowestIndex = 9999;
		int index;
		
		for (int i = 0; i < t.Length; i++) 
		{
			index = t[i].GetSiblingIndex();
			
			if (index < lowestIndex) 
			{
				lowestIndex = index;
			}
		}
		
		return lowestIndex;
	}


	void OnHierarchyChange () 
	{
		if (!autoSort)
			return;

		if (justSorted)
		{
			justSorted = false;
		}
		else
		{
			SortHierarchy ();
		}
	}

}
