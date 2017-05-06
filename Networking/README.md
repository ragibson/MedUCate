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

CSVs should be stored in lowercase with spaces replaced with underscores on the server. I.e. "Default Set Name" becomes default_set_name.csv (the upload script will automatically convert the file name to lowercase and change all spaces to underscores).

Make sure that the .csv contains no "special characters" or the game may be unable to parse it (e.g. using the ellipsis character "…" instead of "...").

The name of the .csv file will become the name of the question set in game.

Note: Question and answer lengths should be limited for reading ease, but the game will simply not accommodate answers longer than ~80 characters and questions longer than ~175 characters.

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

Then, place the [addscore.php](addscore.php), [display.php](display.php), and [uniqueUsername.php](uniqueUsername.php) scripts in "DocumentRoot/html" and update the $username, $password, $database values in these scripts to match your setup.

Finally, update the addScoreURL, highScoreURL, and uniqueUsernameURL in [UIManager.cs](../Assets/Scripts/UIManager.cs) and the leaderboard will work with the game.

The leaderboard currently shows the top five players in the game, but this can by easily changed in [display.php](display.php)

If this game is ever available on a public server, a hash should be added to the leaderboard to prevent spoofing of player scores.

<a name = "Website"></a>
## Website Setup

The specifics of the website setup are yours to change.

Our example website prototype is in this Networking directory: [AddQuestions.html](AddQuestions.html), [index.html](index.html), [style.css](style.css), [upload.php](upload.php). The pictures for the website have to be in the same directory as the other files, but are placed in the [Pictures](Pictures) folder here to avoid clutter.

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