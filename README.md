# Fast Talker (Skia-powered Port)
Fast Talker (Skia port) is a native extension of earlier work to establish a free and open source application for use in the treatment of communication disorders.  Fast Tasker is built upon Xamarin Forms and SkiaSharp, both open-source frameworks for use in native development of applications for Android, iOS, Windows Mobile, and Blackberry.  Fast Talker is fully supported on all platforms, though only Android and iOS are actively maintained and under evaluation at this point.

### Features
 - Native views in both iOS and Android
 - Dynamically add picture icons and text
 - Use as home screen, limit access to non-communication apps
 - Includes single item and autoclitic frame support
 - Constructs speech output using native functionality
 - Dynamically resize, mark icons, and apply other within-stimulus prompts
 - Incorporate images from anywhere, including your own camera and local pictures
 - Use boards from other devices, with the supported Open Book Format
 - Setup boards remotely and deliver prompting from another computer or tablet
 - Save all boards automatically, all locally managed!

### Images
![Alt text](Samples/Anim-IconMode.gif?raw=true "Drag Icons")
![Alt text](Samples/Anim-Folders.gif?raw=true "Drag Icons")
![Alt text](Samples/Anim-SentenceMode.gif?raw=true "Drag Icons")
![Alt text](Samples/Anim-IconSelection.gif?raw=true "Drag Icons")

### Version
 - 1.0.0.8

### Changelog
 - 1.0.0.8 - Add top or bottom-level frame, for fine-motor and physical limitations
 - 1.0.0.7 - Visual cues for activation, some memory cleanup
 - 1.0.0.6 - Changed settings interactions, for more space
 - 1.0.0.5 - Stablity and bug fixes
 - 1.0.0.4 - Help page added to start, softkeyboard fixes
 - 1.0.0.3 - UI Tweaks
 - 1.0.0.2 - Betas
 - 1.0.0.1 - Documentation
 - 1.0.0.0 - Initial push

### Derivative Works
Fast Talker is a derivative work of an earlier project and uses licensed software:
* [Fast Talker](https://github.com/miyamot0/FastTalker) - MIT - Copyright 2016-2018 Shawn Gilroy.
* [Cross-Platform-Communication-App](https://github.com/miyamot0/Cross-Platform-Communication-App) - MIT - Copyright 2016-2017 Shawn Gilroy. [www.smallnstats.com](http://www.smallnstats.com)

Fast Talker uses licensed visual images in order to operate:
* [Mulberry Symbols](https://github.com/straight-street/mulberry-symbols) - [CC-BY-SA 2.0.](http://creativecommons.org/licenses/by-sa/2.0/uk/) - Copyright 2008-2012 Garry Paxton. [www.straight-street-.com](http://straight-street.com/)

### Referenced Works (Packages)
Fast Talker uses a number of open source projects to work properly:
* [ACR UserDialogs](https://github.com/aritchie/userdialogs) - MIT Licensed - Copyright (c) 2016 Allan Ritchie
* [KeyboardOverlap](https://github.com/paulpatarinski/Xamarin.Forms.Plugins/tree/master/KeyboardOverlap) - MIT Licensed - Copyright (c) 2014 Paul Patarinski
* [LauncherHijack](https://github.com/parrotgeek1/LauncherHijack) - Permissively Licensed. Copyright (c) 2017 Ethan Nelson-Moore
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - MIT Licensed. Copyright (c) 2007 James Newton-King 
* [RotorGames.Popup](https://github.com/rotorgames/Rg.Plugins.Popup) - MIT Licensed. Copyright (c) 2017 RotorGames
* [SkiaSharp](https://github.com/mono/SkiaSharp) - MIT Licensed. Copyright (c) 2015-2016 Xamarin, Inc. Copyright (c) 2017-2018 Microsoft Corporation.
* [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) - MIT Licensed. Copyright (c) 2009-2016 Krueger Systems, Inc.
* [Xamarin.Plugins](https://github.com/jamesmontemagno/Xamarin.Plugins) - MIT Licensed. Copyright (c) 2016 James Montemagno / Refractored LLC
* [Xamarin Forms](https://github.com/xamarin/Xamarin.Forms) - MIT Licensed. Copyright (c) 2016 Microsoft

### Acknowledgements and Credits
* Joseph McCleery, Childrens Hospital of Philadelphia, University of Pennsylvania
* Geraldine Leader, National University of Ireland, Galway

### Installation
Fast Talker (Skia port) can be installed as either an Android or iOS application.  

### Device Owner Mode (Android)
Fast Talker (Skia port) can be set to be a dedicated, SGD-only device by having the administrator run the following command from ADB:

<i>adb shell dpm set-device-owner com.smallnstats.FastTalkerSkiaSharp/com.smallnstats.FastTalkerSkiaSharp.Base.DeviceAdminReceiverClass</i>

Optionally, administators can disable the user warnings displayed on the screen by running the following command from ADB:

<i>adb shell appops set android TOAST_WINDOW deny</i>

Issuing this command will perform indefinite screen pinning, much as single-use devices (e.g., inventory counters, touch screen cash registers) function.

### Download
All downloads, if/when posted, will be hosted at [Small N Stats](http://www.smallnstats.com). Formal app store/market release planned following formal evaluation through research and clinical development.

### Development
This is currently under active development and evaluation.

### License
----
Fast Talker (Skia port) - Copyright Shawn Gilroy, Shawn P. Gilroy. MIT
