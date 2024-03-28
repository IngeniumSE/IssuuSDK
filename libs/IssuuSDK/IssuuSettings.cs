// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using FluentValidation;

using Microsoft.Extensions.Options;

namespace IssuuSDK;

/// <summary>
/// Represents the settings for the Issuu API.
/// </summary>
public class IssuuSettings
{
	public const string ConfigurationSection = "Issuu";

	/// <summary>
	/// Gets or sets the environment for the Issuu API.
	/// </summary>
	public string BaseUrl { get; set; } = "https://api.issuu.com/v2";

	/// <summary>
	/// Gets or sets whether to capture request content.
	/// </summary>
	public bool CaptureRequestContent { get; set; }

	/// <summary>
	/// Gets or sets whether to capture response content.
	/// </summary>
	public bool CaptureResponseContent { get; set; }

	/// <summary>
	/// Gets or sets the default settings for the Issuu API.
	/// </summary>
	public IssuuDefaultSettings DefaultSettings { get; set; } = new IssuuDefaultSettings();

	/// <summary>
	/// Gets or sets the authentication token.
	/// </summary>
	public string Token { get; set; } = default!;

	/// <summary>
	/// Returns the settings as a wrapped options instance.
	/// </summary>
	/// <returns>The options instance.</returns>
	public IOptions<IssuuSettings> AsOptions()
		=> Options.Create(this);

	/// <summary>
	/// Validates the current instance.
	/// </summary>
	public void Validate()
		=> IssuuSettingsValidator.Instance.Validate(this);
}

/// <summary>
/// Represents the default settings for the Issuu API.
/// </summary>
public class IssuuDefaultSettings
{
	/// <summary>
	/// Gets or sets the access type for the Issuu document.
	/// </summary>
	public DocumentAccess Access { get; set; } = DocumentAccess.Private;

	/// <summary>
	/// Gets or sets whether the document is downloadable.
	/// </summary>
	public bool Downloadable { get; set; } = false;

	/// <summary>
	/// Gets or sets whether the document is a preview of larger content.
	/// </summary>
	public bool Preview { get; set; } = false;

	/// <summary>
	/// Gets or sets whether to show detected links when the document is published.
	/// </summary>
	public bool ShowDetectedLinks { get; set; } = false;
}

/// <summary>
/// Validates the settings for the Issuu API.
/// </summary>
public class IssuuSettingsValidator : AbstractValidator<IssuuSettings>
{
	public static readonly IssuuSettingsValidator Instance = new();

	public IssuuSettingsValidator()
	{
		RuleFor(x => x.Token).NotEmpty();
		RuleFor(x => x.DefaultSettings).NotEmpty().SetValidator(new IssueDefaultSettingsValidator());
	}
}

/// <summary>
/// Validates the default settings for the Issuu API.
/// </summary>
public class IssueDefaultSettingsValidator : AbstractValidator<IssuuDefaultSettings>
{
	public IssueDefaultSettingsValidator()
	{
		RuleFor(x => x.Access).IsInEnum();
	}
}
