# Networking

# Table of Contents
  * [Adding Question Sets](#QuestionSets)
  * [Setting Up Server](#Server)

<a name = "QuestionSets"></a>
## Adding Question Sets

The [Excel document](TemplateSheet.xlsm) in this directory will output a CSV files to "C:\CSVToAdd" for use on the server.

<a name = "Server"></a>
## Setting Up Server
 
First, install the LAMP stack on a server.

We have question set CSVs placed in apache2's "DocumentRoot/sets" directory, which requires the following changes to apache2.conf (to allow access to files, but not the directory's index):

    <Directory /var/www/>
            Options -Indexes
            AllowOverride All
            Order Allow,Deny
            Allow from all
    </Directory>

    <Directory /var/www/sets>
            AllowOverride None
            Require all granted
    </Directory>

The game currently looks for the following default question sets:
  * Default_Mental_Health_Set.csv
  * Default_Physical_Health_Set.csv
  * Default_Social_Health_Set.csv
  * Default_Nutritional_Health_Set.csv