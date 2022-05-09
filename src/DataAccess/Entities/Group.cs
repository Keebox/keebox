using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Entities;

public class Group
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string Path { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual IEnumerable<Secret> Secrets { get; set; }
}