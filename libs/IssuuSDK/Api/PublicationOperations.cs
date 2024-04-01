// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Runtime;

using FluentValidation.Validators;

namespace IssuuSDK.Api;

partial interface IIssuuApiClient
{
	/// <summary>
	/// Gets the /publications operations.
	/// </summary>
	public IPublicationOperations Publications { get; }
}

partial class IssuuApiClient
{
	Lazy<IPublicationOperations>? _publications;
	public IPublicationOperations Publications => (_publications ??= Defer<IPublicationOperations>(
		c => new PublicationOperations(new("/publications"), c))).Value;
}

/// <summary>
/// Providers operations for the /publications endpoint.
/// </summary>
public partial interface IPublicationOperations
{
	/// <summary>
	/// Delete the publication with the given slug.
	/// HTTP DELETE /publications/{slug}
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse> DeletePublicationAsync(
		string slug,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the publication with the given slug.
	/// HTTP GET /publications/{slug}
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document>> GetPublicationAsync(
		string slug,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the assets associated with the given publication.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="assetType">The asset type.</param>
	/// <param name="documentPageNumber">The document page number.</param>
	/// <param name="page">The current page to fetch.</param>
	/// <param name="size">The size of page to fetch.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<AssetResult[]>> GetPublicationAssetsAsync(
		string slug,
		AssetType assetType,
		int? documentPageNumber = null,
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the embed code for the given publication.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="settings">The embed settings.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<EmbedResult>> GetPublicationEmbedAsync(
		string slug,
		EmbedSettings? settings = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the full-screen share URL for the given publication.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="settings">The display settings.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<ShareResult>> GetPublicationFullScreenShareAsync(
		string slug,
		DisplaySettings? settings = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the reader share URL for the given publication.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<ShareResult>> GetPublicationReaderShareAsync(
		string slug,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the full-screen share URL for the given publication.
	/// </summary>
	/// <param name="slug">The document slug.</param>
	/// <param name="settings">The display settings.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<QrShareResult>> GetPublicationQrCodeAsync(
		string slug,
		QrDisplaySettings? settings = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the publications.
	/// HTTP GET /publications
	/// </summary>
	/// <param name="page">The current page to fetch.</param>
	/// <param name="size">The size of page to fetch.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<Document[]>> GetPublicationsAsync(
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default);
}

public class PublicationOperations(PathString path, ApiClient client) : IPublicationOperations
{
	public async Task<IssuuResponse> DeletePublicationAsync(
		string slug,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(HttpMethod.Delete, path + $"/{slug}");

		return await client.SendAsync(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document>> GetPublicationAsync(
		string slug,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(HttpMethod.Get, path + $"/{slug}");

		return await client.FetchSingleAsync<Document>(request, cancellationToken);
	}

	public async Task<IssuuResponse<AssetResult[]>> GetPublicationAssetsAsync(
		string slug,
		AssetType assetType,
		int? documentPageNumber = null,
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		string assetTypeValue= assetType switch
		{
			AssetType.Cover => "cover1",
			AssetType.Image => "image1",
			AssetType.Text => "text1",
			_ => throw new ArgumentOutOfRangeException(nameof(assetType))
		};

		var query = new QueryStringBuilder()
			.AddParameter("assetType", assetTypeValue)
			.AddParameter("documentPageNumber", documentPageNumber)
			.Build();

		var request = new IssuuRequest(
			HttpMethod.Get,
			path + $"/{slug}/assets",
			query: query,
			page: page,
			size: size);

		if (assetType != AssetType.Cover)
		{
			return await client.FetchManyAsync<AssetResult[]>(request, cancellationToken);
		}

		var result = await client.FetchManyAsync<AssetResult[][]>(request, cancellationToken);

		var data = result.Data is { Length: > 0 } ? result.Data[0] : null;

		return new IssuuResponse<AssetResult[]>(
			request.Method,
			result.RequestUri,
			result.IsSuccess,
			result.StatusCode,
			data,
			result.Meta,
			result.RateLimiting,
			result.Links,
			result.Error
		)
		{
			RequestContent = result.RequestContent,
			ResponseContent = result.ResponseContent
		};
	}

	public async Task<IssuuResponse<EmbedResult>> GetPublicationEmbedAsync(
		string slug,
		EmbedSettings? settings = default,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		settings ??= new();

		var query = new QueryStringBuilder()
			.AddParameter("responsive", settings.Responsive ? "true" : "false")
			.AddParameter("width", settings.Width)
			.AddParameter("height", settings.Height)
			.AddParameter("hideIssuuLogo", settings.HideIssuuLogo ? "true" : "false")
			.AddParameter("hideShareButton", settings.HideShareButton ? "true" : "false")
			.AddParameter("showOtherPublications", settings.ShowOtherPublications ? "true" : "false")
			.Build();

		var request = new IssuuRequest(
			HttpMethod.Get,
			path + $"/{slug}/embed",
			query: query);

		return await client.FetchSingleAsync<EmbedResult>(request, cancellationToken);
	}

	public async Task<IssuuResponse<ShareResult>> GetPublicationFullScreenShareAsync(
		string slug,
		DisplaySettings? settings = default,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest<DisplaySettings>(
			HttpMethod.Post,
			path + $"/{slug}/fullscreen",
			settings ?? new());

		return await client.FetchSingleAsync<DisplaySettings, ShareResult>(request, cancellationToken);
	}

	public async Task<IssuuResponse<ShareResult>> GetPublicationReaderShareAsync(
		string slug,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest(
			HttpMethod.Get,
			path + $"/{slug}/reader");

		return await client.FetchSingleAsync<ShareResult>(request, cancellationToken);
	}

	public async Task<IssuuResponse<QrShareResult>> GetPublicationQrCodeAsync(
		string slug,
		QrDisplaySettings? settings = default,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNullOrEmpty(slug, nameof(slug));

		var request = new IssuuRequest<QrDisplaySettings>(
			HttpMethod.Post,
			path + $"/{slug}/qrcode",
			settings ?? new());

		return await client.FetchSingleAsync<QrDisplaySettings, QrShareResult>(request, cancellationToken);
	}

	public async Task<IssuuResponse<Document[]>> GetPublicationsAsync(
		int? page = null,
		int? size = null,
		CancellationToken cancellationToken = default)
	{
		var request = new IssuuRequest(HttpMethod.Get, path, page: page, size: size);

		return await client.FetchManyAsync<Document[]>(request, cancellationToken);
	}
}

