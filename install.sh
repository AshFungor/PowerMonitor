#!/usr/bin/env bash

# install script

# is project going to be build from source?
SOURCE=true
# in release versions it is set to false

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
        printf "system? OS (linux-x64, win-x64): "
        read -r SYSTEM
        dotnet publish -r "$SYSTEM" --self-contained false --output "$TARGET_DIR"

        exit

    else

        printf "dependencies are not satisfied. consider installing dotnet from\n"
        printf "https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-scripted-manual#scripted-install\n"
        exit 1

    fi



    
fi






