using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ActionTracer : MonoBehaviour {

	private static ActionTracer instance;

	public LayerMask obstacleMask;
	public int curveResolution = 20;
	public Transform playerTransform;

	private LineRenderer lineRenderer;


	private void Start () {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Destroy(gameObject);
			return;
		}

		lineRenderer = GetComponent<LineRenderer>();
	}


	public static void Clear () {
		instance.SetStartPoint(instance.playerTransform.position);
	}

	private void SetStartPoint (Vector2 startPosition) {
		lineRenderer.positionCount = 1;
		lineRenderer.SetPosition(0, startPosition);
	}	
	
	public static void AddMoveTrajectory (float xDirection, float distance) {
		xDirection = Mathf.Sign(xDirection);
		distance = Mathf.Abs(distance);

		Vector2 startPosition = instance.lineRenderer.GetPosition(instance.lineRenderer.positionCount - 1);
		Vector2 endPosition = startPosition + Vector2.right * xDirection * distance;

		instance.AddPoint(endPosition);
	}

	
	public static void AddJumpTrajectory (Vector2 initialVelocity, float t_up, float t_down, float g_up, float g_down) {
		Vector2 startPosition = instance.lineRenderer.GetPosition(instance.lineRenderer.positionCount - 1);

		float time = t_up + t_down;
		float dt = time / instance.curveResolution;
		float t = dt;

		float g, initialVelocityY;

		while (t <= time) {
			initialVelocityY = (t < t_up) ? initialVelocity.y : 0f;
			g = (t < t_up) ? g_up : g_down;

			Vector2 point = startPosition + new Vector2(
				initialVelocity.x * t,
				initialVelocityY * t + g * t * t / 2f
			);

			if (!instance.AddPoint(point))
				break;

			t += dt;
		}
	}

	/*
	public static void AddJumpTrajectory (float xDirection, float height, float horizontalDistance) {
		xDirection = Mathf.Sign(xDirection);
		horizontalDistance = Mathf.Abs(horizontalDistance);

		Vector2 startPosition = instance.lineRenderer.GetPosition(instance.lineRenderer.positionCount - 1);

		if (horizontalDistance == 0f) {
			Vector2 endPosition = startPosition + Vector2.up * height;

			if (instance.AddPoint(endPosition))
				instance.AddPoint(startPosition);
		}

		Vector2 p2 = startPosition + Vector2.up * height + Vector2.right * xDirection * horizontalDistance / 2;
		Vector2 p3 = startPosition + Vector2.right * xDirection * horizontalDistance;

		float a, b, c;
		if (instance.SolveParabolicEcuation(startPosition.x, startPosition.y, p2.x, p2.y, p3.x, p3.y, out a, out b, out c))
			instance.AddParabolicCurve(a, b, c, startPosition, p3, xDirection);
	}

	private bool SolveParabolicEcuation (float x1, float y1, float x2, float y2, float x3, float y3, out float a, out float b, out float c) {
		// 3 points:			P1 = (x1, y1)	P2 = (x2, y2)	P3(x3, y3)
		// Parabola ecuation:	y = a * x^2 + b * x + c
		// 
		// Ecuation system:		
		//						{ y1 = a * x1^2 + b * x1 + c }
		//						{ y2 = a * x2^2 + b * x2 + c }
		//						{ y3 = a * x3^2 + b * x3 + c }

		a = b = c = 0;

		float d, e, f, g, h, i;

		d = x2 * x2 - x1 * x1;
		e = x2 - x1;
		f = y2 - y1;
		g = x3 * x3 - x1 * x1;
		h = x3 - x1;
		i = y3 - y1;

		if ((d == 0 || h == 0) && (g == 0 || e == 0))
			return false;

		b = (d * i - g * f) / (d * h - g * e);
		a = (f - e * b) / d;
		c = y1 - x1 * (x1 * a + b);

		return true;
	}

	private void AddParabolicCurve (float a, float b, float c, Vector2 p1, Vector2 p3, float xDirection) {
		List<Vector3> points = new List<Vector3>();
		float dx = (Mathf.Abs(p3.x - p1.x) / curveResolution) * xDirection; 
		float x = p1.x + dx;

		for (int i = 0; i < curveResolution; i++) {
			if (!AddPoint(new Vector2(x, a * x * x + b * x + c)))
				break;

			x += dx;
		}
	}

	*/


	private bool AddPoint (Vector2 point) {
		bool pointAdded = true;

		if (lineRenderer.positionCount > 0) {
			Vector2 previousPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
			
			// Check if there is an obstacle on the way.
			Vector2 direction = point - previousPoint;
			RaycastHit2D hit = Physics2D.Raycast(previousPoint, direction.normalized, direction.magnitude, obstacleMask);

			if (hit) {
				Debug.Log("collision");
				point = hit.point;
				pointAdded = false;
			}

			// Check if the new point is equal to the previous one.
			if (point == previousPoint) {
				Debug.Log("repeated");
				return false;
			}
		}

		// Add point.
		lineRenderer.positionCount++;
		lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

		return pointAdded;
	}
	
	
	


}
