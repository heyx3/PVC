using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


//TODO: Connected pieces are siblings in the hierarchy. When connecting two things, merge their siblings to all be one big family.


[CustomEditor(typeof(PVCItem))]
public class PVCItemEditor : Editor
{
	public static PVCItem ItemBeingConnected { get; private set; }
	private static int ConnectIndex = 1;


	public override void OnInspectorGUI()
	{
		if (targets.Length != 1)
		{
			GUILayout.Label("Can only edit one item at a time!");
			return;
		}

		var myItem = (PVCItem)target;
		if (myItem == ItemBeingConnected)
		{
			GUILayout.Label("Click the mouth to connect to in the Scene view");
			if (GUILayout.Button("Cancel"))
				ItemBeingConnected = null;
		}
		UnityEngine.Assertions.Assert.IsNull(ItemBeingConnected,
											 "Other item is still being connected??");

		//Do the GUI for each item (i.e. ask to connect or disconnect).
		for (int i = 0; i < myItem.Mouths.Count; ++i)
		{
			var mouth = myItem.Mouths[i];
			if (mouth.IsConnected)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Disconnect " + mouth.Name))
					mouth.OtherItem = null;
				if (GUILayout.Button("Reconnect " + mouth.Name))
				{
					mouth.OtherItem = null;
					ItemBeingConnected = myItem;
					ConnectIndex = i;
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				if (GUILayout.Button("Connect " + mouth.Name))
				{
					ItemBeingConnected = myItem;
					ConnectIndex = i;
				}
			}
		}
	}
	private void OnSceneGUI()
	{
		if (targets.Length != 1)
			return;

		var myItem = (PVCItem)target;
		if (myItem != ItemBeingConnected)
			return;

		var mouth = myItem.Mouths[ConnectIndex];

		//Draw a sphere around the end being connected.
		Gizmos.color = new Color(0.25f, 1.0f, 0.25f, 0.325f);
		Gizmos.DrawSphere(mouth.MyTr.position, 1.0f);

		//See if a compatible item is being moused over.
		var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity,
							myItem.CompatibleObjects))
		{
			var selectedPvcItem = hit.collider.GetComponentInParent<PVCItem>();
			UnityEngine.Assertions.Assert.IsNotNull(selectedPvcItem,
													"Hit something that wasn't a PVC item??");

			int selectedMouthI = -1;
			for (int i = 0; i < selectedPvcItem.Mouths.Count; ++i)
				if (hit.transform.IsChildOf(selectedPvcItem.Mouths[i].MyTr))
				{
					selectedMouthI = i;
					break;
				}
			var selectedMouth = selectedPvcItem.Mouths[selectedMouthI];

			//Draw a sphere around the mouth.
			Gizmos.color = new Color(1.0f, 0.25f, 0.25f, 0.325f);
			Gizmos.DrawSphere(selectedMouth.MyTr.position, 1.0f);

			//If the mouse clicks, choose the connector mouth.
			if (Input.GetMouseButtonDown(0))
			{
				//Push the state onto the Undo stack before modifying it.
				var itemsBeingChanged = new List<UnityEngine.Object>();
				var parent1 = myItem.MyTr.parent;
				itemsBeingChanged.AddRange(GetHierarchyFromRoot(parent1));
				var parent2 = selectedPvcItem.MyTr.parent;
				if (parent2 != parent1)
					itemsBeingChanged.AddRange(GetHierarchyFromRoot(parent2));
				Undo.RecordObjects(itemsBeingChanged.ToArray(),
								   "Connect " + myItem.gameObject.name + " to " +
								       selectedPvcItem.gameObject.name);

				//Make the connection.
				myItem.Mouths[ConnectIndex].OtherItem = selectedPvcItem;
				myItem.Mouths[ConnectIndex].OtherItemMouthI = selectedMouthI;
				selectedPvcItem.Mouths[selectedMouthI].OtherItem = myItem;
				selectedPvcItem.Mouths[selectedMouthI].OtherItemMouthI = ConnectIndex;

				//TODO: Combine the two groups of Transforms into one.
			}
		}
	}

	private void OnDestroy()
	{
		if (target != null && ItemBeingConnected == (PVCItem)target)
			ItemBeingConnected = null;
	}

	/// <summary>
	/// Gets all Transforms in the given hierarchy.
	/// The first item returned is the parameter itself;
	///     the rest are its children, grandchildren, etc.
	/// </summary>
	private IEnumerable<Transform> GetHierarchyFromRoot(Transform tr)
	{
		yield return tr;
		for (int i = 0; i < tr.childCount; ++i)
		{
			var child = tr.GetChild(i);
			foreach (var childTr in GetHierarchyFromRoot(child))
				yield return childTr;
		}
	}
}