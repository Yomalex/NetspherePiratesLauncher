# NetspherePiratesLauncher
Launcher of Netsphere

## Launcher
Launcher like the original of Season 1

Use .Net framework 4.x

### Options
Option file of launcher is patcher_s4.option.s4
this file has FTP information and others options like News URL and Register URL

patcher_s4.option.s4 file:
``````
<?xml version="1.0" encoding="utf-8"?>
<options xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Lang>eng</Lang>
  <Version>0</Version>
  <AuthIP>localhost</AuthIP>
  <Register>http://localhost/Register</Register>
  <News>http://localhost/News</News>
  <FTP>
    <URL>ftp://localhost/</URL>
    <User>S4League</User>
    <Password></Password>
  </FTP>
</options>
``````

## Patch Maker
This is a tool to make the version.xml file used by the launcher to download the patch
Use the client directory as run arguments.

Needs .Net Core 2.x
