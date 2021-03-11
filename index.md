# Retroarch bezels / overlays tool

Overlays (or bezels) are images added "above" the emulator, to mask the black borders around the image.

This tool provides several utilities:

- convert MAME bezels to Retroarch overlays, so they can be used with any Libretro emulator
- convert Retroarch overlays to MAME bezels
- check Retroarch overlays integrity
- generate Retroarch overlays from images

It works under Windows x64/ARM64, Linux x64/ARM64 and MacOS x64. You can build it for any platform supported by .Net 5 (it's very easy).

## Download

**[Download the latest release](https://github.com/cosmo0/mame-retroarch-bezel-converter/releases)**

## Usage

**!!! BACKUP YOUR FILES BEFORE USING THIS TOOL !!!** I have used it on my own files but I cannot guarantee that it will work on yours.

Get a detailed help and list of options by running `bezel-tools --help` or `bezel-tools [verb] --help`.

### Check overlays integrity

Simple check:

> bezel-tools check --overlays-config samples/retroarch/overlays --roms-config samples/retroarch/roms

Check and fix when possible:

> bezel-tools check --overlays-config samples/retroarch/overlays --roms-config samples/retroarch/roms --autofix --input-overlay-path /opt/retropie/configs/all/retroarch/overlay/ --template-overlay templates/overlay.cfg --template-rom templates/game.cfg

### Convert MAME bezels to RetroArch overlays

> bezel-tools mtr --source path/to/mame/zips --output-roms output/roms --output-overlays output/overlay --template-game templates/game.cfg --template-overlay templates/overlay.cfg

- `--source` is the path where you store your zip files containing your MAME bezels
- `--output-overlays` is where the png and overlay cfg files will be created
- `--output-roms` is where rom cfg files will be created (where the screen dimensions are stored)
- `--template-game` is a template rom cfg that will be modified (a sample is provided)
- `--template-overlay` is a template overlay cfg that will be modified (a sample is provided)
- `--overwrite` to overwrite existing files

### Convert RetroArch overlays to MAME bezels

> bezel-tools rtm --source-roms path/to/rom/files --source-configs path/to/config/files --output path/to/output --template templates/default.lay --zip

- `--source-roms` is the path to the rom cfg (the .zip.cfg files)
- `--source-configs` is the path to the folder where the cfg files are located
- `--output` is where the MAME bezels will be created
- `--template` is the template for the lay file that will be modified (a sample is provided)
- `--zip` zips the result (otherwise it just creates a folder)
- `--overwrite` to overwrite existing files

### Common parameters for all actions

- `--error-file` the path to the output CSV file containing the errors and fixes
- `--scan-bezel` to scan transparent pixels in the images instead of relying on the cfg/lay files
- `--output-debug path/to/debug` to see the result of the conversion (it creates an image with a red square where the screen will be)
- `--margin 10` to add or remove a 10px margin (positive value to crop a bit of the screen)
- `--threads 4` to use 4 threads
