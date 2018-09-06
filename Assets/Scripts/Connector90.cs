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
	private MouthData[] mouthData = new MouthData[0];

	private Mouth[] mouths = new Mouth[0];


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
		//TODO: Parent all siblings to this item. Then run the code below, but temporarily unparent the item this connector is adjusting to. Then, reparent all siblings to the group.

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
					Vector3 toOther = (otherMouthPos - myMouthPos),
						    toOtherN = toOther.normalized;
					switch (mouthData[i].Dir)
					{
						case Directions.Forward:
							MyTr.forward = toOtherN;
							break;
						case Directions.Backward:
							MyTr.forward = -toOtherN;
							break;

						case Directions.Right:
							MyTr.right = toOtherN;
							break;
						case Directions.Left:
							MyTr.right = -toOtherN;
							break;

						case Directions.Up:
							MyTr.up = toOtherN;
							break;
						case Directions.Down:
							MyTr.up = -toOtherN;
							break;

						default: throw new NotImplementedException();
					}

					MyTr.position += toOther;
				}
			}
		}
	}
}