using UnityEngine;

public class TextureAnimationHandler : Handler
{
	public TextureAnimator textureAnimator;
	public Animator animator;

	public override void AppearEffect()
	{
		animator.GetBehaviour<SkinAnimationBehaviour>().textureAnimator = textureAnimator;
		CallAppearCompleted();
	}

	public override void DisappearEffect()
	{
		CallDisappearCompleted();
	}
}
