using System;
using System.Collections.Generic;

[Serializable]
public class Content
{
	public News news;
	public Permissions permissions;
	public List<Brand> brands;

	public Content()
	{
		brands = new List<Brand>();
	}
}