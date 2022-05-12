using DataAccess.Contexts;
using DataAccess.Entities;

using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repository;

internal class Repository<T> : IRepository<T> where T : Entity
{
	private readonly Context _context;

	public Repository(Context context)
	{
		_context = context;
	}

	private DbSet<T> Entities
	{
		get => _context.Set<T>();
	}

	public IQueryable<T> Queryable
	{
		get => Entities.AsQueryable();
	}

	public T GetById(Guid id)
	{
		return Entities.Single(x => x.Id == id);
	}

	public void Insert(T entity)
	{
		Entities.Add(entity);
		_context.SaveChanges();
	}

	public void Delete(T entity)
	{
		Entities.Remove(entity);
		_context.SaveChanges();
	}

	public void Delete(Guid id)
	{
		var entity = GetById(id);
		Entities.Remove(entity);
		_context.SaveChanges();
	}

	public void Update(T entity)
	{
		Entities.Update(entity);
		_context.SaveChanges();
	}
}