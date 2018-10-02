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
		PosX, PosY, PosZ,
		NegX, NegY, NegZ,

		COUNT
	}

	[SerializeField]
	private Directions[] mouthDirs = new Directions[0];


	public override void UpdateTransform()
	{
		//Error-checking:
		if (mouthDirs.Length != mouths.Length)
		{
			Debug.LogError("'mouthDirs' is not the same size as 'mouths'");
			return;
		}


		//Temporarily parent all siblings to this item so that they update alongside it.
		List<Transform> siblings = new List<Transform>(MyTr.parent.childCount - 1);
		for (int i = 0; i < MyTr.parent.childCount; ++i)
		{
			var siblingTr = MyTr.parent.GetChild(i);
			if (siblingTr != MyTr)
			{
				siblings.Add(siblingTr);
				siblingTr.SetParent(MyTr, true);
				i -= 1;
			}
		}

		//Rotate this connector based on its connections.
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
					//TODO: Unparent the other mouth if its PVCItem was a sibling of this one (which would mean it currently is a child of this connector).

					Vector3 toOther = (otherMouthPos - myMouthPos),
						    toOtherN = toOther.normalized;
					switch (mouthDirs[i])
					{
						case Directions.PosZ:
							MyTr.forward = toOtherN;
							break;
						case Directions.NegZ:
							MyTr.forward = -toOtherN;
							break;

						case Directions.PosX:
							MyTr.right = toOtherN;
							break;
						case Directions.NegX:
							MyTr.right = -toOtherN;
							break;

						case Directions.PosY:
							MyTr.up = toOtherN;
							break;
						case Directions.NegY:
							MyTr.up = -toOtherN;
							break;

						default: throw new NotImplementedException();
					}

					MyTr.position += toOther;
				}
			}
		}

		//Re-parent all siblings to this group.
		foreach (var siblingTr in siblings)
		{
			siblingTr.SetParent(MyTr.parent, true);
		}
	}
}