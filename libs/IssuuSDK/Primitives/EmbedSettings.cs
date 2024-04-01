// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Represents the settings for embedding a publication.
/// </summary>
/// <param name="responsive">Flag stating whether a responsive embed should be generated.</param>
/// <param name="width">The width of the embed.</param>
/// <param name="height">The height of the embed.</param>
/// <param name="hideIssuuLogo">Flag stating whether to hide the Issuu logo.</param>
/// <param name="hideShareButton">Flag stating whether to hide the share button.</param>
/// <param name="showOtherPublications">Flag stating whether to show other publications.</param>
public class EmbedSettings(
	bool responsive = true,
	string? width = "100%",
	string? height = "100%",
	bool hideIssuuLogo = false,
	bool hideShareButton = false,
	bool showOtherPublications = false)
{
	/// <summary>
	/// Gets a value indicating whether a responsive embed should be generated.
	/// </summary>
	public bool Responsive => responsive;

	/// <summary>
	/// Gets the width of the embed.
	/// </summary>
	public string? Width => width;

	/// <summary>
	/// Gets the height of the embed.
	/// </summary>
	public string? Height => height;

	/// <summary>
	/// Gets a value indicating whether to hide the Issuu logo.
	/// </summary>
	public bool HideIssuuLogo => hideIssuuLogo;

	/// <summary>
	/// Gets a value indicating whether to hide the share button.
	/// </summary>
	public bool HideShareButton => hideShareButton;

	/// <summary>
	/// Gets a value indicating whether to show other publications.
	/// </summary>
	public bool ShowOtherPublications => showOtherPublications;
}

/// <summary>
/// Represents the result of an embed operation.
/// </summary>
public class EmbedResult
{
	/// <summary>
	/// Gets the embed code.
	/// </summary>
	[JsonPropertyName("embed")]
	public string Embed { get; set; } = default!;
}
