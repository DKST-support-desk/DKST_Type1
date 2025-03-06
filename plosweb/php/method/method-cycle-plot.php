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

			$GLOBALS['logger']->info(AddIPAddress("method-cycle-plot インデックス:$funIndex"));

			switch($funIndex)
			{
				case 0:
					//当日稼働時刻取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：当日稼働時刻取得処理"));
					CycleGetOperatingTime();
					break;
				case 1:
					//勤帯取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：勤帯取得処理"));
					CycleGetShift();
					break;
				case 2:
					//作業編成取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：作業編成取得処理"));
					CycleGetComposition();
					break;
				case 3:
					//品番タイプ取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：品番取得処理"));
					CycleGetProductType();
					break;
				case 4:
					//稼働予定日取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：稼働予定日取得処理"));
					CycleGetOperatingPlan();
					break;
				case 5:
					//工程取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：工程取得処理"));
					CycleGetProcess();
					break;
				case 6:
					//作業者取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：作業者取得処理"));
					CycleGetWorker();
					break;
				case 7:
					//標準値マスタ取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：標準値マスタ取得処理"));
					CycleGetStandardVal();
					break;
				case 8:
					//サイクル実績取得
					$GLOBALS['logger']->info(AddIPAddress("サイクル：サイクル実績取得処理"));
					CycleGetResultPlot();
					break;
				case 9:
					//サイクル実績手動除外更新
					$GLOBALS['logger']->info(AddIPAddress("サイクル：サイクル実績手動除外更新処理"));
					CycleUpdateResult();
					break;
				case 10:
					//サイクル実績一括除外更新
					$GLOBALS['logger']->info(AddIPAddress("サイクル：サイクル実績一括除外更新処理"));
					CycleMultiUpdateResult();
					break;
				case 11:
					//標準値マスタ更新
					$GLOBALS['logger']->info(AddIPAddress("サイクル：標準値マスタ更新処理"));
					CycleUpdateStandardVal();
					break;
				case 12:
					//グラフ更新用ダミー処理
					$GLOBALS['logger']->info(AddIPAddress("サイクル：グラフ更新用ダミー処理"));
					CycleDummySend();
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

#region 当日稼働時刻取得 index=0

	/**
	 * 当日稼働時刻取得
	 * index=0
	 */
	function CycleGetOperatingTime()
	{
		$sql = "";
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
			$nowDate = filter_input(INPUT_POST, 'nowDate');

			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:nowData=$nowDate"));

			//当日の実績登録がされていればその開始終了時刻を取得する
			$sql = 
			"
				SELECT
					MIN(StartTime) AS StartTime
				,	MAX(EndTime) AS EndTime
				FROM
					Result_Operating_Shift_Tbl
				WHERE
					OperationDate = :nowDate
			";
			$ps = $conn->prepare($sql);

			$ps->bindValue(":nowDate", $nowDate, PDO::PARAM_STR);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("StartTime" => $row[0], "EndTime" => $row[1] ));
			}

			//見つかればそれを返す
			//if($list[0].StartTime != null)
			if(is_null($list[0]['StartTime']) == false)
			{
				header("Content-type: application/json; charset=UTF-8");

				//JSONデータを出力
				echo json_encode($list);

				return;
			}


			//当日の計画登録がされていればその開始終了時刻を取得する
			$ps = $conn->prepare(
			"
				SELECT
					MIN(StartTime) AS StartTime
				,	MAX(EndTime) AS EndTime
				FROM
					Plan_Operating_Shift_Tbl
				WHERE
					OperationDate = :nowDate
				AND
					UseFlag = 1
			");
		
			$ps->bindValue(":nowDate", $nowDate, PDO::PARAM_STR);
		
			$ps->execute();
		
			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("StartTime" => $row[0], "EndTime" => $row[1] ));
			}

			//見つかればそれを返す
			if(is_null($list[0]['StartTime']) == false)
			{
				header("Content-type: application/json; charset=UTF-8");

				//JSONデータを出力
				echo json_encode($list);

				return;
			}

			$list = array();
			array_push($list, array("StartTime" => ($nowDate . " 00:00:00"), "EndTime" => ($nowDate . " 23:59:59" )));

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("当日稼働時刻取得失敗", $ex));
			$GLOBALS['logger']->error(AddIPAddress("SQL:".$sql));
		}
	}

