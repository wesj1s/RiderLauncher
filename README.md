# RiderLauncher
A simple launcher for JetBrains Rider that will launch the latest installed version of Rider with the option of launching with elevated rights. If Rider is already running, it will be brought to the foreground.

# Usage:
The program executable can be run from a sibling folder adacent to the Rider install location(s), or by specifying the Jetbrains Program Files folder as the Start In location in a shortcut.

Eg: placing the executable in "C:\Program Files\JetBrains\Launcher\RiderLauncher.exe" will enumerate all instances of "C:\Program Files\JetBrains\*\build.txt" for the latest version of Rider, and run "C:\Program Files\JetBrains\*\bin\rider64.exe" from the corresponding location.

# Command line options:
 -elevated: run with elevated privileges (presenting UAC prompt if necessary)