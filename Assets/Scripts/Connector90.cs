using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A set of 90-degree connections between PVC pipes.
/// </summary>
public class Connector90 : PVCItem
{
	public enum Directions
	{
		Forward, Backward,
		Left, Right,
		Up, Down,

		COUNT
	}

	[Serializable]
	private struct MouthData
	{
		public Transform Transform;
		public Directions Dir;
	}


	public override IReadOnlyList<Mouth> Mouths { get { return mouths; } }


	[SerializeField]
	private MouthData[] mouthData;

	private Mouth[] mouths;


	public void RebuildMouthList()
	{
		mouths = new Mouth[mouthData.Length];
		for (int i = 0; i < mouths.Length; ++i)
			mouths[i] = new Mouth()
			{
				MyTr = mouthData[i].Transform,
				Name = mouthData[i].Dir.ToString()
			};
	}
	public override void UpdateTransform()
	{
		for (int i = 0; i < mouths.Length; ++i)
		{
			if (mouths[i].IsConnected)
			{
				var myMouthTr = mouths[i].MyTr;
				var otherMouthTr = mouths[i].ConnectedMouth.MyTr;

				Vector3 myMouthPos = myMouthTr.position,
						otherMouthPos = otherMouthTr.position;

				if (myMouthPos != otherMouthPos)
				{
					Vector3 toOther = (otherMouthPos - myMouthPos).normalized;
					switch (mouthData[i].Dir)
					{
						case Directions.Forward:
							MyTr.forward = toOther;
							break;
						case Directions.Backward:
							MyTr.forward = -toOther;
							break;

						case Directions.Right:
							MyTr.right = toOther;
							break;
						case Directions.Left:
							MyTr.right = -toOther;
							break;

						case Directions.Up:
							MyTr.up = toOther;
							break;
						case Directions.Down:
							MyTr.up = -toOther;
							break;

						default: throw new NotImplementedException();
					}
				}
			}
		}
	}
}