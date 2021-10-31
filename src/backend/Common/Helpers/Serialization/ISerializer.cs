namespace Keebox.Common.Helpers.Serialization
{
	public interface ISerializer
	{
		public string? Serialize(object? @object);

		public T? Deserialize<T>(string? serializedObject) where T : class;
	}
}