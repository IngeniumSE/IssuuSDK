// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Represents a document.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class Document : Model<Document>
{
	/// <summary>
	/// Gets or sets the document changes.
	/// </summary>
	[JsonPropertyName("changes")]
	public DocumentChanges? Changes { get; set; }

	/// <summary>
	/// Gets or sets the document cover images.
	/// </summary>
	[JsonPropertyName("cover")]
	public DocumentCoverImages? Cover { get; set; }

	/// <summary>
	/// Gets or sets when the document was created.
	/// </summary>
	[JsonPropertyName("created")]
	public DateTimeOffset Created { get; set; }

	/// <summary>
	/// Gets or sets the document file info.
	/// </summary>
	[JsonPropertyName("fileInfo")]
	public DocumentFileInfo? FileInfo { get; set; }

	/// <summary>
	/// Gets or sets the document URL.
	/// </summary>
	[JsonPropertyName("location")]
	public string Location { get; set; } = default!;

	/// <summary>
	/// Gets or sets the document owner.
	/// </summary>
	[JsonPropertyName("owner")]
	public string Owner { get; set; } = default!;

	/// <summary>
	/// Gets or sets the document identifier.
	/// </summary>
	[JsonPropertyName("slug")]
	public string Slug { get; set; } = default!;

	/// <summary>
	/// Gets or sets the document state.
	/// </summary>
	[JsonPropertyName("state")]
	public DocumentState State { get; set; }

	public override string ToString()
	{
		var builder = new StringBuilder();

		builder.Append($"Document: [{State}]");
		if (Changes?.Title is { Length: >0 })
		{
			builder.Append($" {Changes.Title} ({Slug})");
		}
		else
		{
			builder.Append($" {Slug}");
		}

		return builder.ToString();
	}
}

[DebuggerDisplay("{ToString(),nq}")]
public class DocumentChanges
{
	/// <summary>
	/// Gets or sets the document access.
	/// </summary>
	[JsonPropertyName("access")]
	public DocumentAccess Access { get; set; }

	/// <summary>
	/// Gets or sets the document description.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the document downloadability.
	/// </summary>
	[JsonPropertyName("downloadable")]
	public bool Downloadable { get; set; }

	/// <summary>
	/// Gets the original publish date.
	/// </summary>
	[JsonPropertyName("originalPublishDate")]
	public DateTimeOffset? OriginalPublishDate { get; set; }

	/// <summary>
	/// Gets or sets the document preview state.
	/// </summary>
	[JsonPropertyName("preview")]
	public bool Preview { get; set; }

	/// <summary>
	/// Gets or sets the document scheduled time.
	/// </summary>
	[JsonPropertyName("scheduledTime")]
	public DateTimeOffset? ScheduledTime { get; set; }

	/// <summary>
	/// Gets or sets whether the show detected links.
	/// </summary>
	[JsonPropertyName("showDetectedLinks")]
	public bool ShowDetectedLinks { get; set; }

	/// <summary>
	/// Gets or sets the document title.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }

	public override string ToString()
		=> $"Changes: [{Access}] {Title} ({(Downloadable ? "Downloadable" : "Not Downloadable")}, {(Preview ? "Preview" : "Not Preview")})";
}

[DebuggerDisplay("{ToString(),nq}")]
public class DocumentCoverImages
{
	/// <summary>
	/// Gets or sets the large cover image.
	/// </summary>
	[JsonPropertyName("large")]
	public DocumentImage? Large { get; set; }

	/// <summary>
	/// Gets or sets the medium cover image.
	/// </summary>
	[JsonPropertyName("medium")]
	public DocumentImage? Medium { get; set; }

	/// <summary>
	/// Gets or sets the small cover image.
	/// </summary>
	[JsonPropertyName("small")]
	public DocumentImage? Small { get; set; }

	public override string ToString()
	{
		int images = 0;
		if (Large is not null)
		{
			images++;
		}
		if (Medium is not null)
		{
			images++;
		}
		if (Small is not null)
		{
			images++;
		}

		return $"{images} image{(images != 1 ? "s" : string.Empty)}";
	}
}

public class DocumentImage
{
	/// <summary>
	/// Gets or sets the image height.
	/// </summary>
	[JsonPropertyName("height")]
	public int Height { get; set; }

	/// <summary>
	/// Gets or sets the image URL.
	/// </summary>
	[JsonPropertyName("url")]
	public string Url { get; set; } = default!;

	/// <summary>
	/// Gets or sets the image width.
	/// </summary>
	[JsonPropertyName("width")]
	public int Width { get; set; }
}

public class DocumentFileInfo
{
	/// <summary>
	/// Gets or sets the document file conversion status.
	/// </summary>
	[JsonPropertyName("conversionStatus")]
	public DocumentConverstionStatus ConversionStatus { get; set; }

	/// <summary>
	/// Gets or sets whether the document is confirmed as copyrighted.
	/// </summary>
	[JsonPropertyName("isCopyrightConfirmed")]
	public bool IsCopyightConfirmed { get; set; }

	/// <summary>
	/// Gets or sets the document file name.
	/// </summary>
	[JsonPropertyName("name")]
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the document page count.
	/// </summary>
	[JsonPropertyName("pageCount")]
	public int PageCount { get; set; }

	/// <summary>
	/// Gets or sets the document file size.
	/// </summary>
	[JsonPropertyName("size")]
	public long Size { get; set; }

	/// <summary>
	/// Gets or sets the document type.
	/// </summary>
	[JsonPropertyName("type")]
	public DocumentfileType Type { get; set; }
}

/// <summary>
/// Represents the possible document states.
/// </summary>
public enum DocumentState
{
	Draft,
	Published,
	Scheduled,
	Unpublished,
	Quaratined
}

/// <summary>
/// Gets the possible access types for an Issuu document.
/// </summary>
public enum DocumentAccess
{
	Public,
	Private
}

/// <summary>
/// Gets the possible document conversion statuses.
/// </summary>
public enum DocumentConverstionStatus
{
	Done,
	Converting,
	Failed
}

/// <summary>
/// Gets the possible document types.
/// </summary>
public enum DocumentType
{
	Editorial,
	Book,
	Promotion,
	Other
}

/// <summary>
/// Gets the possible document file types.
/// </summary>
public enum DocumentfileType
{
	Unknown,
	DOC,
	ODP,
	ODT,
	PDF,
	PPT,
	RTF,
	SXI,
	SXW,
	WPD,
	EPUB,
	MOBI
}
