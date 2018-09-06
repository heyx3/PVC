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
		public Mouth ConnectedMouth { get { return OtherItem.Mouths[OtherItemMouthI]; } }
	}


	public Transform MyTr { get; private set; }

	/// <summary>
	/// Gets all mouths on this item.
	/// </summary>
	public abstract IReadOnlyList<Mouth> Mouths { get; }
	/// <summary>
	/// The types of GameObjects that this item can connect to.
	/// Default behavior: returns the inverse of this GameObject's layer (i.e. every other layer).
	/// </summary>
	public virtual LayerMask CompatibleObjects { get { return ~gameObject.layer; } }

	/// <summary>
	/// Tells this item to update its Transform so that it's touching its connections correctly.
	/// </summary>
	public abstract void UpdateTransform();

	protected virtual void Awake()
	{
		MyTr = transform;

		if (MyTr.parent == null)
			MyTr.SetParent(new GameObject("Group").transform, true);
		else if (MyTr.parent.parent != null)
			Debug.LogWarning("PVC item shouldn't be more than one-deep in the hierarchy");
	}
}