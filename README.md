# MAME bezel converter to Retroarch overlays

Overlays (or bezels) are images added "above" the emulator, to mask the black borders around the image.

This tool converts MAME bezels to Retroarch overlays, so they can be used with any Libretro emulator.

## Usage

````
mame-bezel-converter.exe

  --margin                 (Default: 0) Applies a margin to the screen (to hide a bit of it)

  -d, --output-debug       (Default: ) The folder where debug overlays will be created

  -o, --output-overlays    Required. The folder where the overlays configs and images will be created

  -r, --output-roms        Required. The folder where the ROM configs will be created

  --overwrite              (Default: true) Overwrites existing files

  --scan-bezel             (Default: false) Scans the bezel file for transparent pixels to find the screen position ; otherwise, just
                           convert the LAY file

  -s, --source             Required. The folder where the MAME artworks are located

  --target-resolution      (Default: 1920x1080) The target resolution

  --template-game          Required. The path to the template for the game config

  --template-overlay       Required. The path to the template for the overlay config

  --use-fist-view          (Default: true) Uses the first found view to generate an overlay

  --help                   Display this help screen.

  --version                Display version information.
````

Example usage

`mame-bezel-converter.exe -s ./imput -o /output/config -r /output/roms -d /debug --overwrite --scan-bezel --template-game templates/game.cfg --template-overlay templates/overlay.cfg`

## Development

Lay file specs are located in [lay_file_specs.md](lay_files_specs.md).
