using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KuCloud.Infrastructure.JsonConverters;

/// <summary>
///     TimeSpan Converter
/// </summary>
/// <param name="format">默认为 d':'hh':'mm':'ss'.'fff</param>
/// <param name="absolute">是否只读取/写入时间绝对值</param>
public class TimeSpanConverter(bool absolute = false, string? format = null) : JsonConverter<TimeSpan>
{
    private const string DefaultFormat = "d':'hh':'mm':'ss'.'fff";

    private readonly string _format = format ?? DefaultFormat;

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value)) throw new JsonException("Value is null or empty.");

        var negative = false;
        if (value.StartsWith('-'))
        {
            value = value[1..];
            negative = true;
        }

        if (TimeSpan.TryParseExact(value, _format, null, TimeSpanStyles.None, out var result))
        {
            return !absolute && negative ? result.Negate() : result;
        }

        throw new JsonException($"Unable to convert \"{value}\" to TimeSpan.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        var isNegative = value < TimeSpan.Zero;
        var duration = value.Duration();
        writer.WriteStringValue((isNegative ? "-" : string.Empty) + duration.ToString(_format));
    }
}
