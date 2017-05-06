<?php
    // Configuration
    $hostname = 'localhost';
    $username = 'USERNAME_GOES_HERE';
    $password = 'PASSWORD_GOES_HERE';
    $database = 'DATABASE_GOES_HERE';
    
    try {
        $db = new PDO('mysql:host=' . $hostname . ';dbname=' . $database, $username, $password);
    } catch(PDOException $e) {
        echo 'Failed - ' . $e->getMessage();
    }

    $result = $db->prepare("SELECT COUNT(*) FROM scores WHERE name=:name");
    try {
        $result->execute($_GET);
        $rowCount = $result->fetchColumn(0);
        if ($rowCount > 0) {
            echo "name already exists";
        } else {
            echo "username updated";
        }
    } catch(Exception $e) {
        echo 'Failed - ' . $e->getMessage();
    }
?>