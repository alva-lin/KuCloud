using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KuCloud.Infrastructure.JsonConverters;

/// <summary>
///     Nullable DateTime Converter
/// </summary>
/// <param name="formats">格式化字符串数组，转为 Json 时，默认使用第一个。如果为空，则为 "yyyy-MM-dd HH:mm:ss"</param>
public class NullableDateTimeConverter(string[]? formats = null) : JsonConverter<DateTime?>
{
    private readonly string[] _formats =
        (formats ?? []).Union(new[] {"yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss"}).ToArray();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return null;

        foreach (var format in _formats)
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result))
                return result;

        throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString(_formats[0]));
    }
}