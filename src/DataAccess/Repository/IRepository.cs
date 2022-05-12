using DataAccess.Entities;

using Microsoft.EntityFrameworkCore.Query;


namespace DataAccess.Repository;

public interface IRepository<T> where T : Entity
{
	IQueryable<T> Queryable { get; }

	T GetById(Guid id);

	void Insert(T entity);

	void Delete(Guid id);

	void Update(T entity);
}