#endregion 当日稼働時刻取得 index=0

#region 勤帯取得処理 index=1

	/**
	 * 勤帯取得処理
	 * index=1
	 */
	function CycleGetShift() 
	{
		$sql = "";
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
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate"));

			//SQL
			$ps = $conn->prepare(
			"
				SELECT
					OperationShift
				FROM
					CycleResult_Tbl
				WHERE
					EndTime >= :start
				AND
					EndTime <= :end
				GROUP BY
					OperationShift
				ORDER BY
					OperationShift
			");

			$ps->bindValue(":start", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":end", $endDate, PDO::PARAM_STR);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("OperationShift" => $row[0] ));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("勤帯取得処理失敗", $ex));
		}
	}

#endregion 勤帯取得処理 index=1

#region 編成取得処理 index=2

	/**
	 * 編成取得処理
	 * index=2
	 */
	function CycleGetComposition() 
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
			$shift = filter_input(INPUT_POST, 'shift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate shift=$shift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($shift != "ALL")
			{
				$shiftList = explode(",", $shift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			$ps = $conn->prepare(
			"
				SELECT
					Composition_Mst.UniqueName
				,	Composition_Mst.CompositionId
				,	Composition_Mst.CreateDateTime
				FROM
					CycleResult_Tbl
				INNER JOIN
					Composition_Mst
				ON
					Composition_Mst.CompositionId = CycleResult_Tbl.CompositionId
				WHERE
					CycleResult_Tbl.EndTime >= :start
				AND
					CycleResult_Tbl.EndTime <= :end
				$shiftSerch
				GROUP BY
					Composition_Mst.UniqueName
				,	Composition_Mst.CompositionId
				,	Composition_Mst.CreateDateTime
				ORDER BY
					Composition_Mst.CreateDateTime
			");

			$ps->bindValue(":start", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":end", $endDate, PDO::PARAM_STR);
			if($shift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("UniqueName" => $row[0], "CompositionId" => $row[1] ));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("編成取得処理失敗", $ex));
		}
	}

#endregion 編成取得処理 index=2

#region 品番取得処理 index=3

	/**
	 * 品番取得処理
	 * index=3
	 */
	function CycleGetProductType() 
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
			$shift = filter_input(INPUT_POST, 'shift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate shift=$shift compositionId=$compositionId"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($shift != "ALL")
			{
				$shiftList = explode(",", $shift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			$ps = $conn->prepare(
			"
				SELECT
					ProductType_Mst.ProductTypeName
				,	ProductType_Mst.ProductTypeId
				,	ProductType_Mst.OrderIdx
				FROM
					CycleResult_Tbl
				INNER JOIN
					ProductType_Mst
				ON
					ProductType_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				WHERE
					CycleResult_Tbl.EndTime >= :start
				AND
					CycleResult_Tbl.EndTime <= :end
				$shiftSerch
				AND
					CycleResult_Tbl.CompositionId = :compositionId
				GROUP BY
					ProductType_Mst.ProductTypeId
				,	ProductType_Mst.ProductTypeName
				,	ProductType_Mst.OrderIdx
				ORDER BY
					ProductType_Mst.OrderIdx
			");

			$ps->bindValue(":start", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":end", $endDate, PDO::PARAM_STR);
			if($shift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("ProductTypeName" => $row[0], "ProductTypeId" => $row[1] ));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("品番取得処理失敗", $ex));
		}
	}

#endregion 品番取得処理 index=3

#region 計画予定日取得処理 index=4

	/**
	 * 計画予定日取得処理
	 * index=4
	 */
	function CycleGetOperatingPlan() 
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

#endregion 計画予定日取得処理 index=4

#region 工程取得処理 index=5

	/**
	 * 工程取得処理
	 * index=5
	 */
	function CycleGetProcess() 
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
			$shift = filter_input(INPUT_POST, 'shift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			$logText = "画面からの引数:startDate=$startDate endDate=$endDate shift=$shift compositionId=$compositionId productTypeId=";
			foreach($productTypeId as $id)
			{
				$logText .= $id.". ";
			}
			$GLOBALS['logger']->info(AddIPAddress($logText));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($shift != "ALL")
			{
				$shiftList = explode(",", $shift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//品番ALL検索以外はWHERE条件を足す
			$productSerch = "";
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					if($i == 0)
					{
						$productSerch .=
						"
							AND
							(
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
					else
					{
						$productSerch .=
						"
							OR
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
				}
				$productSerch .=")";
			}

			$ps = $conn->prepare(
			"
				SELECT
					Composition_Process_Mst.ProcessName
				,	Composition_Process_Mst.ProcessIdx
				FROM
					CycleResult_Tbl
				INNER JOIN
					Composition_Process_Mst
				ON
					Composition_Process_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					Composition_Process_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				WHERE
					CycleResult_Tbl.EndTime >= :startDate
				AND
					CycleResult_Tbl.EndTime <= :endDate
				AND
					CycleResult_Tbl.CompositionId = :compositionId
				$shiftSerch
				$productSerch
				GROUP BY
					Composition_Process_Mst.ProcessName
				,	Composition_Process_Mst.ProcessIdx
				ORDER BY
					Composition_Process_Mst.ProcessIdx
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			if($shift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					$ps->bindValue(":productTypeId_$i", $productTypeId[$i], PDO::PARAM_STR);
				}
			}

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("ProcessName" => $row[0], "ProcessIdx" => $row[1] ));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("工程取得処理失敗", $ex));
		}
	}

#endregion 工程取得処理 index=5

#region 作業者取得処理 index=6

	/**
	 * 作業者取得処理
	 * index=6
	 */
	function CycleGetWorker() 
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
			$shift = filter_input(INPUT_POST, 'shift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			$processIdx = filter_input(INPUT_POST, "processIdx");
			
			$logText = "画面からの引数:startDate=$startDate endDate=$endDate shift=$shift compositionId=$compositionId productTypeId=";
			foreach($productTypeId as $id)
			{
				$logText .= $id.". ";
			}
			$logText .= "processIdx=$processIdx";
			$GLOBALS['logger']->info(AddIPAddress($logText));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($shift != "ALL")
			{
				$shiftList = explode(",", $shift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//品番ALL検索以外はWHERE条件を足す
			$productSerch = "";
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					if($i == 0)
					{
						$productSerch .=
						"
							AND
							(
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
					else
					{
						$productSerch .=
						"
							OR
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
				}
				$productSerch .=")";
			}

			$ps = $conn->prepare(
			"
				SELECT
					WorkerName
				FROM
					CycleResult_Tbl
				WHERE
					EndTime >= :startDate
				AND
					EndTime <= :endDate
				AND
					CompositionId = :compositionId
				AND
					ProcessIdx = :processIdx
				$shiftSerch
				$productSerch
				GROUP BY
					WorkerName
				ORDER BY
					WorkerName
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_STR);
			if($shift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					$ps->bindValue(":productTypeId_$i", $productTypeId[$i], PDO::PARAM_STR);
				}
			}

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("WorkerName" => $row[0]));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("作業者取得処理失敗", $ex));
		}
	}

#endregion 作業者取得処理 index=6

#region 標準値マスタ取得処理 index=7

	/**
	 * 標準値マスタ取得処理
	 * index=7
	 */
	function CycleGetStandardVal() 
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
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			$processIdx = filter_input(INPUT_POST, "processIdx");

			$logText = "画面からの引数:compositionId=$compositionId productTypeId=";
			foreach($productTypeId as $id)
			{
				$logText .= $id.". ";
			}
			$logText .= "processIdx=$processIdx";
			$GLOBALS['logger']->info(AddIPAddress($logText));

			//標準値マスタ取得
			$ps = $conn->prepare(
			"
				SELECT
					CycleTimeMin
				,	CycleTimeAverage
				,	CycleTimeMax
				,	CycleTimeUpper
				,	CycleTimeLower
				FROM
					StandardVal_ProductType_Process_Mst
				WHERE
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
				AND
					ProcessIdx = :processIdx
			");

			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId[0], PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("CycleTimeMin" => FormatDigit($row[0], 1),
										"CycleTimeAverage" => FormatDigit($row[1], 1),
										"CycleTimeMax" => FormatDigit($row[2], 1),
										"CycleTimeUpper" => FormatDigit($row[3], 1),
										"CycleTimeLower" => FormatDigit($row[4], 1)
									));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("標準値マスタ取得処理失敗", $ex));
		}
	}

