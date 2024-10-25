# !/bin/bash

# Declaration of all variables
export RIMWORLDPATH=""
export MODFOLDERPATH=""
export FILEWITHGAMEPATH="RimWorldFolderPath.txt"

# Edit this to change your mod name in the builder
export YOURMODNAME="Map Saver"

CheckIfFileWithPathIsPresent()
{
    if [ -f "$FILEWITHGAMEPATH" ]; then
        echo "$FILEWITHGAMEPATH exists, continuing operation";
        echo;
    else
        echo Put/Your/Path/To/Your/Game/In/Here > $FILEWITHGAMEPATH;
        echo "$FILEWITHGAMEPATH was just created, make sure to add your RimWorld path to it before you continue";
        read;
    fi
}
export -f CheckIfFileWithPathIsPresent;

SetAllNeededVariables()
{
    BASEBUILDERPATH=$(pwd);
    RIMWORLDPATH=$(cat $FILEWITHGAMEPATH);
    MODFOLDERPATH="$RIMWORLDPATH/Mods/$YOURMODNAME";
    echo "RimWorld path is set to $RIMWORLDPATH";
    echo "RimWorld mod path is set to $MODFOLDERPATH";
    echo;
}
export -f SetAllNeededVariables;

CreateRequiredFiles()
{
    mkdir "$MODFOLDERPATH";
    mkdir "$MODFOLDERPATH/About";
    mkdir "$MODFOLDERPATH/Current";
    mkdir "$MODFOLDERPATH/Current/Assemblies";

    touch "$MODFOLDERPATH/LoadFolders.xml";
    touch "$MODFOLDERPATH/About/About.xml";
}
export -f CreateRequiredFiles;

BuildMod()
{
    cd ..
    cd Source;
    dotnet build RW-Boilerplate.sln;
    echo $MODFOLDERPATH/$YOURMODNAME.dll
    cp "bin/Debug/net472/RW-Boilerplate.dll" "$MODFOLDERPATH/Current/Assemblies/$YOURMODNAME.dll"; 
}
export -f BuildMod;


DisplayEndingOptions()
{
    echo "Please enter your choice:";
    echo;
    options=("Start RimWorld" "Exit")
    select opt in "${options[@]}"
    do
        case $opt in
            "Start RimWorld") StartGame; break;;
            "Exit") exit; break;;
            *) echo "invalid option '$REPLY'";;
        esac
    done
}
export -f DisplayEndingOptions;

StartGame()
{
    echo;
    sh -c "$RIMWORLDPATH/RimWorldLinux";
}
export -f StartGame;

MultiStartGame()
{
    echo;
    sh -c "$RIMWORLDPATH/RimWorldLinux & $RIMWORLDPATH/RimWorldLinux";
}
export -f MultiStartGame;

CheckIfFileWithPathIsPresent;
SetAllNeededVariables;
CreateRequiredFiles;
BuildMod;
DisplayEndingOptions;