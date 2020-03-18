# ACNH_Dumper
Decompresses and does some unpacking of files of the romfs for Animal Crossing New Horizons

## Building

* Open the `.sln` in Visual Studio
* Edit the path so that it points to where your dumped & extracted romfs files are.
* When you build & run the program, it will create new folders with the dumped content.

## Dependencies
* zs files are decompressed with [ZstdNet](https://github.com/skbkontur/ZstdNet)
* SARC files are unpacked using logic that I've taken from my prior projects (pk3DS).

## Extra Steps
* Open bfres files (models/textures) in [Switch-Toolbox](https://github.com/KillzXGaming/Switch-Toolbox), which also allows for dumping in bulk.
* msbt files (text) in MSBT-Editor-Reloaded or Kuriimu -- pick your poison.
