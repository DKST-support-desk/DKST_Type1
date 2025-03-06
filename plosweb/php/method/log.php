<?php
    // import
    require_once('../../lib/log4php/Logger.php');

    // 設定ファイル読み込み
    Logger::configure('../../config/log4php-config.xml');

    // 'main'という名前のloggerを使用
    $logger = Logger::getLogger('main');

    /**
     * ERRORログ（Exception用）の出力文字列作成
     */
    function CreateLogExceptionError($msg, $ex) 
    {
        //エラーメッセージ
        $errorMsg = $ex->getMessage();

        //スタックトレースアドレス付与
        $stackTrace = $ex->getTraceAsString();

        //ログの出力文字列
        return AddIPAddress("$msg エラーメッセージ:$errorMsg スタックトレース:$stackTrace");
    }

    /**
     * メッセージの後ろにIPアドレス付与
     */
    function AddIPAddress($msg) 
    {
        //ipアドレス取得
        $ip = $_SERVER['REMOTE_ADDR'];

        return "$msg, $ip";
    }
?>