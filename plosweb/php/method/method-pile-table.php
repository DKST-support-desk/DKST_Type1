<?php
	//画面側でエラーを表示する
	ini_set('display_errors',1);

	//import
	require_once("db.php");
	require_once("log.php");

	//mainとして動く処理
	FunctionSwitch();

	exit;

	//↓↓↓↓↓↓↓↓↓↓関数↓↓↓↓↓↓↓↓↓↓

	/**
	 * main処理
	 * indexによる処理分け
	 */
	function FunctionSwitch() 
	{
		try
		{
			$funIndex = filter_input(INPUT_POST, 'funIndex');

			$GLOBALS['logger']->info(AddIPAddress("method-pile-table インデックス:$funIndex"));
			
			switch($funIndex)
			{
				case 0:
					//ライン情報取得
					$GLOBALS['logger']->info(AddIPAddress("山積み表：ライン情報取得処理"));
					PileGetLineInfo();
					break;
				case 1:
					//勤帯取得
					$GLOBALS['logger']->info(AddIPAddress("山積み表：勤帯取得処理"));
					PileGetShift();
					break;
				case 2:
					//作業編成取得
					$GLOBALS['logger']->info(AddIPAddress("山積み表：作業編成取得処理"));
					PileGetComposition();
					break;
				case 3:
					//品番タイプ取得
					$GLOBALS['logger']->info(AddIPAddress("山積み表：品番タイプ取得処理"));
					PileGetProductType();
					break;
				case 4:
					//稼働予定日取得
					$GLOBALS['logger']->info(AddIPAddress("山積み表：稼働予定日取得処理"));
					PileGetOperatingPlan();
					break;
				case 5:
					//上部テーブル取得処理
					$GLOBALS['logger']->info(AddIPAddress("山積み表：上部テーブル取得処理"));
					PileGetTopTable();
					break;
				case 6:
					//下部テーブル取得処理
					$GLOBALS['logger']->info(AddIPAddress("山積み表：下部テーブル取得処理"));
					PileGetUnderTable();
					break;
				case 7:
					//作業者リスト取得処理
					$GLOBALS['logger']->info(AddIPAddress("山積み表：作業者リスト取得処理"));
					PileGetWorkerList();
					break;
				case 8:
					//サイクル期間取得処理
					$GLOBALS['logger']->info(AddIPAddress("山積み表：サイクル期間取得処理"));
					PileGetCycleSpan(true);
					break;
				case 9:
					//作業者リスト取得処理(作業者更新時)
					$GLOBALS['logger']->info(AddIPAddress("山積み表：作業者リスト取得処理(作業者更新時)"));
					PileGetUnderTableWorkerUpdate();
					break;
				case 10:
					//実績存在チェック
					$GLOBALS['logger']->info(AddIPAddress("山積み表：実績存在チェック処理"));
					PileCheckCycleExists();
					break;
				default:
					break;
			}
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("main処理失敗", $ex));
		}
	}

#region ライン名取得処理 index=0

	/**
	 * ライン名取得処理
	 * index=0
	 */
	function PileGetLineInfo() 
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
					LineName
				,	OccupancyRate
				,	CompositionEfficiency
				FROM
					LineInfo_Mst
			");

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, array("LineName" => $row[0], "OccupancyRate" => $row[1], "CompositionEfficiency" => $row[2] ));
				break;
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("ライン名取得処理失敗", $ex));
		}
	}

#endregion ライン名取得処理 index=0

