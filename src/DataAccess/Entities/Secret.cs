namespace DataAccess.Entities;

public class Secret
{
	public Guid Id { get; set; }

	public Guid GroupId { get; set; }

	public string Name { get; set; }

	public string Value { get; set; }

	public byte[] File { get; set; }

	public DateTime CreatedAt { get; set; }
}