using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Character_Controller : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI dubugGUI;
	private CharacterType characterType;

	// Prevent infinite falling vars
	public float maxFallTime = 25f;
	private bool wasFalling;
	private float fallTimer;

	[Header("Movement vars")]
	public LayerMask obstacleMask;
	[SerializeField] private float speed;
	public float fallingGravityScale;
	private float gravityScale;
	[SerializeField] private Transform feet;
    [SerializeField] private Vector2 groundDetectionAreaSize;

    [Header("Combat vars")]
    public LayerMask targetMask;
	[SerializeField] private int maxHealth;
	private int curHealth;
    private int curArmour = 0;
	[SerializeField] private Transform weaponCenterL;
	[SerializeField] private Transform weaponCenterR;
	public Vector2 meleeWeaponArea;
	public float projectileRange;
	[SerializeField] private TextMeshPro statusBar;

	[Header("Visuals vars")]
	[SerializeField] private Transform characterVisuals;

	new private Rigidbody2D rigidbody;
	private ProjectilePool projectilePool;
	private TurnManager turnManager;
	private Animator animator;

	public int faceDirection { get; private set; }
	public bool isMoving {
		get {
			return rigidbody.velocity.x != 0f;
		}
	}
	public bool isGrounded { get; private set; }
	public bool isJumping {
		get {
			return !isGrounded && rigidbody.velocity.y > 0f;
		}
	}
	public bool isFalling {
		get {
			return !isGrounded && rigidbody.velocity.y < 0f;
		}
	}


	private void OnDrawGizmosSelected () {
		if (feet != null) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(feet.position, groundDetectionAreaSize);
		}

		if (weaponCenterL != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(weaponCenterL.position, meleeWeaponArea);
			Gizmos.DrawLine(weaponCenterL.position, weaponCenterL.position - transform.right * projectileRange);
		}

		if (weaponCenterR != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(weaponCenterR.position, meleeWeaponArea);
			Gizmos.DrawLine(weaponCenterR.position, weaponCenterR.position + transform.right * projectileRange);
		}
	}

	private void OnValidate () {
		speed = Mathf.Clamp(speed, 0f, float.MaxValue);
		groundDetectionAreaSize.x = Mathf.Clamp(groundDetectionAreaSize.x, 0f, float.MaxValue);
		groundDetectionAreaSize.y = Mathf.Clamp(groundDetectionAreaSize.y, 0f, float.MaxValue);

		maxHealth = Mathf.Clamp(maxHealth, 0, int.MaxValue);
		meleeWeaponArea.x = Mathf.Clamp(meleeWeaponArea.x, 0f, float.MaxValue);
		meleeWeaponArea.y = Mathf.Clamp(meleeWeaponArea.y, 0f, float.MaxValue);
		projectileRange = Mathf.Clamp(projectileRange, 0f, float.MaxValue);
	}

	private void Start () {
		rigidbody = GetComponent<Rigidbody2D>();
		gravityScale = rigidbody.gravityScale;

		projectilePool = ProjectilePool.GetInstance();

		animator = GetComponent<Animator>();

		// Set initial rotation.
		UpdateRotation((transform.eulerAngles.y >= 0f && transform.eulerAngles.y < 180f) ? 1 : -1);
		transform.eulerAngles = Vector3.zero;

		UpdateHealth(maxHealth);
	}

	private void Update () {
		if (dubugGUI != null)
			dubugGUI.text = string.Format("isMoving = {0}\nisGounded = {1}\nisJumping = {2}\nisFalling = {3}\n\nrigidbody.velocity = {4}\nrigidbody.gravityScale = {5}", isMoving, isGrounded, isJumping, isFalling, rigidbody.velocity.ToString(), rigidbody.gravityScale);

		// Set the gravity scale depending on if it's falling or not.
		if (isFalling)
			rigidbody.gravityScale = fallingGravityScale;
		else
			rigidbody.gravityScale = gravityScale;

		// Avoid infinite falling.
		InfiniteFallingHandler();

		// Set animator parameters.
		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isJumping", isJumping);
		animator.SetBool("isFalling", isFalling);
	}

	private void FixedUpdate () {
		// Check if the chracter is grounded.
		Collider2D[] colliders = Physics2D.OverlapBoxAll(feet.position, groundDetectionAreaSize, 0f, obstacleMask);
		if (colliders == null)
			isGrounded = false;
		isGrounded = colliders.Length > 0;
	}


	#region Action System

	private Queue<CardData> cardsToPlay = new Queue<CardData>();

	private bool executingAction = false;
	public bool isPlayingTurn { get; private set; } = false;


	public void ExecuteAction (IEnumerator action) {
		if (executingAction)
			return;

		StartCoroutine(action);
    }

	private IEnumerator ExecuteActions () {
		isPlayingTurn = true;

		while (cardsToPlay.Count != 0) {
			CardData card = cardsToPlay.Dequeue();

			foreach (ICardAction action in card.actions) {
				if (action == null)
					continue;

				action.Action(this);

				do
					yield return null;
				while (executingAction);
			}
		}

		isPlayingTurn = false;

		if (turnManager == null)
			turnManager = TurnManager.GetInstance();
		turnManager.TurnPhaseFinished();
	}

	public void AddCardToPlay (CardData cardData) {
		if (isPlayingTurn)
			return;

		cardsToPlay.Enqueue(cardData);
	}

	public void PlayTurn () {
		if (isPlayingTurn)
			return;

		isPlayingTurn = true;

		StartCoroutine(ExecuteActions());
	}


	public IEnumerator Move (float amount) {
        executingAction = true;

        // Check if there is an obstacle on the way and move if there isn't.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(amount), Mathf.Abs(amount), obstacleMask);

        if (!hit) {
			UpdateRotation(Mathf.Sign(amount));

            Vector2 startPosition = transform.position;
            Vector2 targetPosition = (Vector2)transform.position + Vector2.right * amount;
			Vector2 newPosition;

            float t = 0f; 
            while (t < 1f) {
                t = Mathf.Clamp01(t + speed * Time.deltaTime);
                newPosition = Vector2.Lerp(startPosition, targetPosition, t);

				while (isFalling)
					yield return null;

				rigidbody.MovePosition(newPosition);

				yield return null;
            }
        }

		while (isFalling || isMoving)
			yield return null;

		executingAction = false;
    }

	public IEnumerator Jump (Vector2 jumpVelocity) {
		executingAction = true;

		UpdateRotation(Mathf.Sign(jumpVelocity.x));

		rigidbody.AddForce(jumpVelocity, ForceMode2D.Impulse);
		
		// Wait a couple frames to be sure that already take off.
		yield return null;
		yield return null;
		yield return null;

		do
			yield return null;
		while (!isGrounded || isMoving);

		executingAction = false;
	}

	public IEnumerator MeleeAttack (int damage) {
		executingAction = true;

		yield return null;

		// Check targets at the left of the character.
		List<Character_Controller> targetsL;
		GetMeleeTargetsAt(weaponCenterL.position, out targetsL);

		yield return null;

		// Check targets at the right of the character.
		List<Character_Controller> targetsR;
		GetMeleeTargetsAt(weaponCenterR.position, out targetsR);

		yield return null;

		// Decide which targets (if any) are attacked.
		if (targetsL.Count > 0 && targetsL.Count > targetsR.Count) {
			animator.SetTrigger("melee_attack");

			MeleeAttack(-1, damage, targetsL);

			yield return new WaitForSeconds(GetCurrentAnimationLength());
		} else if (targetsR.Count > 0 && targetsR.Count >= targetsL.Count) {
			animator.SetTrigger("melee_attack");

			MeleeAttack(1, damage, targetsR);

			yield return new WaitForSeconds(GetCurrentAnimationLength());
		}

		yield return null;

		executingAction = false;
	}

	public IEnumerator RangedAttack (int damage, ProjectileData projectileData) {
		executingAction = true;

		// Check targets at the left of the character.
		Character_Controller targetL = null;
		float dst2targetL = 0f;
		GetRangedTargetsAt(weaponCenterL.position, -transform.right, out dst2targetL, out targetL);

		// Check targets at the right of the character.
		Character_Controller targetR = null;
		float dst2targetR = 0f;
		GetRangedTargetsAt(weaponCenterR.position, transform.right, out dst2targetR, out targetR);

		// Decide which targets (if any) are attacked.
		bool shootLeft = false;
		bool shootRight = false;

		if (targetL != null && targetR != null)
			if (dst2targetL > dst2targetR)
				shootLeft = true;
			else
				shootRight = true;
		else if (targetR != null)
			shootRight = true;
		else if (targetL != null)
			shootLeft = true;

		// Attack.
		if (shootLeft ^ shootRight) {
			animator.SetTrigger("ranged_attack");

			if (shootLeft)
				RangedAttack(-1, weaponCenterL.position, projectileData, damage);
			else
				RangedAttack(1, weaponCenterR.position, projectileData, damage);

			yield return new WaitForSeconds(GetCurrentAnimationLength());
		}

		yield return null;

		executingAction = false;
	}

	public IEnumerator Heal (int healthAmount) {
		executingAction = true;

		UpdateHealth(healthAmount);
		yield return null;

		executingAction = false;
	}

	public IEnumerator Defend (int armourAmount) {
		executingAction = true;

		curArmour = Mathf.Clamp(curArmour + armourAmount, 0, int.MaxValue);

		// Update status bar.
		if (statusBar != null)
			statusBar.text = string.Format("{0}/{1}\n[{2}]", curHealth, maxHealth, curArmour);

		yield return null;

		executingAction = false;
	}

	#endregion

	#region Combat Mecanics

	public void UpdateHealth (int healthAmount) {
        // Check if is damage
        if (Mathf.Sign(healthAmount) < 0) {
            // Calculate difference
            int diff = curArmour + healthAmount;

            // Update armour
            curArmour = Mathf.Clamp(diff, 0, curArmour);
            
            // Update damage
            healthAmount = (curArmour == 0) ? diff : 0;
        }

		curHealth = Mathf.Clamp(curHealth + healthAmount, 0, maxHealth);

		// Update status bar.
		if (statusBar != null)
			statusBar.text = string.Format("{0}/{1}\n[{2}]", curHealth, maxHealth, curArmour);

		// Destroy character on death.
		if (curHealth == 0f) {
			if (turnManager == null)
				turnManager = TurnManager.GetInstance();

			turnManager.OnDeath(characterType);

			Destroy(gameObject);
		}
    }

	private void GetMeleeTargetsAt (Vector2 weaponCenter, out List<Character_Controller> targets) {
		Collider2D[] colliders = Physics2D.OverlapBoxAll(weaponCenter, meleeWeaponArea, 0f, targetMask);
		targets = new List<Character_Controller>();

		if (colliders != null) {
			foreach (Collider2D collider in colliders) {
				Character_Controller candidate = collider.GetComponent<Character_Controller>();

				if (candidate == null)
					continue;

				targets.Add(candidate);
			}
		}
	}

	private void GetRangedTargetsAt (Vector2 weaponCenter, Vector2 direction, out float dst2target, out Character_Controller target) {
		RaycastHit2D[] hitsR = Physics2D.RaycastAll(weaponCenter, direction, projectileRange, targetMask);
		target = null;
		dst2target = float.MaxValue;

		if (hitsR != null) {
			foreach (RaycastHit2D hit in hitsR) {
				if (!hit)
					continue;

				Character_Controller candidate = hit.collider.GetComponent<Character_Controller>();

				if (candidate == null)
					continue;

				if (target == null)
					target = candidate;
				else {
					dst2target = Vector2.Distance(transform.position, target.transform.position);
					float dst2candidate = Vector2.Distance(transform.position, candidate.transform.position);

					if (dst2candidate < dst2target) {
						target = candidate;
						dst2target = dst2candidate;
					}
				}
			}
		}
	}

	private void MeleeAttack (float dir, int damage, List<Character_Controller> targets) {
		UpdateRotation(dir);

		foreach (Character_Controller target in targets)
			target.UpdateHealth(-damage);
	}

	private void RangedAttack (float direction, Vector2 weaponCenter, ProjectileData projectileData, int damage) {
		UpdateRotation(direction);

		if (projectilePool == null)
			projectilePool = ProjectilePool.GetInstance();

		ProjectileController projectileController = projectilePool.GetProjectile();

		Vector2 startPosition = weaponCenter;
		Vector2 endPosition = startPosition + Vector2.right * faceDirection * projectileRange;

		projectileController.Enable(projectileData, startPosition, endPosition, targetMask, damage);
	}

	#endregion

	public IEnumerator SetCharacterType (CharacterType characterType) {
		yield return new WaitForSeconds(0.1f);

		this.characterType = characterType;

		if (this.characterType == CharacterType.ENEMY) {
			if (turnManager == null)
				turnManager = TurnManager.GetInstance();

			turnManager.AddEnemy();
		}
	}

	private void UpdateRotation (float dirX) {
		int oldFaceDirection = faceDirection;

		// Update face direction.
		faceDirection = (dirX > 0) ? 1 : -1;

		if (faceDirection == oldFaceDirection)
			return;

		// Update character visuals.
		characterVisuals.localScale = new Vector3(
			Mathf.Abs(characterVisuals.localScale.x) * faceDirection,
			characterVisuals.localScale.y,
			characterVisuals.localScale.z
		);
	}

	private void InfiniteFallingHandler () {
		if (isFalling) {
			if (!wasFalling)
				fallTimer = 0f;

			fallTimer += Time.deltaTime;

			if (fallTimer > maxFallTime)
				UpdateHealth(-(curHealth + curArmour));
		}

		wasFalling = isFalling;
	}

	private float GetCurrentAnimationLength () {
		return animator.GetCurrentAnimatorStateInfo(0).length;
	}


}