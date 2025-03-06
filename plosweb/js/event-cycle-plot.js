//#region イベント処理
/**
 * イベント処理
 */
$(function()
{
    /**
    * HTML読み込み時イベント
    */
    $(document).ready(function () 
    {
        try 
        {
            //期間取得
            CycleGetSpan();

            //勤帯取得
            var shiftList = CycleGetShift();
            if(shiftList.length != 0)
            {
                $("#cycle-shift").val(shiftList[0]);
            }

            //作業編成取得
            CycleGetComposition();

            //品番取得
            var productList = CycleGetProductType();
            if(productList.length != 0)
            {
                $("#cycle-product").val(productList[0].ProductTypeName);

                //品番ラベルに値を入れる
                var json = JSON.stringify({ Product: Array(productList[0].ProductTypeId) });
                $("#cycle-product-label").val(json);
            }

            //工程取得
            CycleGetProcess();

            //画面更新
            CyclePageUpdate();

        } 
        catch(e) 
        {
            alert("HTML読み込み時イベントエラー:" + e.message );
        }
    });

//#region カレンダーイベント

    /**
     * カレンダー表示イベント
     */
    UIkit.util.on('#cycle-dropdown', 'show', function () 
    {
        try
        {
            //入力期間取得
            spanArray = $('#cycle-span').val().split(' - ');
            startDate = moment(spanArray[0]);
            endDate = moment(spanArray[1]);
            nowMonth = moment();

            var span = CycleGetOperatingPlan();
            startSpan = span[0];
            endSpan = span[1];
            allSpan = span[2];

            //稼働計画が当日以前しか無い場合、当日のカレンダーを表示するために期間末尾を今日に変える
            if(endSpan < nowMonth)
            {
                endSpan = nowMonth;
            }

            //テンプレート表示部に追加する
            var clone = $($('#calendar-template').html());
            $('#cycle-calendar-div').append(clone);

            //カレンダー初期表示
            CalendarCreate(startDate, endDate, startSpan, endSpan, nowMonth, true, "cycle", allSpan);
        } 
        catch(e) 
        {
            alert("カレンダー表示イベントエラー:" +  e.message );
        }
    });

    /**
     * カレンダー非表示イベント
     */
    UIkit.util.on('#cycle-dropdown', 'hide', function () 
    {
        //カレンダー部分を消す
        $("#cycle-calendar-div").empty();
    });

//#endregion カレンダーイベント

//#region 勤帯ドロップダウンイベント

    /**
     * 勤帯プルダウン表示処理
     */
    $('body').on('show', '#cycle-shift-dropdown' , function() 
    {
        try
        {
            //勤帯リスト
            var shiftList = CycleGetShift();
    
            //divリセット
            var divName = "#cycle-shift-div";
            $(divName).empty();
    
            //選択されていた勤帯を取得する
            var shiftVal = "#cycle-shift";
            var nowShiftList = $(shiftVal).val().split(',');
    
            //テーブル追加
            $(divName).append('<table class="cycle-shift-table" id="cycle-shift-table"></table>');
    
            //ALL行追加
            var select = "";
            if(nowShiftList.includes("ALL"))
            {
                select = "cycle-shift-select-td";
            }
            $("#cycle-shift-table").append('<tr><td class="cycle-shift-table-td ' + select + '" id="cycle-shift-all">ALL</td></tr>');
    
            //勤帯リスト追加
            for(var i = 0; i < shiftList.length; i++)
            {
                select = "";
                if(nowShiftList.includes(shiftList[i]))
                {
                    select = "cycle-shift-select-td";
                }
                $("#cycle-shift-table").append('<tr><td class="cycle-shift-table-td ' + select + '">' + shiftList[i] + '</td></tr>');
            }
        } 
        catch(e) 
        {
            alert("勤帯プルダウン表示イベントエラー:" +  e.message );
        }
    });
 
    /**
     * 勤帯プルダウン閉じる処理
     */
    $('body').on('hide', '#cycle-shift-dropdown' , function() 
    {
        try
        {
            var select = "";
            var buttonText = "";
    
            var count = 1;
            //選択行の名称を全て取得する
            $('.cycle-shift-select-td').each(function() 
            {
                select += ",";
    
                if(count == 1)
                {
                    //初回はカンマをつけない
                    select = "";
                    buttonText = $(this).text();
                }
                else if(count == 2)
                {
                    //二人以上選択されていれば他を付ける
                    buttonText += "  他";
                }
    
                select += $(this).text();
    
                count++;
            });
    
            //テーブル部分を消す
            $("#cycle-shift-div").empty();
    
            //勤帯テキストボックスに値を入れる
            $("#cycle-shift").val(select);

            //作業編成取得
            CycleGetComposition();

            var isALL = false;
            if($("#cycle-composition").text() != "-")
            {
                isALL = true;
            }

            //品番取得
            var productList = CycleGetProductType();
            if(productList.length != 0)
            {
                $("#cycle-product").val(productList[0].ProductTypeName);

                //品番ラベルに値を入れる
                var json = JSON.stringify({ Product: Array(productList[0].ProductTypeId) });
                $("#cycle-product-label").val(json);
            }
            else
            {
                //品番初期化
                CycleChangeProductEnabled(false);
            }
            

            //工程取得
            CycleGetProcess();

            //作業者初期化
            CycleChangeWorkerEnabled(isALL);

            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("勤帯プルダウン閉じる処理エラー:" +   e.message );
        }
    });
 
    /**
     * 勤帯リストクリック処理
     */
    $('body').on('click', '.cycle-shift-table-td' , function() 
    {
        try
        {
            //クリックコントロール
            var clickTd = $(this);
    
            //ALLクリック時はほかの選択を解除する
            if(clickTd.text() == "ALL")
            {
                $(".cycle-shift-select-td").removeClass("cycle-shift-select-td");
            }
            else
            {
                $("#cycle-shift-all").removeClass("cycle-shift-select-td");
            }
    
            //選択行なら選択解除、未選択行なら選択状態にする
            if (clickTd.hasClass("cycle-shift-select-td")) 
            {
                if($(".cycle-shift-select-td").length == 1)
                {
                    //選択行が1行しかない状態での選択解除は無視する
                    return;
                }
    
                clickTd.removeClass("cycle-shift-select-td");
            } 
            else 
            {
                clickTd.addClass("cycle-shift-select-td");
            }
    
            //ALLの他にデータが1つしかないならALL強制切替はしない
            if($(".cycle-shift-table-td").length != 2)
            {
                //選択されている行が全行-1個あればALL以外の全てが選択されているのでALL選択に切り替える
                if($(".cycle-shift-select-td").length == $(".cycle-shift-table-td").length - 1)
                {
                    $(".cycle-shift-select-td").removeClass("cycle-shift-select-td");
                    $("#cycle-shift-all").addClass("cycle-shift-select-td");
                }
            }
        } 
        catch(e) 
        {
            alert("勤帯リストクリックイベントエラー:" +  e.message );
        }
    });
 
 //#endregion 勤帯ドロップダウンイベント

    /**
     * 編成変更イベント
     */
    $("#cycle-composition").change(function () 
    {
        try
        {
            //品番初期化
            CycleChangeProductEnabled(true);

            //工程取得
            CycleGetProcess();

            //作業者初期化
            CycleChangeWorkerEnabled(true);

            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("編成変更イベントエラー:" + e.message );
        }
    });

//#region 品番ドロップダウンイベント

    /**
     * 品番プルダウン表示処理
     */
    $('body').on('show', '#cycle-product-dropdown' , function() 
    {
        try
        {
            //品番リスト ※data[i].ProductTypeName,data[i].ProductTypeId
            var productList = CycleGetProductType();
    
            //divリセット
            var divName = "#cycle-product-div";
            $(divName).empty();
    
            //選択されていた品番を取得する
            var nowProductList = JSON.parse($("#cycle-product-label").val()).Product;
    
            //テーブル追加
            $(divName).append('<table class="cycle-product-table" id="cycle-product-table"></table>');
    
            //ALL行追加
            var select = "";
            if(nowProductList.includes("ALL"))
            {
                select = "cycle-product-select-td";
            }
            $("#cycle-product-table").append('<tr><td class="cycle-product-table-td ' + select + '" id="cycle-product-all" value="ALL">ALL</td></tr>');
    
            //品番リスト追加
            for(var i = 0; i < productList.length; i++)
            {
                select = "";
                if(nowProductList.includes(productList[i].ProductTypeId))
                {
                    select = "cycle-product-select-td";
                }
                $("#cycle-product-table").append('<tr><td class="cycle-product-table-td ' + select + '" id="' + productList[i].ProductTypeId + '" value="' + productList[i].ProductTypeId + '">' + productList[i].ProductTypeName + '</td></tr>');
            }
        } 
        catch(e) 
        {
            alert("品番プルダウン表示イベントエラー:" + e.message );
        }
    });
 
    /**
     * 品番プルダウン閉じる処理
     */
    $('body').on('hide', '#cycle-product-dropdown' , function() 
    {
        try
        {
            var boxText = "";
    
            var count = 1;
            var productList = Array();
            //選択行の名称を全て取得する
            $('.cycle-product-select-td').each(function() 
            {
                if(count == 1)
                {
                    //初回はカンマをつけない
                    boxText = $(this).text();
                }
                else if(count == 2)
                {
                    //二つ以上選択されていれば他を付ける
                    boxText += "  他";
                }
                var id = $(this).attr('id');
                if(id == "cycle-product-all")
                {
                    productList.push("ALL");
                }
                else
                {
                    productList.push(id);
                }
    
                count++;
            });
    
            //テーブル部分を消す
            $("#cycle-product-div").empty();
    
            //品番ラベルに値を入れる
            var json = JSON.stringify({ Product: productList });
            $("#cycle-product-label").val(json);
            //品番テキストボックスに値を入れる
            $("#cycle-product").val(boxText);

            //工程取得
            CycleGetProcess();

            //作業者初期化
            CycleChangeWorkerEnabled(true);

            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("品番プルダウン閉じる処理エラー:" + e.message );
        }
    });
 
    /**
     * 品番リストクリック処理
     */
    $('body').on('click', '.cycle-product-table-td' , function() 
    {
        try
        {
            //クリックコントロール
            var clickTd = $(this);
    
            //ALLクリック時はほかの選択を解除する
            if(clickTd.text() == "ALL")
            {
                $(".cycle-product-select-td").removeClass("cycle-product-select-td");
            }
            else
            {
                $("#cycle-product-all").removeClass("cycle-product-select-td");
            }
    
            //選択行なら選択解除、未選択行なら選択状態にする
            if (clickTd.hasClass("cycle-product-select-td")) 
            {
                if($(".cycle-product-select-td").length == 1)
                {
                    //選択行が1行しかない状態での選択解除は無視する
                    return;
                }
    
                clickTd.removeClass("cycle-product-select-td");
            } 
            else 
            {
                clickTd.addClass("cycle-product-select-td");
            }
    
            //ALLの他にデータが1つしかないならALL強制切替はしない
            if($(".cycle-product-table-td").length != 2)
            {
                //選択されている行が全行-1個あればALL以外の全てが選択されているのでALL選択に切り替える
                if($(".cycle-product-select-td").length == $(".cycle-product-table-td").length - 1)
                {
                    $(".cycle-product-select-td").removeClass("cycle-product-select-td");
                    $("#cycle-product-all").addClass("cycle-product-select-td");
                }
            }
        } 
        catch(e) 
        {
            alert("品番リストクリックイベントエラー:" + e.message );
        }
    });
 
//#endregion 品番ドロップダウンイベント

    /**
     * 工程変更イベント
     */
    $("#cycle-process").change(function () 
    { 
        try
        {
            //作業者初期化
            CycleChangeWorkerEnabled(true);
    
            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("工程変更イベントエラー:" + e.message );
        }
    });

    /**
      * サイズ変更アイコンクリックイベント
      */
    $("#cycle-size-change-icon").click(function() 
    { 
        try
        {
            $('#index-frame-1', parent.document).toggle();
            $('#index-frame-3', parent.document).toggle();

            //プロット再描画
            CycleDummySend();

            //山積み表再描画
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("サイズ変更アイコンクリックイベントエラー:" + e.message );
        }
    });

//#region 作業者ドロップダウンイベント

    /**
     * 作業者プルダウン表示処理
     */
    $('body').on('show', '#cycle-worker-dropdown' , function(e) 
    {
        try
        {
            //作業者リスト
            var workerList = CycleGetWorker();
    
            //divリセット
            var divName = "#cycle-worker-div";
            $(divName).empty();
    
            //選択されていた作業者を取得する
            var nowWorkerList = JSON.parse($("#cycle-worker-label").val()).Worker;
    
            //テーブル追加
            $(divName).append('<table class="cycle-worker-table" id="cycle-worker-table"></table>');
    
            //ALL行追加
            var select = "";
            if(nowWorkerList.includes("ALL"))
            {
                select = "cycle-worker-select-td";
            }
            $("#cycle-worker-table").append('<tr><td class="cycle-worker-table-td ' + select + '" id="cycle-worker-all" value="ALL">ALL</td></tr>');
    
            //作業者リスト追加
            for(var i = 0; i < workerList.length; i++)
            {
                select = "";
                if(nowWorkerList.includes(workerList[i]))
                {
                    select = "cycle-worker-select-td";
                }
                $("#cycle-worker-table").append('<tr><td class="cycle-worker-table-td ' + select + '">' + workerList[i] + '</td></tr>');
            }
        } 
        catch(e) 
        {
            alert("作業者プルダウン表示イベントエラー:" + e.message );
        }
    });
 
    /**
     * 作業者プルダウン閉じる処理
     */
    $('body').on('hide', '#cycle-worker-dropdown' , function() 
    {
        try
        {
            var boxText = "";
    
            var count = 1;
            var workerList = Array();
            //選択行の名称を全て取得する
            $('.cycle-worker-select-td').each(function() 
            {
                if(count == 1)
                {
                    boxText = $(this).text();
                }
                else if(count == 2)
                {
                    //二つ以上選択されていれば他を付ける
                    boxText += "  他";
                }

                workerList.push($(this).text());

                count++;
            });
    
            //テーブル部分を消す
            $("#cycle-worker-div").empty();
    
            //作業者ラベルに値を入れる
            var json = JSON.stringify({ Worker: workerList });
            $("#cycle-worker-label").val(json);
            //作業者テキストボックスに値を入れる
            $("#cycle-worker").val(boxText);

            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("作業者プルダウン閉じる処理エラー:" + e.message );
        }
    });
 
    /**
     * 作業者リストクリック処理
     */
    $('body').on('click', '.cycle-worker-table-td' , function() 
    {
        try
        {
            //クリックコントロール
            var clickTd = $(this);
    
            //ALLクリック時はほかの選択を解除する
            if(clickTd.text() == "ALL")
            {
                $(".cycle-worker-select-td").removeClass("cycle-worker-select-td");
            }
            else
            {
                $("#cycle-worker-all").removeClass("cycle-worker-select-td");
            }
    
            //選択行なら選択解除、未選択行なら選択状態にする
            if (clickTd.hasClass("cycle-worker-select-td")) 
            {
                if($(".cycle-worker-select-td").length == 1)
                {
                    //選択行が1行しかない状態での選択解除は無視する
                    return;
                }
    
                clickTd.removeClass("cycle-worker-select-td");
            } 
            else 
            {
                clickTd.addClass("cycle-worker-select-td");
            }
    
            //ALLの他にデータが1つしかないならALL強制切替はしない
            if($(".cycle-worker-table-td").length != 2)
            {
                //選択されている行が全行-1個あればALL以外の全てが選択されているのでALL選択に切り替える
                if($(".cycle-worker-select-td").length == $(".cycle-worker-table-td").length - 1)
                {
                    $(".cycle-worker-select-td").removeClass("cycle-worker-select-td");
                    $("#cycle-worker-all").addClass("cycle-worker-select-td");
                }
            }
        } 
        catch(e) 
        {
            alert("作業者リストクリックイベントエラー:" + e.message );
        }
    });

//#endregion 作業者ドロップダウンイベント

    /**
     * 異常のみONOFFボタンクリックイベント
     */
    $("#cycle-on-off-button").click(function()
    {
        try
        {
            if($("#cycle-on-off-button").text() == "OFF")
            {
                $("#cycle-on-off-button").text("ON");
            }
            else
            {
                $("#cycle-on-off-button").text("OFF");
            }

            //画面更新
            CyclePageUpdate();
        } 
        catch(e) 
        {
            alert("異常のみONOFFボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * CT CSV出力ダウンロードクリックイベント
     */
    $("#cycle-csv-download").click(function(e)
    {
        try
        {
            var csvData = "開始時間,終了時間,勤帯,編成,品番,工程,作業者,CT実績,上限値,下限値\r\n";

            //描画されている折れ線グラフから描画データを取得する
            $('.cycle-plot-point').each(function() 
            {
                var plotVal = $(this).attr("value");
                var plotData = JSON.parse(plotVal);
                var textLine = CycleCreateCSVFormat(plotData);
                csvData += textLine;
            });

            //取得できなければグラフが表示されていないので処理しない
            if(csvData == "開始時間,終了時間,勤帯,編成,品番,工程,作業者,CT実績,上限値,下限値\r\n")
            {
                alert("CSV出力するデータがありません。");
                return;
            }

            var fileName = "CTData_" + moment().format("YYYYMMDDHHmmss") + ".csv";

            //UTF8指定
            let bom  = new Uint8Array([0xEF, 0xBB, 0xBF]);
            const blob = new Blob([bom,csvData], {type: 'text/csv'});
            const url = URL.createObjectURL(blob);
            const aElement = document.createElement("a");
            document.body.appendChild(aElement);
            aElement.download = fileName;
            aElement.href = url;
            aElement.click();
            aElement.remove();
            URL.revokeObjectURL(url);
        } 
        catch(e) 
        {
            alert("CT CSV出力クリックイベントエラー:" + e.message );
        }
    });

//#region 標準値マスタ更新ボタンイベント
    /**
     * 登録ボタンテキストボックスキーダウンイベント
     */
    $(".cycle-input").keydown(function(event)
    {
        try
        {
            var keyCode = event.keyCode;
            //eが押下されていたら
            if(keyCode == 69)
            {
                //入力をキャンセルする
                return false;
            }
        } 
        catch(e) 
        {
            alert("登録ボタンテキストボックスキーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * 最小値登録ボタンクリックイベント
     */
    $("#cycle-min-button").click(function()
    {
        try
        {
            //数値チェック
            var val = $("#cycle-min-val").val();
            if(isNaN(val) || Number(val) < 0 || Number(val) > 9999)
            {
                alert("0～9999の値を入力してください。");
                return;
            }

            //標準値マスタ更新
            var ret = CycleUpdateStandardVal("CycleTimeMin", val);

            //画面更新
            CyclePageUpdate();

            //山積み表更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("最小値登録ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 平均値登録ボタンクリックイベント
     */
    $("#cycle-average-button").click(function()
    {
        try
        {
            //数値チェック
            var val = $("#cycle-average-val").val();
            if(isNaN(val) || Number(val) < 0 || Number(val) > 9999)
            {
                alert("0～9999の値を入力してください。");
                return;
            }

            //標準値マスタ更新
            var ret = CycleUpdateStandardVal("CycleTimeAverage", val);

            //画面更新
            CyclePageUpdate();

            //山積み表更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("平均値登録ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 最大値登録ボタンクリックイベント
     */
    $("#cycle-max-button").click(function()
    {
        try
        {
            //数値チェック
            var val = $("#cycle-max-val").val();
            if(isNaN(val) || Number(val) < 0 || Number(val) > 9999)
            {
                alert("0～9999の値を入力してください。");
                return;
            }

            //標準値マスタ更新
            var ret = CycleUpdateStandardVal("CycleTimeMax", val);

            //画面更新
            CyclePageUpdate();

            //山積み表更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("最大値登録ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 上限値登録ボタンクリックイベント
     */
    $("#cycle-upper-button").click(function()
    {
        try
        {
            //数値チェック
            var val = $("#cycle-upper-val").val();
            if(isNaN(val) || Number(val) < 0 || Number(val) > 9999)
            {
                alert("0～9999の値を入力してください。");
                return;
            }
            
            //標準値マスタ更新
            var ret = CycleUpdateStandardVal("CycleTimeUpper", val);

            //画面更新
            CyclePageUpdate();

            //山積み表更新
            PilePageUpdate();

            //動画再生部の異常のみがONになっている場合は動画再生部も更新する
            if($("#video-on-off-button").text() == "ON")
            {
                //動画再生部更新
                VideoPageUpdate();
            }
        } 
        catch(e) 
        {
            alert("上限値登録ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 下限値登録ボタンクリックイベント
     */
    $("#cycle-lower-button").click(function()
    {
        try
        {
            //数値チェック
            var val = $("#cycle-lower-val").val();
            if(isNaN(val) || Number(val) < 0 || Number(val) > 9999)
            {
                alert("0～9999の値を入力してください。");
                return;
            }

            //標準値マスタ更新
            var ret = CycleUpdateStandardVal("CycleTimeLower", val);

            //画面更新
            CyclePageUpdate();

            //山積み表更新
            PilePageUpdate();

            //動画再生部の異常のみがONになっている場合は動画再生部も更新する
            if($("#video-on-off-button").text() == "ON")
            {
                //動画再生部更新
                VideoPageUpdate();
            }
        } 
        catch(e) 
        {
            alert("下限値登録ボタンクリックイベントエラー:" + e.message );
        }
    });

//#endregion 標準値マスタ更新ボタンイベント

//#region 最大最小テキストボックスイベント

    /**
     * 最大最小テキストボックスキーダウンイベント
     */
    $(".cycle-plot-input").keydown(function(event)
    {
        try
        {
            var keyCode = event.keyCode;
            //eが押下されていたら
            if(keyCode == 69)
            {
                //入力をキャンセルする
                return false;
            }
        } 
        catch(e) 
        {
            alert("最大最小テキストボックスキーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * 最大最小テキストボックスインプットイベント
     */
    $('.cycle-plot-input').on('input', function()
    {
        try
        {
            var val = parseInt($(this).val());
            var valMax = parseInt($(this).attr("max"));
            var valMin = parseInt($(this).attr("min"));
            if(val >= valMax)
            { 
                $(this).val(valMax); 
                return;
            }
            if(val <= valMin)
            { 
                $(this).val(valMin); 
                return;
            }
            if(isNaN(val))
            { 
                $(this).val(0); 
                return;
            }

            $(this).val(val);
        } 
        catch(e) 
        {
            alert("最大最小テキストボックスキーマウスアップイベントエラー:" + e.message );
        }
    });

    /**
     * 最大最小テキストボックス値変更イベント
     */
    $('.cycle-plot-input').change(function()
    {
        try
        {
            //最大最小がかぶっていたら最小を0にしてグラフが消えるのを防ぐ
            if(Number($("#cycle-graph-max-val").val()) <= Number($("#cycle-graph-min-val").val()))
            {
                $("#cycle-graph-min-val").val(0);
            }

            //グラフ更新
            CycleDummySend();
        } 
        catch(e) 
        {
            alert("最大最小テキストボックス値変更イベントエラー:" + e.message );
        }
    });

//#endregion 最大最小テキストボックスイベント

//#region サイクルプロットクリックイベント
    /**
     * プロット左クリックイベント
     */
    $('body').on('click', '.ct-pos' , function()
    {
        try
        {
            //除外Fromがあれば解除する
            $(".right-click-plot").removeClass("right-click-plot");

            if($(".click-plot").length > 0)
            {
                //Fromのデータ取得
                var fromVal = $(".click-plot").attr("value");
                var fromJson = JSON.parse(fromVal);

                //Toのデータ取得
                var toVal = $(this).attr("value");
                var toJson = JSON.parse(toVal);

                var fromStartTime = moment(fromJson.StartTime);
                var toStartTime = moment(toJson.StartTime);
                var toEndTime = moment(toJson.EndTime);
                if(fromStartTime > toStartTime)
                {
                    alert("From指定はTo指定の前のサイクルプロットを選択してください。");
                    return;
                }

                var videoSpan = fromStartTime.format("YYYY/MM/DD HH:mm:ss") + " - " + toEndTime.format("YYYY/MM/DD HH:mm:ss");

                //動画部分期間設定
                $("#video-span").val(videoSpan);

                //動画変更
                VideoAllUpdate() 

                //動画Fromを解除する
                $(".click-plot").removeClass("click-plot");
                return;
            }

            $(this).addClass("click-plot");
        } 
        catch(e) 
        {
            alert("プロット左クリックイベントエラー:" + e.message );
        }
    });

    /**
     * プロット右クリックイベント
     */
    $(document).on('contextmenu', '.ct-pos', function() 
    {
        try
        {
            //動画Fromがあれば解除する
            $(".click-plot").removeClass("click-plot");

            //Shiftキー押下中か
            if($("#cycle-plot-area").val() == true)
            {
                if($(".right-click-plot").length > 0)
                {
                    //Fromのデータ取得
                    var fromVal = $(".right-click-plot").attr("value");
                    var fromJson = JSON.parse(fromVal);

                    //Toのデータ取得
                    var toVal = $(this).attr("value");
                    var toJson = JSON.parse(toVal);

                    var fromStartTime = moment(fromJson.StartTime);
                    var toStartTime = moment(toJson.StartTime);
                    var toEndTime = moment(toJson.EndTime);
                    if(fromStartTime > toStartTime)
                    {
                        alert("From指定はTo指定の前のサイクルプロットを選択してください。");
                        //Shift離したことにする
                        $("#cycle-plot-area").val(false);

                        $(".right-click-plot").removeClass("right-click-plot");
                        return false;
                    }

                    //除外変更From指定
                    $(this).addClass("right-click-plot");

                    //ポップアップメッセージ設定
                    var popupMsg = fromStartTime.format("YYYY/MM/DD HH:mm:ss") + "から" + toEndTime.format("YYYY/MM/DD HH:mm:ss") + "までのサイクル除外時間を変更しますか？";
                    $("#cycle-popup-msg").text(popupMsg);

                    //ポップアップの初期位置を設定
                    var topVal = ($(document).height() / 2) - 75; //ポップアップの縦幅の半分を引く
                    var leftVal = ($(document).width() / 2) - 170; //ポップアップの横幅の半分を引く
                    $('.cycle-popup').offset({
                        top: topVal,
                        left: leftVal
                    });

                    //ポップアップの表示
                    $(".cycle-popup").addClass("active");
                }
                else
                {
                    //除外変更From指定
                    $(this).addClass("right-click-plot");
                }
            }
            else
            {
                //除外手動切替
                var val = $(this).attr("value");
                var json = JSON.parse(val);

                //除外切替
                var errorFlag = 1;
                if(json.IsException == "1")
                {
                    json.IsException = "0";
                    errorFlag = 2; //手動除外解除
                }
                else
                {
                    json.IsException = "1";
                    errorFlag = 1; //手動除外
                }

                //DB更新
                var ret = CycleUpdateResult(json, errorFlag);
                if(ret)
                {
                    $(this).attr("value", JSON.stringify(json));
                
                    //折れ線グラフ描画
                    CycleDummySend();

                    //山積み表更新
                    PilePageUpdate();
                }
            }
        } 
        catch(e) 
        {
            alert("プロット右クリックイベントエラー:" + e.message );
        }

        //右クリックメニューを出さない
        return false; 
    });

    //************************* */ 
    // event-commonに関連処理あり
    //************************* */

//#endregion サイクルプロットクリックイベント

//#region ポップアップボタンクリックイベント
    /**
     * 全除外クリックイベント
     */
    $("#cycle-all-error-button").click(function()
    {
        try
        {
            //一括除外
            CyclePopupButtonUpdate(1);
        } 
        catch(e) 
        {
            alert("全除外クリックイベントエラー:" + e.message );
        }
    });

    /**
     * 全除外解除クリックイベント
     */
    $("#cycle-all-release-button").click(function()
    {
        try
        {
            //一括除外解除
            CyclePopupButtonUpdate(2);
        } 
        catch(e) 
        {
            alert("全除外解除クリックイベントエラー:" + e.message );
        }
    });

    /**
     * キャンセルボタンクリックイベント
     */
    $("#cycle-cancel-button").click(function()
    {
        try
        {
            //From入力を解除する
            $(".click-plot").removeClass("click-plot");
            $(".right-click-plot").removeClass("right-click-plot");

            //ダイアログを閉じる
            $(".cycle-popup").removeClass("active");
        } 
        catch(e) 
        {
            alert("キャンセルボタンイベントエラー:" + e.message );
        }
    });
//#endregion ポップアップボタンクリックイベント

    /**
     * スライドバー変更イベント
     */
    $("#cycle-plot-disp-bar").on('input', function() 
    {
        try
        {
            //スライドバー値表示更新
            $("#cycle-plot-disp-num").val($(this).val());

            //プロット更新
            CycleDummySend();
        } 
        catch(e) 
        {
            alert("スライドバー変更イベントエラー:" + e.message );
        }
    });

//#region サイクル表示数テキストボックスイベント

    /**
     * サイクル表示数テキストボックスキーダウンイベント
     */
    $("#cycle-plot-disp-num").keydown(function(event)
    {
        try
        {
            var keyCode = event.keyCode;
            //eが押下されていたら
            if(keyCode == 69)
            {
                //入力をキャンセルする
                return false;
            }
        } 
        catch(e) 
        {
            alert("サイクル表示数テキストボックスキーダウンイベントエラー:" + e.message );
        }
    });
 
    /**
     * サイクル表示数テキストボックスインプットイベント
     */
    $('#cycle-plot-disp-num').on('input', function()
    {
        try
        {
            var val = parseInt($(this).val());
            var valMax = parseInt($(this).attr("max"));
            var valMin = parseInt($(this).attr("min"));
            if(val >= valMax)
            { 
                $(this).val(valMax); 
                return;
            }
            if(val <= valMin)
            { 
                $(this).val(valMin); 
                return;
            }
            if(isNaN(val))
            { 
                $(this).val(1); 
                return;
            }
 
            $(this).val(val);
            //スライドバー値表示更新
            $("#cycle-plot-disp-num").val($(this).val());
        } 
        catch(e) 
        {
            alert("サイクル表示数テキストボックスキーマウスアップイベントエラー:" + e.message );
        }
    });
 
    /**
     * サイクル表示数テキストボックス値変更イベント
     */
    $('#cycle-plot-disp-num').change(function()
    {
        try
        {
            //スライドバー値更新
            $("#cycle-plot-disp-bar").val($(this).val());
            //グラフ更新
            CycleDummySend();
        } 
        catch(e) 
        {
            alert("サイクル表示数テキストボックス値変更イベントエラー:" + e.message );
        }
    });
 
//#endregion サイクル表示数テキストボックスイベント
});
 
//#endregion イベント処理

//#region ajax処理
 
/**
 * 期間取得
 */
function CycleGetSpan()
{            
    //関数インデックス
    funIndex = 0;

    //期間を当日に設定
    var nowDate = (new moment()).format("YYYY/MM/DD");

    $.ajax(
        {
            type: "POST",
            url: "../method/method-cycle-plot.php",
            data: { "funIndex": funIndex, "nowDate": nowDate },
            dataType: "json",
            async: false //同期通信
        })
        //正常時
        .done(function (data) 
        {
            if (data.length == 1) 
            {
                var span = moment(data[0].StartTime).format("YYYY/MM/DD HH:mm:ss") + " - " + moment(data[0].EndTime).format("YYYY/MM/DD HH:mm:ss");

                $('#cycle-span').val(span);
            }
        })
        //失敗時
        .fail(function (XMLHttpRequest, textStatus, error) 
        {
            alert("勤帯取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
        })
        //必ず実行
        .always(function () 
        {
            $('#cycle-shift').append($('<option>').html("ALL").val("ALL"));
        });
}

/**
 * 勤帯取得
 */
function CycleGetShift() 
{
    //関数インデックス
    funIndex = 1;
 
    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    
    var shiftList = new Array();
 
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        for (let i = 0; i < data.length; i++) 
        {
            shift = "";
            switch (data[i].OperationShift) 
            {
                case "1":
                    shift = "一直";
                    break;
                case "2":
                    shift = "二直";
                    break;
                case "3":
                    shift = "三直";
                    break;
            }
            shiftList.push(shift);
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("勤帯取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {
        
    });

    return shiftList;
}
 
/**
 * 編成取得
 */
function CycleGetComposition() 
{
    //関数インデックス
    funIndex = 2;
 
    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = CycleGetShiftValue();
     
    //クリア
    $("#cycle-composition").children().remove();
 
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data != null) 
        {
            for (let i = 0; i < data.length; i++) 
            {
                $('#cycle-composition').append($('<option>').html(data[i].UniqueName).val(data[i].CompositionId));
            }
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("編成取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always
    (function () 
    {
        if ($('#cycle-composition').children("option").length == 0) 
        {
            //編成
            $('#cycle-composition').append($('<option>').html("-").val("-"));

            //編成がないなら品番もない
            CycleChangeProductEnabled(false);

            //編成がないなら作業者もない
            CycleChangeWorkerEnabled(false);
        }
        else
        {
            //編成があるなら品番もある
            CycleChangeProductEnabled(true);

            //編成があるなら作業者もある
            CycleChangeWorkerEnabled(true);
        }
    });
}
 
/**
 * 品番取得
 */
function CycleGetProductType() 
{
    //関数インデックス
    funIndex = 3;
  
    //作業編成が無いなら品番も無い
    if ($('#cycle-composition').val() == "-") 
    {
        $('#cycle-product').append($('<option>').html("-").val("-"));
        return Array();
    }
 
    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = CycleGetShiftValue();
    compositionId = $("#cycle-composition").val();

    let productList;
 
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift, "compositionId": compositionId },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        productList = data;
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("品番取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {
    });

    return productList;
}
 
/**
 * 稼働予定日取得
 */
function CycleGetOperatingPlan() 
{
    //関数インデックス
    funIndex = 4;
 
    var startSpan = moment();
    var endSpan = moment();
    var allData = null;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data != null) 
        {
            startSpan = moment(data[0].OperationDate);
            endSpan = moment(data[data.length - 1].OperationDate);
            allData = data;
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("稼働予定日取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function (data) 
    {
 
    });
    return [startSpan, endSpan, allData];
}

/**
 * 工程取得
 */
function CycleGetProcess()
{
    //関数インデックス
    funIndex = 5;
    
    //クリア
    $("#cycle-process").children().remove();

    //作業品番が無いなら工程も無い
    if ($('#cycle-product').val() == "-") 
    {
        $('#cycle-process').append($('<option>').html("-").val("-"));
        return;
    }

    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = CycleGetShiftValue();
    compositionId = $("#cycle-composition").val();
    productTypeId = JSON.parse($("#cycle-product-label").val()).Product;

    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift, "compositionId": compositionId, "productTypeId": productTypeId },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data != null) 
        {
            for (let i = 0; i < data.length; i++) 
            {
                $('#cycle-process').append($('<option>').html(data[i].ProcessName).val(data[i].ProcessIdx));
            }
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("工程取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {
        if ($('#cycle-process').children("option").length == 0) 
        {
            $('#cycle-process').append($('<option>').html("-").val("-"));
        }
    });
}

/**
 * 作業者取得
 */
function CycleGetWorker()
{
    //関数インデックス
    funIndex = 6;
    
    //作業工程が無いなら作業者も無い
    if ($('#cycle-process').val() == "-") 
    {
        $('#cycle-worker').append($('<option>').html("-").val("-"));
        return;
    }

    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = CycleGetShiftValue();
    compositionId = $("#cycle-composition").val();
    productTypeId = JSON.parse($("#cycle-product-label").val()).Product;
    processIdx = $("#cycle-process").val();

    var workerList = Array();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data != null) 
        {
            for (let i = 0; i < data.length; i++) 
            {
                workerList.push(data[i].WorkerName);
            }
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("作業者取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    return workerList;
}

/**
 * 標準値取得
 */
function CycleGetStandardVal()
{
    //関数インデックス
    funIndex = 7;
    
    compositionId = $("#cycle-composition").val();
    productTypeId = JSON.parse($("#cycle-product-label").val()).Product;
    processIdx = $("#cycle-process").val();

    //品番が複数選択なら取得しない
    if(productTypeId[0] == "ALL" || productTypeId.length > 1)
    {
        //最小値
        $("#cycle-min-val").val(0.0);
        
        //平均値
        $("#cycle-average-val").val(0.0);

        //最大値
        $("#cycle-max-val").val(0.0);

        //上限値
        $("#cycle-upper-val").val(0.0);

        //下限値
        $("#cycle-lower-val").val(0.0);
        return;
    }

    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        //最小値
        $("#cycle-min-val").val(data[0].CycleTimeMin);
        //平均値
        $("#cycle-average-val").val(data[0].CycleTimeAverage);
        //最大値
        $("#cycle-max-val").val(data[0].CycleTimeMax);
        //上限値
        $("#cycle-upper-val").val(data[0].CycleTimeUpper);
        //下限値
        $("#cycle-lower-val").val(data[0].CycleTimeLower);
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("標準値取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

/**
 * サイクル実績取得
 */
function CycleGetResultPlot()
{
    //関数インデックス
    funIndex = 8;

    //期間を開始日と終了日に分ける
    spanArray = $('#cycle-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = CycleGetShiftValue();
    compositionId = $("#cycle-composition").val();
    productTypeId = JSON.parse($("#cycle-product-label").val()).Product;
    processIdx = $("#cycle-process").val();
    workerName = JSON.parse($("#cycle-worker-label").val()).Worker;

    //除外のみボタン
    if($("#cycle-on-off-button").text() == "ON")
    {
        isErrorOnly = true;
    }
    else
    {
        isErrorOnly = false;
    }

    if($("#cycle-on-off-button").text() == "ON" || (productTypeId != null && productTypeId[0] == "ALL") || productTypeId.length > 1)
    {
        //登録ボタン部を無効にする
        CycleChangeRegistEnabled(false);
    }
    else
    {
        //登録ボタン部を有効にする
        CycleChangeRegistEnabled(true);
    }

    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx, "workerName": workerName, "isErrorOnly": isErrorOnly },
        dataType: "json",
        async: true //非同期
    })
    //正常時
    .done(function (data) 
    {
        if(data.length != 0)
        {
            //CSV保存ボタン有効化
            CycleChangeCSVButtonEnabled(true);
        }
        else
        {
            //CSV保存ボタン無効化
            CycleChangeCSVButtonEnabled(false);
        }

        let ctdata = [];
        var ctMax = 0;
        var count = 0;
        var manualErrorCount = 0;
        var lowerErrorCount = 0;
        var upperErrorCount = 0;
        for (let i = 0; i < data.length; i++) 
        {
            //異常数取得　手動除外のものでも上下限範囲外のものは上下限除外でカウントする
            if(Number(data[i].CycleTimeLower) > Number(data[i].CycleTime) && Number(data[i].ErrorFlag) != 2)
            {
                //下限カット数
                data[i].ErrorFlag = "1";
                lowerErrorCount++;
            }
            else if(Number(data[i].CycleTime) > Number(data[i].CycleTimeUpper) && Number(data[i].ErrorFlag) != 2)
            {
                //上限カット数
                data[i].ErrorFlag = "1";
                upperErrorCount++;
            }
            else if(Number(data[i].ErrorFlag) == 1)
            {
                //手動除外数
                manualErrorCount++;
            }

            //除外のみONの場合でCTが小数点第二位切り上げされた結果異常ではなくなってしまった場合は取り除く
            if(isErrorOnly && data[i].ErrorFlag == "0")
            {
                continue;
            }

            if(Number(data[i].CycleTime) > ctMax)
            {
                ctMax = Number(data[i].CycleTime);
            }
            var plotData = CycleCreateCTData(data[i],i + 1);
            ctdata.push(plotData);
            count++;
        }

        //CT最大
        $("#cycle-graph-max-val").val(Math.ceil(ctMax * 1.25));
        //CT最小
        $("#cycle-graph-min-val").val(0);

        //プロット描画数バーの最大値設定
        $("#cycle-plot-disp-bar").attr("max", count);
        $("#cycle-plot-disp-bar").val(count);
        $("#cycle-plot-disp-num").attr("max", count);
        $("#cycle-plot-disp-num").val(count);

        //合計サイクル数
        $("#cycle-sum-cycle-num").val(count);
        //異常発生件数
        $("#cycle-error-num").val(manualErrorCount + lowerErrorCount + upperErrorCount);
        //上限カット数
        $("#cycle-upper-num").val(upperErrorCount);
        //下限カット数
        $("#cycle-lower-num").val(lowerErrorCount);
        //手動除外数
        $("#cycle-manual-error-num").val(manualErrorCount);

        //サイクルプロットの描画
        CycleCreatePlot(ctdata);
        $("#cycle-progress").hide();
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("サイクル実績取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    //プログレスダイアログを表示する
    $("#cycle-progress").show();
}

/**
 * サイクル実績手動除外更新
 */
function CycleUpdateResult(jsonData, errorFlag)
{
    //関数インデックス
    funIndex = 9;
    
    compositionId = jsonData.CompositionId;
    productTypeId = jsonData.ProductTypeId;
    processIdx = jsonData.ProcessIdx;
    startTime = jsonData.StartTime;
    isException = errorFlag;

    var ret = false;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx, "startTime": startTime, "isException": isException },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if(data.UpdateCount > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("サイクル実績手動除外更新失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    return ret;
}

/**
 * サイクル実績一括除外更新
 */
function CycleMultiUpdateResult(isError, fromStartTime, toStartTime, processIdx)
{
    //関数インデックス
    funIndex = 10;
    
    var ret = false;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "isError": isError, "fromStartTime": fromStartTime, "toStartTime": toStartTime, "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if(data.UpdateCount > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("サイクル実績一括除外更新失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    return ret;
}

/**
 * 標準値マスタ更新
 */
function CycleUpdateStandardVal(updateName, updateVal)
{
    //関数インデックス
    funIndex = 11;
    
    compositionId = $("#cycle-composition").val();
    productTypeId = JSON.parse($("#cycle-product-label").val()).Product;
    processIdx = $("#cycle-process").val();

    var ret = false;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        data: { "funIndex": funIndex, "updateName": updateName, "updateVal": updateVal,"compositionId": compositionId, "productTypeId": productTypeId[0], "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if(data.UpdateCount > 0)
        {
            ret = true;
        }
        else
        {
            if(data.ErrorMsg != "")
            {
                alert(data.ErrorMsg);
            }
            else
            {
                alert("データの更新に失敗しました。");
            }
            ret = false;
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("標準値マスタ更新失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    return ret;
}

/**
 * グラフ再描画用ダミー処理
 */
function CycleDummySend()
{
    //関数インデックス
    funIndex = 12;

    $.ajax(
    {
        type: "POST",
        url: "../method/method-cycle-plot.php",
        dataType: "json",
        async: true //非同期通信
    })
    //正常時
    .done(function (data) 
    {
        CycleDispPlot();
        $("#cycle-progress").hide();
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("グラフ再描画用ダミー処理失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    //プログレスダイアログを表示する
    $("#cycle-progress").show();
}

//#endregion ajax処理

//#region "関数処理"

//#region "ドロップダウン表示有効無効切替"
/**
 * 品番ドロップダウン表示有効無効切替
 * @param {bool} isEnabled true:表示、false:非表示
 */
function CycleChangeProductEnabled(isEnabled)
{
    //クリック時に表示するドロップダウンが追加されていれば削除する
    $("#cycle-product-dropdown").remove();

    if(isEnabled)
    {
        //テキストボックスに初期値ALLセット
        $('#cycle-product').val("ALL");
        
        //ラベル部分に選択値を保持する
        var json = JSON.stringify({ Product: Array("ALL") });
        $("#cycle-product-label").val(json);

        //クリック時に表示するドロップダウンを追加する
        $('<div uk-dropdown="mode: click" id="cycle-product-dropdown"><div id="cycle-product-div"></div></div>').insertAfter("#cycle-product-label");
    }
    else
    {
        //テキストボックスにハイフンをセット
        $('#cycle-product').val("-");

        //ラベル部分に選択値を保持する
        var json = JSON.stringify({ Product: Array("-") });
        $("#cycle-product-label").val(json);
    }
}

/**
 * 作業者ドロップダウン表示有効無効切替
 * @param {bool} isEnabled true:表示、false:非表示
 */
function CycleChangeWorkerEnabled(isEnabled)
{
    //クリック時に表示するドロップダウンが追加されていれば削除する
    $("#cycle-worker-dropdown").remove();

    if(isEnabled)
    {
        //テキストボックスに初期値ALLセット
        $('#cycle-worker').val("ALL");
        
        //ラベル部分に選択値を保持する
        var json = JSON.stringify({ Worker: Array("ALL") });
        $("#cycle-worker-label").val(json);

        //クリック時に表示するドロップダウンを追加する
        $('<div uk-dropdown="mode: click" id="cycle-worker-dropdown"><div id="cycle-worker-div"></div></div>').insertAfter("#cycle-worker-label");
    }
    else
    {
        //テキストボックスにハイフンをセット
        $('#cycle-worker').val("-");

        //ラベル部分に選択値を保持する
        var json = JSON.stringify({ Worker: Array("-") });
        $("#cycle-worker-label").val(json);
    }
}
//#endregion "ドロップダウン表示有効無効切替"

/**
 *　全更新処理 ※主にカレンダー入力時
 */
function CycleAllUpdate() 
{
    //勤帯取得
    var shiftList = CycleGetShift();
    if(shiftList.length != 0)
    {
        $("#cycle-shift").val(shiftList[0]);
    }
    else
    {
        $("#cycle-shift").val("ALL");
    }
 
    //作業編成取得
    CycleGetComposition();
 
    //品番取得
    var productList = CycleGetProductType();
    if(productList.length != 0)
    {
        $("#cycle-product").val(productList[0].ProductTypeName);

        //品番ラベルに値を入れる
        var json = JSON.stringify({ Product: Array(productList[0].ProductTypeId) });
        $("#cycle-product-label").val(json);
    }

    //工程取得
    CycleGetProcess();
 
    //画面更新
    CyclePageUpdate();
}

/**
 *　画面更新処理
 */
function CyclePageUpdate() 
{
    if(CycleCheckPageUpdate())
    {
        //標準値取得
        CycleGetStandardVal();
    }
    else
    {
        //登録ボタン部を無効にする
        //最小値
        $("#cycle-min-val").val(0.0);
        
        //平均値
        $("#cycle-average-val").val(0.0);

        //最大値
        $("#cycle-max-val").val(0.0);

        //上限値
        $("#cycle-upper-val").val(0.0);

        //下限値
        $("#cycle-lower-val").val(0.0);
        CycleChangeRegistEnabled(false);
    }
    
    if(CycleCheckPageUpdate())
    {
        //サイクル実績取得
        CycleGetResultPlot();
    }
    else
    {
        //CSV保存ボタン無効化
        CycleChangeCSVButtonEnabled(false);

        //CT最大
        $("#cycle-graph-max-val").val(0);
        //CT最小
        $("#cycle-graph-min-val").val(0);

        //プロット描画数バーの最大値設定
        $("#cycle-plot-disp-bar").attr("max", 0);
        $("#cycle-plot-disp-bar").val(0);
        $("#cycle-plot-disp-num").attr("max", 0);
        $("#cycle-plot-disp-num").val(0);

        //合計サイクル数
        $("#cycle-sum-cycle-num").val(0);
        //異常発生件数
        $("#cycle-error-num").val(0);
        //上限カット数
        $("#cycle-upper-num").val(0);
        //下限カット数
        $("#cycle-lower-num").val(0);
        //手動除外数
        $("#cycle-manual-error-num").val(0);

        //サイクルプロットの描画
        CycleCreatePlot(Array());
    }
}

/**
 *　折れ線グラフ表示
 */
function CycleDispPlot() 
{
    let ctdata= [];

    var lowerErrorCount = 0;
    var upperErrorCount = 0;
    var manualErrorCount = 0;
    //描画されている折れ線グラフから描画データを取得する
    $('.cycle-plot-point').each(function() 
    {
        var plotVal = $(this).attr("value");
        var plotData = JSON.parse(plotVal);


        //異常数取得　手動除外のものでも上下限範囲外のものは上下限除外でカウントする
        if(Number(plotData.IsException) == 1)
        {
            if(Number(plotData.CycleTime) < Number(plotData.Lower))
            {
                //下限カット数
                lowerErrorCount++;
            }
            else if(Number(plotData.CycleTime) > Number(plotData.Upper))
            {
                //上限カット数
                upperErrorCount++;
            }
            else
            {
                //手動除外数
                manualErrorCount++;
            }
        }

        ctdata.push(plotData);
    });

    //取得できなければグラフが表示されていないので処理しない
    if(ctdata.length < 1)
    {
        return;
    }

    //異常件数は取得しなおす(手動除外切替で変わるので)
    //異常発生件数
    $("#cycle-error-num").val(manualErrorCount + lowerErrorCount + upperErrorCount);
    //上限カット数
    $("#cycle-upper-num").val(upperErrorCount);
    //下限カット数
    $("#cycle-lower-num").val(lowerErrorCount);
    //手動除外数
    $("#cycle-manual-error-num").val(manualErrorCount);

    //折れ線グラフ描画
    CycleCreatePlot(ctdata);
}

/**
 *　折れ線グラフ描画
 */
function CycleCreatePlot(ctdata) 
{
    //グラフエリアをリセットする
    $("#cycle-plot-area").empty();
    var clone = '<svg class="cycle-plot" id="cycle-plot"></svg> ';
    $("#cycle-plot-area").append(clone);

    let graph = new CtGraph('#cycle-plot');
    // graph.initializeAxis("CT【秒】", 20, 80, 1, 10, 8, 15, 12);
    graph.setData(ctdata);

    //プロット基準範囲
    graph.setPlotWidth($("#cycle-plot-area").width() -70); //縦線の数値部分を確保するための-70
    graph.setGraphArea($("#cycle-plot-area").width(), $("#cycle-plot").height());
    graph.setYAxisWidth($("#cycle-plot").height() - 70); //横線の数値部分を確保するための-70
    // graph.setGraphArea($("#cycle-plot-area").width(), 270);
    
    //最大最小範囲
    graph.setMax(Number($("#cycle-graph-max-val").val()));
    graph.setMin(Number($("#cycle-graph-min-val").val()));

    //描画プロット範囲　0～プロット数
    graph.setArea(0, ctdata.length);

    if($("#cycle-plot-disp-num").val() > 30)
    {
        var plotSpan = parseInt($("#cycle-plot-disp-num").val() / 30);
    }
    else
    {
        var plotSpan = 1;
    }

    //X軸の指標セット
    graph.setXAxisSetting(-45, 3, plotSpan, 1);

    //プロットの生成、描画数を渡し、実際のプロット横幅を返す
    var graphWidth = graph.update2($("#cycle-plot-disp-num").val());

    //プロットそのものの横幅
    $("#cycle-plot").width(graphWidth + 70);
}

/**
 * 取得した値からプロットデータ1つ分を作成する
 * @param {json} plotData DB取得プロットデータ(1レコード)
 * @param {int} index サイクルインデックス
 * @returns json サイクルプロットデータ
 */
function CycleCreateCTData(plotData, index)
{
    var data = {};

    //除外時間変更時に用いる検索キー
    data["CompositionId"] = plotData.CompositionId;
    data["ProductTypeId"] = plotData.ProductTypeId;
    data["ProcessIdx"] = plotData.ProcessIdx;
    data["StartTime"] = plotData.StartTime;
    data["EndTime"] = plotData.EndTime;

    //CSV出力で用いるデータ
    data["OperationShift"] = plotData.OperationShift;
    data["CompositionName"] = plotData.CompositionName;
    data["ProductTypeName"] = plotData.ProductTypeName;
    data["ProcessName"] = plotData.ProcessName;
    data["WorkerName"] = plotData.WorkerName;

    //サイクルインデックス
    data["Number"] = index;
    //サイクルタイム
    data["CycleTime"] = plotData.CycleTime;    
    //除外フラグ
    data["IsException"] = plotData.ErrorFlag;
    //上限カット
    data["Upper"] = plotData.CycleTimeUpper;
    //下限カット
    data["Lower"] = plotData.CycleTimeLower;
    //標準値（CT最小標準値）
    data["DefValue"] = plotData.CycleTimeMin;
    //開始時刻
    var startTime = moment(plotData.StartTime);
    //終了時刻
    var endTime = moment(plotData.EndTime);

    var text = "";
    text += "サイクル数:" + index + "\n";
    text += "開始時刻:" + startTime.format("YYYY/MM/DD HH:mm:ss") + "\n";
    text += "終了時刻:" + endTime.format("YYYY/MM/DD HH:mm:ss") + "\n";
    text += "品番タイプ:" + plotData.ProductTypeName + "\n";
    text += "作業者:" + plotData.WorkerName + "\n";
    text += "実績CT:" + plotData.CycleTime + "\n";
    if(plotData.VideoExists == 1)
    {
        data["HasVideo"] = 1;
        text += "動画有無:有";
    }
    else
    {
        data["HasVideo"] = 0;
        text += "動画有無:無";
    }
    data["Title"] = text;

    return data;
}

/**
 * CSVデータの1行を作成する
 * @param {json} plotData 
 */
function CycleCreateCSVFormat(plotData)
{
    var text = "";
    //開始時間
    text += moment(plotData.StartTime).format("YYYY/MM/DD HH:mm:ss");
    text += ",";
    //終了時間
    text += moment(plotData.EndTime).format("YYYY/MM/DD HH:mm:ss");
    text += ",";
    //勤帯
    text += plotData.OperationShift;
    text += ",";
    //編成
    text += plotData.CompositionName;
    text += ",";
    //品番
    text += plotData.ProductTypeName;
    text += ",";
    //工程
    text += plotData.ProcessName;
    text += ",";
    //作業者名
    text += plotData.WorkerName;
    text += ",";
    //CT実績
    text += plotData.CycleTime;
    text += ",";
    //上限値
    text += plotData.Upper;
    text += ",";
    //下限値
    text += plotData.Lower;
    text += "\r\n";

    return text;
}

/**
 * 直を文字列から数字に変換して返す
 * @returns string 直を数字に戻したCSV
 */
function CycleGetShiftValue()
{
    shift = $('#cycle-shift').val().split(",");
    if(shift[0] == "ALL")
    {
        return "ALL";
    }

    var returnStr = "";
    for(i = 0; i < shift.length; i++)
    {
        if(i != 0)
        {
            returnStr += ",";
        }
        switch(shift[i])
        {
            case "一直":
                returnStr += "1";
                break;
            case "二直":
                returnStr += "2";
                break;
            case "三直":
                returnStr += "3";
                break;
        }
    }
    return returnStr;
}

/**
 * 登録ボタン部の有効無効切替
 * @param {bool} isEnabled true:Enable false:disable
 */
function CycleChangeRegistEnabled(isEnabled)
{
    if(isEnabled)
    {
        //最小値
        $("#cycle-min-val").prop('disabled', false);
        $("#cycle-min-button").prop('disabled', false);
        //平均値
        $("#cycle-average-val").prop('disabled', false);
        $("#cycle-average-button").prop('disabled', false);
        //最大値
        $("#cycle-max-val").prop('disabled', false);
        $("#cycle-max-button").prop('disabled', false);
        //上限値
        $("#cycle-upper-val").prop('disabled', false);
        $("#cycle-upper-button").prop('disabled', false);
        //下限値
        $("#cycle-lower-val").prop('disabled', false);
        $("#cycle-lower-button").prop('disabled', false);
    }
    else
    {
        //最小値
        $("#cycle-min-val").prop('disabled', true);
        $("#cycle-min-button").prop('disabled', true);
        //平均値
        $("#cycle-average-val").prop('disabled', true);
        $("#cycle-average-button").prop('disabled', true);
        //最大値
        $("#cycle-max-val").prop('disabled', true);
        $("#cycle-max-button").prop('disabled', true);
        //上限値
        $("#cycle-upper-val").prop('disabled', true);
        $("#cycle-upper-button").prop('disabled', true);
        //下限値
        $("#cycle-lower-val").prop('disabled', true);
        $("#cycle-lower-button").prop('disabled', true);
    }
}

/**
 * CTCSV出力ボタンの有効無効切替
 * @param {bool} isEnabled true:Enable false:disable
 */
function CycleChangeCSVButtonEnabled(isEnabled)
{
    if(isEnabled)
    {
        //CSV出力ボタン
        $("#cycle-csv-download").prop('disabled', false);
        //表示件数変更バー
        $("#cycle-plot-disp-bar").prop('disabled', false);
        $("#cycle-plot-disp-num").prop('disabled', false);
    }
    else
    {
        //CSV出力ボタン
        $("#cycle-csv-download").prop('disabled', true);
        //表示件数変更バー
        $("#cycle-plot-disp-bar").prop('disabled', true);
        $("#cycle-plot-disp-num").prop('disabled', true);
    }
}

/**
 * ポップアップ一括除外変更処理
 * @param {bit} isError 
 * @returns 
 */
function CyclePopupButtonUpdate(isError)
{
    var count = 0;
    var fromStartTime = "";
    var toEndTime = "";
    var processIdx = 0;
    //除外指定のプロットからデータを取得
    $('.right-click-plot').each(function() 
    {
        if(count >= 2)
        {
            //※break
            return;
        }
        else if(count == 0)
        {
            var fromVal = $(this).attr("value");
            var fromJson = JSON.parse(fromVal);
            fromStartTime = fromJson.StartTime;
            processIdx = fromJson.ProcessIdx;
            count++;
        }
        else
        {
            var toVal = $(this).attr("value");
            var toJson = JSON.parse(toVal);
            toEndTime = toJson.StartTime;
            count++;
        }
    });

    //普通ありえない
    if(fromStartTime == "" || toEndTime == "")
    {
        alert("異常が発生しました。再度範囲指定を行ってください。");

        //From入力を解除する
        $(".click-plot").removeClass("click-plot");
        $(".right-click-plot").removeClass("right-click-plot");

        //ダイアログを閉じる
        $(".cycle-popup").removeClass("active");
        return;
    }

    //一括除外
    var ret = CycleMultiUpdateResult(isError, fromStartTime, toEndTime, processIdx);
    if(ret == false)
    {
        alert("データの更新に失敗しました。");
    }

    //From入力を解除する
    $(".click-plot").removeClass("click-plot");
    $(".right-click-plot").removeClass("right-click-plot");

    //ダイアログを閉じる
    $(".cycle-popup").removeClass("active");

    //画面更新
    CyclePageUpdate();

    //山積み表更新
    PilePageUpdate();
}

/**
 * 画面更新実行OKチェック
 * @returns 
 */
function CycleCheckPageUpdate()
{
    if($("#cycle-composition").text() != "-" && 
       $("#cycle-composition").text() != "" && 
       $("#cycle-product").val() != "-" && 
       $("#cycle-product").val() != "" && 
       $("#cycle-process").text() != "-" && 
       $("#cycle-process").text() != "" && 
       $("#cycle-worker").val() != "-" &&
       $("#cycle-worker").val() != "")
    {
        return true;
    }
    else
    {
        return false;
    }
}

//#endregion "関数処理"