#region 勤帯取得処理 index=1

	/**
	 * 勤帯取得処理
	 * index=1
	 */
	function PileGetShift() 
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
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate"));

			//SQL
			$ps = $conn->prepare(
			"
				SELECT
					OperationShift
				FROM
					CycleResult_Tbl
				WHERE
					OperationDate >= :start
				AND
					OperationDate <= :end
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
	function PileGetComposition() 
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
					CycleResult_Tbl.OperationDate >= :start
				AND
					CycleResult_Tbl.OperationDate <= :end
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
	function PileGetProductType() 
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
					CycleResult_Tbl.OperationDate >= :start
				AND
					CycleResult_Tbl.OperationDate <= :end
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
	function PileGetOperatingPlan() 
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

#region 上部テーブル取得処理 index=5

	/**
	 * 上部テーブル取得処理
	 * index=5
	 */
	function PileGetTopTable() 
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

			//必要数、定時稼働時間、T.T
			list($requiredNum, $planSecond, $tt) = PileGetTopTablePlan($conn);

			//実績登録がされているかチェック
			$isRegist = PileGetTopTableCheckResult($conn);

			//良品数
			$resultNum = PileGetTopTableResultNum($conn);

			$resultTime = 0;
			$pitch =  0;
			if($isRegist)
			{
				//実績稼働時間
				$resultTime = PileGetTopTableResultTime($conn);

				//出来高ピッチ ※小数点第二位で切り上げ
				if($resultNum != 0 && $resultTime != 0)
				{
					$pitch = RoundUpSecond($resultTime / $resultNum);
				}
			}

			//ネックCT加重
			$neckWeight = PileGetTopTableNeckWeight($conn);

			//ネックCT個別
			$neckSingle = PileGetTopTableNeckSingle($conn);

			$occupancyRate = 0;
			if($isRegist && $pitch != 0)
			{	
				//可動率
				// ネックCT加重÷出来高ピッチ 小数点第一位で四捨五入
				$occupancyRate = round($neckWeight / $pitch * 100, 0);
			}
			

			//値を格納する
			$list = array();

			array_push($list, 
				array( "RequiredNum" => $requiredNum,
					"PlanSecond" => $planSecond,
					"TT" => FormatDigit($tt, 1),
					"ResultNum" => $resultNum,
					"ResultTime" => $resultTime,
					"Pitch" => FormatDigit($pitch, 1),
					"NeckWeight" => FormatDigit($neckWeight, 1),
					"NeckSingle" => FormatDigit($neckSingle, 1),
					"OccupancyRate" => FormatDigit($occupancyRate, 1)
					));

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("上部テーブル取得処理失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルの実績の登録の有無チェック
	 */
	function PileGetTopTableCheckResult($conn) 
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部テーブルの実績登録チェック"));

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								Plan_Operating_Shift_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								Plan_Operating_Shift_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//期間内に計画があるかチェックする
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					*
				FROM
					Plan_Operating_Shift_Tbl
				WHERE
					Plan_Operating_Shift_Tbl.OperationDate >= :startDate
				AND
					Plan_Operating_Shift_Tbl.OperationDate <= :endDate
				AND
					UseFlag = 1
				$shiftSerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			$isPlan = false;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				$isPlan = true;
			}

			if($isPlan == false)
			{
				$GLOBALS['logger']->info(AddIPAddress("選択期間に稼働計画が含まれていない"));
				return false;
			}

			//期間勤帯の範囲に稼働計画があり、稼働実績が登録がされていない稼働日があるかチェックする
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					*
				FROM
					Plan_Operating_Shift_Tbl
				WHERE
					NOT EXISTS
					(
						SELECT
							*
						FROM
							Result_Operating_Shift_Tbl
						WHERE
							Result_Operating_Shift_Tbl.OperationDate = Plan_Operating_Shift_Tbl.OperationDate
						AND
							Result_Operating_Shift_Tbl.OperationShift = Plan_Operating_Shift_Tbl.OperationShift
					)
				AND
					Plan_Operating_Shift_Tbl.OperationDate >= :startDate
				AND
					Plan_Operating_Shift_Tbl.OperationDate <= :endDate
				AND
					Plan_Operating_Shift_Tbl.UseFlag = 1
				$shiftSerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				if($row[0] != null)
				{
					//取得されたなら実績登録されていない日が含まれている
					$GLOBALS['logger']->info(AddIPAddress("実績登録されていない稼働日が選択期間に含まれている"));
					return false;
				}

			}

			$GLOBALS['logger']->info(AddIPAddress("実績登録されていない稼働日が選択期間に含まれていない"));
			return true;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルの実績の登録の有無チェック失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルの予定取得
	 */
	function PileGetTopTablePlan($conn) 
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部テーブルの予定取得"));

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//必要数と定時稼働時間を取得する
			$ps = $conn->prepare(
			"
				SELECT
					SUM(ProductionQuantity) AS ProductionQuantity
				,	SUM(OperationSecond) AS OperationSecond
				FROM
					Plan_Operating_Shift_Tbl
				WHERE
					OperationDate >= :startDate
				AND
					OperationDate <= :endDate
				AND
					UseFlag = 1
				$shiftSerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			$requiredNum = 0;
			$planSecond = 0;
			$tt = 0.0;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				if($row[0] != null)
				{
					//必要数
					$requiredNum = $row[0];
				}
				if($row[1] != null)
				{
					//定時稼働時間
					$planSecond = $row[1];
				}
				if($row[0] != null && $row[1] != null)
				{
					//T.T ※定時稼働時間÷必要数
					$ttOriginal = $planSecond / $requiredNum;
					//小数点第二位で切り下げ
					$tt = DevaluationSecond($ttOriginal);
				}
			}

			return array($requiredNum, $planSecond, $tt);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルの予定取得失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルの良品数取得
	 */
	function PileGetTopTableResultNum($conn)
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部のテーブルの良品数取得"));

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//良品数を取得する
			$ps = $conn->prepare(
			"
				SELECT
					ISNULL(
						SUM(
							CASE	
								WHEN ProductionQuantity IS NOT NULL THEN ProductionQuantity
								ELSE ProductionQuantityOnCycle
							END
						), 0 
					)AS ProductionQuantity
				FROM
					Operating_Shift_ProductionQuantity_Tbl
				WHERE
					OperationDate >= :startDate
				AND
					OperationDate <= :endDate
				$shiftSerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			$resultNum = 0;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				if($row[0] != null)
				{
					//良品数
					$resultNum = (int)$row[0];
				}
			}

			return $resultNum;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルの良品数取得失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルの実績稼働時間取得
	 */
	function PileGetTopTableResultTime($conn)
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部テーブルの実績稼働時間取得"));

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//実績稼働開始時間と終了時間を取得する
			$ps = $conn->prepare(
			"
				SELECT
					StartTime
				,	EndTime
				FROM
					Result_Operating_Shift_Tbl
				WHERE
					OperationDate >= :startDate
				AND
					OperationDate <= :endDate
				$shiftSerch
			");
		
			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
		
			$ps->execute();
		
			$resultTime = 0;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				//開始時間と終了時間の差分から実績時間秒を取得する
				$startTime = strtotime($row[0]);
				$endTime = strtotime($row[1]);
				$resultTime += $endTime - $startTime;
			}

			//除外合計時間を取得する
			$ps = $conn->prepare(
			"
				SELECT
					SUM(ExclusionTime) AS ExclusionTime
				FROM
					Operating_Shift_Exclusion_Tbl
				WHERE
					ExclusionCheck = 1
				AND
					OperationDate >= :startDate
				AND
					OperationDate <= :endDate
				$shiftSerch
			");
			
			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			
			$ps->execute();
			
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				if($row[0] != null)
				{
					AddTopZero($row);
					//除外時間分を引く
					$resultTime -= $row[0];
				}
			}

			return $resultTime;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルの実績稼働時間取得失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルのネックCT加重取得
	 */
	function PileGetTopTableNeckWeight($conn)
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部テーブルのネックCT加重取得"));

			//画面から送られたきた値
			$startDate = filter_input(INPUT_POST, 'startDate');
			$endDate = filter_input(INPUT_POST, 'endDate');
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								Operating_Shift_ProductionQuantity_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								Operating_Shift_ProductionQuantity_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//編成品番毎の工程最大のCT最小値と生産数を取得する
			$ps = $conn->prepare(
			"
				SELECT
					Operating_Shift_ProductionQuantity_Tbl.OperationDate
				,	Operating_Shift_ProductionQuantity_Tbl.OperationShift
				,	StandardVal_ProductType_Process_Mst.CompositionId
				,	StandardVal_ProductType_Process_Mst.ProductTypeId
				,	MAX(StandardVal_ProductType_Process_Mst.CycleTimeMin) AS CycleTimeMin
				,	ISNULL(
						SUM(
							CASE	
								WHEN ProductionQuantity IS NOT NULL THEN ProductionQuantity
								ELSE ProductionQuantityOnCycle
							END
						), 0 
					)AS ProductionQuantity
				FROM
					StandardVal_ProductType_Process_Mst
				INNER JOIN
					Operating_Shift_ProductionQuantity_Tbl
				ON
					Operating_Shift_ProductionQuantity_Tbl.CompositionId = StandardVal_ProductType_Process_Mst.CompositionId
				AND
					Operating_Shift_ProductionQuantity_Tbl.ProductTypeId = StandardVal_ProductType_Process_Mst.ProductTypeId
				WHERE
					Operating_Shift_ProductionQuantity_Tbl.OperationDate >= :startDate
				AND
					Operating_Shift_ProductionQuantity_Tbl.OperationDate <= :endDate
				$shiftSerch
				GROUP BY
					Operating_Shift_ProductionQuantity_Tbl.OperationDate
				,	Operating_Shift_ProductionQuantity_Tbl.OperationShift
				,	StandardVal_ProductType_Process_Mst.CompositionId
				,	StandardVal_ProductType_Process_Mst.ProductTypeId
				,	Operating_Shift_ProductionQuantity_Tbl.ProductionQuantity
			");
		
			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
		
			$ps->execute();
		
			//分子 編成品番毎の工程最大CT最小×生産数
			$numerator = 0;
			//分母 生産数
			$denominator = 0;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				$numerator += $row[4] * $row[5];
				$denominator += $row[5];
			}

			if($numerator != 0 && $denominator != 0)
			{
				//ネックCT加重
				return RoundUpSecond($numerator / $denominator);
			}
			else
			{
				return 0;
			}
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルのネックCT加重取得失敗", $ex));
		}
	}

	/**
	 * 山積み表上部のテーブルのネックCT個別取得
	 */
	function PileGetTopTableNeckSingle($conn)
	{
		try
		{
			$GLOBALS['logger']->info(AddIPAddress("山積み表上部テーブルのネックCT個別取得"));

			//画面から送られたきた値
			$compositionId = filter_input(INPUT_POST, 'compositionId');
			$productTypeId = filter_input(INPUT_POST, 'productTypeId');

			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:compositionId=$compositionId productTypeId=$productTypeId"));

			//編成品番が未選択なら検索しない
			if($compositionId == "-" || $productTypeId == "-")
			{
				return 0;
			}

			//編成品番の最大ネックCT最小値を取得する
			$ps = $conn->prepare(
			"
			SELECT
				MAX(CycleTimeMin) AS CycleTimeMin
			FROM
				StandardVal_ProductType_Process_Mst
			WHERE
				CompositionId = :compositionId
			AND
				ProductTypeId = :productTypeId
			");
		
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
		
			$ps->execute();
		
			$neckSingle = 0;
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				if($row[0] != null)
				{
					AddTopZero($row);
					$neckSingle = RoundUpSecond($row[0]);
				}
			}

			return $neckSingle;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("山積み表上部のテーブルのネックCT個別取得失敗", $ex));
		}
	}

