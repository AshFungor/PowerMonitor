#!/usr/bin/env bash

# install script

# is project going to be build from source?
SOURCE=true
# in release versions it is set to false
BUILD_FAIL=false


on_buildfail() {
    printf "build failed\n"
    printf "something wrong with the sript?\n"
    $BUILD_FAIL=true
}

if [[ $SOURCE ]]; then


    printf "####################\n" 
    printf "#    welcome to    #\n"
    printf "#   PowerMonitor   #\n"
    printf "#    installer     #\n"
    printf "####################\n\n"


    printf "this script compiles and installs PowerMonitor from source code\n"
    printf "for source see: https://github.com/AshFungor/PowerMonitor\n"


    if [[ "$(whereis dotnet)" = *"dotnet" && "$(dotnet --version)" = "6."* ]]; then

        printf "dependencies are satisfied. proceeding with installation...\n"
        printf "where to install? dir: "
        read -r TARGET_DIR
        printf "system? 1 - linux, 2 - win10 (enter number 1 or 2): "
        read -r SYSTEM
        while [[ $SYSTEM != [12] ]]; do
            printf "incorrect input, try again\n"
            printf "system? 1 - linux, 2 - win10 (enter number 1 or 2): "
            read -r SYSTEM
        done
        [[ "$SYSTEM" == "1" ]] && SYSTEM="linux" || SYSTEM="win"
        dotnet publish --output "$TARGET_DIR/build" -c "release_$SYSTEM" -r "$SYSTEM-x64" -p:PublishSingleFile=true --self-contained || on_buildfail
        if [[ $BUILD_FAIL ]]; then exit 1 
        fi
        mv "$TARGET_DIR/build" "$TARGET_DIR/PowerMonitor"
        printf "to run a project, use the command:\n"
        printf "dotnet $TARGET_DIR/PowerMonitor/PowerMonitor.dll\n"
        printf "note, it might be easier create a launcher for that command"

        exit 0

    else

        printf "dependencies are not satisfied. consider installing dotnet from\n"
        printf "https://docs.microsoft.com/ru-ru/dotnet/core/install\n"
        exit 1

    fi



    
fi






