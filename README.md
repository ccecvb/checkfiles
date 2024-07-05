# Check OpenEdge source files for inconsistent line endings

Credits to Raymond Chen https://devblogs.microsoft.com/oldnewthing/20190422-00/?p=102433 for the initial program

This program is currently very rough with hardcoded parameters for my particular needs

## Usage

* go to root folder op OpenEdge project
* execute checkfiles

This will recursively check all files in the current directory for inconsistent line endings in OpenEdge sources.
The extensions are hardcoded *.w *.p *.i *.cls *.wx *.df