#endregion 上部テーブル取得処理 index=5

#region 下部テーブル取得処理 index=6

	/**
	 * 下部テーブル取得処理
	 * index=6
	 */
	function PileGetUnderTable() 
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
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$compositionId = filter_input(INPUT_POST, 'compositionId');
			$productTypeId = filter_input(INPUT_POST, 'productTypeId');

			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift compositionId=$compositionId productTypeId=$productTypeId"));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
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

			//ネックMCT以外の山積み表データ取得
			$ps = $conn->prepare(
			"
				SELECT
					Composition_Process_Mst.ProcessName
				,	ISNULL(MAX(CycleResult_Tbl.CycleTime), 0) AS ResultCTMax
				,	ISNULL(AVG(CycleResult_Tbl.CycleTime), 0) AS ResultCTAvg
				,	ISNULL(MIN(CycleResult_Tbl.CycleTime), 0) AS ResultCTMin
				,	StandardVal_ProductType_Process_Mst.CycleTimeMax
				,	StandardVal_ProductType_Process_Mst.CycleTimeAverage
				,	StandardVal_ProductType_Process_Mst.CycleTimeMin
				,	StandardVal_ProductType_Process_Mst.CycleTimeDispersion
				,	StandardVal_ProductType_Process_Mst.CycleTimeUpper
				,	StandardVal_ProductType_Process_Mst.CycleTimeLower
				,	StandardVal_ProductType_Process_Mst.Ancillary
				,	StandardVal_ProductType_Process_Mst.Setup
				,	StandardVal_ProductType_Process_Mst.ProcessIdx
				FROM
					StandardVal_ProductType_Process_Mst
				INNER JOIN
					Composition_Process_Mst
				ON
					Composition_Process_Mst.CompositionId = StandardVal_ProductType_Process_Mst.CompositionId
				AND
					Composition_Process_Mst.ProcessIdx = StandardVal_ProductType_Process_Mst.ProcessIdx
				LEFT JOIN
					CycleResult_Tbl
				ON
					CycleResult_Tbl.CompositionId = StandardVal_ProductType_Process_Mst.CompositionId
				AND
					CycleResult_Tbl.ProductTypeId = StandardVal_ProductType_Process_Mst.ProductTypeId
				AND
					CycleResult_Tbl.ProcessIdx = StandardVal_ProductType_Process_Mst.ProcessIdx
				AND
					CycleResult_Tbl.OperationDate >= :startDate
				AND
					CycleResult_Tbl.OperationDate <= :endDate
				$shiftSerch
				AND
				(
					CycleResult_Tbl.ErrorFlag = 2
					OR
					(
							CycleResult_Tbl.ErrorFlag = 0
						AND
							CycleResult_Tbl.CycleTime >= StandardVal_ProductType_Process_Mst.CycleTimeLower
						AND
							CycleResult_Tbl.CycleTime <= StandardVal_ProductType_Process_Mst.CycleTimeUpper
					)
				)
				WHERE
					StandardVal_ProductType_Process_Mst.CompositionId = :compositionId
				AND
					StandardVal_ProductType_Process_Mst.ProductTypeId = :productTypeId
				GROUP BY
					Composition_Process_Mst.ProcessName
				,	StandardVal_ProductType_Process_Mst.CycleTimeMax
				,	StandardVal_ProductType_Process_Mst.CycleTimeAverage
				,	StandardVal_ProductType_Process_Mst.CycleTimeMin
				,	StandardVal_ProductType_Process_Mst.CycleTimeDispersion
				,	StandardVal_ProductType_Process_Mst.CycleTimeUpper
				,	StandardVal_ProductType_Process_Mst.CycleTimeLower
				,	StandardVal_ProductType_Process_Mst.Ancillary
				,	StandardVal_ProductType_Process_Mst.Setup
				,	StandardVal_ProductType_Process_Mst.ProcessIdx
				ORDER BY
					StandardVal_ProductType_Process_Mst.ProcessIdx		
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);

			$ps->execute();

			//取得した値を格納
			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, 
					array("ProcessName" => $row[0], 
						  "ResultCTMax" => FormatDigit(RoundUpSecond($row[1]), 1),
						  "ResultCTAvg" => FormatDigit(RoundUpSecond($row[2]), 1),
						  "ResultCTMin" => FormatDigit(RoundUpSecond($row[3]), 1),
						  "CycleTimeMax" => FormatDigit($row[4], 1),
						  "CycleTimeAverage" => FormatDigit($row[5], 1),
						  "CycleTimeMin" => FormatDigit($row[6], 1),
						  "CycleTimeDispersion" => FormatDigit($row[7], 1),
						  "CycleTimeUpper" => FormatDigit($row[8], 1),
						  "CycleTimeLower" => FormatDigit($row[9], 1),
						  "Ancillary" => FormatDigit($row[10], 1),
						  "Setup" => FormatDigit($row[11], 1),
						  "ProcessIdx" => $row[12]
						));
			}

			//---------
			//ネックMCT以外の山積み表データ取得
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					MachineTime
				,	HumanTime
				,	MachineCycleTime
				FROM
					StandardVal_ProductType_Mst
				WHERE
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
			");

			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
	
			$ps->execute();
	
			//取得した値を格納
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				array_push($list, 
					array("MachineTime" => FormatDigit(RoundUpSecond($row[0]), 1),
						  "HumanTime" => FormatDigit(RoundUpSecond($row[1]), 1),
						  "MachineCycleTime" => FormatDigit(RoundUpSecond($row[2]), 1)
						));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("下部テーブル取得処理失敗", $ex));
		}
	}

