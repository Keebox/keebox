using DataAccess.Entities;


namespace Services;

public interface ISecretsService
{
	void CreateGroup(string path, Dictionary<string, string> secrets);

	void UpdateGroup(string path, Dictionary<string, string> secrets);

	Secret[] GetGroupSecrets(string path);

	Secret GetSecret(string path);
}