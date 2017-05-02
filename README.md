# MedUCate

A trivia/learning game created for a client as the primary project of COMP 585H at UNC-Chapel Hill.

# Table of Contents
  * [TODO](#TODO)
  * [Requirements](#Requirements)
  * [Installation](#Installation)
  * [Networking Setup](#Networking)

<a name = "TODO"></a>
## TODO
  * General
    * Review Questions menu needs to take the form "Set name; Question # of # - ...; Correct Answer - ...; Other Answers - ...;"
    * Vertically center text on all menus?
    * Text input field should delete everything when clicked on and have centered default text, if possible.
    * Singleplayer quickplay menu should have (option # of #) removed.
    * Change Campaign Menu (option # of #)?
    * Multiplayer menu should have (option 1 of 1) removed.
    * Combat time should be reduced to 5 seconds, results to 2.5 seconds (preferably, roundTime/2 and roundTime/4, respectively).
    * Slider should maintain at zero after adding extra time in a trivia round.
    * Slider max value should be set to round start time to have the slider visually start at the far right of the screen in all cases.
    * Usernames should be unique.
    * Make game pieces larger -- 25% of the game area, but keep the star block somewhat small.
    * Start blocks with some overlap at start of new combat round?
    * Multiplayer add +100 reputation on game win.

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

<a name = "Networking"></a>
## Networking Setup
Instructions for setting up the game's networking can be found [here](Networking/README.md).