#endregion 下部テーブル取得処理 index=6

#region 作業者リスト取得処理 index=7

	/**
	 * 作業者リスト取得処理
	 * index = 7
	 */
	function PileGetWorkerList() 
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
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift compositionId=$compositionId productTypeId=$productTypeId processIdx=$processIdx"));


			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			$ps = $conn->prepare(
			"
				SELECT
					WorkerName
				FROM
					CycleResult_Tbl
				WHERE
					OperationDate >= :start
				AND
					OperationDate <= :end
				AND
					CompositionId = :compositionId
				AND
					ProductTypeId = :productTypeId
				AND
					ProcessIdx = :processIdx
				$shiftSerch
				GROUP BY
					WorkerName
				ORDER BY
					WorkerName
			");

			$ps->bindValue(":start", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":end", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);
			if($operationShift != "ALL")
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
				array_push($list, array("WorkerName" => $row[0]));
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("作業者処理失敗", $ex));
		}
	}

#endregion 作業者リスト取得処理 index=7

#region サイクル期間取得処理 index=8

	/**
	 * サイクル期間取得処理
	 * index = 8
	 */
	function PileGetCycleSpan($isEcho) 
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
			$operationShift = filter_input(INPUT_POST, 'operationShift');

			$GLOBALS['logger']->info(AddIPAddress("画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift"));


			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
				for($i = 0; $i < count($shiftList); $i++ )
				{
					if($i == 0)
					{
						$shiftSerch .=
						"
							AND
							(
								Plan_Operating_Shift_Tbl.OperationShift = :shift_$i
						";
					}
					else
					{
						$shiftSerch .=
						"
							OR
								Plan_Operating_Shift_Tbl.OperationShift = :shift_$i
						";
					}
				}
				$shiftSerch .=")";
			}

			//指定稼働日、シフトの最も早い開始時刻取得
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					Plan_Operating_Shift_Tbl.StartTime AS PlanStartTime
				,	Result_Operating_Shift_Tbl.StartTime AS ResultStartTime 
				,	Plan_Operating_Shift_Tbl.OperationShift
				FROM
					Plan_Operating_Shift_Tbl
				LEFT JOIN
					Result_Operating_Shift_Tbl
				ON
					Result_Operating_Shift_Tbl.OperationDate = Plan_Operating_Shift_Tbl.OperationDate
				AND
					Result_Operating_Shift_Tbl.OperationShift = Plan_Operating_Shift_Tbl.OperationShift
				WHERE
					Plan_Operating_Shift_Tbl.OperationDate = :start
				AND
					Plan_Operating_Shift_Tbl.UseFlag = 1
				$shiftSerch
				ORDER BY
					Plan_Operating_Shift_Tbl.OperationShift
			");

			//Where句セット
			$ps->bindValue(":start", $startDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			$startSpan = "";
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				if($row[1] != null)
				{
					$startSpan = date("Y/m/d H:i:s",strtotime($row[1]));
				}
				else
				{
					$startSpan = date("Y/m/d H:i:s",strtotime($row[0]));
				}
			}

			if($startSpan == "")
			{
				$startSpan = date("Y/m/d 00:00:00",strtotime($startDate));
			}

			//指定稼働日、シフトの最も遅い終了時刻取得
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					Plan_Operating_Shift_Tbl.EndTime AS PlanEndTime
				,	Result_Operating_Shift_Tbl.EndTime AS ResultEndTime 
				,	Plan_Operating_Shift_Tbl.OperationShift
				FROM
					Plan_Operating_Shift_Tbl
				LEFT JOIN
					Result_Operating_Shift_Tbl
				ON
					Result_Operating_Shift_Tbl.OperationDate = Plan_Operating_Shift_Tbl.OperationDate
				AND
					Result_Operating_Shift_Tbl.OperationShift = Plan_Operating_Shift_Tbl.OperationShift
				WHERE
					Plan_Operating_Shift_Tbl.OperationDate = :end
				AND
					Plan_Operating_Shift_Tbl.UseFlag = 1
				$shiftSerch
				ORDER BY
					Plan_Operating_Shift_Tbl.OperationShift DESC
			");

			//Where句セット
			$ps->bindValue(":end", $endDate, PDO::PARAM_STR);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
				}
			}

			$ps->execute();

			$endSpan = "";
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				AddTopZero($row);
				if($row[1] != null)
				{
					$endSpan = date("Y/m/d H:i:s",strtotime($row[1]));
				}
				else
				{
					$endSpan = date("Y/m/d H:i:s",strtotime($row[0]));
				}
			}

			if($endSpan == "")
			{
				$endSpan = date("Y/m/d 23:59:59",strtotime($endDate));
			}

			$returnSpan = "";
			if($startSpan != "" && $endSpan != "")
			{
				if($startSpan <= $endSpan)
				{
					$returnSpan = $startSpan." - ".$endSpan;
				}
			}

			if($isEcho)
			{
				//取得した値を格納
				$list = array();
				array_push($list, array("CycleSpan" => $returnSpan));

				header("Content-type: application/json; charset=UTF-8");

				//JSONデータを出力
				echo json_encode($list);
			}
			
			return $returnSpan;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("サイクル期間取得処理失敗", $ex));
			return "";
		}
	}

