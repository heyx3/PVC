using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Pipe : PVCItem
{
	private Mouth[] mouths = new Mouth[2];

	public override IReadOnlyList<Mouth> Mouths { get { return mouths; } }

	public override void UpdateTransform()
	{
		if (!mouths[0].IsConnected && !mouths[1].IsConnected)
			return;

		if (mouths[0] == null || mouths[1] == null)
		{
			var singleConnector = ((mouths[0] == null) ? mouths[1] : mouths[0]).MyTr;
			MyTr.position = singleConnector.position;
			MyTr.forward = singleConnector.forward;
			return;
		}

		MyTr.position = mouths[0].MyTr.position;
		Vector3 toC2 = mouths[1].MyTr.position - mouths[0].MyTr.position;
		float pipeLen = toC2.magnitude;
		MyTr.forward = toC2 / pipeLen;
		MyTr.localScale = new Vector3(1.0f, 1.0f, pipeLen);
	}
}