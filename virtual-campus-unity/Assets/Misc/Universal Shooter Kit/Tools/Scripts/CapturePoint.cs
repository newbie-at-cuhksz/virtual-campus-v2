using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	public class CapturePoint : MonoBehaviour
	{
		public enum Type
		{
			Circle,
			Rectangle
		}

		public Type type;
		
		public int Radius = 3;
		public Vector3 Size = new Vector3(3, 3, 3);


#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			
			var roomManager = FindObjectOfType<RoomManager>();

			if (roomManager && Selection.gameObjects.Contains(roomManager.gameObject) || Selection.gameObjects.Contains(gameObject))
			{
				if (type == Type.Circle)
				{
					Gizmos.DrawWireSphere(transform.position, Radius);
				}
				else
				{
					Gizmos.DrawWireCube(transform.position, Size);
				}
			}
		}
#endif
	}
}
