<?php
	//画面側でエラーを表示する
	ini_set('display_errors',1);

	//import
	require_once("db.php");
	require_once("log.php");

	//mainとして動く処理
	FunctionSwitch();

	exit;

	/**
	 * main処理
	 * indexによる処理分け
	 */
	function FunctionSwitch() 
	{
		try
		{
			$funIndex = filter_input(INPUT_POST, 'funIndex');
	
			$GLOBALS['logger']->info(AddIPAddress("method-video-plot インデックス:$funIndex"));

			switch($funIndex)
			{
				case 0:
					//当日稼働時刻取得
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：当日稼働時刻取得"));
					VideoGetOperatingPlan();
					break;
				case 1:
					//カメラ取得
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：カメラ取得"));
					VideoGetCamera();
					break;
				case 2:
					//動画ファイル取得
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：動画ファイル取得"));
					VideoGetMovie();
					break;
				case 3:
					//zipファイル作成
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：zipファイル作成"));
					VideoCreateZipFile();
					break;
				case 4:
					//zipファイルチェック
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：zipファイルチェック"));
					VideoCheckZipFile();
					break;
				case 5:
					//zipファイル削除
					$GLOBALS['logger']->info(AddIPAddress("動画表示部：zipファイル削除"));
					VideoDeleteZipFile();
					break;
				default:
					$GLOBALS['logger']->error(AddIPAddress("登録されていない処理インデックス:$funIndex"));
					break;
			}
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("main処理失敗", $ex));
		}
	}

#region 計画予定日取得処理 index=0

	/**
	 * 計画予定日取得処理
	 * index=0
	 */
	function VideoGetOperatingPlan() 
	{
		try
		{
			//DB接続
			$conn = dbConnect();

			if($conn == null)
			{
				$GLOBALS['logger']->error(AddIPAddress("DBへの接続に失敗しました。"));
				exit;
			}

			//SQL
			$ps = $conn->prepare(
			"
				SELECT
					OperationDate
				FROM
					Plan_Operating_Shift_Tbl
				WHERE
					UseFlag = 1
				GROUP BY
					OperationDate
				ORDER BY
					OperationDate
			");

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("OperationDate" => $row[0] ));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("計画予定日取得処理失敗", $ex));
		}
	}

#endregion 計画予定日取得処理 index=0

#region カメラ取得処理 index=1

	/**
	 * カメラ取得処理
	 * index=1
	 */
	function VideoGetCamera() 
	{
		try
		{
			//DB接続
			$conn = dbConnect();

			if($conn == null)
			{
				$GLOBALS['logger']->error(AddIPAddress("DBへの接続に失敗しました。"));
				exit;
			}

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate processIdx=$processIdx"));

			//実績に紐づくカメラ名を取得する
			$ps = $conn->prepare(
			"
				SELECT
					CycleResult_Tbl.StartTime
				,	Camera_1.CameraName AS CameraName_1
				,	Camera_2.CameraName AS CameraName_2
				,	Camera_3.CameraName AS CameraName_3
				,	Camera_4.CameraName AS CameraName_4
				,	Camera_5.CameraName AS CameraName_5
				,	Camera_6.CameraName AS CameraName_6
				,	Camera_7.CameraName AS CameraName_7
				FROM
					CycleResult_Tbl
				INNER JOIN
					DataCollection_Camera_Mst
				ON
					DataCollection_Camera_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					DataCollection_Camera_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				AND
					DataCollection_Camera_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				LEFT JOIN
					Camera_Device_Mst AS Camera_1
				ON
					Camera_1.CameraId = DataCollection_Camera_Mst.CameraID1
				LEFT JOIN
					Camera_Device_Mst AS Camera_2
				ON
					Camera_2.CameraId = DataCollection_Camera_Mst.CameraID2
				LEFT JOIN
					Camera_Device_Mst AS Camera_3
				ON
					Camera_3.CameraId = DataCollection_Camera_Mst.CameraID3
				LEFT JOIN
					Camera_Device_Mst AS Camera_4
				ON
					Camera_4.CameraId = DataCollection_Camera_Mst.CameraID4
				LEFT JOIN
					Camera_Device_Mst AS Camera_5
				ON
					Camera_5.CameraId = DataCollection_Camera_Mst.CameraID5
				LEFT JOIN
					Camera_Device_Mst AS Camera_6
				ON
					Camera_6.CameraId = DataCollection_Camera_Mst.CameraID6
				LEFT JOIN
					Camera_Device_Mst AS Camera_7
				ON
					Camera_7.CameraId = DataCollection_Camera_Mst.CameraID7
				WHERE
					CycleResult_Tbl.StartTime >= :startDate
				AND
					CycleResult_Tbl.StartTime < :endDate
				AND
					CycleResult_Tbl.ProcessIdx = :processIdx
				ORDER BY
					CycleResult_Tbl.StartTime
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$ps->execute();

			//設定ファイル読み込み
			$config = parse_ini_file("..\..\config\setting.ini", true);
			$baseVideoPath = $config["MOVIE"]["MoviePath"];

			$processIdx = $processIdx + 1;

			//カメラ名リスト ※カメラは7個が最大
			$cameraList = array("","","","","","","",);

			//取得した値から動画が存在するカメラ名を取得
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				$dateTime = date('Ymd_His',strtotime($row[0]));

				//動画があるかチェックする
				$videoPathBase = $baseVideoPath."\CYCLE_".$dateTime."_Process$processIdx"."_";

				if($cameraList[0] == "" && $row[1] != null && file_exists($videoPathBase."Cam1.mp4"))
				{
					$cameraList[0] = $row[1];
				}
				if($cameraList[1] == "" && $row[2] != null && file_exists($videoPathBase."Cam2.mp4"))
				{
					$cameraList[1] = $row[2];
				}
				if($cameraList[2] == "" && $row[3] != null && file_exists($videoPathBase."Cam3.mp4"))
				{
					$cameraList[2] = $row[3];
				}
				if($cameraList[3] == "" && $row[4] != null && file_exists($videoPathBase."Cam4.mp4"))
				{
					$cameraList[3] = $row[4];
				}
				if($cameraList[4] == "" && $row[5] != null && file_exists($videoPathBase."Cam5.mp4"))
				{
					$cameraList[4] = $row[5];
				}
				if($cameraList[5] == "" && $row[6] != null && file_exists($videoPathBase."Cam6.mp4"))
				{
					$cameraList[5] = $row[6];
				}
				if($cameraList[6] == "" && $row[7] != null && file_exists($videoPathBase."Cam7.mp4"))
				{
					$cameraList[6] = $row[7];
				}
			}

			$list = array();
			for($i = 0; $i < count($cameraList); $i++)
			{
				if($cameraList[$i] != "")
				{
					array_push($list, array("CameraName" => $cameraList[$i], "CameraIdx" => $i + 1 ));
				}
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("計画予定日取得処理失敗", $ex));
		}
	}

