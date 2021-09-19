# Basic Qpang Patcher
This tool will allow you to patch `QpangBin.exe` and connect to a private server using the `QPangID.ini` file.

## Dependencies
- You will need `.NET Framework v4.7.2`, which you can [download here](https://dotnet.microsoft.com/download/dotnet-framework/net472).
- You will need the original QPang/MangeFighter game files.

## Patching
First of all, you will need to either build the tool yourself or simply [download](https://github.com/AnimeShooter/QPangMiniLauncher/releases/download/v1.0.0/QpangLauncher.exe) the binary. Once you obtained a binary you will have to do the following steps:

Step 1: Move the `QpangPatcher.exe` into your Qpang folder.

Step 2: Drag and drop `QPangBin.exe` on top of `QpangPatcher.exe`

Step 3: Open `QpangID.ini` and change `IP=` to your preferred IP *(For example: `IP=AnimeShooter.com`)*.

Step 4: Launch `QPangBin.exe` with the following set of args `-fullscreen:0 -width:800 -height:600 -forcevsync:0 -locale:English`
