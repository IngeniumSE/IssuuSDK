// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace IssuuSDK.Api;

/// <summary>
/// Provides factory methods for creating a Issuu API client.
/// </summary>
public interface IIssuuApiClientFactory
{
	/// <summary>
	/// Creates a Issuu API client.
	/// </summary>
	/// <returns>The API settings.</returns>
	IIssuuApiClient CreateApiClient(IssuuSettings settings);
}

public class IssuuApiClientFactory : IIssuuApiClientFactory
{
	readonly IIssuuHttpClientFactory _httpClientFactory;

	public IssuuApiClientFactory(IIssuuHttpClientFactory httpClientFactory) =>
		_httpClientFactory = Ensure.IsNotNull(httpClientFactory, nameof(httpClientFactory));

	public IIssuuApiClient CreateApiClient(IssuuSettings settings)
	{
		Ensure.IsNotNull(settings, nameof(settings));

		var http = _httpClientFactory.CreateHttpClient("Issuu");

		return new IssuuApiClient(http, settings);
	}
}
