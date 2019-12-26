# Kaleidoscope

A Kaleidoscope HLSL shader for Unity's custom post process with HDRP. 

There's a demo-assets folder with an example scene. Don't forget to Edit>Project Settings>HDRP Settings>Add the effect in the After Post Process.

I've tested this with unity 2019.2.0f3 and HDRP version 7.1.7.

How to include the Assets/_KaleidoscopeStuff/Kaleidoscope folder with the 2 files for the effect as a dependency:

Add this line to your Packages/manifest.json:
"ch.brogli.kaleidoscope" : "https://github.com/brogli/Kaleidoscope#upm"
