using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace GercStudio.USK.Scripts
{
	public class SpawnZone : MonoBehaviour
	{

		[Range(-180, 180)] public float spawnDirection;
		
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			var roomManager = FindObjectOfType<RoomManager>();
			var gameManager = FindObjectOfType<GameManager>();
			
			if (roomManager && Selection.gameObjects.Contains(roomManager.gameObject) || gameManager && Selection.gameObjects.Contains(gameManager.gameObject) || Selection.gameObjects.Contains(gameObject))
			{
				Gizmos.color = new Color32(0, 255, 0, 150);
				Gizmos.DrawWireCube(transform.position, transform.localScale);
				
				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 255, 0, 255);
				Handles.ArrowHandleCap(0, transform.position + Vector3.up * 2, Quaternion.Euler(0,spawnDirection,0), 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position + Vector3.up * 2, Quaternion.identity, 0.5f, EventType.Repaint);
				
				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 255, 0, 100);
				Handles.ArrowHandleCap(0, transform.position + Vector3.up * 2, Quaternion.Euler(0,spawnDirection,0), 4, EventType.Repaint);
				Handles.SphereHandleCap(0, transform.position + Vector3.up * 2, Quaternion.identity, 0.5f, EventType.Repaint);

			}
		}
#endif
	}
}

