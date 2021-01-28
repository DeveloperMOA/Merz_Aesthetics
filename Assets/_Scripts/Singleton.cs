using UnityEngine;

namespace Nox.Core
{
	//Soft singleton class. It's up to the developer to only have one instance around
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<T>();
				return instance;
			}
		}

		private static T instance;
	}
}
