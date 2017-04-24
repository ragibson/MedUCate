<?php

    // Configuration
    $hostname = 'localhost';
    $username = 'USERNAME_GOES_HERE';
    $password = 'PASSWORD_GOES_HERE';
    $database = 'DATABASE_GOES_HERE';

    try {
        $dbh = new PDO('mysql:host='. $hostname .';dbname='. $database, $username, $password);
    } catch(PDOException $e) {
        echo '<h1>An error has occurred.</h1><pre>', $e->getMessage() ,'</pre>';
    }

    $sth = $dbh->query('SELECT * FROM scores ORDER BY score DESC LIMIT 5');
    $sth->setFetchMode(PDO::FETCH_ASSOC);

    $result = $sth->fetchAll();

    if(count($result) > 0) {
        echo "ONE MAN ARMY LEADERBOARD", "\n", "\n";
        echo "Player  -  Reputation", "\n", "\n";
        foreach($result as $r) {
            echo $r['name'], "\t -  ", $r['score'], "\n";
        }
    }
    
?>
