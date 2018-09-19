using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Pipe : PVCItem
{
	[SerializeField]
	private Transform mouth1, mouth2;
	[SerializeField]
	private Transform bodyTr;

	private Mouth[] mouths = new Mouth[2] { new Mouth(), new Mouth() };

	public override IReadOnlyList<Mouth> Mouths { get { return mouths; } }

	public void RebuildMouthList()
	{
		mouths[0] = new Mouth() { MyTr = mouth1, Name = "C1" };
		mouths[1] = new Mouth() { MyTr = mouth2, Name = "C2" };
	}
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
		bodyTr.localScale = new Vector3(1.0f, 1.0f, pipeLen);
	}

	protected override void Awake()
	{
		base.Awake();
		RebuildMouthList();
	}
}