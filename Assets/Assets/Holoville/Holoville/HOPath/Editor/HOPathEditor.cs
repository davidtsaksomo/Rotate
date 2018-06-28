using System;
using System.Linq;
using Holoville.HOTween;
using Holoville.HOTween.Path;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CanEditMultipleObjects]
[CustomEditor(typeof(HOPath))]
public class HOPathEditor : Editor {
	private HOPath[] _targets;
	private bool _showHOTweenPathGizmos;

	private void OnEnable() {
		_targets = targets.Where(editTarget => editTarget as HOPath != null).Select(path => (HOPath) path).ToArray();
		_showHOTweenPathGizmos = HOTween.showPathGizmos;
		HOTween.showPathGizmos = true;
	}

	public HOPath First {
		get { return _targets[0]; }
	}

	private void OnDisable() {
		HOTween.showPathGizmos = _showHOTweenPathGizmos;
	}

	public override void OnInspectorGUI() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Relative", GUILayout.Width(50));
		bool newRelative = EditorGUILayout.Toggle(GUIContent.none, First.IsRelative, GUILayout.Width(20));
		if(newRelative != First.IsRelative) {
			RecordPaths("HOPath relative");
			Array.ForEach(_targets, path => path.IsRelative = newRelative);
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Closed", GUILayout.Width(45));
		bool newClosed = EditorGUILayout.Toggle(GUIContent.none, First.IsClosed, GUILayout.Width(20));
		if(newClosed != First.IsClosed) {
			RecordPaths("HOPath closed");
			Array.ForEach(_targets, path => path.IsClosed = newClosed);
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Type", GUILayout.Width(40));
		PathType newPathType = (PathType) EditorGUILayout.EnumPopup(GUIContent.none, First.PathType, GUILayout.Width(80));
		if(newPathType != First.PathType) {
			RecordPaths("HOPath path type");
			Array.ForEach(_targets, path => path.PathType = newPathType);
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();

		bool showPointEdit = true;
		if(_targets.Length > 1) {
			var pointCount = First.Points.Length;
			if(_targets.Any(t => t.Points.Length != pointCount)) {
				EditorGUILayout.HelpBox("Multi-Object Editing: Paths must have the same number of points.", MessageType.Info);
				showPointEdit = false;
			}
		}

		if(showPointEdit) {
			for(var i = 0; i < First.Points.Length; i++) {
				// draw the node and remove button
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Node " + i, GUILayout.Width(60));
				Vector3 newNode = EditorGUILayout.Vector3Field(GUIContent.none, First.Points[i]);
				if(newNode != First.Points[i]) {
					RecordPaths("HOPath change point");
					Array.ForEach(_targets, path => path.Points[i] = newNode);
				}
				if(First.Points.Length > 2 && GUILayout.Button("Remove", GUILayout.Width(60))) {
					RecordPaths("HOPath remove point");
					Array.ForEach(_targets, path => path.RemovePoint(i));
				}
				EditorGUILayout.EndHorizontal();
				// draw the add new node and swap nodes button
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(15))) {
					RecordPaths("HOPath add point");
					Array.ForEach(_targets, path => path.AddPoint(i + 1));
				}
				if(i < First.Points.Length - 1 && GUILayout.Button("swap", GUILayout.Width(60), GUILayout.Height(15))) {
					RecordPaths("HOPath swap points");
					Array.ForEach(_targets, path => {
						var t = path.Points[i];
						path.Points[i] = path.Points[i + 1];
						path.Points[i + 1] = t;
					});
				}
				EditorGUILayout.EndHorizontal();
			}
		}


		//update and redraw:
		if(GUI.changed) {
			Array.ForEach(_targets, EditorUtility.SetDirty);
		}
	}

	private void OnSceneGUI() {
		if(Application.isPlaying) return;
		foreach(var currentPath in _targets) {
			currentPath.DrawPath();

			if(!currentPath.enabled || currentPath.Points.Length < 2) return;
			Handles.color = new Color(1f, .3f, .3f, .6f);

			// node positions in the preferred space
			var handlePositions = !currentPath.IsRelative ? currentPath.Points : currentPath.PathPointsInternal;
			var relativeAdjustment = currentPath.IsRelative ? 1 : 0;

			var handleLabel = new GUIStyle();
			handleLabel.fontSize = 15;
			handleLabel.fontStyle = FontStyle.Bold;
			handleLabel.normal.textColor = new Color(.9f, .9f, .9f, .9f);

			for(var i = 0; i < currentPath.Points.Length; i++) {
				Handles.Label(handlePositions[i + relativeAdjustment], " " + i, handleLabel);
			}

			//node handle display:
			for(var i = 0 + relativeAdjustment; i < currentPath.Points.Length + relativeAdjustment; i++) {
				var newPosition = Handles.PositionHandle(handlePositions[i], Quaternion.identity);
				if(!currentPath.IsRelative) {
					RecordPath(currentPath, "HOPath change point");
					currentPath.Points[i] = newPosition;
				}
				else {
					RecordPath(currentPath, "HOPath change point");
					currentPath.Points[i - relativeAdjustment] = newPosition + currentPath.RelativeOffset;
				}
			}
		}
	}

	private void RecordPaths(string action) {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		Undo.RegisterUndo(path, objectName);
#else
		Undo.RecordObjects(_targets, action);
#endif
	}

	private void RecordPath(Object path, string action) {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		Undo.RegisterUndo(path, objectName);
#else
		Undo.RecordObject(path, action);
#endif
	}
}