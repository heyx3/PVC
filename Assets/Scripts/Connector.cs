using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO: Update whole class.

/// <summary>
/// A PVC connector.
/// Has some number of "mouths" that pipe ends can connect to.
/// </summary>
[ExecuteInEditMode]
public class Connector : PVCItem
{
	[SerializeField]
	private List<Transform> mouths = new List<Transform>();

	public IReadOnlyList<Transform> Mouths { get { return mouths; } }
}