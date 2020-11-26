using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character_Controller), typeof(Animator))]
public class AnimationController : MonoBehaviour {

	private Character_Controller characterController;
	private Animator animator;


    private void Start () {
		characterController = GetComponent<Character_Controller>();
		animator = GetComponent<Animator>();
    }

	private void Update () {
		animator.SetBool("isMoving", characterController.isMoving);
		animator.SetBool("isMoving", characterController.isMoving);
		animator.SetBool("isMoving", characterController.isMoving);
	}


}
