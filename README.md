# Ansys Discovery D3D Enabler
Forces Ansys Discovery to use Direct3D11 instead of OpenGL 4.6, making it run in Parallels on Apple Silicon.

![ANSYSDiscovery](Discovery.png "Ansys Discovery in Parallels on MacOS")

# Installation
1. Build the project
1. Copy DiscoveryLoader.exe and DiscoveryLoader.exe.config to the same folder as Discovery.exe (usually C:\Program Files\ANSYS Inc\v251\Discovery)
1. Run DiscoveryLoader.exe

# Uninstallation
1. Delete DiscoveryLoader.exe and DiscoveryLoader.exe.config

# Testing
Tested on Ansys Discovery 2025 R1 25.1 only.

# Limitations
Only the Modeling configuration is supported, other configurations require HW support and fail to initialize. This has been tested on Ansys Discovery 2025 R1 only, it's likely it might break in the future versions.

# How does it work?
It exploits the fact that Direct3D 11 support is still in the code, it's just turned off so that it can't be configured from the outside. The loader bypasses this limitation by loading the EXE and making all the necessary configuration changes through reflection in the runtime.