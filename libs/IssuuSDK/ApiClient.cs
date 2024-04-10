// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace IssuuSDK;

/// <summary>
/// Defines the contract for an API client.
/// </summary>
public interface IApiClient
{
	/// <summary>
	/// Sends the specified request to the Issuu API.
	/// </summary>
	/// <param name="request">The Issuu request.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse> SendAsync(
		IssuuRequest request,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends the specified request to the Issuu API.
	/// </summary>
	/// <param name="request">The Issuu request.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse> SendAsync<TData>(
		IssuuRequest<TData> request,
		CancellationToken cancellationToken = default)
		where TData : notnull;

	/// <summary>
	/// Sends the specified request to the Issuu API and returns the response data.
	/// </summary>
	/// <param name="request">The Issuu request.</param>
	/// <param name="responseFactory">The response data factory.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<TData>> FetchAsync<TData>(
		IssuuRequest request,
		MapResponse<TData> responseFactory,
		CancellationToken cancellationToken = default)
		where TData : class;

	/// <summary>
	/// Sends the specified request to the Issuu API and returns the response data.
	/// </summary>
	/// <param name="request">The Issuu request.</param>
	/// <param name="responseFactory">The response data factory.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The Issuu response.</returns>
	Task<IssuuResponse<TResponseData>> FetchAsync<TRequestData, TResponseData>(
		IssuuRequest<TRequestData> request,
		MapResponse<TResponseData> responseFactory,
		CancellationToken cancellationToken = default)
		where TRequestData : notnull
		where TResponseData : class;
}

/// <summary>
/// Provides a base implementation of an API client.
/// </summary>
public abstract class ApiClient : IApiClient
{
	readonly HttpClient _http;
	readonly IssuuSettings _settings;
	readonly JsonSerializerOptions _serializerOptions = JsonUtility.CreateSerializerOptions();
	readonly JsonSerializerOptions _deserializerOptions = JsonUtility.CreateDeserializerOptions();
	readonly Uri _baseUrl;

	protected ApiClient(HttpClient http, IssuuSettings settings)
	{
		_http = Ensure.IsNotNull(http, nameof(http));
		_settings = Ensure.IsNotNull(settings, nameof(settings));
		_settings.Validate();

		_baseUrl = new Uri(settings.BaseUrl);
	}

	#region Send and Fetch

