using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KuCloud.Infrastructure.JsonConverters;

/// <summary>
///     DateTime DateTime(UTC) Converter
/// </summary>
/// <param name="isJsonSourceUtc">需要转换的 json 数据是否已经是 utc 时间</param>
/// <param name="formats">时间解析格式</param>
public class UtcDateTimeConverter(bool isJsonSourceUtc = false, string[]? formats = null) : JsonConverter<DateTime>
{
    private readonly string[] _formats = (formats ?? [ ]).Union(new[] { "yyyy-MM-ddTHH:mm:ss.fffZ" }).ToArray();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) throw new JsonException("Value is null or empty.");

        foreach (var format in _formats)
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result))
                return isJsonSourceUtc ? DateTime.SpecifyKind(result, DateTimeKind.Utc) : result.ToUniversalTime();

        throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var time = value.Kind != DateTimeKind.Utc ? value.ToUniversalTime() : value;
        writer.WriteStringValue(time.ToString(_formats[0]));
    }
}
