---
sidebar_position: 5
tags: [Draft, Document]
---

# Uploading a file

This page discusses how to upload a file to an existing Draft document on Issuu

### Uploading a file

:::tip[Issuu API]

This method corresponds to calling `HTTP PATCH https://api.issuu.com/v2/drafts/{slug}/upload`.<br />
API documentation: https://api.issuu.com/v2/reference/#patch-/drafts/-slug-/upload

:::

To upload a file to an existing draft, you need the __slug__ value for the document.  You can then call one of the `Drafts.UploadDocumentContentAsync` operations provided by the API client.

### Using a file path

```csharp
public async Task UploadFileToDraftAsync(string slug, string filePath, CancellationToken ct)
{
    var response = await _client.Drafts.UploadDocumentContentAsync(
        slug, 
        filePath: filePath,
        cancellationToken: ct);
    if (response.IsSuccess && response.HasData)
    {
        var document = response.Data;
        // Do something with the document.
    }
}
```

:::tip[File Name]

The `UploadDocumentContentAsync` overload that accepts the `filePath` parameter, has an optional parameter for `fileName`. When this is _not_ provided, the file name is extracted from the `filePath` parameter.

:::

A successful response for this operation will contain the updated draft `Document`.

### Using a file stream

```csharp
public async Task UploadFileToDraftAsync(string slug, string filePath, CancellationToken ct)
{
    using var fileStream = new FileStream(filePath, FileMode.Open);

    var response = await _client.Drafts.UploadDocumentContentAsync(
        slug, 
        fileStream: fileStream,
        fileName: Path.GetFileName(filePath),
        cancellationToken: ct);
    if (response.IsSuccess && response.HasData)
    {
        var document = response.Data;
        // Do something with the document.
    }
}
```

:::warning[Note on Disposal]

Whilst the underlying `HttpRequestMessage` is not automatically disposed of by the calling `HttpClient` instance, the IssuuSDK will dispose of the `HttpRequestMessage` it creates when calling the API. This means if you provide a `Stream` representing the file content, the IssuuSDK will call `Dispose()` on the `StreamContent` that is used.

This is by design, as the IssuSDK needs to clean up resources it creates, and there is no facility provided by `StreamContent` to leave the underlying stream undisposed, except to _not_ dispose of the `StreamContent`.

Reference change: https://github.com/dotnet/corefx/pull/19082

:::

A successful response for this operation will contain the updated draft `Document`.

### File conversion

When a file is uploaded, it then needs to be convereted by the Issuu service. This happens automatically on upload, however you will __not__ be able to publish your draft, until the conversion operation has been completed.

The best strategy, is to poll the `Drafts.GetDraftAsync(...)` operation until you get an updated conversion status. Be mindful of rate limiting when performing this operation.

When the `document.FileInfo.ConversionStatus` value returns `Done`, the draft can then be published. If it the conversion status is returned as `Failed`, you will not be able to publish the draft. A conversion status of `Converting` means this operation has not yet completed.

:::danger[Support]

The IssuuSDK team cannot support failures on document conversion. Please contact Issuu for assistance with this issue.

:::