	#region Send
	public async Task<IssuuResponse> SendAsync(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNull(request, nameof(request));
		using var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		string? requestContent = await CaptureRequestContent(httpReq).ConfigureAwait(false);
		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
				.ConfigureAwait(false);

			transformedResponse.RequestContent = requestContent;
			transformedResponse.ResponseContent = await CaptureResponseContent(httpResp).ConfigureAwait(false);

			return transformedResponse;
		}
		catch (Exception ex)
		{
			return await GetIssuuErrorResponse(httpReq, requestContent, httpResp, ex).ConfigureAwait(false);
		}
		finally
		{
			httpReq?.Dispose();
		}
	}

	public async Task<IssuuResponse> SendAsync<TRequest>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
	{
		Ensure.IsNotNull(request, nameof(request));
		using var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		string? requestContent = await CaptureRequestContent(httpReq).ConfigureAwait(false);
		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken).ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
				.ConfigureAwait(false);

			transformedResponse.RequestContent = requestContent;
			transformedResponse.ResponseContent = await CaptureResponseContent(httpResp).ConfigureAwait(false);

			return transformedResponse;
		}
		catch (Exception ex)
		{
			return await GetIssuuErrorResponse(httpReq, requestContent, httpResp, ex).ConfigureAwait(false);
		}
		finally
		{
			httpReq?.Dispose();
		}
	}
	#endregion

	#region Fetch
	public async Task<IssuuResponse<TData>> FetchAsync<TData>(
		IssuuRequest request,
		MapResponse<TData> mapper,
		CancellationToken cancellationToken = default)
		where TData : class
	{
		Ensure.IsNotNull(request, nameof(request));
		using var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		string? requestContent = await CaptureRequestContent(httpReq).ConfigureAwait(false);
		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				mapper,
				cancellationToken)
				.ConfigureAwait(false);

			transformedResponse.RequestContent = requestContent;
			transformedResponse.ResponseContent = await CaptureResponseContent(httpResp).ConfigureAwait(false);

			return transformedResponse;
		}
		catch (Exception ex)
		{
			return await GetIssuuErrorResponse<TData>(httpReq, requestContent, httpResp, ex).ConfigureAwait(false);
		}
		finally
		{
			httpReq?.Dispose();
		}
	}

	public async Task<IssuuResponse<TResponseData>> FetchAsync<TRequestData, TResponseData>(
		IssuuRequest<TRequestData> request,
		MapResponse<TResponseData> mapper,
		CancellationToken cancellationToken = default)
		where TRequestData : notnull
		where TResponseData : class
	{
		Ensure.IsNotNull(request, nameof(request));
		using var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		string? requestContent = await CaptureRequestContent(httpReq).ConfigureAwait(false);
		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				mapper,
				cancellationToken)
				.ConfigureAwait(false);

			transformedResponse.RequestContent = requestContent;
			transformedResponse.ResponseContent = await CaptureResponseContent(httpResp).ConfigureAwait(false);

			return transformedResponse;
		}
		catch (Exception ex)
		{
			return await GetIssuuErrorResponse<TResponseData>(
				httpReq,
				requestContent,
				httpResp,
				ex).ConfigureAwait(false);
		}
		finally
		{
			httpReq?.Dispose();
		}
	}
	#endregion

	#region Fetch Many
	protected internal async Task<IssuuResponse<TResponse>> FetchManyAsync<TResponse>(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		var response = await FetchAsync(request, MapMany<TResponse>, cancellationToken)
			.ConfigureAwait(false);

		if (response.Meta is not null)
		{
			response.Meta.Page = request.Page.GetValueOrDefault(1);
		}

		return response;
	}

	protected internal async Task<IssuuResponse<TResponse>> FetchManyAsync<TRequest, TResponse>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
		where TResponse : class
	{
		var response = await FetchAsync(request, MapMany<TResponse>, cancellationToken)
			.ConfigureAwait(false);

		if (response.Meta is not null)
		{
			response.Meta.Page = request.Page.GetValueOrDefault(1);
		}

		return response;
	}
	#endregion

	#region Fetch Single
	protected internal Task<IssuuResponse<TResponse>> FetchSingleAsync<TResponse>(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
		where TResponse : class
		=> FetchAsync(request, MapSingle<TResponse>, cancellationToken);

	protected internal Task<IssuuResponse<TResponse>> FetchSingleAsync<TRequest, TResponse>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
		where TResponse : class
		=> FetchAsync(request, MapSingle<TResponse>, cancellationToken);
	#endregion

	#endregion

	#region Preprocessing
	protected internal HttpRequestMessage CreateHttpRequest(
		IssuuRequest request)
	{
		string pathAndQuery = request.Resource.ToUriComponent();
		var query = BuildQuery(request.Query, request.Page, request.Size);

		if (query.HasValue)
		{
			pathAndQuery += query.Value.ToUriComponent();
		}
		var uri = new Uri(_baseUrl, CombineRelativePaths(_baseUrl.PathAndQuery, pathAndQuery));


		var message = new HttpRequestMessage(request.Method, uri);
		message.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_settings.Token}");

		if (request.UseMultipartContent)
		{
			var content = new MultipartFormDataContent();
			if (request.FormData is { Count: > 0 })
			{
				foreach (var item in request.FormData)
				{
					content.Add(new StringContent(item.Value), item.Key);
				}
			}

			if (request.FilePath is not null)
			{
				string fileName = request.FileName is { Length: > 0 } ? request.FileName : Path.GetFileName(request.FilePath);

				content.Add(
					new StreamContent(new FileStream(request.FilePath, FileMode.Open)),
					"file",
					fileName);
			}
			else if (request.FileStream is not null)
			{
				content.Add(
					new StreamContent(request.FileStream),
					"file",
					request.FileName);
			}

			message.Content = content;
		}

		return message;
	}

	protected internal HttpRequestMessage CreateHttpRequest<TRequest>(
		IssuuRequest<TRequest> request)
		where TRequest : notnull
	{
		var message = CreateHttpRequest((IssuuRequest)request);

		message.Content = JsonContent.Create(
			inputValue: request.Data, options: _serializerOptions);

		return message;
	}
	#endregion

	#region Postprocessing
	protected internal async Task<IssuuResponse> TransformResponse(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		CancellationToken cancellationToken = default)
	{
		var rateLimiting = GetRateLimiting(response);

		if (response.IsSuccessStatusCode)
		{
			return new IssuuResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				rateLimiting: rateLimiting
			);
		}
		else
		{
			Error? error = await GetIssuuError(response, cancellationToken).ConfigureAwait(false);

			return new IssuuResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				error: error,
				rateLimiting: rateLimiting
			);
		}
	}

	protected internal async Task<IssuuResponse<TData>> TransformResponse<TData>(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		MapResponse<TData> mapper,
		CancellationToken cancellationToken = default)
		where TData : class
	{
		var rateLimiting = GetRateLimiting(response);

		if (response.IsSuccessStatusCode)
		{
			var mapped = await mapper(response, cancellationToken).ConfigureAwait(false);

			return new IssuuResponse<TData>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				data: mapped?.Data,
				meta: mapped?.Meta,
				rateLimiting: rateLimiting,
				links: mapped?.Links
			);
		}
		else
		{
			Error? error = await GetIssuuError(response, cancellationToken).ConfigureAwait(false);

			return new IssuuResponse<TData>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				error: error,
				rateLimiting: rateLimiting
			);
		}
	}

	async Task<MappedResponse<TResponse>> MapMany<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
		where TResponse : class
	{
		DataContainer<TResponse>? data = default;
		Meta? meta = default;
		Dictionary<string, string>? links = default;
		if (response.Content is not null)
		{
			data = await response.Content.ReadFromJsonAsync<DataContainer<TResponse>>(
				_deserializerOptions, cancellationToken)
				.ConfigureAwait(false);

			if (data is not null)
			{
				if (data.Count.HasValue && data.PageSize.HasValue)
				{
					meta = new()
					{
						PageSize = data.PageSize.Value,
						TotalItems = data.Count.Value,
						TotalPages = (int)Math.Ceiling(data.Count.Value / (double)data.PageSize.Value)
					};
				}

				if (data.Links is { Count: > 0 })
				{
					links = new();
					foreach (var link in data.Links)
					{
						links.Add(link.Key, link.Value.Href);
					}
				}
			}
		}

		return new MappedResponse<TResponse>(data?.Results, meta, links);
	}

	async Task<MappedResponse<TResponse>> MapSingle<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
		where TResponse : class
	{
		TResponse? data = null;
		if (response.Content is not null)
		{
			data = await response.Content.ReadFromJsonAsync<TResponse>(
				_deserializerOptions, cancellationToken)
				.ConfigureAwait(false);
		}

		return new MappedResponse<TResponse>(data);
	}

	async Task<Error> GetIssuuError(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		Error error;
		if (response.Content is not null)
		{
			try
			{
				var result = await response.Content.ReadFromJsonAsync<ErrorContainer>(
					_deserializerOptions, cancellationToken)
					.ConfigureAwait(false);

				Dictionary<string, string>? errorItems = null;
				var content = result?.Details ?? result?.Fields;
				if (content is { Count: > 0 })
				{
					errorItems = content
						.ToDictionary(x => x.Key, x => x.Value.Message);
				}

				if (result?.Message is not { Length: > 0 })
				{

					error = new(Resources.ApiClient_UnknownResponse, errorItems);
				}
				else
				{
					error = new(result.Message, errorItems);
				}
			}
			catch (Exception ex)
			{
				error = new Error(Resources.ApiClient_DidntHandleErrorContent, exception: ex);
			}
		}
		else
		{
			error = new Error(Resources.ApiClient_NoErrorMessage);
		}

		return error;
	}

	async Task<IssuuResponse> GetIssuuErrorResponse(
		HttpRequestMessage httpReq,
		string? requestContent,
		HttpResponseMessage? httpResp,
		Exception exception)
	{
		var response = new IssuuResponse(
			httpReq.Method,
			httpReq.RequestUri,
			false,
			(HttpStatusCode)0,
			error: new Error(exception.Message, exception: exception));

		response.RequestContent = requestContent;

		if (httpResp?.Content is not null)
		{
			response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
				.ConfigureAwait(false); ;
		}

		return response;
	}

	async Task<IssuuResponse<TResponse>> GetIssuuErrorResponse<TResponse>(
		HttpRequestMessage httpReq,
		string? requestContent,
		HttpResponseMessage? httpResp,
		Exception exception)
		where TResponse : class
	{
		var response = new IssuuResponse<TResponse>(
			httpReq.Method,
			httpReq.RequestUri,
			false,
			(HttpStatusCode)0,
			error: new Error(exception.Message, exception: exception));

		response.RequestContent = requestContent;
		if (httpResp?.Content is not null)
		{
			response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
				.ConfigureAwait(false); ;
		}

		return response;
	}

	RateLimiting? GetRateLimiting(HttpResponseMessage response)
	{
		var headers = response.Headers;

		return
			int.TryParse(GetHeader("x-ratelimit-remaining", headers), out int remaining)
			&& int.TryParse(GetHeader("x-ratelimit-limit", headers), out int limit)
			&& long.TryParse(GetHeader("x-ratelimit-reset", headers), out long reset)
			? new RateLimiting { Limit = limit, Remaining = remaining, Reset = DateTimeOffset.FromUnixTimeSeconds(reset) } : null;
	}

	async Task<string?> CaptureRequestContent(HttpRequestMessage httpReq)
	{
		if (_settings.CaptureRequestContent && httpReq.Content is not null)
		{
			var request = await httpReq.Content.ReadAsStringAsync()
				.ConfigureAwait(false);

			return request;
		}

		return null;
	}

	async Task<string?> CaptureResponseContent(HttpResponseMessage httpResp)
	{
		if (_settings.CaptureResponseContent && httpResp.Content is not null)
		{
			var request = await httpResp.Content.ReadAsStringAsync()
				.ConfigureAwait(false);

			return request;
		}

		return null;
	}

	string? GetHeader(string name, HttpHeaders headers)
		=> headers.TryGetValues(name, out var values)
		? values.First()
		: null;
	#endregion

	protected internal Lazy<TOperations> Defer<TOperations>(Func<ApiClient, TOperations> factory)
		=> new Lazy<TOperations>(() => factory(this));

	protected internal Uri Root(string resource)
		=> new Uri(resource, UriKind.Relative);

	string CombineRelativePaths(string parent, string child)
	{
		if (parent is { Length: > 0 } && child is { Length: > 0 })
		{
			return $"{parent.TrimEnd('/')}/{child.TrimStart('/')}";
		}
		else
		{
			return parent + child;
		}
	}

	QueryString? BuildQuery(QueryString? query, int? page, int? size)
	{
		if (query.HasValue && page is null && size is null)
		{
			return query;
		}

		var builder = new QueryStringBuilder(query);
		if (page.HasValue)
		{
			builder = builder.AddParameter("page", page.Value);
		}
		if (size.HasValue)
		{
			builder = builder.AddParameter("size", size.Value);
		}

		if (builder.HasQuery)
		{
			return builder.Build();
		}

		return default;
	}
}

