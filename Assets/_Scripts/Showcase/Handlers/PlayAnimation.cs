using System.Collections;
using UnityEngine;

//This handlerss assumes the animator contains an appear and disappear animation states
public class PlayAnimation : Handler
{
	public Animator anim;

	private static readonly int APPEAR;
	private static readonly int DISAPPEAR;

	static PlayAnimation()
	{
		APPEAR = Animator.StringToHash("Appear");
		DISAPPEAR = Animator.StringToHash("Disappear");
	}

	public override void AppearEffect()
	{
		anim.Play(APPEAR);
	}

	public override void DisappearEffect()
	{
		anim.Play(DISAPPEAR);
	}

	public void AppearAnimationFinished()
	{
		CallAppearCompleted();
	}

	public void DisappearAnimationFinished()
	{
		CallDisappearCompleted();
	}
}
