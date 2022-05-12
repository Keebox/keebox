using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Entities;

public class Group : Entity
{
	public string Path { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual ICollection<Secret> Secrets { get; set; }
}