using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IPointerClickHandler {

	private Camera mainCamera;

	[SerializeField] private Transform boardViewCameraHolder;
	[SerializeField] private float boardViewCameraSize = 5f;

	[SerializeField] private Transform worldViewCameraHolder;
	[SerializeField] private float worldViewCameraSize = 1.75f;

	private bool boardView = true;
	private bool makingTransition = false;

	void Start () {
		mainCamera = GetComponent<Camera>();

		if (mainCamera != Camera.main)
			Destroy(this);
    }

	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
			ChangeView();
	}

	public void OnPointerClick (PointerEventData pointerEventData) {

	}

	public void ChangeView () {
		if (makingTransition)
			return;

		makingTransition = true;

		if (boardView) {
			// board view -> world view
			StartCoroutine(Transition(boardViewCameraHolder.position, worldViewCameraHolder.position, boardViewCameraSize, worldViewCameraSize));
		} else {
			// world view -> board view
			StartCoroutine(Transition(worldViewCameraHolder.position, boardViewCameraHolder.position, worldViewCameraSize, boardViewCameraSize));
		}

		boardView = !boardView;
	}

	private IEnumerator Transition (Vector3 positionA, Vector3 positionB, float sizeA, float sizeB) {
		float t = 0f;

		while (t < 1f) {
			mainCamera.transform.position = Vector3.Lerp(positionA, positionB, t);
			mainCamera.orthographicSize = Mathf.Lerp(sizeA, sizeB, t);
			t += Time.deltaTime;

			yield return null;
		}

		makingTransition = false;
	}




}