#endregion 標準値マスタ取得処理 index=7

#region サイクルプロット実績取得処理 index=8

	/**
	 * サイクルプロット実績取得取得処理
	 * index=8
	 */
	function CycleGetResultPlot() 
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
			$shift = filter_input(INPUT_POST, 'shift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$workerName = filter_input(INPUT_POST, "workerName", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);
			$isErrorOnly = filter_input(INPUT_POST, "isErrorOnly");

			$logText = "画面からの引数:startDate=$startDate endDate=$endDate shift=$shift compositionId=$compositionId productTypeId=";
			foreach($productTypeId as $id)
			{
				$logText .= $id.". ";
			}
			$logText .= "processIdx=$processIdx workerName=";
			foreach($workerName as $name)
			{
				$logText .= $name.". ";
			}
			$logText .= "isErrorOnly=$isErrorOnly";
			$GLOBALS['logger']->info(AddIPAddress($logText));


			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($shift != "ALL")
			{
				$shiftList = explode(",", $shift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								CycleResult_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//品番ALL検索以外はWHERE条件を足す
			$productSerch = "";
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					if($i == 0)
					{
						$productSerch .=
						"
							AND
							(
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
					else
					{
						$productSerch .=
						"
							OR
								CycleResult_Tbl.ProductTypeId = :productTypeId_$i
						";
					}
				}
				$productSerch .=")";
			}

			//作業者ALL検索以外はWHERE条件を足す
			$workerSerch = "";
			if($workerName[0] != "ALL")
			{
				for($i = 0; $i < count($workerName); $i++ )
				{
					if($i == 0)
					{
						$workerSerch .=
						"
							AND
							(
								CycleResult_Tbl.WorkerName = :workerName_$i
						";
					}
					else
					{
						$workerSerch .=
						"
							OR
								CycleResult_Tbl.WorkerName = :workerName_$i
						";
					}
				}
				$workerSerch .=")";
			}

			$errorOnlySerch = "";
			if($isErrorOnly == "true")
			{
				$errorOnlySerch .=
				"
					AND
						CycleResult_Tbl.ErrorFlag != 2
					AND
					(
							CycleResult_Tbl.ErrorFlag = 1
						OR
							CycleResult_Tbl.CycleTime < StandardVal_ProductType_Process_Mst.CycleTimeLower
						OR
							CycleResult_Tbl.CycleTime > StandardVal_ProductType_Process_Mst.CycleTimeUpper
					)
				";
			}

			//標準値マスタ取得
			$ps = $conn->prepare(
			"
				SELECT
					CycleResult_Tbl.StartTime
				,	CycleResult_Tbl.EndTime
				,	ProductType_Mst.ProductTypeName
				,	CycleResult_Tbl.WorkerName
				,	CycleResult_Tbl.CycleTime
				,	StandardVal_ProductType_Process_Mst.CycleTimeUpper
				,	StandardVal_ProductType_Process_Mst.CycleTimeMin
				,	StandardVal_ProductType_Process_Mst.CycleTimeLower
				,	CycleResult_Tbl.ErrorFlag
				,	CycleResult_Tbl.CompositionId
				,	CycleResult_Tbl.ProductTypeId
				,	CycleResult_Tbl.ProcessIdx
				,	CycleResult_Tbl.OperationShift
				,	Composition_Mst.UniqueName AS CompositionName
				,	Composition_Process_Mst.ProcessName
				,	CycleResult_Tbl.VideoFileName
				FROM
					CycleResult_Tbl
				INNER JOIN
					Composition_Mst
				ON
					Composition_Mst.CompositionId = CycleResult_Tbl.CompositionId
				INNER JOIN
					ProductType_Mst
				ON
					ProductType_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				INNER JOIN
					Composition_Process_Mst
				ON
					Composition_Process_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					Composition_Process_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				INNER JOIN
					StandardVal_ProductType_Process_Mst
				ON
					StandardVal_ProductType_Process_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				AND
					StandardVal_ProductType_Process_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					StandardVal_ProductType_Process_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				WHERE
					CycleResult_Tbl.EndTime >= :startDate
				AND
					CycleResult_Tbl.EndTime <= :endDate
				AND
					CycleResult_Tbl.CompositionId = :compositionId
				AND
					CycleResult_Tbl.ProcessIdx = :processIdx
				$shiftSerch
				$productSerch
				$workerSerch
				$errorOnlySerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);
			if($shift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			if($productTypeId[0] != "ALL")
			{
				for($i = 0; $i < count($productTypeId); $i++ )
				{
					$ps->bindValue(":productTypeId_$i", $productTypeId[$i], PDO::PARAM_STR);
				}
			}
			if($workerName[0] != "ALL")
			{
				for($i = 0; $i < count($workerName); $i++ )
				{
					$ps->bindValue(":workerName_$i", $workerName[$i], PDO::PARAM_STR);
				}
			}

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				
				//動画ファイルがあるかチェックする
				if($row[15] == null || $row[15] == "")
				{
					$videoExists = 0;
				}
				else
				{
					$videoExists = 1;
				}
 
				$shiftName = "";
				switch($row[12])
				{
					case 1:
						$shiftName = "一直";
						break;
					case 2:
						$shiftName = "二直";
						break;
					case 3:
						$shiftName = "三直";
						break;
				}

				array_push($list, array("StartTime" => $row[0],
										"EndTime" => $row[1],
										"ProductTypeName" => $row[2],
										"WorkerName" => $row[3],
										"CycleTime" => FormatDigit(RoundUpSecond($row[4]), 1),
										"VideoExists" => $videoExists,
										"CycleTimeUpper" => FormatDigit($row[5], 1),
										"CycleTimeMin" => FormatDigit($row[6], 1),
										"CycleTimeLower" => FormatDigit($row[7], 1),
										"ErrorFlag" => $row[8],
										"CompositionId" => $row[9],
										"ProductTypeId" => $row[10],
										"ProcessIdx" => $row[11],
										"OperationShift" => $shiftName,
										"CompositionName" => $row[13],
										"ProcessName" => $row[14],
									));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("標準値マスタ取得処理失敗", $ex));
		}
	}

#endregion サイクルプロット実績取得処理 index=8

#region サイクル実績手動除外更新処理 index=9

	/**
	 * サイクル実績手動除外更新処理
	 * index=9
	 */
	function CycleUpdateResult() 
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
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$startTime = filter_input(INPUT_POST, "startTime");
			$isException = filter_input(INPUT_POST, "isException");

			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:compositionId=$compositionId productTypeId=$productTypeId processIdx=$processIdx startTime=$startTime isException=$isException"));

			//サイクル実績手動除外更新
			$ps = $conn->prepare(
			"
				UPDATE
					CycleResult_Tbl
				SET
					ErrorFlag = :isException
				WHERE
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
				AND
					ProcessIdx = :processIdx
				AND
					StartTime = :startTime
			");

			$ps->bindValue(":isException", $isException, PDO::PARAM_INT);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);
			$ps->bindValue(":startTime", $startTime, PDO::PARAM_STR);

			$GLOBALS['logger']->info(AddIPAddress("サイクル実績手動除外更新 [SET]ErrorFlag=$isException [WHERE]CompositionId=$compositionId ProductTypeId=$productTypeId ProcessIdx=$processIdx StartTime=$startTime"));

			$conn->beginTransaction();
			$ps->execute();
			$conn->commit();

			//更新した件数を取得
			$count = $ps->rowCount();

			$list = array("UpdateCount" => $count);
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("サイクル実績手動除外更新処理失敗", $ex));

			$list = array("UpdateCount" => 0);
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
	}

