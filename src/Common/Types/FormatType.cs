using System.Runtime.Serialization;


namespace Keebox.Common.Types
{
	public enum FormatType
	{
		[EnumMember(Value = "None")]
		None,
		[EnumMember(Value = "Env")]
		Env,
		[EnumMember(Value = "Json")]
		Json,
		[EnumMember(Value = "Xml")]
		Xml,
		[EnumMember(Value = "Yaml")]
		Yaml
	}
}