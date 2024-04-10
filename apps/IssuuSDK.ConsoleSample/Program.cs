// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Net.Http.Headers;

using IssuuSDK;
using IssuuSDK.Api;

using Microsoft.Extensions.Configuration;

var settings = GetSettings();
var http = CreateHttpClient();
var api = new IssuuApiClient(http, settings);

var drafts = await api.Drafts.GetDraftsAsync().ConfigureAwait(false);

var doc = await api.Publications.GetPublicationAsync("xryumedetev").ConfigureAwait(false);
if (doc.IsSuccess)
{
	//var assets = await api.Publications.GetPublicationAssetsAsync("xryumedetev", AssetType.Cover, documentPageNumber: 3).ConfigureAwait(false);
	//if (assets.IsSuccess)
	//{

	//}

	var fullScreen = await api.Publications.GetPublicationFullScreenShareAsync(doc.Data!.Slug).ConfigureAwait(false);
	var reader = await api.Publications.GetPublicationReaderShareAsync(doc.Data.Slug).ConfigureAwait(false);
	var qrCode = await api.Publications.GetPublicationQrCodeAsync(doc.Data.Slug).ConfigureAwait(false);
	var embed = await api.Publications.GetPublicationEmbedAsync(doc.Data.Slug).ConfigureAwait(false);
}

IssuuSettings GetSettings()
{
	var configuration = new ConfigurationBuilder()
		.AddJsonFile("./appsettings.json", optional: false)
		.AddJsonFile("./appsettings.env.json", optional: true)
		.Build();

	IssuuSettings settings = new();
	configuration.GetSection(IssuuSettings.ConfigurationSection).Bind(settings);

	settings.Validate();

	return settings;
}

HttpClient CreateHttpClient()
{
	var http = new HttpClient();

	http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

	return http;
}
