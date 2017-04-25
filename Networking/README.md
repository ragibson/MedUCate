# Networking

# Table of Contents
  * [Adding Question Sets](#QuestionSets)
  * [Setting Up Server](#Server)
  * [Leaderboard Setup](#Leaderboard)

<a name = "QuestionSets"></a>
## Adding Question Sets

The [Excel document](TemplateSheet.xlsm) in this directory will output a CSV files to "C:\CSVToAdd" for use on the server.

CSVs should be stored in lowercase with spaces replaced with underscores on the server. I.e. "Default Set Name" becomes default_set_name.csv

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