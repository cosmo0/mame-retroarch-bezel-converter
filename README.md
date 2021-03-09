# MAME / Retroarch bezels and overlays converter

Overlays (or bezels) are images added "above" the emulator, to mask the black borders around the image.

This tool converts MAME bezels to Retroarch overlays, so they can be used with any Libretro emulator.

It works under Windows x64/ARM64, Linux x64/ARM64 and MacOS x64.

## Download

**[Download the latest release](https://github.com/cosmo0/mame-retroarch-bezel-converter/releases)**

## Usage

Get a detailed help by running `mame-bezel-converter --help`.

### Convert MAME bezels to RetroArch overlays

> mame-bezel-converter.exe mtr --source path/to/mame/zips --output-roms output/roms --output-overlays output/overlay --template-game templates/game.cfg --template-overlay templates/overlay.cfg

- `--source` is the path where you store your zip files containing your MAME bezels
- `--output-overlays` is where the png and overlay cfg files will be created
- `--output-roms` is where rom cfg files will be created (where the screen dimensions are stored)
- `--template-game` is a template rom cfg that will be modified (a sample is provided)
- `--template-overlay` is a template overlay cfg that will be modified (a sample is provided)

## Convert RetroArch overlays to MAME bezels

> mame-bezel-converter.exe rtm --source-roms path/to/rom/files --source-configs path/to/config/files --output path/to/output --template templates/default.lay --zip

- `--source-roms` is the path to the rom cfg (the .zip.cfg files)
- `--source-configs` is the path to the folder where the cfg files are located
- `--output` is where the MAME bezels will be created
- `--template` is the template for the lay file that will be modified (a sample is provided)
- `--zip` zips the result (otherwise it just creates a folder)

## Common parameters

- `--overwrite` to overwrite existing files
- `--scan-bezel` to scan transparent pixels in the images instead of relying on the cfg/lay files
- `--output-debug path/to/debug` to see the result of the conversion (it creates an image with a red square where the screen will be)
- `--margin 10` to add or remove a 10px margin (positive value to crop a bit of the screen)
- `--threads 4` to use 4 threads

## Development

### Run it locally

Just install the .Net 5 SDK.

Build:

`dotnet build src /MAMEBezelConverter.sln`

Run:

`dotnet run -p src/MAMEBezelConverter.csproj -- mtr
    --source tmp/source_mame
    --source-configs tmp/source_mame
    --output-roms tmp/output/roms_ra
    --output-overlays tmp/output/overlay_ra
    --template-game src/templates/game.cfg
    --template-overlay src/templates/overlay.cfg
    --overwrite
    --scan-bezel
    --output-debug tmp/debug_ra
    --margin 10
    --threads 4`

`dotnet run -p src/MAMEBezelConverter.csproj -- rtm
    --source-roms tmp/source_ra/roms
    --source-configs tmp/source_ra/configs
    --output tmp/output/mame
    --template src/templates/default.lay
    --zip
    --overwrite
    --scan-bezel
    --output-debug tmp/debug_mame
    --margin 10
    --threads 4`

Publish:

````shell
dotnet publish src/MAMEBezelConverter.sln -r win-x64 -p:PublishSingleFile=true --self-contained true -o out/win-x64
dotnet publish src/MAMEBezelConverter.sln -r win-arm64 -p:PublishSingleFile=true --self-contained true -o out/win-arm64
dotnet publish src/MAMEBezelConverter.sln -r linux-x64 -p:PublishSingleFile=true --self-contained true -o out/linux-x64
dotnet publish src/MAMEBezelConverter.sln -r linux-arm64 -p:PublishSingleFile=true --self-contained true -o out/linux-arm64
dotnet publish src/MAMEBezelConverter.sln -r osx-x64 -p:PublishSingleFile=true --self-contained true -o out/osx-x64
````

### Contribute

Lay file specs are located in [lay_file_specs.md](lay_files_specs.md).

It's a standard .Net console app, starting in `Program.cs` / `Main`. It uses [CommandLine](https://github.com/commandlineparser/commandline)
to parse arguments and execute the right verb.

The class `Importer` launches threads. They scan the input folders, get the files, send them to the processors and converters, then save the result.

`MameProcessor` and `RetroArchProcessor` read the files and extract data in a standardized way.
