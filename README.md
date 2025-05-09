﻿
# Dead Cells Core Modding (WIP)

![GitHub License](https://img.shields.io/github/license/dead-cells-core-modding/core) 
[![Build](https://github.com/dead-cells-core-modding/core/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/dead-cells-core-modding/core/actions/workflows/build.yml)


A Dead Cells Modding API/loader. 

> [!WARNING]
> This project is under active development. Breaking changes may be made to APIs with zero notice.

Download the latest build [here](https://nightly.link/dead-cells-core-modding/core/workflows/build/main)

## Roadmap

- [x] Simple Hook
- [x] Basic interoperability
- [ ] Game object proxy objects in .NET
- [x] Edit game bytecode dynamically
- [ ] More convenient BuildSystem
- [ ] Linux platform support

## Requirement

- .NET 9 runtime or higher
- Microsoft Visual C++ Redistributable package (2015-2022)

## Installation

1. Get the core file from [nightly link](https://nightly.link/dead-cells-core-modding/core/workflows/build/main) or [releases](https://github.com/dead-cells-core-modding/core/releases)
2. Unzip it to the game root directory

The folder structure should be similar to the following
```txt
<DeadCellsGameRoot>
|
+- coremod
|  |
|  +- core
|  |  |
|  |  +- native
|  |  |  |
|  |  |  +- ...
|  |  |
|  |  +- mdk
|  |  |	 |
|  |  |  +- install.ps1
|  |  |  |
|  |  |  +- uninstall.ps1
|  |  |  |
|  |  |  +- ...
|  |  |
|  |  +- host
|  |  |  |
|  |  |  +- startup
|  |  |  |  |
|  |  |  |  +- DeadCellsModding.exe
|  |  |  |  |
|  |  |  |  +- ...
|  |  |  +- ...
|  |  +- ...
|  +- ...
|
+- deadcells.exe
|
+- deadcells_gl.exe
|
+- ...
```

## Mods Development

Here are some [examples](https://github.com/dead-cells-core-modding/core/tree/main/sample).

### Preparation

1. Install .NET SDK 9
2. Install Dead Cells Core Modding as above
3. Run `<DeadCellsGameRoot>/coremod/core/mdk/install.ps1` to configure the environment

### Create a mod project

1. Create a library project based on .NET 9
2. Add package reference `DeadCellsCoreModding.MDK`
3. Add the following to your csproj file
```xml
<PropertyGroup>
	<!--Enter the mod name here-->
	<ModName>$(AssemblyName)</ModName>

	<!--
	Enter mod type here

	Available values:
		mod: Normal mod
		library: Library
	-->
	<ModType>mod</ModType>

	<!--Enter the full name of the mod's main type here-->
	<ModMain>ModNamespace.MainModClass</ModMain>
</PropertyGroup>
```

### Build

Build the mod using `dotnet build`.
The default output directory is `$(OutputPath)/output/`

## Usage

### Startup

Start the game from `<DeadCellsGameRoot>/coremod/core/host/startup/DeadCellsModding.exe`

### Mods Installation

1. Create `<DeadCellsGameRoot>/coremod/mods` folder if it does not exist.
2. Move the mods files into the `mods` folder. The folder structure should look like this:
```txt
mods
|
+- <ModName>
|  |
|  +- modinfo.json
|  |
|  +- ...
|
+- ...
```

> [!WARNING]
> `<ModName>` must be exactly the same as the `name` property in `modinfo.json`, otherwise the mods loader will refuse to load the mods

## Development

### Requirement

- .NET SDK 9
- CMake

### Build

#### Windows

1. Clone the repository
2. Run `buildWin.bat`

## Credit

- [DeadCellsDecomp](https://github.com/N3rdL0rd/DeadCellsDecomp) and [alivecells](https://github.com/N3rdL0rd/alivecells) from N3rdL0rd
- [Hashlink](https://github.com/HaxeFoundation/hashlink) from HaxeFoundation

## License

Distributed under the MIT [license](https://github.com/DreamBoxSpy/DeadCellsCoreModding/blob/main/LICENSE).

## Disclaimer
Dead Cells Core Modding is in no way associated with Motion Twin.
