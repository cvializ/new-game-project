#!/bin/bash

rm -f tileset.png
magick montage -mode concatenate -background transparent -tile 2x1 "*.png" tileset.png
