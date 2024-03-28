// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json.Serialization;

namespace IssuuSDK;

/// <summary>
/// Represents a draft document.
/// </summary>
/// <param name="title">The document title.</param>
/// <param name="description">The document description.</param>
/// <param name="fileUrl">The URL of the file to be processed (if not uploaded separately).</param>
/// <param name="access">The access setting.</param>
/// <param name="type">The document type.</param>
/// <param name="fileId">The ID of the file.</param>
/// <param name="confirmCopyright">Is copyright confirmed for this document?</param>
/// <param name="preview">Is the document a preview of larger content.</param>
/// <param name="showDetectedLinks">Should links be extracted from the processed document.</param>
/// <param name="originalPublishDate">The original publish date for historically published content.</param>
/// <param name="scheduledTime">The scheduled date/time.</param>
public class Draft(
	string? title = default,
	string? description = default,
	string? fileUrl = default,
	DocumentAccess access = DocumentAccess.Private,
	DocumentType type = DocumentType.Other,
	long? fileId = default,
	bool confirmCopyright = default,
	bool downloadable = default,
	bool preview = default,
	bool showDetectedLinks = default,
	DateTimeOffset? originalPublishDate = default,
	DateTimeOffset? scheduledTime = default)
{
	[JsonPropertyName("confirmCopyright")]
	public bool ConfirmCopyright => confirmCopyright;

	[JsonPropertyName("fileUrl")]
	public string? FileUrl => fileUrl;

	[JsonPropertyName("info")]
	public DraftInfo Info => new DraftInfo(
		title, description, access, type,
		fileId, downloadable, preview, showDetectedLinks,
		originalPublishDate, scheduledTime);
}

/// <summary>
/// Represents a draft document info
/// </summary>
/// <param name="title">The document title.</param>
/// <param name="description">The document description.</param>
/// <param name="access">The access setting.</param>
/// <param name="type">The document type.</param>
/// <param name="fileId">The ID of the file.</param>
/// <param name="preview">Is the document a preview of larger content.</param>
/// <param name="showDetectedLinks">Should links be extracted from the processed document.</param>
/// <param name="originalPublishDate">The original publish date for historically published content.</param>
/// <param name="scheduledTime">The scheduled date/time.</param>
public class DraftInfo(
	string? title = default,
	string? description = default,
	DocumentAccess access = DocumentAccess.Private,
	DocumentType type = DocumentType.Other,
	long? fileId = default,
	bool downloadable = default,
	bool preview = default,
	bool showDetectedLinks = default,
	DateTimeOffset? originalPublishDate = default,
	DateTimeOffset? scheduledTime = default)
{
	[JsonPropertyName("fileId")]
	public long? File => fileId;

	[JsonPropertyName("access")]
	public DocumentAccess Access => access;

	[JsonPropertyName("title")]
	public string? Title => title;

	[JsonPropertyName("description")]
	public string? Description => description;

	[JsonPropertyName("preview")]
	public bool Preview => preview;

	[JsonPropertyName("type"), JsonConverter(typeof(LowerCaseJsonStringEnumConverter))]
	public DocumentType Type => type;

	[JsonPropertyName("showDetectedLinks")]
	public bool ShowDetectedLinks => showDetectedLinks;

	[JsonPropertyName("downloadable")]
	public bool Downloadable => downloadable;

	[JsonPropertyName("originalPublishDate")]
	public DateTimeOffset? OriginalPublishDate => originalPublishDate;

	[JsonPropertyName("scheduledTime")]
	public DateTimeOffset? ScheduledTime => scheduledTime;
}

public class PublishRequest
{
	/// <summary>
	/// Gets or sets the desired name for the public document.
	/// </summary>
	[JsonPropertyName("desiredName")]
	public string? DesiredName { get; set; }
}

public class PublishResult
{
	/// <summary>
	/// Gets the public location of the document (for displaying).
	/// </summary>
	[JsonPropertyName("publicLocation")]
	public string PublicLocation { get; set; } = default!;

	/// <summary>
	/// Gets the private location of the document (for editing).
	/// </summary>
	[JsonPropertyName("location")]
	public string Location { get; set; } = default!;
}
