using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class ProjectileController : MonoBehaviour {

    private ProjectilePool projectilePool;
    private ProjectileData data;

    private bool active = false;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private float t;

    private LayerMask targetMask;
	private int damage;

	[SerializeField] private Collider2D trigger;
	[SerializeField] private SpriteRenderer visuals;


    private void Start () {
        projectilePool = ProjectilePool.GetInstance();

		visuals = GetComponent<SpriteRenderer>();

		gameObject.SetActive(false);
	}

    private void Update () {
        if (!active)
            return;

        t = Mathf.Clamp01(t + data.speed * Time.deltaTime);

        transform.position = Vector2.Lerp(startPosition, endPosition, t);

        if (t >= 1f)
            Disable();
    }

    private void OnTriggerEnter2D (Collider2D collider) {
        if (!active)
            return;

		if ((1 << collider.gameObject.layer) == targetMask.value) {
            Character_Controller target = collider.gameObject.GetComponent<Character_Controller>();

			if (target == null)
				return;

			target.UpdateHealth(-damage);
        }

        Disable();
    }

    public void Enable (ProjectileData data, Vector2 startPosition, Vector2 endPosition, LayerMask targetMask, int damage) {
        this.data = data;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.targetMask = targetMask;
		this.damage = damage;

        t = 0f;
        transform.position = startPosition;
        transform.localScale = data.size;

        visuals.sprite = data.sprite;

		gameObject.SetActive(true);

		active = true;
    }

    public void Disable () {
        active = false;

		if (projectilePool == null)
			projectilePool = ProjectilePool.GetInstance();
        projectilePool.ReturnProjectile(this);

		gameObject.SetActive(false);
    }

	#region Prototype

	public ProjectileController Clone () {
		GameObject clone = Instantiate(gameObject);
		ProjectileController controller = clone.GetComponent<ProjectileController>();

		controller.Disable();

		return controller;
	}

	#endregion


}