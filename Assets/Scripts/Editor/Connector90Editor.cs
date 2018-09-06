using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Connector90))]
public class Connector90Editor : PVCItemEditor
{
	protected override void CustomInspectorGUI()
	{
		//Do the normal inspector GUI for the item's fields.
		//If any of the mouths change, update the object.
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
			((Connector90)target).RebuildMouthList();
	}
}