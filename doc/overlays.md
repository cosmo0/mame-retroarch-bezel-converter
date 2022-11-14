# How Retroarch overlays work

Retroarch's overlays are composed of 3 files:

- a "rom config" file
- an "overlay config" file
- one or more overlay image(s)

The config files use the INI format: `key=value`, ";" for comments, etc.

## The rom config file

This file describes which overlays is used, and how.

Yeah, it's pretty stupid that the "how" is defined here, instead of the overlay file.

It's usually placed alongside the rom files, but some configurations allow (or force) you to place them in other folders. In all cases, it should have the same name as the rom file: `sf2.zip` should have the config file `sf2.zip.cfg`.

````ini
input_overlay = /path/to/the/overlay/config/file.cfg
input_overlay_enable = true
input_overlay_opacity = 0.900000
input_overlay_scale = 1.000000
custom_viewport_width = 1434
custom_viewport_height = 1074
custom_viewport_x = 243
custom_viewport_y = 2
aspect_ratio_index = 23
video_force_aspect = true
video_scale_integer = false
````

Most parameters should be pretty self-explanatory. Here are some details:

- `input_overlay` points to the overlay config file, with an absolute path.
- `custom_viewport_xxx` define the position of the game's screen.
- `aspect_ratio_index` defines the target aspect ratio (16/9, 4/3, etc). You want to look for the "custom" value (here 23), but be warned that not all distros (RA/Windows, Retropie, Recalbox, Batocera...) use the same values. Thanks a lot.
- `video_scale_integer` will force viewport width/height to the closest pixel-perfect aspect ratios.

## The overlay config file

This file describes the overlay images that are used. Multiple images can be superposed on top of each other.

They can be located anywhere on the system, as long as RA has access to this path.

````ini
overlays = 1
overlay0_overlay = file.png
overlay0_full_screen = true
overlay0_descs = 0
````

The `overlay0_overlay` parameter here uses a relative path. Why here and not in the rom config file, it's all a mystery.

## The overlay image

The overlay images are displayed on top of the game screen, using the parameters in the config files.

Obviously you should use images in PNG format, so that they have transparency. Otherwise your screen will be masked.