/// <summary>
/// Represents a mapped response.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="Data">The response data.</param>
/// <param name="Meta">The metadata.</param>
/// <param name="Links">The set of links for the resource.</param>
public record MappedResponse<TResponse>(
	TResponse? Data,
	Meta? Meta = default,
	Dictionary<string, string>? Links = default);

/// <summary>
/// Provides a mapping delegate for transforming the response payload
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="response">The HTTP response message.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The response data and meta.</returns>
public delegate Task<MappedResponse<TResponse>> MapResponse<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken);

/// <summary>
/// A container type that represents an Issuu error.
/// </summary>
public class ErrorContainer
{
	/// <summary>
	/// The set of field errors.
	/// </summary>
	[JsonPropertyName("fields")]
	public Dictionary<string, ErrorMessageContainer>? Fields { get; set; }

	/// <summary>
	/// The set of specific error details.
	/// </summary>
	[JsonPropertyName("details")]
	public Dictionary<string, ErrorMessageContainer>? Details { get; set; }

	/// <summary>
	/// The error message.
	/// </summary>
	[JsonPropertyName("message")]
	public string Message { get; set; } = default!;
}

/// <summary>
/// A container type for a specific Issuu error.
/// </summary>
public class ErrorMessageContainer
{
	/// <summary>
	/// The specific error message.
	/// </summary>
	[JsonPropertyName("message")]
	public string Message { get; set; } = default!;
}

/// <summary>
/// Represents a data container, used for set results.
/// </summary>
/// <typeparam name="TData">The data type.</typeparam>
public class DataContainer<TData>
{
	/// <summary>
	/// The total number of items.
	/// </summary>
	[JsonPropertyName("count")]
	public int? Count { get; set; }

	/// <summary>
	/// The size of the page.
	/// </summary>
	[JsonPropertyName("pageSize")]
	public int? PageSize { get; set; }

	/// <summary>
	/// The data.
	/// </summary>
	[JsonPropertyName("results")]
	public TData? Results { get; set; }

	/// <summary>
	/// The set of links for the resource.
	/// </summary>
	[JsonPropertyName("links")]
	public Dictionary<string, LinkContainer>? Links { get; set; }
}

/// <summary>
/// Represents a link container.
/// </summary>
public class LinkContainer
{
	/// <summary>
	/// Gets the link HREF.
	/// </summary>
	[JsonPropertyName("href")]
	public string Href { get; set; } = default!;
}
