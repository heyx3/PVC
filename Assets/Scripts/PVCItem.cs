using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A PVC item (i.e. pipe or connector).
/// </summary>
[ExecuteInEditMode]
[DisallowMultipleComponent]
public abstract class PVCItem : MonoBehaviour
{
	/// <summary>
	/// A part on this item that connects to another part.
	/// </summary>
	public class Mouth
	{
		public Transform MyTr;
		public string Name;

		public PVCItem OtherItem;
		public int OtherItemMouthI;

		public bool IsConnected { get { return OtherItem != null; } }
	}


	public Transform MyTr { get; private set; }

	/// <summary>
	/// Gets all mouths on this item.
	/// </summary>
	public abstract IReadOnlyList<Mouth> Mouths { get; }
	/// <summary>
	/// The types of GameObjects that this item can connect to.
	/// </summary>
	public abstract LayerMask CompatibleObjects { get; }

	/// <summary>
	/// Connects the given item's mouth to this item's given mouth.
	/// </summary>
	public abstract void Connect(int myMouthI, PVCItem item, int otherMouthI);

	protected virtual void Awake()
	{
		MyTr = transform;

		if (MyTr.parent == null)
			MyTr.SetParent(new GameObject("Group").transform, true);
		else if (MyTr.parent.parent != null)
			Debug.LogWarning("PVC item shouldn't be more than one-deep in the hierarchy");
	}
}