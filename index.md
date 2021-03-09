# MAME / Retroarch bezels and overlays converter

Overlays (or bezels) are images added "above" the emulator, to mask the black borders around the image.

This tool converts MAME bezels to Retroarch overlays, so they can be used with any Libretro emulator.

## Download

[Download the latest release](https://github.com/cosmo0/mame-retroarch-bezel-converter/releases)

## Usage

### Convert MAME bezels to Retroarch overlays

````
mame-bezel-converter.exe mtr

  -o, --output-overlays    Required. The folder where the overlays configs and images will be created

  -r, --output-roms        Required. The folder where the ROM configs will be created

  -s, --source             Required. The folder where the MAME artworks are located

  --template-game          Required. The path to the template for the game config

  --template-overlay       Required. The path to the template for the overlay config

  --use-fist-view          (Default: true) Uses the first found view to generate an overlay

  --margin                 (Default: 0) Applies a margin to the screen (to hide a bit of it)

  -d, --output-debug       (Default: ) The folder where debug overlays will be created

  --error-file             (Default: errors.txt) The path to the file containing the list of errors

  --threads                (Default: 1) Number of threads on which to run

  --overwrite              (Default: true) Overwrites existing files

  --scan-bezel             (Default: false) Scans the bezel file for transparent pixels to find the screen position ; otherwise, just convert the LAY file

  --target-resolution      (Default: 1920x1080) The target resolution

  --help                   Display this help screen.

  --version                Display version information.
````

Example usage

`mame-bezel-converter.exe mtr
	-s samples/mame
	-r tmp/output_ra/roms
	-o tmp/output_ra/overlay
	-d tmp/debug
	--template-game src/templates/game.cfg
	--template-overlay src/templates/overlay.cfg
	--overwrite --scan-bezel --margin 10 --threads 4`

### Converts Retroarch overlays to MAME bezels

````
mame-bezel-converter.exe rtm

  -o, --output            Required. The folder where the bezels will be created

  -c, --source-configs    Required. The folder where the Retroarch bezels configs are located

  -r, --source-roms       Required. The folder where the Retroarch roms configs are located

  -t, --template          Required. The path to the default.lay template

  -z, --zip               (Default: false) Whether to zip the output bezels

  --margin                (Default: 0) Applies a margin to the screen (to hide a bit of it)

  -d, --output-debug      (Default: ) The folder where debug overlays will be created

  --error-file            (Default: errors.txt) The path to the file containing the list of errors

  --threads               (Default: 1) Number of threads on which to run

  --overwrite             (Default: true) Overwrites existing files

  --scan-bezel            (Default: false) Scans the bezel file for transparent pixels to find the screen position ; otherwise, just convert the LAY file

  --target-resolution     (Default: 1920x1080) The target resolution

  --help                  Display this help screen.

  --version               Display version information.

````

Example usage

`mame-bezel-converter.exe rtm
	-r samples/retroarch/
	-c samples/retroarch
	-o tmp/output_mame/
	-d tmp/debug/
	-t src/templates/default.lay
	--overwrite --zip --scan-bezel --margin 10 --threads 4`

## Development

Lay file specs are located in [lay_file_specs.md](lay_files_specs.md).