#endregion カメラ取得処理 index=1

#region 動画ファイル取得処理 index=2

	/**
	 * 動画ファイル取得処理
	 * index=2
	 */
	function VideoGetMovie() 
	{
		try
		{
			//DB接続
			$conn = dbConnect();

			if($conn == null)
			{
				$GLOBALS['logger']->error(AddIPAddress("DBへの接続に失敗しました。"));
				exit;
			}

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$cameraIdx = filter_input(INPUT_POST, "cameraIdx");
			$isErrorFlag = filter_input(INPUT_POST, "isErrorFlag");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate processIdx=$processIdx cameraIdx=$cameraIdx isErrorFlag=$isErrorFlag"));

			$errorSerch = "";
			if($isErrorFlag == "true")
			{
				$errorSerch =
				"
				INNER JOIN
					StandardVal_ProductType_Process_Mst
				ON
					StandardVal_ProductType_Process_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					StandardVal_ProductType_Process_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				AND
					StandardVal_ProductType_Process_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				AND
					CycleResult_Tbl.ErrorFlag != 2
				AND
				(
						CycleResult_Tbl.ErrorFlag = 1
					OR
						StandardVal_ProductType_Process_Mst.CycleTimeUpper < CycleResult_Tbl.CycleTime
					OR
						StandardVal_ProductType_Process_Mst.CycleTimeLower > CycleResult_Tbl.CycleTime
				)
				";
			}

			//実績の開始時間を取得する
			$ps = $conn->prepare(
			"
				SELECT
					CycleResult_Tbl.StartTime
				FROM
					CycleResult_Tbl
				$errorSerch
				WHERE
					CycleResult_Tbl.endTime > :startDate
				AND
					CycleResult_Tbl.StartTime < :endDate
				AND
					CycleResult_Tbl.ProcessIdx = :processIdx
				AND
					CycleResult_Tbl.VideoFileName <> ''
				ORDER BY
					CycleResult_Tbl.StartTime
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$ps->execute();

			//設定ファイル読み込み
			$config = parse_ini_file("..\..\config\setting.ini", true);
			$baseVideoPath = $config["MOVIE"]["MoviePath"];
			$fakeDirectory = $config["MOVIE"]["FakeDirectoryName"];
			
			$processIdx = $processIdx + 1;

			$list = array();
			//取得した値から動画が存在するカメラ名を取得
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				$dateTime = date('Ymd_His',strtotime($row[0]));

				//動画があるかチェックする
				$filePath = "\CYCLE_".$dateTime."_Process$processIdx"."_Cam$cameraIdx".".mp4";
				$videoPath = $baseVideoPath.$filePath;

				//存在チェックは絶対パスで行う
				if(file_exists($videoPath))
				{
					//返すパスは疑似ディレクトリパスで行う
					$path = "..\\..\\".$fakeDirectory.$filePath;
					array_push($list, array("FilePath" => $path));
				}
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("動画ファイル取得処理失敗", $ex));
		}
	}

