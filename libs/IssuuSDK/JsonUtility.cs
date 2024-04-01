// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace IssuuSDK;

static class JsonUtility
{
	public static JsonSerializerOptions CreateSerializerOptions()
	{
		JsonSerializerOptions options = new()
		{
			WriteIndented = false,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		options.Converters.Add(new UpperCaseJsonStringEnumConverter());

		return options;
	}

	public static JsonSerializerOptions CreateDeserializerOptions()
	{
		JsonSerializerOptions options = new()
		{
			WriteIndented = false,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		options.Converters.Add(new UpperCaseJsonStringEnumConverter());

		return options;
	}
}

public class UpperCaseJsonStringEnumConverter : JsonStringEnumConverter
{
	public UpperCaseJsonStringEnumConverter() : base(namingPolicy: UpperCaseNamingPolicy.Instance) { }
}

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
	public static readonly UpperCaseNamingPolicy Instance = new();

	public override string ConvertName(string name) => name.ToUpperInvariant();
}

public class LowerCaseJsonStringEnumConverter : JsonStringEnumConverter
{
	public LowerCaseJsonStringEnumConverter() : base(namingPolicy: LowerCaseNamingPolicy.Instance) { }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
	public static readonly LowerCaseNamingPolicy Instance = new();

	public override string ConvertName(string name) => name.ToLowerInvariant();
}

public class CamelCaseJsonStringEnumConverter : JsonStringEnumConverter
{
	public CamelCaseJsonStringEnumConverter() : base(JsonNamingPolicy.CamelCase) { }
}
