<?php

    $target_dir = "uploads/";
    $uploadOk = 1;
    $name = $_FILES["fileToUpload"]["name"];
    $name = strtolower($name);
    preg_match('/^(.*?)(\.\w+)?$/', $name, $matches);
    $stem = $matches[1];
    $extension = isset($matches[2]) ? $matches[2] : '';
    $stem = preg_replace('/\s+/', '_', $stem);
    $stem = preg_replace('/[^\w]/', '', $stem);
    $suffix = '';
    $counter = 0;
    $FileExists = 0;

    while ($FileExists == 0) {
        $Check_File = "$target_dir$stem$suffix$extension";
        if (file_exists($Check_File)) {
            $suffix = (string)$counter;
            $counter++;
        }
        else {
            $FileExists = 1;
        }
    }

    $name = "$stem$suffix$extension";
    $target_file = $target_dir . "$stem$suffix$extension";
    $FileType = pathinfo($target_file, PATHINFO_EXTENSION);

    if ($FileType != "csv") {
        echo "Sorry, only csv allowed.";
        $uploadOk = 0;
    }

    // Check if $uploadOk is set to 0 by an error

    if ($uploadOk == 0) {
        echo "Sorry, your file was not uploaded.";

        // if everything is ok, try to upload file

    }
    else {
        if (move_uploaded_file($_FILES["fileToUpload"]["tmp_name"], $target_file)) {
            echo "The file " . basename($_FILES["fileToUpload"]["name"]) . " has been uploaded.";
        }
        else {
            echo "Sorry, there was an error uploading your file.";
        }
    }

?>