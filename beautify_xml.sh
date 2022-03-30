#!/usr/bin/env bash


printf "enter path: "
read -r RPATH

# format file
xmllint --format "$RPATH" --output "$RPATH"

printf "your file: \n"
# bat needs to be installed first
bat "$RPATH"
