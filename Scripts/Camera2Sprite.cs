using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class Camera2Sprite : MonoBehaviour {

	[SerializeField] new private Camera camera;
	new private SpriteRenderer renderer;

	private RenderTexture curRenderTexture;
	private Texture2D image;
	private Sprite sprite;


	private void Start () {
		renderer = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate () {
		FreeMemory();

		RTImage();
		Texture2DToSprite();

		renderer.sprite = sprite;
	}


	// docs.unity3d.com/ScriptReference/CameraRenderer.html
	private void RTImage () {
		// The RenderTexture in RenderTexture.active is the one that will be read by ReadPixels.
		curRenderTexture = RenderTexture.active;
		RenderTexture.active = camera.targetTexture;

		// Render the camera's view.
		camera.Render();

		// Make a new texture and read the active RenderTexture into it.
		image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
		image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
		image.Apply();

		// Replace he original active RenderTexture.
		RenderTexture.active = curRenderTexture;
	}

	private void Texture2DToSprite () {
		sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one / 2);
		sprite.name = camera.name + " View Sprite";
	}

	private void FreeMemory () {
		if (Application.isEditor) {
			DestroyImmediate(curRenderTexture);
			DestroyImmediate(image);
			DestroyImmediate(sprite);
		} else {
			Destroy(curRenderTexture);
			Destroy(image);
			Destroy(sprite);
		}
	}

	
}