#endregion サイクル期間取得処理 index=8

#region 下部テーブル取得処理(作業者更新時) index=9

	/**
	 * 下部テーブル取得処理(作業者更新時)
	 * index=9
	 */
	function PileGetUnderTableWorkerUpdate() 
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
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$compositionId = filter_input(INPUT_POST, 'compositionId');
			$productTypeId = filter_input(INPUT_POST, 'productTypeId');
			$processMaxIdx = filter_input(INPUT_POST, 'processMaxIdx');
			$jsonWorker = filter_input(INPUT_POST, 'jsonWorker', FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);

			$logText = "画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift compositionId=$compositionId productTypeId=$productTypeId processMaxIdx=$processMaxIdx jsonWorker=";
			foreach($jsonWorker["Worker"] as $workerList)
			{
				foreach($workerList as $worker)
				{
					$logText .= $worker.". ";
				}
			}
			$GLOBALS['logger']->info(AddIPAddress($logText));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
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

			//ネックMCT以外の工程分回す
			$list = array();
			for($i = 0; $i < $processMaxIdx - 1; $i++)
			{
				//作業者ALL検索以外はWHERE条件を足す
				$workerSerch = "";
				if($jsonWorker["Worker"][$i][0] != "ALL")
				{
					for($j = 0; $j < count($jsonWorker["Worker"][$i]); $j++ )
					{
						if($j == 0)
						{
							$workerSerch .=
							"
								AND
								(
									CycleResult_Tbl.WorkerName = :workerName_$j
							";
						}
						else
						{
							$workerSerch .=
							"
								OR
									CycleResult_Tbl.WorkerName = :workerName_$j
							";
						}
					}
					$workerSerch .=")";
				}

				//ネックMCT以外の山積み表データ取得
				$ps = $conn->prepare(
				"
					SELECT TOP 1
						Composition_Process_Mst.ProcessName
					,	ISNULL(MAX(CycleResult_Tbl.CycleTime), 0) AS ResultCTMax
					,	ISNULL(AVG(CycleResult_Tbl.CycleTime), 0) AS ResultCTAvg
					,	ISNULL(MIN(CycleResult_Tbl.CycleTime), 0) AS ResultCTMin
					,	StandardVal_ProductType_Process_Mst.CycleTimeMax
					,	StandardVal_ProductType_Process_Mst.CycleTimeAverage
					,	StandardVal_ProductType_Process_Mst.CycleTimeMin
					,	StandardVal_ProductType_Process_Mst.CycleTimeDispersion
					,	StandardVal_ProductType_Process_Mst.CycleTimeUpper
					,	StandardVal_ProductType_Process_Mst.CycleTimeLower
					,	StandardVal_ProductType_Process_Mst.Ancillary
					,	StandardVal_ProductType_Process_Mst.Setup
					,	StandardVal_ProductType_Process_Mst.ProcessIdx
					FROM
						StandardVal_ProductType_Process_Mst
					INNER JOIN
						Composition_Process_Mst
					ON
						Composition_Process_Mst.CompositionId = StandardVal_ProductType_Process_Mst.CompositionId
					AND
						Composition_Process_Mst.ProcessIdx = StandardVal_ProductType_Process_Mst.ProcessIdx
					LEFT JOIN
						CycleResult_Tbl
					ON
						CycleResult_Tbl.CompositionId = StandardVal_ProductType_Process_Mst.CompositionId
					AND
						CycleResult_Tbl.ProductTypeId = StandardVal_ProductType_Process_Mst.ProductTypeId
					AND
						CycleResult_Tbl.ProcessIdx = StandardVal_ProductType_Process_Mst.ProcessIdx
					AND
						CycleResult_Tbl.OperationDate >= :startDate
					AND
						CycleResult_Tbl.OperationDate <= :endDate
					$shiftSerch
					$workerSerch
					AND
						CycleResult_Tbl.ErrorFlag != 1
					AND
						CycleResult_Tbl.CycleTime >= StandardVal_ProductType_Process_Mst.CycleTimeLower
					AND
						CycleResult_Tbl.CycleTime <= StandardVal_ProductType_Process_Mst.CycleTimeUpper
					WHERE
						StandardVal_ProductType_Process_Mst.CompositionId = :compositionId
					AND
						StandardVal_ProductType_Process_Mst.ProductTypeId = :productTypeId
					AND
						StandardVal_ProductType_Process_Mst.ProcessIdx = :processIdx
					GROUP BY
						Composition_Process_Mst.ProcessName
					,	StandardVal_ProductType_Process_Mst.CycleTimeMax
					,	StandardVal_ProductType_Process_Mst.CycleTimeAverage
					,	StandardVal_ProductType_Process_Mst.CycleTimeMin
					,	StandardVal_ProductType_Process_Mst.CycleTimeDispersion
					,	StandardVal_ProductType_Process_Mst.CycleTimeUpper
					,	StandardVal_ProductType_Process_Mst.CycleTimeLower
					,	StandardVal_ProductType_Process_Mst.Ancillary
					,	StandardVal_ProductType_Process_Mst.Setup
					,	StandardVal_ProductType_Process_Mst.ProcessIdx
					ORDER BY
						StandardVal_ProductType_Process_Mst.ProcessIdx		
				");
	
				$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
				$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
				if($operationShift != "ALL")
				{
					for($j = 0; $j < count($shiftList); $j++ )
					{
						$ps->bindValue(":shift_$j", $shiftList[$j], PDO::PARAM_INT);
					}
				}
				if($jsonWorker["Worker"][$i][0] != "ALL")
				{
					for($j = 0; $j < count($jsonWorker["Worker"][$i]); $j++ )
					{
						$ps->bindValue(":workerName_$j", $jsonWorker["Worker"][$i][$j], PDO::PARAM_STR);
					}
				}
				$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
				$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
				$ps->bindValue(":processIdx", $i, PDO::PARAM_INT);
	
				$ps->execute();

				while($row = $ps->fetch(PDO::FETCH_NUM))
				{
					AddTopZero($row);
					array_push($list, 
						array("ProcessName" => $row[0], 
							"ResultCTMax" => FormatDigit(RoundUpSecond($row[1]), 1),
							"ResultCTAvg" => FormatDigit(RoundUpSecond($row[2]), 1),
							"ResultCTMin" => FormatDigit(RoundUpSecond($row[3]), 1),
							"CycleTimeMax" => FormatDigit($row[4], 1),
							"CycleTimeAverage" => FormatDigit($row[5], 1),
							"CycleTimeMin" => FormatDigit($row[6], 1),
							"CycleTimeDispersion" => FormatDigit($row[7], 1),
							"CycleTimeUpper" => FormatDigit($row[8], 1),
							"CycleTimeLower" => FormatDigit($row[9], 1),
							"Ancillary" => FormatDigit($row[10], 1),
							"Setup" => FormatDigit($row[11], 1),
							"ProcessIdx" => $row[12]
							));
				}
			}

			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("下部テーブル取得処理失敗(作業者更新時)", $ex));
		}
	}

