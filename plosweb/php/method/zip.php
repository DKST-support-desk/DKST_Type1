<?php
	try
	{
		//$argv 0:コマンド 1:zipファイル名 2~:zipに格納するファイルのパス

		//設定ファイル読み込み
		$config = parse_ini_file("..\..\config\setting.ini", true);
		$baseVideoPath = $config["MOVIE"]["MoviePath"];

		//zipファイル作成
		$zip = new ZipArchive;
		$zip->open($baseVideoPath."\\".$argv[1].".zip", ZipArchive::CREATE|ZipArchive::OVERWRITE);
		for($i = 0; $i < count($argv) - 2; $i++)
		{
			//zipファイルの中身を追加
			$fileName = basename($argv[$i + 2]);
			$zip->addFile($baseVideoPath."\\".$fileName, $fileName);
		}

		$zip->close();
	}
	catch(Exception $ex)
	{
		$GLOBALS['logger']->error(CreateLogExceptionError("zipファイル生成処理失敗", $ex));
	}
?>