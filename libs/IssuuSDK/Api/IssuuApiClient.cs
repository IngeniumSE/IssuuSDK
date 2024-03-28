// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace IssuuSDK.Api;

public partial interface IIssuuApiClient
{

}

public partial class IssuuApiClient : ApiClient, IIssuuApiClient
{
	public IssuuApiClient(HttpClient http, IssuuSettings settings)
		: base(http, settings, settings.BaseUrl) { }
}
