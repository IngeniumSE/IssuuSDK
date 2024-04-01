// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Resources;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using IssuuSDK;

namespace IssuuSDK;

/// <summary>
/// Provides a base implementation of an API client.
/// </summary>
public abstract class ApiClient
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
	protected internal async Task<IssuuResponse> SendAsync(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
				.ConfigureAwait(false);

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}

	protected internal async Task<IssuuResponse> SendAsync<TRequest>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}
	#endregion

	#region Fetch Many
	protected internal async Task<IssuuResponse<TResponse>> FetchManyAsync<TResponse>(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformManyResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				request.Page,
				cancellationToken)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}

	protected internal async Task<IssuuResponse<TResponse>> FetchManyAsync<TRequest, TResponse>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformManyResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				request.Page,
				cancellationToken)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}
	#endregion

	#region Fetch Single
	protected internal async Task<IssuuResponse<TResponse>> FetchSingleAsync<TResponse>(
		IssuuRequest request,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformSingleResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				cancellationToken)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}


	protected internal async Task<IssuuResponse<TResponse>> FetchSingleAsync<TRequest, TResponse>(
		IssuuRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformSingleResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp,
				cancellationToken)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new IssuuResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}
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
				content.Add(new StreamContent(new FileStream(request.FilePath, FileMode.Open)), "file", Path.GetFileName(request.FilePath));
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
		if (response.IsSuccessStatusCode)
		{
			return new IssuuResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode);
		}
		else
		{
			Error? error = await GetIssuuError(response, cancellationToken);

			return new IssuuResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				error: error
			);
		}
	}

	protected internal async Task<IssuuResponse<TResponse>> TransformManyResponse<TResponse>(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		int? page = null,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		var rateLimiting = GetRateLimiting(response);

		if (response.IsSuccessStatusCode)
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
							Page = page ?? 1,
							PageSize = data.PageSize.Value,
							TotalItems = data.Count.Value,
							TotalPages = (int)Math.Ceiling(data.Count.Value / (double)data.PageSize.Value)
						};
					}

					if (data.Links is { Count: >0 })
					{
						links = new();
						foreach (var link in data.Links)
						{
							links.Add(link.Key, link.Value.Href);
						}
					}
				}
			}

			return new IssuuResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				data: data?.Results,
				meta: meta,
				rateLimiting: rateLimiting,
				links: links
			);
		}
		else
		{
			Error? error = await GetIssuuError(response, cancellationToken);

			return new IssuuResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				rateLimiting: rateLimiting,
				error: error
			);
		}
	}

	protected internal async Task<IssuuResponse<TResponse>> TransformSingleResponse<TResponse>(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		var rateLimiting = GetRateLimiting(response);

		if (response.IsSuccessStatusCode)
		{
			TResponse? data = null;
			if (response.Content is not null)
			{
				data = await response.Content.ReadFromJsonAsync<TResponse>(
					_deserializerOptions, cancellationToken)
					.ConfigureAwait(false);
			}

			return new IssuuResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				data: data,
				rateLimiting: rateLimiting
			);
		}
		else
		{
			Error? error = await GetIssuuError(response, cancellationToken);

			return new IssuuResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				rateLimiting: rateLimiting,
				error: error
			);
		}
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
				if (content is { Count : >0 })
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

	RateLimiting? GetRateLimiting(HttpResponseMessage response)
	{
		var headers = response.Headers;

		return
			int.TryParse(GetHeader("x-ratelimit-remaining", headers), out int remaining)
			&& int.TryParse(GetHeader("x-ratelimit-limit", headers), out int limit)
			&& long.TryParse(GetHeader("x-ratelimit-reset", headers), out long reset)
			? new RateLimiting { Limit = limit, Remaining = remaining, Reset = DateTimeOffset.FromUnixTimeSeconds(reset) } : null;
	}

	string? GetHeader(string name, HttpHeaders headers)
		=> headers.TryGetValues(name, out var values)
		? values.First()
		: null;

	class ErrorContainer
	{
		[JsonPropertyName("fields")]
		public Dictionary<string, ErrorMessageContainer>? Fields { get; set; }

		[JsonPropertyName("details")]
		public Dictionary<string, ErrorMessageContainer>? Details { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; } = default!;
	}

	class ErrorMessageContainer
	{
		[JsonPropertyName("message")]
		public string Message { get; set; } = default!;
	}

	class DataContainer<TData>
	{
		[JsonPropertyName("count")]
		public int? Count { get; set; }

		[JsonPropertyName("pageSize")]
		public int? PageSize { get; set; }

		[JsonPropertyName("results")]
		public TData? Results { get; set; }

		[JsonPropertyName("links")]
		public Dictionary<string, LinkContainer>? Links { get; set; }
	}

	class LinkContainer
	{
		[JsonPropertyName("href")]
		public string Href { get; set; } = default!;
	}
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
