---
sidebar_position: 7
tags: [Draft, Document]
---

# Publish a draft

This page discusses how to publish an existing Draft document on Issuu

### Publishing a draft document

:::tip[Issuu API]

This method corresponds to calling `HTTP POST https://api.issuu.com/v2/drafts/{slug}/publish`.<br />
API documentation: https://api.issuu.com/v2/reference/#post-/drafts/-slug-/publish

:::

To publish an existing draft, you need the __slug__ value for the document.  You can then call the `Drafts.DeletePublishDraftAsync` operation provided by the API client.

```csharp
public async Task PublishDraftDocumentAsync(string slug, string desiredName, CancellationToken ct)
{
    var response = await _client.Drafts.PublishDraftAsync(slug, desiredName, ct);
    if (response.IsSuccess && response.HasData)
    {
        var result = response.Data;
    }    
}
```
The publish operation accepts an optional `desiredName` parameter, which should be a URL-friendly value that is used in place of the document's `Slug` property. The `desiredName` parameter is automatically converted to URL-friendly string when it is provided, so you do not need to do this.

A successful response for this operation will contain a `PublishResult`:

| Property | Type | Notes |
| --- | --- | --- |
| `PublicLocation` | `string` | The public URL of the document, for reading |
| `Location` | `string` | The private URL of the documnet, for editing |

:::warning[Note on Publishing]

You will not be able to publish a draft, until its associated document is [uploaded](/docs/drafts/uploading-files) and converted.

:::