#endregion 下部テーブル取得処理(作業者更新時) index=9

#region サイクル実績存在チェック処理 index=10

	/**
	 * サイクル実績存在チェック処理
	 * index=10
	 */
	function PileCheckCycleExists() 
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

			$GLOBALS['logger']->info(AddIPAddress("サイクル期間取得処理"));
			$span = PileGetCycleSpan(false);
			if($span == "")
			{
				return;
			}
			
			$split = explode(" - ", $span);

			//画面から送られたきた値
			$startDate = $split[0];
			$endDate = $split[1];
			$operationShift = filter_input(INPUT_POST, 'operationShift');
			$compositionId = filter_input(INPUT_POST, "compositionId");
			$productTypeId = filter_input(INPUT_POST, "productTypeId");
			$processIdx = filter_input(INPUT_POST, "processIdx");
			$workerName = filter_input(INPUT_POST, "workerName", FILTER_DEFAULT, FILTER_REQUIRE_ARRAY);

			$logText = "画面からの引数:startDate=$startDate endDate=$endDate operationShift=$operationShift compositionId=$compositionId productTypeId=$compositionId processIdx=$processIdx workerName=";

			foreach($workerName as $name)
			{
				$logText .= $name.". ";
			}
			$GLOBALS['logger']->info(AddIPAddress($logText));

			//勤帯ALL検索以外はWHERE条件を足す
			$shiftSerch = "";
			if($operationShift != "ALL")
			{
				$shiftList = explode(",", $operationShift);
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

			//サイクル実績があるかチェックする
			$ps = $conn->prepare(
			"
				SELECT TOP 1
					*
				FROM
					CycleResult_Tbl
				INNER JOIN
					Composition_Mst
				ON
					Composition_Mst.CompositionId = CycleResult_Tbl.CompositionId
				INNER JOIN
					Composition_Process_Mst
				ON
					Composition_Process_Mst.CompositionId = CycleResult_Tbl.CompositionId
				AND
					Composition_Process_Mst.ProcessIdx = CycleResult_Tbl.ProcessIdx
				INNER JOIN
					ProductType_Mst
				ON
					ProductType_Mst.ProductTypeId = CycleResult_Tbl.ProductTypeId
				WHERE
					CycleResult_Tbl.EndTime >= :startDate
				AND
					CycleResult_Tbl.EndTime <= :endDate
				AND
					CycleResult_Tbl.CompositionId =:compositionId
				AND
					CycleResult_Tbl.ProductTypeId =:productTypeId
				AND
					CycleResult_Tbl.ProcessIdx =:processIdx
				$shiftSerch
				$workerSerch
			");

			$ps->bindValue(":startDate", $startDate, PDO::PARAM_STR);
			$ps->bindValue(":endDate", $endDate, PDO::PARAM_STR);
			$ps->bindValue(":compositionId", $compositionId, PDO::PARAM_STR);
			$ps->bindValue(":productTypeId", $productTypeId, PDO::PARAM_STR);
			$ps->bindValue(":processIdx", $processIdx, PDO::PARAM_INT);
			if($operationShift != "ALL")
			{
				for($i = 0; $i < count($shiftList); $i++ )
				{
					$ps->bindValue(":shift_$i", $shiftList[$i], PDO::PARAM_INT);
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

			$list = array();
			while($row = $ps->fetch(PDO::FETCH_NUM))
			{
				$GLOBALS['logger']->info(AddIPAddress("実績存在あり"));
				array_push($list, array("Exists" => true));
				header("Content-type: application/json; charset=UTF-8");
	
				//JSONデータを出力
				echo json_encode($list);
				return;
			}

			$GLOBALS['logger']->info(AddIPAddress("実績存在なし"));
			array_push($list, array("Exists" => false));
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
			return;
		}
		catch(Exception $ex)
		{
			$GLOBALS['logger']->error(CreateLogExceptionError("サイクル実績存在チェック処理", $ex));
			array_push($list, array("Exists" => false));
			header("Content-type: application/json; charset=UTF-8");

			//JSONデータを出力
			echo json_encode($list);
		}
	}

#endregion サイクル実績存在チェック処理 index=10

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
	 * 小数点第二位で切り下げ
	 * 
	 * @param double $number 数値
	 * @return double 小数点第二位で切り下げした数値
	 */
	function DevaluationSecond($number)
	{
		return (floor($number * 10) / 10);
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