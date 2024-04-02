---
sidebar_position: 6
tags: [Draft, Document]
---

# Deleting a draft

This page discusses how to delete an existing Draft document on Issuu

### Deleting a draft document

:::tip[Issuu API]

This method corresponds to calling `HTTP DELETE https://api.issuu.com/v2/drafts/{slug}`.<br />
API documentation: https://api.issuu.com/v2/reference/#delete-/drafts/-slug-

:::

To delete an existing draft, you need the __slug__ value for the document.  You can then call the `Drafts.DeleteDraftAsync` operation provided by the API client.

```csharp
public async Task DeleteDraftDocumentAsync(string slug, CancellationToken ct)
{
    var response = await _client.Drafts.DeleteDraftAsync(slug, ct);
    if (response.IsSuccess)
    {
        // Do something if required
    }
}
```