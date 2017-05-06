# MedUCate

A trivia/learning game created for a client as the primary project of COMP 585H at UNC-Chapel Hill.

# Table of Contents
  * [Requirements](#Requirements)
  * [Installation](#Installation)
  * [Networking Setup](#Networking)

<a name = "Requirements"></a>
## Requirements
Should work natively on Unity versions >= 5.5.1f1, but will be compatible with most recent versions of Unity if packaged.

<a name = "Installation"></a>
## Installation
Simply importing all the assets into a new Unity project and opening the Main scene should suffice.

In Unity,

    Edit > Project Settings > Player > Resolution and Presentation > Orientation

will allow you to restrict the game to portrait only (for mobile devices).

If you're building the game for older devices, make sure

    Edit > Project Settings > Player > Other Settings > Minimum API Level

agrees with your version of Android.

To reduce some audio decoding delay on mobile devices, change

    Edit > Project Settings > Audio > DSP Buffer Size

to "Best latency".

To reduce some artifacting on specific game textures, uncheck

    Image > Import Settings > Generate Mip Maps

and alter the "Filter Mode" settings as desired.

<a name = "Networking"></a>
## Networking Setup
Instructions for setting up the game's networking can be found [here](Networking/README.md).