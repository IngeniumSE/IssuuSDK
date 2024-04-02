---
sidebar_position: 3
tags: [Draft, Document]
---

# Create a draft

This page discusses how to create a new Draft document on Issuu

### Creating a draft document

:::tip[Issuu API]

This method corresponds to calling `HTTP POST https://api.issuu.com/v2/drafts`.<br />
API documentation: https://api.issuu.com/v2/reference/#post-/drafts

:::

To create a new draft, we can use the `Drafts.CreateDraftAsync` operation provided by the API client.

```csharp
public async Task CreateDraftDocumentAsync(CancellationToken ct)
{
    var draft = new Draft();

    var response = await _client.Drafts.CreateDraftAsync(draft, ct);
    if (response.IsSuccess && response.HasData)
    {
        var document = response.Data;
        // Do something with the document.
    }    
}
```

There are a number of optional parameters that you can provide at this stage for a draft:

```csharp
var draft = new Draft(
  title: "",
  description: "",
  fileUrl: "",
  access: DocumentAccess.Private,
  type: DocumentType.Other,
  confirmCopyright: false,
  downloadable: false,
  preview: false,
  showDetectedLinks: false,
  originalPublishDate: null,
  scheduledTime: null
);
```

| Parameter | Type | Default | Notes |
| --- | --- | --- | --- |
| `title` | `string` | `null` | The title of your document |
| `description` | `string` | `null` | The description of your document |
| `fileUrl` | `string` | `null` | _Unsure, Issuu API documentation is incomplete_ |
| `access` | `DocumentAccess` | `Private` | The access type. `Private` are only accessible on URLs. `Public` are accessible through search and recommendations |
| `type` | `DocumentType` | `Other` | The document type. One of `Editorial`, `Book`, `Promotional` or `Other` |
| `confirmCopright` | `bool` | `false` | _Unsure, Issuu API documentation is incomplete_ |
| `downloadable` | `bool` | `false` | After publishing, will readers be able to download the document? |
| `preview` | `bool` | `false` | Is this document a preview of a larger document? |
| `showDetectedLinks` | `bool` | `false` | During the conversion phase, hyperlinks can be collated and displayed alongside the document |
| `originalPublishDate` | `DateTimeOffset?` | `null` | If the document was originally published previously, this date can be made public alongside the document |
| `scheduledTime` | `DateTimeOffset?` | `null` | Set the date you wish the draft to be automatically published. If this is not provided, the draft will need to be manually published |

A successful response for this operation will contain a draft `Document`:

```csharp
if (response.IsSuccess && response.HasData)
{
    var document = response.Data;
    // Do something with the document.
}
```

All Issuu document responses will be the same, but different values may be populated based on the document state.

For a complete runthrough of the `Document` type, see [Documents](/docs/documents).

:::warning[Note on Publishing]

You will not be able to publish a draft, until its associated document is [uploaded](/docs/drafts/uploading-files) and converted.

:::