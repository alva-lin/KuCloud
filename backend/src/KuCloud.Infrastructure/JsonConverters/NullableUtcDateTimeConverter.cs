using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KuCloud.Infrastructure.JsonConverters;

/// <summary>
///     Nullable DateTime(UTC) Converter
/// </summary>
/// <param name="isJsonSourceUtc">json 源值是否已经是 UTC 时间</param>
/// <param name="formats">格式化字符串数组，转为 Json 时，默认使用第一个。如果为空，则为 "yyyy-MM-dd HH:mm:ss"</param>
public class NullableUtcDateTimeConverter(bool isJsonSourceUtc = false, string[]? formats = null)
    : JsonConverter<DateTime?>
{
    private readonly string[] _formats =
        (formats ?? []).Union(new[] {"yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss"}).ToArray();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) return null;

        foreach (var format in _formats)
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out var result))
                return isJsonSourceUtc ? DateTime.SpecifyKind(result, DateTimeKind.Utc) : result.ToUniversalTime();

        throw new JsonException($"Unable to convert \"{value}\" to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            var time = value.Value.Kind != DateTimeKind.Utc ? value.Value.ToUniversalTime() : value.Value;
            writer.WriteStringValue(time.ToString(_formats[0]));
        }
    }
}