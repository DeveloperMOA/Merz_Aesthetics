using Nox.Core;
using UnityEngine.UI;

public class NavTutorialLayer : UILayer
{
	public UICarousel carousel;
	public Text next;

	public void OnCarouselIndexChanged(int index)
	{
		next.text = index == 2 ? "Iniciar" : "Siguiente";
	}

	public void OnCarouselEnd(bool isTouch)
	{
		if (!isTouch)
			Back();
	}

	public override void Back()
	{
		carousel.SetIndex(0,false);
		carousel.ForcePositionUpdate();
		handler.Hide();
	}

	public override void OnScreenStart()
	{
		carousel.Setup(3);
		carousel.IndexChangedEvent += OnCarouselIndexChanged;
		carousel.EndReachedEvent += OnCarouselEnd;
	}

	public override void OnScreenUp()
	{
		carousel.RegisterTouch();
	}

	public override void OnScreenDown()
	{
		carousel.UnRegisterTouch();
	}
}
