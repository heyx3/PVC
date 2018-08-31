using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO: Update whole class.


/// <summary>
/// A PVC pipe.
/// </summary>
[ExecuteInEditMode]
public class Pipe : PVCItem
{
	/// <summary>
	/// The connectors that this pipe uses.
	/// A "null" value indicates that it isn't connected to anything on that end.
	/// </summary>
	public Transform C1, C2;

	public Vector3 P1 { get { return MyTr.position; } }
	public Vector3 P2 { get { return MyTr.position + (MyTr.forward * MyTr.localScale.z); } }


	/// <summary>
	/// Updates this pipe's Transform to fit between its two connectors.
	/// If one of the connectors is null, it will attach itself to the non-null end.
	/// If both of the connectors is null, it will not do anything.
	/// </summary>
	/// <param name="length">
	/// The new length of this pipe, if exactly one of the two connectors is not null.
	/// If NaN, its length will remain unchanged.
	/// </param>
	public void UpdateTransform(float length = float.NaN)
	{
		if (C1 == null && C2 == null)
			return;

		if (C1 == null || C2 == null)
		{
			var singleConnector = (C1 == null) ? C2 : C1;
			MyTr.position = singleConnector.position;
			MyTr.forward = singleConnector.forward;
			MyTr.localScale = new Vector3(1.0f, 1.0f, length);
			return;
		}

		MyTr.position = C1.position;
		Vector3 toC2 = C2.position - C1.position;
		float pipeLen = toC2.magnitude;
		MyTr.forward = toC2 / pipeLen;
		MyTr.localScale = new Vector3(1.0f, 1.0f, pipeLen);
	}
}