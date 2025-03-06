<html lang="ja">
    <head>
    <meta charset="UTF-8">
        <!-- CSS -->
        <link rel="stylesheet" type="text/css" href="..\..\css\lib\uikit.min.css">

        <link rel="stylesheet" type="text/css" href="..\..\css\common.css">
        <link rel="stylesheet" type="text/css" href="..\..\css\index.css">
        <link rel="stylesheet" type="text/css" href="..\..\css\calendar.css">
		<link rel="stylesheet" type="text/css" href="..\..\css\page-pile-table.css">  
        <link rel="stylesheet" type="text/css" href="..\..\css\page-cycle-plot.css">
        <link rel="stylesheet" type="text/css" href="..\..\css\page-video-play.css">

        <!-- ライブラリ -->
        <script src="..\..\js\lib\jquery-3.6.0.min.js"></script>
        <script src="..\..\js\lib\snap.svg-min.js"></script>
        <script src="..\..\js\lib\uikit.min.js"></script>
        <script src="..\..\js\lib\moment.js"></script>
        <script src="..\..\js\lib\js.cookie.js"></script>

        <!-- イベント処理 -->
        
        <script src="..\..\js\event-common.js" charset="utf-8"></script>
        <script src="..\..\js\event-calendar.js" charset="utf-8"></script>
        <script src="..\..\js\event-pile-table.js" charset="utf-8"></script>
        <script src="..\..\js\event-cycle-plot.js" charset="utf-8"></script>
        <script src="..\..\js\event-video-play.js" charset="utf-8"></script>
        <script src="..\..\js\graph.js" charset="utf-8"></script>

        <title>DKST</title>
    </head>

	<body>
        <table class="index-table" cellspacing="0">
            <tr class="index-tr">
                <td class="index-td" id="index-frame-1" rowspan="2">
                    <!-- 山積み表表示部 -->
                    <?php include('page-pile-table.php'); ?>
                </td>
                <td class="index-td" id="index-frame-2">
                    <!-- 1サイクル毎プロット表示部 -->
                    <?php include('page-cycle-plot.php'); ?>
                </td>
            </tr>
            <tr class="index-tr">
                <td class="index-td" id="index-frame-3">
                    <!-- サイクル動画表示部 -->
                    <?php include('page-video-play.php'); ?>
                </td>
            </tr>
        </table>

        <!-- カレンダーテンプレート -->
        <?php include('calendar.php'); ?>
	</body>
</html>