#endregion サイクル実績手動除外更新処理 index=9

#region サイクル実績一括除外更新処理 index=10

	/**
	 * サイクル実績一括除外更新処理
	 * index=10
	 */
	function CycleMultiUpdateResult()
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
			$isError = filter_input(INPUT_POST, "isError");
			$fromStartTime = filter_input(INPUT_POST, "fromStartTime");
			$toStartTime = filter_input(INPUT_POST, "toStartTime");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:isError=$isError fromStartTime=$fromStartTime toStartTime=$toStartTime processIdx=$processIdx"));
			
			//サイクル実績一括除外更新
			$ps = $conn->prepare(
			"
				UPDATE
					CycleResult_Tbl
				SET
					ErrorFlag = :isError
				WHERE
					StartTime >= :fromStartTime
				AND
					StartTime <= :toStartTime
				AND
					ProcessIdx = :processIdx
			");

			$ps->bindValue(":isError", $isError, PDO::PARAM_INT);
			$ps->bindValue(":fromStartTime", $fromStartTime, PDO::PARAM_STR);
			$ps->bindValue(":toStartTime", $toStartTime, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$GLOBALS['logger']->info(AddIPAddress("サイクル実績一括除外更新 [SET]ErrorFlag=$isError [WHERE]StartTime>=$fromStartTime StartTime<=$toStartTime ProcessIdx=$processIdx"));

			$conn->beginTransaction();
			$ps->execute();
			$conn->commit();

			//更新した件数を取得
			$count = $ps->rowCount();
			$GLOBALS['logger']->info(AddIPAddress("サイクル実績一括除外更新成功 更新件数$count"."件"));

			$list = array("UpdateCount" => $count);
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("サイクル実績一括除外更新処理失敗", $ex));

			$list = array("UpdateCount" => 0);
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
	}

