# Networking

# Table of Contents
  * [Adding Question Sets](#QuestionSets)
  * [Setting Up Server](#Server)
  * [Leaderboard Setup](#Leaderboard)
  * [Website Setup](#Website)
  * [Multiplayer Setup](#Multiplayer)
  * [Debugging](#Debug)

<a name = "QuestionSets"></a>
## Adding Question Sets

The [Excel document](TemplateSheet.xlsm) in this directory will output a CSV files to "C:\CSVToAdd" for use on the server.

CSVs should be stored in lowercase with spaces replaced with underscores on the server. I.e. "Default Set Name" becomes default_set_name.csv

Make sure that the .csv contains no "special characters" or the game may be unable to parse it (e.g. using the ellipsis character "…" instead of "...").

<a name = "Server"></a>
## Setting Up Server
 
First, install the LAMP stack on a server.

We have question set CSVs placed in apache2's "DocumentRoot/sets" (/var/www/html/sets by default) directory, which requires the following changes to apache2.conf (to allow access to files, but not the directory indexes):

    <Directory /var/www/>
            Options -Indexes
            AllowOverride All
            Order Allow,Deny
            Allow from all
    </Directory>

The game currently looks for the following default question sets:
  * default_mental_health_set.csv
  * default_physical_health_set.csv
  * default_social_health_set.csv
  * default_nutritional_health_set.csv

<a name = "Leaderboard"></a>
## Leaderboard Setup

First, create a mysql database and a table to hold leaderboard scores (e.g. create the database meducate and the table scores).

Then, place the addscore.php and display.php scripts in "DocumentRoot/html" and update the $username, $password, $database values in these scripts to match your setup.

Finally, update the addScoreURL and highScoreURL in [UIManager.cs](../Assets/Scripts/UIManager.cs) and the leaderboard will work with the game.

<a name = "Website"></a>
## Website Setup

The specifics of the website setup are yours to change.

Our example website prototype is in this Networking directory: [AddQuestions.html](AddQuestions.html), [index.html](index.html), [style.css](style.css), [upload.php](upload.php).

Our upload php script places the uploaded .csv files into the "DocumentRoot/uploads" folder, which will then have to be manually curated and placed into the "DocumentRoot/sets" folder in order to be used in game.

<a name = "Multiplayer"></a>
## Multiplayer Setup

The game currently runs using my Unity multiplayer personal subscription. You will NOT be able to access this.

You must set the game up in the Unity editor for your own multiplayer subscription (20 concurrent users / 10 concurrent games for free).

Alternatively, install Unity's matchmaking utilities on the server (for "unlimited" concurrent users) and update the Network Manager's "Matchmaker Host" in the Unity editor.

<a name = "Debug"></a>
## Debugging

If something goes wrong on the server, the following commands may help.

    # Will restart the web server (for the website)
    sudo /etc/init.d/apache2 restart
    # OR
    sudo service apache2 restart
    
    # Will restart the mysql server (for the leaderboard)
    sudo /etc/init.d/mysql restart
    # OR
    sudo service mysql restart