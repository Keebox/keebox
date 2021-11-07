using Newtonsoft.Json;


namespace Keebox.Common.Helpers.Serialization
{
	public class JsonSerializer : ISerializer
	{
		public string? Serialize(object? @object)
		{
			if (@object is null)
				return null;

			return JsonConvert.SerializeObject(@object);
		}

		public T? Deserialize<T>(string? serializedObject) where T : class
		{
			if (serializedObject is null)
				return default;

			return JsonConvert.DeserializeObject<T>(serializedObject)!;
		}
	}
}