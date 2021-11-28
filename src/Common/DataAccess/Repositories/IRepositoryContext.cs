using Keebox.Common.DataAccess.Repositories.Abstractions;


namespace Keebox.Common.DataAccess.Repositories
{
	public interface IRepositoryContext
	{
		ISecretRepository GetSecretRepository();

		IAccountRepository GetAccountRepository();

		IGroupRepository GetGroupRepository();

		IRoleRepository GetRoleRepository();
		
		IPermissionRepository GetPermissionRepository();
	}
}