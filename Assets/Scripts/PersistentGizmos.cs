using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class PersistentGizmos
{
	public static void Sphere(UnityEngine.Object owner, int id,
							  Vector3 pos, float radius, Color col)
	{
		var gizmoTr = GetOrMakeGizmo(owner, id);

		//Move the sphere to the right position.
		gizmoTr.position = pos;
		gizmoTr.localScale = Vector3.one * radius * 2.0f;
		gizmoTr.GetComponent<MeshRenderer>().material.color = col;
	}

	public static void CleanUp(UnityEngine.Object owner)
	{
		if (!gizmos.ContainsKey(owner))
			return;

		foreach (var tr in gizmos[owner].Values)
			if (UnityEngine.Application.isPlaying)
				UnityEngine.GameObject.Destroy(tr.gameObject);
			else
				UnityEngine.GameObject.DestroyImmediate(tr.gameObject);

		gizmos.Remove(owner);
	}


	private static Dictionary<UnityEngine.Object, Dictionary<int, Transform>> gizmos
		= new Dictionary<UnityEngine.Object, Dictionary<int, Transform>>();

	private static Transform GetOrMakeGizmo(UnityEngine.Object owner, int id)
	{
		//Get the gizmo set for this object.
		if (!gizmos.ContainsKey(owner))
			gizmos.Add(owner, new Dictionary<int, Transform>());

		//Get the gizmo with the given ID, or create it if it doesn't exist.
		var myGizmos = gizmos[owner];
		if (!myGizmos.ContainsKey(id))
		{
			var go = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("GizmoSphere"));
			myGizmos.Add(id, go.transform);
		}

		return myGizmos[id];
	}
}