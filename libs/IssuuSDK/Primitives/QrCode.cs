// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Represents the settings for displaying a QR code.
/// </summary>
public class QrDisplaySettings
{
	/// <summary>
	/// The format of the QR code.
	/// </summary>
	[JsonPropertyName("format"), JsonConverter(typeof(UpperCaseJsonStringEnumConverter))]
	public QrFormat Format { get; set; } = QrFormat.Png;

	/// <summary>
	/// The display settings for the full screen share
	/// </summary>
	[JsonPropertyName("fullScreenSettings")]
	public DisplaySettings? FullScreenSettings { get; set; }
}

/// <summary>
/// Represents the result of a QR code share operation.
/// </summary>
public class QrShareResult
{
	[JsonPropertyName("qrCodeUrl")]
	public string QrCodeUrl { get; set; } = default!;

	[JsonPropertyName("pointedUrl")]
	public string PointedUrl { get; set; } = default!;
}

/// <summary>
/// Represents the format of the QR code.
/// </summary>
public enum QrFormat
{
	Png,
	Svg
}
