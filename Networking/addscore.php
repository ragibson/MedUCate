<?php

    // Configuration
    $hostname = 'localhost';
    $username = 'USERNAME_GOES_HERE';
    $password = 'PASSWORD_GOES_HERE';
    $database = 'DATABASE_GOES_HERE';
    try {
        $dbh = new PDO('mysql:host=' . $hostname . ';dbname=' . $database, $username, $password);
    }

    catch(PDOException $e) {
        echo '<h1>An error has occurred.</h1><pre>', $e->getMessage() , '</pre>';
    }

    $sth = $dbh->prepare('INSERT INTO scores VALUES (null, :name, :score) ON DUPLICATE KEY UPDATE score = :score');
    try {
        $sth->execute($_GET);
    }

    catch(Exception $e) {
        echo '<h1>An error has occurred.</h1><pre>', $e->getMessage() , '</pre>';
    }
    
?>