# Actions details

This documentation might not be completely up to date; check the actual list of parameters using `bezel-tools [verb] --help`.

## Check overlays integrity

Checks that:

- the rom config points to an existing overlay
- the overlay config points to an existing image
- all images in the overlays folder have an associated overlay config and rom config, and can create one
- all overlay configs are used by a rom config, and can create one
- the path written in the config matches the expected one (to check overlays that will be used on another machine)
- the image has a size matching the output resolution
- the coordinates of the game in the rom config match the transparency in the image

It assumes that rom config are named `xxxx.zip.cfg`, and images are stored in the same folder as the overlay config.

**Simple check:**

> bezel-tools check --overlays-config samples/retroarch/overlays --roms-config samples/retroarch/roms --output-debug debug/

- `--overlay-config` is the path where the overlays are (cfg files and images)
- `--roms-config` is the path to the cfg files for the roms
- `--output-debug` is the path to the debug images, to see what will be computed

Example of debug output from configuration:

![from config](dstlku_conf.jpg)

Example of debug output computed from transparency in image:

![from config](dstlku_computed.jpg)

**Check and fix when possible:**

> bezel-tools check --overlays-config samples/retroarch/overlays --roms-config samples/retroarch/roms --autofix --input-overlay-path /opt/retropie/configs/all/retroarch/overlay/ --template-overlay templates/overlay.cfg --template-rom templates/game.cfg

- `--overlay-config` is the path where the overlays are (cfg files and images)
- `--roms-config` is the path to the cfg files for the roms
- `--autofix` fixes encountered problems
- `--input-overlay-path` is the expected path to the overlay in the rom config (ex: /opt/retropie/configs/all/retroarch/overlay/ for a Retropie overlay)
- `--template-overlay` is the path to the overlay template
- `--template-rom` is the path to the rom template

## Generate overlays from images

Generates overlay and rom configs based on the position of the screen transparent area, and the file name.

> bezel-tools generate --images samples/images --roms-configs samples/roms --template-overlay templates/overlay.cfg --template-rom templates/game.cfg

- `--images` is the path to the images folder (the overlay configs will be generated inside)
- `--roms-configs` is the path to the rom configs folder
- `--template-overlay` is the path to the overlay template
- `--template-rom` is the path to the rom template

## Convert MAME bezels to RetroArch overlays

Scans a folder containing MAME bezel files (it can scan zip files), and converts the content to a Retroarch overlay.

It can also read apply offsets stored in MAME configs.

> bezel-tools mtr --source path/to/mame/zips --output-roms output/roms --output-overlays output/overlay --template-game templates/game.cfg --template-overlay templates/overlay.cfg

- `--source` is the path where you store your zip files containing your MAME bezels
- `--output-overlays` is where the png and overlay cfg files will be created
- `--output-roms` is where rom cfg files will be created (where the screen dimensions are stored)
- `--template-game` is a template rom cfg that will be modified (a sample is provided)
- `--template-overlay` is a template overlay cfg that will be modified (a sample is provided)
- `--overwrite` to overwrite existing files

## Convert RetroArch overlays to MAME bezels

Scans a folder containing Retroarch overlays and converts them to MAME bezels.

> bezel-tools rtm --source-roms path/to/rom/files --source-configs path/to/config/files --output path/to/output --template templates/default.lay --zip

- `--source-roms` is the path to the rom cfg (the .zip.cfg files)
- `--source-configs` is the path to the folder where the cfg files are located
- `--output` is where the MAME bezels will be created
- `--template` is the template for the lay file that will be modified (a sample is provided)
- `--zip` zips the result (otherwise it just creates a folder)
- `--overwrite` to overwrite existing files

## Common parameters for all actions

- `--error-file path/to/file.csv` the path to the output CSV file containing the errors and fixes
- `--output-debug path/to/debug` to see the result of the conversion (it creates an image with a red square where the screen will be)
- `--margin 10` to add or remove a 10px margin to the screen position (positive value to crop a bit of the screen)
- `--threads 4` to use 4 threads
