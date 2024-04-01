// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Gets the possible asset types.
/// </summary>
public enum AssetType
{
	Text,
	Image,
	Cover
}

/// <summary>
/// Represents an asset set.
/// </summary>
public class AssetSet
{
	/// <summary>
	/// Gets the text assets.
	/// </summary>
	[JsonPropertyName("text")]
	public Dictionary<string, string[]>? Text { get; set; }

	/// <summary>
	/// Gets the image assets.
	/// </summary>
	[JsonPropertyName("image")]
	public Dictionary<string, string[]>? Images { get; set; }
}

/// <summary>
/// Represents a thumbnail set.
/// </summary>
public class ThumbnailSet
{
	/// <summary>
	/// Gets the small thumbnail.
	/// </summary>
	[JsonPropertyName("small")]
	public string? Small { get; set; }

	/// <summary>
	/// Gets the medium thumbnail.
	/// </summary>
	[JsonPropertyName("medium")]
	public string? Medium { get; set; }

	/// <summary>
	/// Gets the large thumbnail.
	/// </summary>
	[JsonPropertyName("large")]
	public string? Large { get; set; }
}

/// <summary>
/// Represents an asset result.
/// </summary>
public class AssetResult
{
	/// <summary>
	/// The set of assets.
	/// </summary>
	[JsonPropertyName("assets")]
	public AssetSet? Assets { get; set; }

	/// <summary>
	/// The set of thumbnails.
	/// </summary>
	[JsonPropertyName("thumbnails")]
	public ThumbnailSet? Thumbnails { get; set; }

	/// <summary>
	/// Gets the page image.
	/// </summary>
	[JsonPropertyName("pageImage")]
	public string? PageImage { get; set; }

	/// <summary>
	/// Gets the page number.
	/// </summary>
	[JsonPropertyName("pageNumber")]
	public int PageNumber { get; set; }
}
