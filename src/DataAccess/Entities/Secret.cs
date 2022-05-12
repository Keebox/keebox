namespace DataAccess.Entities;

public class Secret : Entity
{
	public string Name { get; set; }

	public string? Value { get; set; }

	public byte[]? File { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual Group Group { get; set; }

	public Guid GroupId { get; set; }
}