---
sidebar_position: 2
tags: [Draft, Document]
---

# Getting a single draft

This page discusses how to read a specific raft on Issuu

### Getting a specific draft

:::tip[Issuu API]

This method corresponds to calling `HTTP GET https://api.issuu.com/v2/drafts/{slug}`.<br />
API documentation: https://api.issuu.com/v2/reference/#get-/drafts/-slug-

:::

To read an existing draft, you need the __slug__ value for the document.  You can then call the `Drafts.GetDraftAsync` operation provided by the API client.

```csharp
public async Task GetDraftDocumentAsync(string slug, CancellationToken ct)
{
    var response = await _client.Drafts.GetDraftAsync(slug, ct);
    if (response.IsSuccess && response.HasData)
    {
        var document = response.Data;
        // Do something with the document
    }
}