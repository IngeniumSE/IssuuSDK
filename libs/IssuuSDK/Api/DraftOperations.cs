// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using SlugGenerator;

namespace IssuuSDK.Api;

partial interface IIssuuApiClient
{
	/// <summary>
	/// Gets the /draft operations.
	/// </summary>
	public IDraftOperations Drafts { get; }
}

partial class IssuuApiClient
{
	Lazy<IDraftOperations>? _drafts;
	public IDraftOperations Drafts => (_drafts ??= Defer<IDraftOperations>(
		c => new DraftOperations(new("/drafts"), c))).Value;
}

/// <summary>
/// Providers operations for the /drafts endpoint.
/// </summary>
public partial interface IDraftOperations
{
	/// <summary>
	/// Creates a draft.
	/// HTTP POST /drafts
	/// </summary>
	/// <param name="draft">The draft document.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document>> CreateDraftAsync(
		Draft draft,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Delete the draft with the given slug.
	/// HTTP DELETE /drafts/{slug}
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse> DeleteDraftAsync(
		string slug,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the draft with the given slug.
	/// HTTP GET /drafts/{slug}
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document>> GetDraftAsync(
		string slug,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the drafts.
	/// HTTP GET /drafts
	/// </summary>
	/// <param name="page">The current page to fetch.</param>
	/// <param name="size">The size of page to fetch.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document[]>> GetDraftsAsync(
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Publishes the draft with the given slug.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="desiredName">The desired name for the public document.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<PublishResult>> PublishDraftAsync(
		string slug,
		string? desiredName = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates a draft.
	/// HTTP PATCH /drafts/{slug}
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="draft">The draft document.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document>> UpdateDraftAsync(
		string slug,
		Draft draft,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Uploads file content to attach to a draft.
	/// HTTP PATH /drafts/{slug}/upload
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="filePath">The file path.</param>
	/// <param name="confirmCopyright">Has confirmation of copyright be applied?</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document>> UploadDocumentContentAsync(
		string slug,
		string filePath,
		bool confirmCopyright = false,
		CancellationToken cancellationToken = default);
}

public class DraftOperations(PathString path, ApiClient client) : IDraftOperations
{
	public async Task<IssuuResponse<Document>> CreateDraftAsync(
		Draft draft,
		CancellationToken cancellationToken = default)
	{
		var request = new IssuuRequest<Draft>(HttpMethod.Post, path, draft);

		return await client.FetchSingleAsync<Draft, Document>(request, cancellationToken);
	}

	public async Task<IssuuResponse> DeleteDraftAsync(
		string slug,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(HttpMethod.Delete, path + $"/{slug}");

		return await client.SendAsync(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document>> GetDraftAsync(
		string slug,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(HttpMethod.Get, path + $"/{slug}");

		return await client.FetchSingleAsync<Document>(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document[]>> GetDraftsAsync(
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default)
	{
		var request = new IssuuRequest(HttpMethod.Get, path, page: page, size: size);

		return await client.FetchManyAsync<Document[]>(request, cancellationToken);
	}

	public async Task<IssuuResponse<PublishResult>> PublishDraftAsync(
		string slug,
		string? desiredName = null,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		if (desiredName is { Length: > 0})
		{
			desiredName = desiredName.GenerateSlug();
		}

		var request = new IssuuRequest<PublishRequest>(
			HttpMethod.Post,
			path + $"/{slug}/publish",
			new PublishRequest
			{
				DesiredName = desiredName
			});

		return await client.FetchSingleAsync<PublishRequest, PublishResult>(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document>> UpdateDraftAsync(
		string slug,
		Draft draft,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest<Draft>(new HttpMethod("PATCH"), path + $"/{slug}", draft);

		return await client.FetchSingleAsync<Draft, Document>(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document>> UploadDocumentContentAsync(
		string slug,
		string filePath,
		bool confirmCopyright = false,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(
			new HttpMethod("PATCH"), path + $"/{slug}/upload",
			useMultipartContent: true,
			formData: new()
			{
				["confirmCopyright"] = confirmCopyright ? "true" : "false"
			},
			filePath: filePath);

		return await client.FetchSingleAsync<Document>(request, cancellationToken);
	}
}
