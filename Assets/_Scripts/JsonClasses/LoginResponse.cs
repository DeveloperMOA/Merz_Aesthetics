[System.Serializable]
public class LoginResponse
{
	public string token;
	public int life;
	public Profile userProfile;

	public LoginResponse()
	{
		token = "token";
		life = 0;
		userProfile = new Profile();
	}
}