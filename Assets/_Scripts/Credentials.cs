using System;

[Serializable]
public class Credentials
{
	public string token_type;
	public string expires_at;
	public string token;
	public User user;
}
