using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories
{
	public interface IRepositoryContext
	{
		ISecretsRepository GetSecretRepository();

		IAccountRepository GetAccountRepository();

		IGroupRepository GetGroupRepository();

		IRoleRepository GetRoleRepository();
	}
}