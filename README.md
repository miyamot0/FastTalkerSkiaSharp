# FastTalkerSkiaSharp
Fast Talker is a native extension of earlier work to establish a free and open source application for use in the treatment of communication disorders.  Fast Tasker is built upon Xamarin Forms and SkiaSharp, both open-source frameworks for use in native development of applications for Android, iOS, Windows Mobile, and Blackberry.  Fast Talker is fully supported on all platforms, though only Android and iOS are actively maintained and under evaluation at this point.

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
 - Todo

### Version
 - 1.0.0.0

### Changelog
 - 1.0.0.0 - Initial push

### Derivative Works
Fast Talker is a derivative work of an earlier project and uses licensed software:
* [Fast Talker](https://github.com/miyamot0/FastTalker) - [GPL-V3](https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html) - Copyright 2016-2018 Shawn Gilroy. [www.smallnstats.com](http://www.smallnstats.com)
* [Cross-Platform-Communication-App](https://github.com/miyamot0/Cross-Platform-Communication-App) - [GPL-V3](https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html) - Copyright 2016-2017 Shawn Gilroy. [www.smallnstats.com](http://www.smallnstats.com)

Fast Talker uses licensed visual images in order to operate:
* [Mulberry Symbols](https://github.com/straight-street/mulberry-symbols) - [CC-BY-SA 2.0.](http://creativecommons.org/licenses/by-sa/2.0/uk/) - Copyright 2008-2012 Garry Paxton. [www.straight-street-.com](http://straight-street.com/)

### Referenced Works (Packages)
Fast Talker uses a number of open source projects to work properly:
* [LauncherHijack](https://github.com/parrotgeek1/LauncherHijack) - Permissively Licensed. Copyright (c) 2017 Ethan Nelson-Moore
* [SimpleHTTPServer](https://gist.github.com/aksakalli/9191056) - MIT Licensed. Copyright (c) 2016 Can Güney Aksakalli
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - MIT Licensed. Copyright (c) 2007 James Newton-King 
* [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) - MIT Licensed. Copyright (c) 2009-2016 Krueger Systems, Inc.
* [Xamarin.Plugin.Permissions](https://github.com/jamesmontemagno/Xamarin.Plugins) - MIT Licensed. Copyright (c) 2016 James Montemagno / Refractored LLC
* [Xamarin.Plugin.Media](https://github.com/jamesmontemagno/Xamarin.Plugins) - MIT Licensed. Copyright (c) 2016 James Montemagno / Refractored LLC
* [Xamarin Forms](https://github.com/xamarin/Xamarin.Forms) - MIT Licensed. Copyright (c) 2016 Microsoft
* [CocosSharp2d Forms](https://github.com/mono/CocosSharp) - MIT Licensed. Copyright (c) 2016 Microsoft

### Acknowledgements and Credits
* Joseph McCleery, Childrens Hospital of Philadelphia, University of Pennsylvania
* Geraldine Leader, National University of Ireland, Galway

### Installation
Fast Talker can be installed as either an Android or iOS application.  

### Device Owner Mode (Android)
Fast Talker can be set to be a dedicated, SGD-only device by having the administrator run the following command from ADB:

<i>adb shell dpm set-device-owner com.smallnstats.fasttalkerskiasharp/com.smallnstats.fasttalkerskiasharp.Base.DeviceAdminReceiverClass</i>

Optionally, administators can disable the user warnings displayed on the screen by running the following command from ADB:

<i>adb shell appops set android TOAST_WINDOW deny</i>

Issuing this demand will perform indefinite screen pinning, much as single-use devices (e.g., inventory counters, touch screen cash registers) function.

### Download
All downloads, if/when posted, will be hosted at [Small N Stats](http://www.smallnstats.com). Formal app store/market release planned following formal evaluation through research and clinical development.

### Development
This is currently under active development and evaluation.

### License
----
Fast Talker - Copyright Shawn Gilroy, Shawn P. Gilroy. GPL-Version 3