#endregion サイクル実績一括除外更新処理 index=10

#region 標準値マスタ更新処理 index=11

	/**
	 * 標準値マスタ更新処理
	 * index=11
	 */
	function CycleUpdateStandardVal()
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
			$updateName = filter_input(INPUT_POST, "updateName");
			$updateVal = filter_input(INPUT_POST, "updateVal");
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:updateName=$updateName updateVal=$updateVal compositionId=$compositionId productTypeId=$productTypeId processIdx=$processIdx"));

			//標準値マスタ値チェック処理
			list($ret, $retMsg) = CycleCheckStandardVal($conn);
			if($ret == false)
			{
				$list = array("UpdateCount" => 0, "ErrorMsg" => $retMsg);
				header("Content-type: application/json; charset=UTF-8");

				//JSONデータを出力
				echo json_encode($list);
				return;
			}

			$dispersionSet = "";
			if($updateName == "CycleTimeAverage")
			{
				$dispersionSet =
				"
				,	CycleTimeDispersion = $updateVal - StandardVal_ProductType_Process_Mst.CycleTimeMin
				";
			}
			else if($updateName == "CycleTimeMin")
			{
				$dispersionSet =
				"
				,	CycleTimeDispersion = StandardVal_ProductType_Process_Mst.CycleTimeAverage - $updateVal
				";
			}

			//サイクル実績手動除外更新
			$ps = $conn->prepare(
			"
				UPDATE
					StandardVal_ProductType_Process_Mst
				SET
					$updateName = :updateVal
				$dispersionSet
				WHERE
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
				AND
					ProcessIdx = :processIdx
			");

			$ps->bindValue(":updateVal", $updateVal, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$GLOBALS['logger']->info(AddIPAddress("標準値マスタ更新 [SET]$updateName=$updateVal [WHERE]CompositionId=$compositionId ProductTypeId=$productTypeId ProcessIdx=$processIdx"));

			$conn->beginTransaction();
			$ps->execute();
			$conn->commit();

			//更新した件数を取得
			$count = $ps->rowCount();
			$GLOBALS['logger']->info(AddIPAddress("標準値マスタ更新成功 更新件数$count"."件"));

			$list = array("UpdateCount" => $count, "ErrorMsg" => $retMsg);
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("標準値マスタ更新処理失敗", $ex));

			$list = array("UpdateCount" => 0, "ErrorMsg" => "標準値マスタ更新処理失敗");
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
	}

	/**
	 * 標準値マスタ値チェック処理
	 */
	function CycleCheckStandardVal($conn)
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("標準値マスタ値チェック処理"));

			//画面から送られたきた値
			$updateName = filter_input(INPUT_POST, "updateName");
			$updateVal = filter_input(INPUT_POST, "updateVal");
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:updateName=$updateName updateVal=$updateVal compositionId=$compositionId productTypeId=$productTypeId processIdx=$processIdx"));

			//サイクル実績手動除外更新
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					CycleTimeMax
				,	CycleTimeAverage
				,	CycleTimeMin
				,	CycleTimeUpper
				,	CycleTimeLower
				FROM
					StandardVal_ProductType_Process_Mst
				WHERE
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
				AND
					ProcessIdx = :processIdx
			");

			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				$ctMax = $row[0];
				$ctAverage = $row[1];
				$ctMin = $row[2];
				$ctUpper = $row[3];
				$ctLower = $row[4];

				switch($updateName)
				{
					case "CycleTimeMax":
						//CT最大値更新時
						if($updateVal < $ctAverage || $updateVal < $ctMin)
						{
							$GLOBALS['logger']->error(AddIPAddress("最大値は平均値、最小値よりも大きい値にしてください"));
							return array(false, "最大値は平均値、最小値よりも大きい値にしてください");
						}
						break;

					case "CycleTimeAverage":
						//CT平均値更新時
						if($updateVal > $ctMax || $updateVal < $ctMin)
						{
							$GLOBALS['logger']->error(AddIPAddress("平均値は最大値以下かつ最小値以上の値にしてください"));
							return array(false, "平均値は最大値以下かつ最小値以上の値にしてください");
						}
						break;

					case "CycleTimeMin":
						//CT最小値更新時
						if($updateVal > $ctMax || $updateVal > $ctAverage)
						{
							$GLOBALS['logger']->error(AddIPAddress("最小値は最大値、平均値よりも小さい値にしてください"));
							return array(false, "最小値は最大値、平均値よりも小さい値にしてください");
						}
						break;

					case "CycleTimeUpper":
						//上限カット値更新時
						if($updateVal < $ctLower)
						{
							$GLOBALS['logger']->error(AddIPAddress("上限カット値は下限カット値よりも大きい値にしてください"));
							return array(false, "上限カット値は下限カット値よりも大きい値にしてください");
						}
						break;

					case "CycleTimeLower":
						//下限カット値更新時
						if($updateVal > $ctUpper)
						{
							$GLOBALS['logger']->error(AddIPAddress("下限カット値は上限カット値よりも小さい値にしてください"));
							return array(false, "下限カット値は上限カット値よりも小さい値にしてください");
						}
						break;
					
					default:
						$GLOBALS['logger']->error(AddIPAddress("引数異常 updateName:$updateName"));
						return array(false, "引数異常 updateName:$updateName");
						break;
				}

				return array(true, "");
			}

			$GLOBALS['logger']->error(AddIPAddress("標準値マスタ取得失敗"));
			return array(false, "標準値マスタ取得失敗");
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("標準値マスタ値チェック処理失敗", $ex));

			return array(false, "標準値マスタ値チェック処理失敗");
		}
	}

#endregion 標準値マスタ更新処理 index=11

#region ダミーデータ送信処理 index=12

	/**
	 * ダミーデータ送信処理
	 * index=12
	 */
	function CycleDummySend()
	{
		try
		{
			$list = array("DummyData" => "");
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("ダミーデータ送信処理失敗", $ex));

			$list = array("DummyData" => "");
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
	}

#endregion ダミーデータ送信処理 index=12

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
	 * 小数点第二位で切り上げ
	 * 
	 * @param double $number 数値
	 * @return double 小数点第二位で切り上げした数値
	 */
	function RoundUpSecond($number)
	{
		//100倍してから小数点以下を切り捨てることで小数点第三位以下を消す
		$floorNum = floor($number * 100);
		//10で割ってから小数点以下を切り上げることで小数点第二位の切り上げとする
		$celiNum = ceil($floorNum / 10);
		//10で割ったものを返すことで元の桁に戻す
		return ($celiNum / 10);
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