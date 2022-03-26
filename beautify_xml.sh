#!/usr/bin/env bash

printf "enter path: "
read -r PATH

cat "$PATH" | xmllint --format - > "$PATH"
printf "your file: \n"
cat "$PATH"