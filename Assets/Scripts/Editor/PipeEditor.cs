using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Pipe))]
public class PipeEditor : PVCItemEditor
{
	protected override void CustomInspectorGUI()
	{
		//Do the normal inspector GUI for the item's fields.
		//If any of the mouths change, update the object.
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
			((Pipe)target).RebuildMouthList();

		GUILayout.Space(20.0f);
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnRecompiled()
	{
		foreach (var pipe in FindObjectsOfType<Pipe>())
			pipe.RebuildMouthList();
	}
}