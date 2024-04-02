---
sidebar_position: 5
tags: [Document]
---

# Documents

The primary result _shape_ provided by the Issuu API is the `Document`, and contains the following properties:

| Property | Type | Notes |
|---|---|---|
| `Changes` | `DocumentChanges?` | The set of changes to the draft (if state = `Draft`) |
| `Cover` | `DocumentCoverImages?` | Any cover images generated for the document |
| `Created` | `DateTimeOffset` | The date/time the draft document was created |
| `FileId` | `long` | The internal ID of the file |
| `FileInfo` | `DocumentFileInfo?` | Any file-specific information for the document | 
| `Location` | `string` | The Issuu URI of the document |
| `Owner` | `string` | The Issuu user who owns the document |
| `Slug` | `string` | The unique URL-friendly slug for the document |
| `State` | `DocumentState` | One of `Draft`, `Published`, `Scheduled`, `Unpublished` or `Quarantined` |

The definition of this type can be found here: https://github.com/IngeniumSE/IssuuSDK/blob/main/libs/IssuuSDK/Primitives/Document.cs

## Changes

If the document is in a draft state, the `Changes` property will be populated with an instance of `DocumentChanges`:

| Property | Type | Notes |
| --- | --- | --- |
| `Access` | `DocumentAccess` | The access type. `Private` are only accessible on URLs. `Public` are accessible through search and recommendations |
| `Description` | `string` | The description of your document |
| `Downloadable` | `bool` | After publishing, will readers be able to download the document? |
| `OriginalPublishDate` | `DateTimeOffset?` | +The date was originally published previously |
| `Preview` | `bool` | Is this document a preview of a larger document? |
| `ScheduledTime` | `DateTimeOffset?` | The date you wish the draft to be automatically published. If this is not provided, the draft will need to be manually published |
| `Title` | `string` | The title of your document |
| `ShowDetectedLinks` | `bool` | During the conversion phase, hyperlinks can be collated and displayed alongside the document |

## Cover Images

If the document has cover images generated, these are available from the `Cover` property, which returns an instance of `DocumentCoverImages`:

| Property | Type | Notes |
| --- | --- | --- |
| `Large` | `DocumentImage?` | The large cover image |
| `Medium` | `DocumentImage?` | The medium cover image |
| `Small` | `DocumentImage?` | The small cover image |

The `DocumentImage` type is defined as:

| Property | Type | Notes |
| --- | --- | --- |
| `Height` | `int` | The image height |
| `Url` | `string` | The URL of the image |
| `Width` | `int` | The image width |

## File Information

If a file has been uploaded to a document, the `FileInfo` property will return information about that, using an instance of `DocumentFileInfo`:

| Property | Type | Notes |
| --- | --- | --- |
| `ConversionStatus` | `DocumentConversionStatus` | One of `Done` `Converting`, `Failed` |
| `IsCopyrightConfirmed` | `bool` | Has copyright been confirmed for this document file? |
| `Name` | `string` | The name of the file |
| `PageCount` | `int` | The number of pages in the file |
| `Size` | `long` | The size of the file, in bytes |
| `Type` | `DocumentFileType` | One of `Unknown`, `DOC`, `ODP`, `ODT`,`PDF`, `PPT`, `RTF`, `SXI`, `SXW`, `WPD`, `EPUB` or `MOBI` |