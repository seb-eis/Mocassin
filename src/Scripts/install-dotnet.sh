#!/usr/bin/env bash

### installs dotnet on a linux system without admin rights
### using the CI install scripts

append_if_required() {
    line=$1
    file=$2
    vGrep=$(grep -x "$line" "$file")
    echo -n "Try adding '$line' to '$file' ... "
    if [ "$vGrep" = "" ]; then
        echo "$line" >> "$file"
        echo "done!"
    else
        echo "already there!"
    fi
}

### Choose your profile file for your shell
profile="$HOME/.zprofile"

### Install dotnet sdk
fileName="$(uuid).sh"
curl -Lo "$fileName" https://dot.net/v1/dotnet-install.sh
chmod +x "$fileName"
eval "$fileName" -c 5.0
rm "$fileName"

append_if_required "export PATH=\$PATH:~/.dotnet" "$profile"
append_if_required "export PATH=\$PATH:~/.dotnet/tools" "$profile"
append_if_required "export DOTNET_ROOT=~/.dotnet" "$profile"

export "PATH=$PATH:$HOME/.dotnet"
export "PATH=$PATH:$HOME/.dotnet/tools"
export "DOTNET_ROOT=$HOME/.dotnet"

### Uncomment this to section to add the current PowerShell (pwsh)
### as a global dotnet tool

### Install pwsh and setup the required PATH info 
#dotnet tool install --global PowerShell

### Make it available in the current shell session
#export "PATH=$PATH:$HOME/.dotnet/tools"
#export "DOTNET_ROOT=$HOME/.dotnet"
