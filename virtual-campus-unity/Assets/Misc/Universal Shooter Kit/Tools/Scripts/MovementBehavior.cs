using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace GercStudio.USK.Scripts
{
	public class MovementBehavior : MonoBehaviour
	{
	//	public List<GameObject> Waypoints;
	
		[Serializable]
		public class Behavior
		{
			public GameObject point;
			public GameObject nextPoint;
			public float waitTime;
			public bool isLookAround;
			public Helper.NextPointAction action;
			public Helper.NextPointAction curAction;
		}

		public bool asjustment;

		public Behavior currentPoint;
		public List<Behavior> points = new List<Behavior>();

		public void ChangeIcon(Behavior point)
		{
#if UNITY_EDITOR
			switch (point.action)
			{
				case Helper.NextPointAction.NextPoint:
					Helper.AddObjectIcon(point.point, points[0] == point ? "WaypointNextGreen" : "WaypointNextYellow");
					break;
				case Helper.NextPointAction.RandomPoint:
					Helper.AddObjectIcon(point.point, points[0] == point ? "WaypointRandomGreen" : "WaypointRandomYellow");
					break;
				case Helper.NextPointAction.ClosestPoint:
					Helper.AddObjectIcon(point.point, points[0] == point ? "WaypointClosestGreen" : "WaypointClosestYellow");
					break;
				case Helper.NextPointAction.Stop:
					Helper.AddObjectIcon(point.point, "StopWaypoint");
					break;
			}
#endif
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!asjustment)
				return;
			
			for (var i = 0; i < points.Count; i++)
			{
				if (points[i] == null) continue;
				if (!points[i].point) continue;

//				Handles.color = new Color32(0, 255, 0, 150);
//				Handles.SphereHandleCap(0, points[i].point.transform.position, Quaternion.Euler(0, 0, 0), 2, EventType.Repaint);

//				Gizmos.color = new Color32(0, 255, 0, 255);
//				Gizmos.DrawSphere(points[i].point.transform.position, 1);
				
				switch (points[i].action)
				{
					case Helper.NextPointAction.NextPoint:
					{
						var nextPoint = i + 1;
						if (nextPoint >= points.Count)
						{
							nextPoint = 0;
						}

						if (points[nextPoint] != null)
							if (points[nextPoint].point)
							{
								points[i].nextPoint = points[nextPoint].point;
								var direction = points[nextPoint].point.transform.position - points[i].point.transform.position;

								var distance = Vector3.Distance(points[nextPoint].point.transform.position, points[i].point.transform.position);
							
								direction.Normalize();

								if (points.Count > 1)
								{
									var endPosition = points[i].point.transform.position + direction * (distance - 4.4f);
									var startPosition = points[i].point.transform.position + direction * 1.2f;
									
									Handles.zTest = CompareFunction.Less;
									Handles.color = new Color32(0, 255, 0, 255);
									Handles.ArrowHandleCap(0, endPosition, Quaternion.LookRotation(points[nextPoint].point.transform.position -
										points[i].point.transform.position), 3, EventType.Repaint);
									Handles.DrawLine(startPosition, endPosition);
									
									Handles.zTest = CompareFunction.Greater;
									Handles.color = new Color32(0, 255, 0, 50);
									Handles.ArrowHandleCap(0, endPosition, Quaternion.LookRotation(points[nextPoint].point.transform.position -
										points[i].point.transform.position), 3, EventType.Repaint);
									Handles.DrawLine(startPosition, endPosition);
								}
							}

						break;
					}

					case Helper.NextPointAction.RandomPoint:
////						Handles.ArrowHandleCap(0,
////							points[i].point.transform.position + points[i].point.transform.up * 1.2f,
////							Quaternion.Euler(-90, 0, 0), 3, EventType.Repaint);
//
//						GUIStyle style = new GUIStyle();
//						style.fontSize = 15;
//						style.fontStyle = FontStyle.Bold;
//						style.normal.textColor = new Color32(255, 166, 0, 255);
//
//						Handles.Label(points[i].point.transform.position + points[i].point.transform.up * 2, "R",
//							style);

						
						break;
				}
			}
		}
#endif
	}
}
