# Development

You'll need to install the [.Net 6 SDK](https://dotnet.microsoft.com/download/).

## Build

`dotnet build src/BezelTools.sln`

## Publish

Run `publish.bat` (or copy the commands in your console).

The binaries will be located in the `out` folder.

## Run

**Simple check:**

`dotnet run -p src/BezelTools.csproj -- check
    --overlays-config tmp/retroarch/configs
    --roms-config tmp/retroarch/roms
    --threads 4
    -e tmp/errors.csv
    --error-margin 10`

**Check and fix:**

`dotnet run -p src/BezelTools.csproj -- check
    --overlays-config tmp/retroarch/configs
    --roms-config tmp/retroarch/roms
    --autofix
    --input-overlay-path /opt/retropie/configs/all/retroarch/overlay/arcade-realistic
    --template-overlay src/templates/overlay.cfg
    --template-rom src/templates/game.cfg
    --threads 4
    -e tmp/errors.csv
    -d tmp/debug
    --margin 5
    --error-margin 10`

**Generate from images:**

`dotnet run -p src/BezelTools.csproj -- generate
    --images tmp/images
    --roms-config tmp/output/roms
    --template-overlay src/templates/overlay.cfg
    --template-rom src/templates/game.cfg
    -e tmp/errors.csv
    -d tmp/debug
    --threads 4`

**MAME to RA conversion:**

`dotnet run -p src/BezelTools.csproj -- mtr
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
    --threads 4
    -e tmp/errors.csv`

**RA to MAME conversion:**

`dotnet run -p src/BezelTools.csproj -- rtm
    --source-roms tmp/source_ra/roms
    --source-configs tmp/source_ra/configs
    --output tmp/output/mame
    --template src/templates/default.lay
    --zip
    --overwrite
    --scan-bezel
    --output-debug tmp/debug_mame
    --margin 10
    --threads 4
    -e tmp/errors.csv`
