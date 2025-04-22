# L-System Greenhouse

L-System Greenhouse renders beautiful L-System images on your computer! Written by 
[`Eric Bergman-Terrell`](https://EricBT.com).

# L-Systems

L-Systems (Lindenmayer Systems) are scripts written in a simple *formal grammar*, consisting of a starting string,
called an *axiom*, and rewrite rules, called *productions*.

The axiom is converted into output by using the productions to convert letters in the axiom. Then the output can be
converted, for a specified number of iterations. The final output can be displayed graphically by converting it into
"turtle graphics" commands:

# Turtle Graphics Commands

![L-System Greenhouse Help](https://www.ericbt.com/uploaded_images/L-System%20Greenhouse%20Help.png "L-System Greenhouse Help")

Remarkably, these simple grammars can generate graphics of spectacular beauty.

# Screenshots

![L-System Greenhouse](https://www.ericbt.com/uploaded_images/L-System%20Greenhouse.png "Cherry Blossoms")

![L-System Greenhouse](https://www.ericbt.com/uploaded_images/L-System%20Greenhouse%202.png "Eric's Twig")

# Sample L-System Greenhouse Files

Open the sample files in the release to generate some striking images. They're in the "Sample L-System Greenhouse Files" folder.

# Requirements

L-System Greenhouse runs on Windows or Linux. It could be easily updated to run on Apple OSX machines.

# How to Build

Go to the "L-System Greenhouse\\L-System Greenhouse\\deploy" folder. Run "deploy-all-framework-dependent.ps1" or 
"deploy-all-self-contained.ps1". 

The framework dependent build scripts will create relatively small executables, 
which will require that .NET 9 has already been installed. 

The self-contained scripts will create relatively large 
executables which will not require a previous .NET 9 installation.

# Developers

L-System Greenhouse was developed in the C# programming language, using [`Avalonia`](https://avaloniaui.net/platforms), which allows developers to create .NET UI apps for Windows, Linux, and OSX. 

The app was developed with the [`JetBrains Rider`](https://www.jetbrains.com/rider/) IDE.

After building the app, use the scripts in the "deploy" folder to generate the executable and dependencies.

# References

* [`L-Systems`](https://en.wikipedia.org/wiki/L-system)
* [`The Algorithmic Beauty of Plants`](https://archive.org/details/the-algorithmic-beauty-of-plants)
* [`Eric Bergman-Terrell's Website`](https://EricBT.com)
