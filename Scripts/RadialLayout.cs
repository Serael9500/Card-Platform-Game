using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RadialLayout : MonoBehaviour {

	[SerializeField] private float radius = 1f;
	[SerializeField] private float padding = 0f;
	[SerializeField] private float spacing = 0.5f;
	[SerializeField] [Range(0f, 360f)] private float alpha = 0f;


	private bool hasAnyChildChange {
		get {
			for (int i = 0; i < transform.childCount; i++)
				if (transform.GetChild(i).hasChanged)
					return true;
			return false;
		}
	}



	private void OnDrawGizmosSelected () {
		Gizmos.DrawWireSphere(transform.position, radius);
	}

	void Start () {
        
    }
	
    void Update () {
		float maxSizeY = transform.GetChild(0).lossyScale.y;
		for (int i = 1; i < transform.childCount; i++)
			if (transform.GetChild(i).lossyScale.y > maxSizeY)
				maxSizeY = transform.GetChild(i).lossyScale.y;

		float r = radius - (padding + maxSizeY / 2);
		float angleOld = alpha * Mathf.Deg2Rad;

		transform.GetChild(0).position = new Vector2(Mathf.Cos(angleOld), Mathf.Sin(angleOld)) * r;
		for (int i = 1; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);

			float chord = child.lossyScale.x + ((i > 0) ? spacing : 0f);

			float angle = angleOld + 2 * Mathf.Asin(chord / (2 * r));
			Vector2 position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;

			if (!float.IsNaN(position.x) && !float.IsNaN(position.y))
				child.localPosition = position;

			angleOld = angle;
		}

    }


}
