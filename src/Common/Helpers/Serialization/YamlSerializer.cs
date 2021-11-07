using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace Keebox.Common.Helpers.Serialization
{
	public class YamlSerializer : ISerializer
	{
		public string? Serialize(object? @object)
		{
			if (@object is null)
				return null;

			var serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();

			return serializer.Serialize(@object);
		}

		public T? Deserialize<T>(string? serializedObject) where T : class
		{
			if (serializedObject is null)
				return default;

			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();

			return deserializer.Deserialize<T>(serializedObject);
		}
	}
}