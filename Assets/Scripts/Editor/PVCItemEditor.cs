using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
[CustomEditor(typeof(PVCItem))]
public class PVCItemEditor : Editor
{
	public static PVCItem ItemBeingConnected { get; private set; }
	protected static int ConnectIndex = 1;


	public override void OnInspectorGUI()
	{
		if (targets.Length != 1)
		{
			GUILayout.Label("Can only edit one item at a time!");
			return;
		}

		var myItem = (PVCItem)target;

		CustomInspectorGUI();

		if (myItem == ItemBeingConnected)
		{
			GUILayout.Label("Click the mouth to connect to in the Scene view");
			if (GUILayout.Button("Cancel"))
				ItemBeingConnected = null;

			//Force the scene GUI to be repainted every frame so it can catch mouse events.
			Repaint();
		}
		else
		{
			UnityEngine.Assertions.Assert.IsNull(ItemBeingConnected,
												 "Other item is still being connected??");

			//Do the GUI for each item (i.e. ask to connect or disconnect).
			for (int mouthI = 0; mouthI < myItem.Mouths.Count; ++mouthI)
			{
				var mouth = myItem.Mouths[mouthI];
				if (mouth.IsConnected)
				{
					GUILayout.BeginHorizontal();
					if (GUILayout_CompactButton("Disconnect " + mouth.Name))
					{
						mouth.ConnectedMouth.OtherItem = null;
						mouth.OtherItem = null;

						//Find all siblings that still connect to this item.
						var connectedObjs = new HashSet<GameObject>();
						var toSearch = new Stack<PVCItem>();
						toSearch.Push(myItem);
						while (toSearch.Count > 0)
						{
							var item = toSearch.Pop();
							if (!connectedObjs.Contains(item.gameObject))
							{
								connectedObjs.Add(item.gameObject);
								foreach (var itemMouth in item.Mouths)
									if (itemMouth.IsConnected)
										toSearch.Push(itemMouth.OtherItem);
							}
						}

						//Take any siblings that aren't connected to this item
						//    and move them to a new group.
						Transform myParent = myItem.MyTr.parent,
								  newParent = null;
						for (int childI = 0; childI < myParent.childCount; ++childI)
						{
							if (!connectedObjs.Contains(myParent.GetChild(childI).gameObject))
							{
								if (newParent == null)
									newParent = new GameObject("Group").transform;

								myParent.GetChild(childI).SetParent(newParent, true);
								childI -= 1;
							}
						}
					}
					if (GUILayout_CompactButton("Reconnect " + mouth.Name))
					{
						mouth.OtherItem = null;
						ItemBeingConnected = myItem;
						ConnectIndex = mouthI;
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					if (GUILayout_CompactButton("Connect " + mouth.Name))
					{
						ItemBeingConnected = myItem;
						ConnectIndex = mouthI;
					}
				}
			}
		}
	}
	protected virtual void CustomInspectorGUI()
	{
		DrawDefaultInspector();
		GUILayout.Space(20.0f);
	}
	private bool GUILayout_CompactButton(string label)
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool result = GUILayout.Button(label);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		return result;
	}

	protected virtual void OnSceneGUI()
	{
		if (targets.Length != 1)
			return;

		var myItem = (PVCItem)target;

		//Only need to do scene GUI stuff if this item is being connected to something.
		if (myItem != ItemBeingConnected)
			return;

		//Intercept mouse events.
		if (Event.current.type == EventType.Layout)
			HandleUtility.AddDefaultControl(0);

		var mouth = myItem.Mouths[ConnectIndex];

		//Draw a sphere around the end being connected.
		PersistentGizmos.Sphere(myItem, 0, mouth.MyTr.position, 1.0f,
								new Color(0.25f, 1.0f, 0.25f, 0.325f));

		//See if a compatible item is being moused over.
		PVCItem selectedPvcItem;
		int selectedMouthI;
		FindMousedOverItem(myItem.CompatibleObjects, out selectedPvcItem, out selectedMouthI);
		if (selectedMouthI >= 0)
		{
			//Draw a sphere around the mouth.
			PersistentGizmos.Sphere(myItem, 1,
									selectedPvcItem.Mouths[selectedMouthI].MyTr.position, 1.0f,
									new Color(1.0f, 0.25f, 0.25f, 0.325f));

			//If the mouse clicks, choose the connector mouth.
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
			{
				//Push the state onto the Undo stack before modifying it.
				var itemsBeingChanged = new List<UnityEngine.Object>();
				var parent1 = myItem.MyTr.parent;
				itemsBeingChanged.AddRange(GetHierarchyFromRoot(parent1));
				var parent2 = selectedPvcItem.MyTr.parent;
				if (parent2 != parent1)
					itemsBeingChanged.AddRange(GetHierarchyFromRoot(parent2));

				//Make the connection.
				//TODO: Doesn't always work?
				selectedPvcItem.Mouths[selectedMouthI].OtherItem = myItem;
				selectedPvcItem.Mouths[selectedMouthI].OtherItemMouthI = ConnectIndex;
				var prop_thisMouth = serializedObject.FindProperty("mouths").GetArrayElementAtIndex(ConnectIndex);
				prop_thisMouth.FindPropertyRelative("OtherItem").objectReferenceValue = selectedPvcItem;
				prop_thisMouth.FindPropertyRelative("OtherItemMouthI").intValue = selectedMouthI;
				//myItem.Mouths[ConnectIndex].OtherItem = selectedPvcItem;
				//myItem.Mouths[ConnectIndex].OtherItemMouthI = selectedMouthI;

				//Merge the groups.
				if (parent1 != parent2)
				{
					while (parent2.childCount > 0)
						parent2.GetChild(0).SetParent(parent1, true);
					DestroyImmediate(parent2.gameObject);
				}

				//Clean up.
				ItemBeingConnected = null;
				PersistentGizmos.CleanUp(myItem);
				serializedObject.ApplyModifiedProperties();
				myItem.UpdateTransform();
				Repaint();
			}
		}
	}

	public override bool RequiresConstantRepaint()
	{
		return (target != null) && ((PVCItem)target) == ItemBeingConnected;
	}

	private void OnDestroy()
	{
		if (target != null && ItemBeingConnected == (PVCItem)target)
			ItemBeingConnected = null;

		PersistentGizmos.CleanUp(target);
	}


	private static void FindMousedOverItem(LayerMask compatibleObjects,
										   out PVCItem selectedItem, out int selectedMouthIndex)
	{
		selectedItem = null;
		selectedMouthIndex = -1;

		var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, compatibleObjects))
		{
			//Get the PVC item that was hit.
			selectedItem = hit.collider.GetComponentInParent<PVCItem>();
			UnityEngine.Assertions.Assert.IsNotNull(selectedItem,
													"Hit something that wasn't a PVC item??");

			//Get the specific mouth that was hit.
			for (int i = 0; i < selectedItem.Mouths.Count; ++i)
				if (hit.transform.IsChildOf(selectedItem.Mouths[i].MyTr))
				{
					selectedMouthIndex = i;
					break;
				}
		}
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