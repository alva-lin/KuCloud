using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KuCloud.Infrastructure.JsonConverters;

/// <summary>
///     DateTime Converter
/// </summary>
/// <param name="formats">时间解析格式</param>
public class DateTimeConverter(string[]? formats = null) : JsonConverter<DateTime>
{
    private readonly string[] _formats = (formats ?? [ ]).Union(new[] { "yyyy-MM-ddTHH:mm:ss.fffZ" }).ToArray();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) throw new JsonException("Value is null or empty.");

        foreach (var format in _formats)
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result))
                return result;

        throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_formats[0]));
    }
}
