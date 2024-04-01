// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Represents the settings for displaying a publication.
/// </summary>
public class DisplaySettings
{
	/// <summary>
	/// Gets or sets a value indicating whether to automatically flip pages.
	/// </summary>
	[JsonPropertyName("autoflip")]
	public bool AutoFlip { get; set; }

	/// <summary>
	/// Gets or sets the background color.
	/// </summary>
	[JsonPropertyName("backgroundColor")]
	public string? BackgroundColor { get; set; }

	/// <summary>
	/// Gets or sets the background image position.
	/// </summary>
	[JsonPropertyName("backgroundImagePosition"), JsonConverter(typeof(CamelCaseJsonStringEnumConverter))]
	public ShareBackgroundImagePosition backgroundImagePosition { get; set; } = ShareBackgroundImagePosition.TopLeft;

	/// <summary>
	/// Gets or sets the background image URL.
	/// </summary>
	[JsonPropertyName("backgroundImageUrl")]
	public string? BackgroundImageUrl { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to hide the share button.
	/// </summary>
	[JsonPropertyName("hideShare")]
	public bool HideShare { get; set; }

	/// <summary>
	/// Gets or sets the logo URL.
	/// </summary>
	[JsonPropertyName("logoUrl")]
	public string? LogoUrl { get; set; }

	/// <summary>
	/// Gets or sets the page layout.
	/// </summary>
	[JsonPropertyName("pageLayout"), JsonConverter(typeof(LowerCaseJsonStringEnumConverter))]
	public SharePageLayout PageLayout { get; set; } = SharePageLayout.Double;

	/// <summary>
	/// Gets or sets a value indicating whether to show other publications.
	/// </summary>
	[JsonPropertyName("showOtherPublications")]
	public bool ShowOtherPublications { get; set; }

	/// <summary>
	/// Gets or sets the start page.
	/// </summary>
	[JsonPropertyName("startPage")]
	public int StartPage { get; set; } = 1;
}

/// <summary>
/// Represents the result of a publication share operation.
/// </summary>
public class ShareResult
{
	/// <summary>
	/// Gets or sets the URL.
	/// </summary>
	[JsonPropertyName("url")]
	public string Url { get; set; } = default!;
}

/// <summary>
/// Represents the layout of the share page.
/// </summary>
public enum SharePageLayout
{
	Double,
	Single
}

/// <summary>
/// Represents the position of the background image.
/// </summary>
public enum ShareBackgroundImagePosition
{
	TopLeft,
	Stretch
}
