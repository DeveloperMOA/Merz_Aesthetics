using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
	public int materialIndex = 0;
	public bool loop = true;
	public bool setAtStart;
	public float swapInterval;
	public bool customUpdate;

	public Renderer meshRenderer;

	private float timer;
	public int index;

	public Texture2D[] textures;

	private bool loopFlag;

	private void OnDisable()
	{
		timer = 0;
		index = 0;
		meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[index]);
		loopFlag = false;
	}

	private void Start()
	{
		timer = 0;
		index = 0;

		if(setAtStart)
			meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[index]);
	}
	// Update is called once per frame
	void Update()
    {
		if (customUpdate || loopFlag)
			return;
		timer += Time.deltaTime;
		if(timer > swapInterval)
		{
			timer = 0;
			index++;
			if (index >= textures.Length)
			{
				index = 0;
				loopFlag = !loop;
				if(loop)
					meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[index]);
			}
			else
				meshRenderer.materials[materialIndex].SetTexture("_MainTex",textures[index]);
		}
    }

	public void NextTexture()
    {
		if(index < textures.Length - 1)
        {
			index++;
			meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[index]);
		}
		else
        {
			if(loop)
            {
				index = 0;
				meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[index]);
			}
        }		
	}

	public void SetTexture(int textureIndex)
    {
		meshRenderer.materials[materialIndex].SetTexture("_MainTex", textures[textureIndex]);
    }
}
