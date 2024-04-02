---
sidebar_position: 1
tags: [Install, NuGet, PackageReference]
---

# Installation

To install the IssuuSDK into your project, you need to add a reference to the package.

The simplest approach is to use the .NET CLI, but this is not always possible, especially if you are targeting an older version of the .NET platform.

## Installing via the .NET CLI

```
dotnet add package IssuuSDK
```

## Installing via the NuGet  CLI

```
nuget install IssuuSDK
```

## Manual installation using `<PackageReference>` for modern and SDK projects

If you project is an SDK-style project, or is an older project using `<PackageReference>` you can add the IssuuSDK to your project by editing your project file to add the reference:

```xml
<Project>
  <ItemGroup>
    <PackageReference Include="IssuuSDK" Version="{version}" />
  </ItemGroup>
</Project>
```

:::tip[Note]

`{version}` represents the version of the package you want to use.

:::

### Working with centralised package management

If you are centralising your package management using a `<PackageVersion>` items, use a variation of the above.

In your project file:
```xml
<Project>
  <ItemGroup>
    <PackageReference Include="IssuuSDK" />
  </ItemGroup>
</Project>
```

In your `Directory.Packages.props` file:

```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="IssuuSDK" Version="{version}" />
  </ItemGroup>
</Project>
```

:::tip[Note]

`{version}` represents the version of the package you want to use.

:::

## Installation on .NET Framework projects not using `<PackageReference>`

It is recommended to use your IDE's NuGet package manager to install the package.