using UnityEngine;

namespace GercStudio.USK.Scripts
{

	public class DrawGizmo : MonoBehaviour
	{

		void OnDrawGizmosSelected()
		{
			if (gameObject.CompareTag("Spawn"))
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(transform.position, 1);
			}

			else if (gameObject.CompareTag("WayPoint"))
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(transform.position, 1);
				var foundPoints = GameObject.FindGameObjectsWithTag("WayPoint");

				for (int i = 0; i < foundPoints.Length; i++)
					Gizmos.DrawLine(transform.position, foundPoints[i].transform.position);
			}

			else if (gameObject.CompareTag("SpawnArea"))
			{
				Gizmos.color = new Color(1, 0, 0, 0.5f);
				Gizmos.DrawSphere(transform.position, 10);
			}

			else
			{
				Gizmos.color = new Color(1, 1, 0, 0.8f);
				Gizmos.DrawSphere(transform.position, 7);
			}
		}
	}

}


