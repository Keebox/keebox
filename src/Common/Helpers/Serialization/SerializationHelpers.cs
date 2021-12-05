using System;
using System.Collections.Generic;


namespace Keebox.Common.Helpers.Serialization
{
	public static class SerializationHelpers
	{
		public static T? CreateInstanceOf<T>(this Dictionary<string, object>? properties) where T : class
		{
			if (properties == null) return null;

			return CreateInstanceOfInternal(typeof(T), properties) as T;
		}

		private static object? CreateInstanceOfInternal(Type type, Dictionary<string, object> properties)
		{
			var @object = Activator.CreateInstance(type);

			foreach (var (key, value) in properties)
			{
				var property = type.GetProperty(key);

				if (property == null) continue;

				var targetValue = value;

				if (value is Dictionary<string, object> valueProperties)
				{
					targetValue = CreateInstanceOfInternal(property.PropertyType, valueProperties);
				}

				property.SetValue(@object, targetValue, null);
			}

			return @object;
		}
	}
}