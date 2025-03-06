<?php
    /**
     * DB接続
     * 
     * return DBインスタンス 異常時null
     */
    function dbConnect() 
    {
        //設定ファイル読み込み
        $config = parse_ini_file("..\..\config\setting.ini", true);

        //DB接続
        $server = $config["DB"]["Server"];
        $db = $config["DB"]["DBName"];
        $user = $config["DB"]["User"];
        $pwd = $config["DB"]["Password"];
        
        if($server == "" || $db == "" || $user == "" || $pwd == "")
        {
            return null;
        }

        try
        {
            $conn = new PDO( "sqlsrv:Server= $server ; Database = $db ", $user, $pwd);
            $conn->setAttribute( PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION );
        }
        catch(Exception $e)
        {
            $conn = null;
        }

        return $conn;
    }
?>