using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinAnimationBehaviour : StateMachineBehaviour
{
	public TextureAnimator textureAnimator;
	private int frameCounter;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		frameCounter = 0;
		textureAnimator.index = 0;
		textureAnimator.meshRenderer.material.SetTexture("_MainTex", textureAnimator.textures[textureAnimator.index]);
		
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int frame = Mathf.FloorToInt((stateInfo.normalizedTime % 1f) * 180f);
		frameCounter = frame;

		if (frameCounter >= textureAnimator.textures.Length)
			frameCounter = textureAnimator.textures.Length - 1;
		textureAnimator.meshRenderer.material.SetTexture("_MainTex", textureAnimator.textures[frameCounter]);
	}
}