#endregion 動画ファイル取得処理 index=2

#region zipファイル作成処理 index=3

	/**
	 * zipファイル作成処理
	 * index=3
	 */
	function VideoCreateZipFile() 
	{
		try
		{
			//画面から送られたきた値
			$moviePath = filter_input(INPUT_POST, 'moviePath', FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			
			$logText = "画面からの引数:moviePath=";
			foreach($moviePath as $path)
			{
				$logText .= $path.". ";
			}

			$GLOBALS['logger']->info(AddIPAddress($logText));
			
			$today = date("YmdHis");
			$ip = $_SERVER['REMOTE_ADDR'];

			//::1は自IPアドレス
			if($ip == "::1")
			{
				$ip = "127.0.0.1";
			}

			$fileName = $ip."_".$today;
			$cmd = 'start C:\php\php.exe zip.php '.$fileName;
			for($i = 0; $i < count($moviePath); $i++)
			{
				$cmd .= " ".$moviePath[$i];
			}

			//zip生成を行う処理を別プロセスで実行
			$fp = popen($cmd, 'r');
			pclose($fp);

			$list = array("ZipFileName" => $fileName);
			header("Content-type: application/json; charset=UTF-8");

			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("zipファイル作成処理失敗", $ex));
		}
	}

#endregion zipファイル作成処理 index=3

#region zipファイル存在確認処理 index=4

	/**
	 * zipファイル存在確認処理
	 * index=4
	 */
	function VideoCheckZipFile() 
	{
		try
		{
			//画面から送られたきた値
			$zipFileName = filter_input(INPUT_POST, 'zipFileName');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:zipFileName=$zipFileName"));
			
			//設定ファイル読み込み
			$config = parse_ini_file("..\..\config\setting.ini", true);
			$baseVideoPath = $config["MOVIE"]["MoviePath"];
			$fakeDirectory = $config["MOVIE"]["FakeDirectoryName"];

			$ret = "";
			if(file_exists($baseVideoPath."\\".$zipFileName.".zip"))
			{
				$GLOBALS['logger']->info(AddIPAddress("zipファイル発見"));
				$ret = "..\\..\\".$fakeDirectory."\\".$zipFileName.".zip";
			}
			else
			{
				$GLOBALS['logger']->info(AddIPAddress("zipファイル未発見"));
			}

			$list = array("ZipFilePath" => $ret);

			header("Content-type: application/json; charset=UTF-8");

			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("zipファイル作成処理失敗", $ex));
		}
	}

#endregion zipファイル存在確認処理 index=4

#region zipファイル削除処理 index=5

	/**
	 * zipファイル削除処理
	 * index=5
	 */
	function VideoDeleteZipFile() 
	{
		try
		{
			//画面から送られたきた値
			$zipFileName = filter_input(INPUT_POST, 'zipFileName');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:zipFileName=$zipFileName"));
			
			//設定ファイル読み込み
			$config = parse_ini_file("..\..\config\setting.ini", true);
			$baseVideoPath = $config["MOVIE"]["MoviePath"];

			$ret = "true";
			if(unlink($baseVideoPath."\\".$zipFileName.".zip") == false)
			{
				$ret = "false";
				$GLOBALS['logger']->error(AddIPAddress("zipファイル削除失敗:".$baseVideoPath."\\".$zipFileName.".zip"));
			}

			$list = array("isDelete" => $ret);
			header("Content-type: application/json; charset=UTF-8");

			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("zipファイル作成処理失敗", $ex));
		}
	}

#endregion zipファイル削除処理 index=5

	/**
	 * 数値を小数以下付きで文字列として返す
	 * 
	 * @param double $number 数値
	 * @param int $decimalDigit 小数点以下の桁数
	 */
	function FormatDigit($number, $decimalDigit)
	{
		$format = number_format($number, $decimalDigit);
		return str_replace(',', '',$format);
	}

	/**
	 * 1未満の数値が返ってくると小数点前の0が消えているので0を付ける処理
	 */
	function AddTopZero(&$row)
	{
		for($i = 0; $i < count($row); $i++ )
		{
			//nullじゃなくて先頭が小数点なら先頭に0を付ける
			if($row[$i] != null && substr($row[$i], 0, 1) == ".")
			{
				$addZero = "0".$row[$i];
				//数字なら適用する
				if(is_numeric($addZero))
				{
					$row[$i] = $addZero;
				}
			}			
		}
	}
?>