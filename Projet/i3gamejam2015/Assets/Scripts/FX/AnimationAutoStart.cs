using UnityEngine;
using System.Collections;

public class AnimationAutoStart : MonoBehaviour 
{
	public Animator animator;
	public string startTrigger;

	public void OnEnable()
	{
		animator.SetTrigger(startTrigger);
	}
}
