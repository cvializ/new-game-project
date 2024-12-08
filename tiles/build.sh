#!/bin/bash

rm -f tileset.png
magick montage -mode concatenate -background transparent -tile 8x3 "*.png" tileset.png
