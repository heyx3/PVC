using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A PVC pipe.
/// </summary>
public class Pipe : MonoBehaviour
{
	/// <summary>
	/// The connectors that this pipe uses.
	/// A "null" value indicates that it isn't connected to anything on that end.
	/// </summary>
	[HideInInspector]
	public Transform C1, C2;


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

		Transform tr = transform;

		if (C1 == null || C2 == null)
		{
			var singleConnector = (C1 == null) ? C2 : C1;
			tr.position = singleConnector.position;
			tr.forward = singleConnector.forward;
			tr.localScale = new Vector3(1.0f, 1.0f, length);
			return;
		}

		tr.position = C1.position;
		Vector3 toC2 = C2.position - C1.position;
		float pipeLen = toC2.magnitude;
		tr.forward = toC2 / pipeLen;
		tr.localScale = new Vector3(1.0f, 1.0f, pipeLen);
	}
}