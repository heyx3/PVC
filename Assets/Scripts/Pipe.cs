using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteInEditMode]
[DisallowMultipleComponent]
public class Pipe : PVCItem
{
	[SerializeField]
	private Transform bodyTr;

	public override void UpdateTransform()
	{
		if (mouths.Length != 2)
		{
			Debug.LogError("Expected 2 mouths for a pipe...");
			return;
		}

		if (!mouths[0].IsConnected && !mouths[1].IsConnected)
			return;

		if (!mouths[0].IsConnected || !mouths[1].IsConnected)
		{
			var singleConnector = (mouths[0].IsConnected ? mouths[0] : mouths[1]).ConnectedMouth.MyTr;
			MyTr.position = singleConnector.position;
			MyTr.forward = singleConnector.forward;
			return;
		}

		MyTr.position = mouths[0].ConnectedMouth.MyTr.position;
		Vector3 toC2 = mouths[1].ConnectedMouth.MyTr.position - mouths[0].ConnectedMouth.MyTr.position;
		float pipeLen = toC2.magnitude;
		MyTr.forward = toC2 / pipeLen;
		bodyTr.localScale = new Vector3(1.0f, 1.0f, pipeLen);
	}

	public override void Awake()
	{
		base.Awake();

		if (mouths == null)
			mouths = new Mouth[2] { new Mouth() { Name = "C1" }, new Mouth() { Name = "C2" } };
	}
}