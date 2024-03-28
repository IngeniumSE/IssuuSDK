// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace IssuuSDK;

/// <summary>
/// Represents a request to a Issuu API resource.
/// </summary>
/// <param name="method">The HTTP method.</param>
/// <param name="resource">The relative resource.</param>
/// <param name="query">The query string.</param>
/// <param name="page">The current page (if applicable).</param>
/// <param name="size">The size of the page (if applicable).</param>
/// <param name="filePath">The file path for upload.</param>
/// <param name="formData">The form data.</param>
/// <param name="useMultipartContent">Use multipart file content for upload</param>
public class IssuuRequest(
	HttpMethod method,
	PathString resource,
	QueryString? query = null,
	int? page = null,
	int? size = null,
	string? filePath = null,
	Dictionary<string, string?>? formData = null,
	bool useMultipartContent = false)
{
	/// <summary>
	/// Gets the file path of an upload.
	/// </summary>
	public string? FilePath => filePath;

	/// <summary>
	/// Gets the form data.
	/// </summary>
	public Dictionary<string, string?>? FormData => formData;

	/// <summary>
	/// Gets the HTTP method for the request.
	/// </summary>
	public HttpMethod Method => method;

	/// <summary>
	/// Gets the relative resource for the request.
	/// </summary>
	public PathString Resource => resource;

	/// <summary>
	/// Gets the query string.
	/// </summary>
	public QueryString? Query => query;

	/// <summary>
	/// Gets the current page.
	/// </summary>
	public int? Page => page;

	/// <summary>
	/// Gets the size of the page.
	/// </summary>
	public int? Size => size;

	/// <summary>
	/// Gets a value indicating whether to use multipart content for upload.
	/// </summary>
	public bool UseMultipartContent => useMultipartContent;
}

/// <summary>
/// Represents a request to a Issuu API resource.
/// </summary>
/// <param name="method">The HTTP method.</param>
/// <param name="resource">The relative resource.</param>
/// <param name="data">The data.</param>
/// <typeparam name="TData">The data type.</typeparam>
public class IssuuRequest<TData>(
	HttpMethod method,
	PathString resource,
	TData data,
	QueryString? query = null) : IssuuRequest(method, resource, query)
	where TData : notnull
{
	/// <summary>
	/// Gets the model for the request.
	/// </summary>
	public TData Data => data;
}
