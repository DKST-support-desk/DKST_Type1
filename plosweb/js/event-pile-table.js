//#region イベント処理
/**
 * イベント処理
 */
$(function () 
{
    /**
    * HTML読み込み時イベント
    */
    $(document).ready(function () 
    {
        try
        {
            //クッキーの情報を取得
            PileGetCookie();

            //ライン情報取得
            PileGetLineInfo();

            //期間を当日に設定
            var nowDate = new moment();
            startDate = nowDate.format("YYYY/MM/DD");
            $('#pile-span').val(startDate + " - " + startDate);

            //勤帯取得
            var shiftList = PileGetShift();
            if(shiftList.length != 0)
            {
                $("#pile-shift").val(shiftList[0]);
            }

            //作業編成取得
            PileGetComposition();

            //品番取得
            PileGetProductType();

            //画面更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("HTML読み込み時イベントエラー:" + e.message );
        }
    });

    /**
     * カレンダー表示イベント
     */
    UIkit.util.on('#pile-dropdown', 'show', function () 
    {
        try
        {
            //入力期間取得
            spanArray = $('#pile-span').val().split(' - ');
            startDate = moment(spanArray[0]);
            endDate = moment(spanArray[1]);
            nowMonth = moment();

            var span = PileGetOperatingPlan();
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
            $('#pile-calendar-div').append(clone);

            //カレンダー初期表示
            CalendarCreate(startDate, endDate, startSpan, endSpan, nowMonth, false, "pile", allSpan);
        } 
        catch(e) 
        {
            alert("カレンダー表示イベントエラー:" + e.message );
        }
    });

    /**
     * カレンダー非表示イベント
     */
    UIkit.util.on('#pile-dropdown', 'hide', function () 
    {
        //カレンダー部分を消す
        $("#pile-calendar-div").empty();
    });

//#region 勤帯ドロップダウンイベント

    /**
     * 勤帯プルダウン表示処理
     */
    $('body').on('show', '#pile-shift-dropdown' , function() 
    {
        try
        {
            //勤帯リスト
            var shiftList = PileGetShift();
    
            //divリセット
            var divName = "#pile-shift-div";
            $(divName).empty();
    
            //選択されていた勤帯を取得する
            var shiftVal = "#pile-shift";
            var nowShiftList = $(shiftVal).val().split(',');
    
            //テーブル追加
            $(divName).append('<table class="pile-shift-table" id="pile-shift-table"></table>');
    
            //ALL行追加
            var select = "";
            if(nowShiftList.includes("ALL"))
            {
                select = "pile-shift-select-td";
            }
            $("#pile-shift-table").append('<tr><td class="pile-shift-table-td ' + select + '" id="pile-shift-all">ALL</td></tr>');
    
            //勤帯リスト追加
            for(var i = 0; i < shiftList.length; i++)
            {
                select = "";
                if(nowShiftList.includes(shiftList[i]))
                {
                    select = "pile-shift-select-td";
                }
                $("#pile-shift-table").append('<tr><td class="pile-shift-table-td ' + select + '">' + shiftList[i] + '</td></tr>');
            }
        } 
        catch(e) 
        {
            alert("勤帯プルダウン表示イベントエラー:" + e.message );
        }
    });
 
    /**
     * 勤帯プルダウン閉じる処理
     */
    $('body').on('hide', '#pile-shift-dropdown' , function() 
    {
        try
        {
            var select = "";
            var buttonText = "";
    
            var count = 1;
            //選択行の名称を全て取得する
            $('.pile-shift-select-td').each(function() 
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
            $("#pile-shift-div").empty();
    
            //勤帯テキストボックスに値を入れる
            $("#pile-shift").val(select);

            //作業編成取得
            PileGetComposition();

            var isALL = false;
            if($("#pile-composition").text() != "-")
            {
                isALL = true;
            }

            //作業編成取得
            PileGetComposition();

            //品番取得
            PileGetProductType();

            //画面更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("勤帯プルダウン閉じる処理エラー:" + e.message );
        }
    });
 
    /**
     * 勤帯リストクリック処理
     */
    $('body').on('click', '.pile-shift-table-td' , function() 
    {
        try
        {
            //クリックコントロール
            var clickTd = $(this);
    
            //ALLクリック時はほかの選択を解除する
            if(clickTd.text() == "ALL")
            {
                $(".pile-shift-select-td").removeClass("pile-shift-select-td");
            }
            else
            {
                $("#pile-shift-all").removeClass("pile-shift-select-td");
            }
    
            //選択行なら選択解除、未選択行なら選択状態にする
            if (clickTd.hasClass("pile-shift-select-td")) 
            {
                if($(".pile-shift-select-td").length == 1)
                {
                    //選択行が1行しかない状態での選択解除は無視する
                    return;
                }
    
                clickTd.removeClass("pile-shift-select-td");
            } 
            else 
            {
                clickTd.addClass("pile-shift-select-td");
            }
    
            //ALLの他にデータが1つしかないならALL強制切替はしない
            if($(".pile-shift-table-td").length != 2)
            {
                //選択されている行が全行-1個あればALL以外の全てが選択されているのでALL選択に切り替える
                if($(".pile-shift-select-td").length == $(".pile-shift-table-td").length - 1)
                {
                    $(".pile-shift-select-td").removeClass("pile-shift-select-td");
                    $("#pile-shift-all").addClass("pile-shift-select-td");
                }
            }
        } 
        catch(e) 
        {
            alert("勤帯リストクリックイベントエラー:" + e.message );
        }
    });
 
//#endregion 勤帯ドロップダウンイベント

    /**
     * 編成変更イベント
     */
    $("#pile-composition").change(function () 
    {
        try
        {
            //品番取得
            PileGetProductType();

            //画面更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("編成変更イベントエラー:" + e.message );
        }
    });

    /**
     * 品番変更イベント
     */
    $("#pile-product").change(function () 
    {
        try
        {
            //画面更新
            PilePageUpdate();
        } 
        catch(e) 
        {
            alert("品番変更イベントエラー:" + e.message );
        }
    });

    /**
     * サイズ変更アイコンクリックイベント
     */
    $("#pile-size-change-icon").click(function () 
    {
        try
        {
            $('#index-frame-2').toggle();
            $('#index-frame-3').toggle();

            //グラフ再描画
            PileDispGraph();

            //プロット再描画
            CycleDispPlot();
        } 
        catch(e) 
        {
            alert("サイズ変更アイコンクリックイベントエラー:" + e.message );
        }
    });

//#region チェックボックスイベント

    /**
     * T.Tラインチェックボックス変更イベント
     */
    $("#pile-tt-checkbox").change(function()
    {
        try
        {
            if($("#pile-tt-checkbox").prop("checked"))
            {
                //1年保持
                Cookies.set("pile-tt-check", "true", { expires: 365 });
            }
            else
            {
                Cookies.set("pile-tt-check", "false");
            }

            //棒グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("T.Tラインチェックボックス変更イベントエラー:" + e.message );
        }
    });

    /**
     * 出来高ピッチラインチェックボックス変更イベント
     */
    $("#pile-pitch-checkbox").change(function()
    {
        try
        {
            if($("#pile-pitch-checkbox").prop("checked"))
            {
                //1年保持
                Cookies.set("pile-pitch-check", "true", { expires: 365 });
            }
            else
            {
                Cookies.set("pile-pitch-check", "false");
            }

            //棒グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("出来高ピッチラインチェックボックス変更イベントエラー:" + e.message );
        }
    });

    /**
     * ネックCT(加重)ラインチェックボックス変更イベント
     */
    $("#pile-weight-checkbox").change(function()
    {
        try
        {
            if($("#pile-weight-checkbox").prop("checked"))
            {
                //1年保持
                Cookies.set("pile-weight-check", "true", { expires: 365 });
            }
            else
            {
                Cookies.set("pile-weight-check", "false");
            }

            //棒グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("ネックCT(加重)ラインチェックボックス変更イベントエラー:" + e.message );
        }
    });

    /**
     * ネックCT(個別)ラインチェックボックス変更イベント
     */
    $("#pile-single-checkbox").change(function()
    {
        try
        {
            if($("#pile-single-checkbox").prop("checked"))
            {
                //1年保持
                Cookies.set("pile-single-check", "true", { expires: 365 });
            }
            else
            {
                Cookies.set("pile-single-check", "false");
            }

            //棒グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("ネックCT(個別)ラインチェックボックス変更イベントエラー:" + e.message );
        }
    });

//#endregion チェックボックスイベント


//#region 最大最小テキストボックスイベント

    /**
     * 最大最小テキストボックスキーダウンイベント
     */
    $(".pile-grapf-input").keydown(function(event)
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
     * 最大最小テキストボックスマウスアップ、キーアップイベント
     */
    $('.pile-grapf-input').on('input', function()
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
            alert("最大最小テキストボックスマウスキーアップイベントエラー:" + e.message );
        }
    });

    /**
     * 最大最小テキストボックス値変更イベント
     */
    $('.pile-grapf-input').change(function()
    {
        try
        {
            //最大最小がかぶっていたら最小を0にしてグラフが消えるのを防ぐ
            if(Number($("#pile-graph-max-val").val()) <= Number($("#pile-graph-min-val").val()))
            {
                $("#pile-graph-min-val").val(0);
            }

            //グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("最大最小テキストボックス値変更イベントエラー:" + e.message );
        }
    });

//#endregion 最大最小テキストボックスイベント

//#region  表示列数テキストボックスイベント
    /**
     * 表示列数テキストボックスキーダウンイベント
     */
    $("#pile-graph-disp-num").keydown(function(event)
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
            alert("表示列数テキストボックスキーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * 表示列数テキストボックスマウスアップ、キーアップイベント
     */
    $('#pile-graph-disp-num').on('input', function()
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
                $(this).val(valMin); 
                return;
            }

            $(this).val(val);
        } 
        catch(e) 
        {
            alert("表示列数テキストボックスマウスキーアップイベントエラー:" + e.message );
        }
    });

    /**
     * 表示列数テキストボックス値変更イベント
     */
    $('#pile-graph-disp-num').change(function()
    {
        try
        {
            //表示数
            dispNum = Number($(this).val());
            //最大列数
            maxIndex = Number($('#pile-process-top').val());
            //グラフ左端のインデックス
            graphStart = Number($('#pile-before-graph').val());
            //グラフ右端のインデックス
            graphEnd = Number($('#pile-after-graph').val());

            //表示数が最大列数を超えている場合何もしない
            if(dispNum > maxIndex)
            {
                dispNum = maxIndex;
            }

            //現在表示数
            var nowDisp = (graphEnd - graphStart) + 1;

            //現在の表示列数よりも表示数が小さくなっていた場合
            if(nowDisp > dispNum)
            {
                var remDisp = nowDisp - dispNum;
                //表示されている右端列を非表示にする
                for(var i = 0; i < nowDisp - dispNum; i++)
                {
                    if(remDisp == 0)
                    {
                        break;
                    }
                    PileChangeHideTable(graphEnd - i, false);
                    $('#pile-after-graph').val(graphEnd - i - 1);
                    remDisp--;
                }
            }  
            else
            {
                //後ろが最後のインデックスまでいっていたら前を表示する
                if(maxIndex <= graphEnd)
                {
                    //左端がすでに最初のインデックスでないなら
                    if(graphStart != 1)
                    {
                        for(var i = 0; i < dispNum - nowDisp; i++)
                        {
                            //左列を表示する
                            PileChangeHideTable(graphStart - i - 1, true);
                            $('#pile-before-graph').val(graphStart - i - 1);
                        }
                    }
                }
                else
                {
                    //追加列数
                    var addDisp = dispNum - nowDisp;

                    for(var i = graphEnd; i < maxIndex; i++)
                    {
                        if(addDisp == 0)
                        {
                            break;
                        }
                        //右列を表示する
                        PileChangeHideTable(i + 1, true);
                        $('#pile-after-graph').val(i + 1);
                        addDisp--;
                    }

                    for(var i = 1; i < graphStart; i++)
                    {
                        if(addDisp == 0)
                        {
                            break;
                        }
                        //左列を表示する
                        PileChangeHideTable(graphStart - i, true);
                        $('#pile-before-graph').val(graphStart - i);
                        addDisp--;
                    }
                }
            }

            //グラフ更新
            PileDispGraph(); 
        } 
        catch(e) 
        {
            alert("表示列数テキストボックス値変更イベントエラー:" + e.message );
        }
    });

//#endregion 表示列数テキストボックスイベント

    /**
     * 表戻るボタン押下イベント
     */
    $("#pile-before-graph").click(function()
    {
        try
        {
            graphStart = Number($('#pile-before-graph').val());
            graphEnd = Number($('#pile-after-graph').val());

            //最初のインデックスまでいっていたら何もしない
            if(1 >= graphStart)
            {
                return;
            }

            //列の表示非表示を切り替える
            PileChangeHideTable((graphStart - 1), true);
            PileChangeHideTable(graphEnd, false);

            $('#pile-before-graph').val(graphStart-1);
            $('#pile-after-graph').val(graphEnd-1);

            //グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("表戻るボタン押下イベントエラー:" + e.message );
        }
    });

    /**
     * 表進むボタン押下イベント
     */
    $("#pile-after-graph").click(function()
    {
        try
        {
            maxIndex = Number($('#pile-process-top').val());

            graphStart = Number($('#pile-before-graph').val());
            graphEnd = Number($('#pile-after-graph').val());

            //最後のインデックスまでいっていたら何もしない
            if(maxIndex <= graphEnd)
            {
                return;
            }

            //列の表示非表示を切り替える
            PileChangeHideTable((graphEnd + 1), true);
            PileChangeHideTable(graphStart, false);

            $('#pile-before-graph').val(graphStart+1);
            $('#pile-after-graph').val(graphEnd+1);

            //グラフ更新
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("表進むボタン押下イベントエラー:" + e.message );
        }
    });
    
    /**
     * 作業者プルダウン表示処理　※後で追加されるコントロール
     */
    $('body').on('show', '.pile-worker-dropdown' , function() 
    {
        try
        {
            //表示インデックス取得
            var clickControl = $(this).attr('id').split('-');
            var index = clickControl[clickControl.length - 1];

            //作業者項目が空文字なら工程のない列なので無視する
            var workerName = "#pile-worker-button-" + index;
            if($(workerName).val() == "")
            {
                //ドロップダウンを隠すことで表示を無視する
                $(this).hide();
                return;
            }

            //工程index
            var processName = "#pile-process-" + index;
            var processIndex = $(processName).val();

            //実績作業者リスト
            var resultWorkerList = PileGetWorker(processIndex);

            //divリセット
            var divName = "#pile-worker-div-" + index;
            $(divName).empty();

            //選択されていた作業者を取得する
            var workerButton = "#pile-worker-button-" + index;
            var workerList = JSON.parse($(workerButton).val()).Worker;

            //テーブル追加
            $(divName).append('<table class="pile-worker-table" id="pile-worker-table"></table>');

            //ALL行追加
            var select = "";
            if(workerList.includes("ALL"))
            {
                select = "pile-worker-select-td";
            }
            $("#pile-worker-table").append('<tr><td class="pile-worker-table-td ' + select + '" id="pile-worker-all">ALL</td></tr>');

            //作業者リスト追加
            for(var i = 0; i < resultWorkerList.length; i++)
            {
                select = "";
                if(workerList.includes(resultWorkerList[i]))
                {
                    select = "pile-worker-select-td";
                }
                $("#pile-worker-table").append('<tr><td class="pile-worker-table-td ' + select + '">' + resultWorkerList[i] + '</td></tr>');
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
    $('body').on('hide', '.pile-worker-dropdown' , function() 
    {
        try
        {
            var buttonText = "";

            var count = 1;
            var workerList = Array();

            //選択行の名称を全て取得する
            $('.pile-worker-select-td').each(function() 
            {
                if(count == 1)
                {
                    buttonText = $(this).text();
                }
                else if(count == 2)
                {
                    //二人以上選択されていれば他を付ける
                    buttonText += "  他";
                }

                workerList.push($(this).text());

                count++;
            });

            //プルダウンを表示していた列インデックスを取得
            var clickControl =  $(this).attr('id').split('-');
            var index = clickControl[clickControl.length - 1];

            var divName = "#pile-worker-div-" + index;
            var workerName = "#pile-worker-button-" + index;

            //テーブル部分を消す
            $(divName).empty();

            //山積み表の作業者ボタンに値を入れる
            var json = JSON.stringify({ Worker: workerList });
            $(workerName).val(json);
            $(workerName).text(buttonText);

            //作業者を考慮した山積み表更新
            PileGetUnderTableWorkerUpdate();
        } 
        catch(e) 
        {
            alert("作業者プルダウン閉じる処理エラー:" + e.message );
        }
    });

    /**
     * 作業者リストクリック処理
     */
    $('body').on('click', '.pile-worker-table-td' , function() 
    {
        try
        {
            //クリックコントロール
            var clickTd = $(this);

            //ALLクリック時はほかの選択を解除する
            if(clickTd.text() == "ALL")
            {
                $(".pile-worker-select-td").removeClass("pile-worker-select-td");
            }
            else
            {
                $("#pile-worker-all").removeClass("pile-worker-select-td");
            }

            //選択行なら選択解除、未選択行なら選択状態にする
            if (clickTd.hasClass("pile-worker-select-td")) 
            {
                if($(".pile-worker-select-td").length == 1)
                {
                    //選択行が1行しかない状態での選択解除は無視する
                    return;
                }

                clickTd.removeClass("pile-worker-select-td");
            } 
            else 
            {
                clickTd.addClass("pile-worker-select-td");
            }

            //ALLの他にデータが1つしかないならALL強制切替はしない
            if($(".pile-worker-table-td").length != 2)
            {
                //選択されている行が全行-1個あればALL以外の全てが選択されているのでALL選択に切り替える
                if($(".pile-worker-select-td").length == $(".pile-worker-table-td").length - 1)
                {
                    $(".pile-worker-select-td").removeClass("pile-worker-select-td");
                    $("#pile-worker-all").addClass("pile-worker-select-td");
                }
            }
        } 
        catch(e) 
        {
            alert("作業者リストクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 表示設定変更イベント
     */
    $("body").on("change", ".pile-disp-option" , function() 
    {
        try
        {
            //作業者を考慮した山積み表更新
            PileGetUnderTableWorkerUpdate();

            //グラフ更新
            //PileDispGraph();
        } 
        catch(e) 
        {
            alert("表示設定変更イベントエラー:" + e.message );
        }
    });

//#region 棒グラフクリック処理

    /**
     * グラフエリアをクリックした際の処理
     */
    $("body").on("psclick", ".ps-bar-class", function()
    {   
        idNameList = $(this).attr("id").split('-');
        PileCheckCycleExists(Number(idNameList[1]))
    });


//#endregion 棒グラフクリック処理
});

//#endregion イベント処理

//#region ajax処理

/**
 * ライン情報取得
 */
function PileGetLineInfo() 
{
    //関数インデックス
    funIndex = 0;

    $.ajax(
        {
            type: "POST",
            url: "../method/method-pile-table.php",
            data: { "funIndex": funIndex },
            dataType: "json",
            async: false //同期通信
        })
        //正常時
        .done(function (data) 
        {
            if (data.length == 1) 
            {
                //ライン名
                $("#pile-line-name").text(data[0].LineName);

                //稼働率
                $("#pile-occupancy-rate").text(data[0].OccupancyRate);

                //編成効率
                $("#pile-composition-efficiency").text(data[0].CompositionEfficiency);
            }
        })
        //失敗時
        .fail(function (XMLHttpRequest, textStatus, error) 
        {
            alert("ライン情報取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
        })
        //必ず実行
        .always(function (data) 
        {

        });
}

/**
 * 勤帯取得
 */
function PileGetShift() 
{
    //関数インデックス
    funIndex = 1;
 
    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    
    var shiftList = new Array();
 
    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
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
function PileGetComposition() 
{
    //関数インデックス
    funIndex = 2;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = PileGetShiftValue();

    //クリア
    $("#pile-composition").children().remove();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
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
                $('#pile-composition').append($('<option>').html(data[i].UniqueName).val(data[i].CompositionId));
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
        if ($('#pile-composition').children("option").length == 0) 
        {
            $('#pile-composition').append($('<option>').html("-").val("-"));
        }
    });
}

/**
 * 品番取得
 */
function PileGetProductType() 
{
    //関数インデックス
    funIndex = 3;

    //クリア
    $("#pile-product").children().remove();

    //作業編成が無いなら品番も無い
    if ($('#pile-composition').val() == "-") 
    {
        $('#pile-product').append($('<option>').html("-").val("-"));
        return;
    }

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "shift": shift, "compositionId": compositionId },
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
                $('#pile-product').append($('<option>').html(data[i].ProductTypeName).val(data[i].ProductTypeId));
            }
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("品番取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {
        if ($('#pile-product').children("option").length == 0) 
        {
            $('#pile-product').append($('<option>').html("-").val("-"));
        }
    });
}

/**
 * 稼働予定日取得
 */
function PileGetOperatingPlan() 
{
    //関数インデックス
    funIndex = 4;

    var startSpan = moment();
    var endSpan = moment();
    var allData = null;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data[0] != null) 
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
 * 上部テーブル取得処理
 */
function PileGetTopTable()
{
    //関数インデックス
    funIndex = 5;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    operationShift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();
    productTypeId = $("#pile-product").val();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": operationShift, "compositionId": compositionId, "productTypeId": productTypeId },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if (data != null) 
        {
            $('#pile-required-num').text(data[0].RequiredNum);
            $('#pile-plan-second').text(data[0].PlanSecond);
            $('#pile-tt').text(data[0].TT);
            $('#pile-result-num').text(data[0].ResultNum);
            $('#pile-result-time').text(data[0].ResultTime);
            $('#pile-pitch').text(data[0].Pitch);
            $('#pile-neck-weight').text(data[0].NeckWeight);
            $('#pile-neck-single').text(data[0].NeckSingle);
            $('#pile-table-occupancy-rate').text(data[0].OccupancyRate);
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("上部テーブル取得処理失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

/**
 * 下部テーブル更新処理
 */
function PileGetUnderTable()
{
    //関数インデックス
    funIndex = 6;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    operationShift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();
    productTypeId = $("#pile-product").val();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": operationShift, "compositionId": compositionId, "productTypeId": productTypeId },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        //表示数
        var processNum = Number($("#pile-graph-disp-num").val());
        if(data.length > processNum)
        {
            //取得工程数のほうが多ければ取得工程数分だけ列は作る
            var processNum = data.length;
        }
        
        //山積み表の初期化
        PileInitializePileTable(data.length);

        //グラフの最大値を初期化
        $("#pile-graph-max-val").val(0);
        
        //山積み表更新処理
        PileUpdateUnderTable(data);

        //推奨戦略チェック更新処理
        PileUpdateStrategy();
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("下部テーブル取得処理失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

/**
 * 作業者リスト取得
 * @returns 
 */
function PileGetWorker(index) 
{
    //関数インデックス
    funIndex = 7;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    operationShift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();
    productTypeId = $("#pile-product").val();
    processIdx = index; 

    var workerList = new Array();
    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": operationShift, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        for(var i = 0; i < data.length; i++)
        {
            workerList.push(data[i].WorkerName);
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("作業者一覧取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function (data) 
    {

    });
    return workerList;
}

/**
 * サイクル期間取得
 */
function PileGetCycleSpan() 
{
    //関数インデックス
    funIndex = 8;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    shift = PileGetShiftValue();

    var cycleSpan = "";
    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": shift },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        cycleSpan = data[0].CycleSpan;
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("サイクル期間取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always
    (function () 
    {

    });

    return cycleSpan;
}

/**
 * 下部テーブル更新処理(作業者変更時)
 */
function PileGetUnderTableWorkerUpdate()
{
    //関数インデックス
    funIndex = 9;

    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    operationShift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();
    productTypeId = $("#pile-product").val();
    processMaxIdx = Number($('#pile-process-top').val());

    var workerList = Array();
    $('.pile-worker-button').each(function() 
    {
        worker = JSON.parse($(this).attr("value")).Worker;
        workerList.push(worker);
    });
    jsonWorker = JSON.parse(JSON.stringify({ Worker: workerList }));

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": operationShift, "compositionId": compositionId, "productTypeId": productTypeId, "processMaxIdx": processMaxIdx, "jsonWorker": jsonWorker },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {        
        //山積み表更新処理
        PileUpdateUnderTableWorkerUpdate(data);

        //推奨戦略チェック更新処理
        PileUpdateStrategy();

        //グラフ更新
        PileDispGraph();
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("下部テーブル取得処理失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}


/**
 * サイクル実績チェック処理
 */
function PileCheckCycleExists($index)
{
    //インデックスを取得する
    var graphStart = Number($('#pile-before-graph').val()) + $index;
  
    //関数インデックス
    funIndex = 10;
    
    //期間を開始日と終了日に分ける
    spanArray = $('#pile-span').val().split(' - ');
    startDate = spanArray[0];
    endDate = spanArray[1];
    operationShift = PileGetShiftValue();
    compositionId = $("#pile-composition").val();
    productTypeId = $("#pile-product").val();
    processIdx = graphStart - 1;
    var workContorol = "#pile-worker-button-" + graphStart;
    workerName = JSON.parse($(workContorol).val()).Worker;

    $.ajax(
    {
        type: "POST",
        url: "../method/method-pile-table.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "operationShift": operationShift, "compositionId": compositionId, "productTypeId": productTypeId, "processIdx": processIdx, "workerName": workerName },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {        
        if(data[0].Exists)
        {
            var idName = "#pile-disp-option-select-" + graphStart;

            //サイクル時刻取得
            var cycleSpan = PileGetCycleSpan();
            if(cycleSpan == "")
            {
                alert("サイクル情報の取得に失敗しました。");
                return;
            }

            //期間設定
            $("#cycle-span").val(cycleSpan);

            //勤帯設定
            $("#cycle-shift").val( $("#pile-shift").val() );

            //作業編成取得
            CycleGetComposition();

            //作業編成設定
            $("#cycle-composition").val( $("#pile-composition").val() );

            //品番設定
            //ラベル部分に選択値を保持する
            var json = JSON.stringify({ Product: Array($("#pile-product").val()) });
            $("#cycle-product-label").val(json);

            $("#cycle-product").val( $("#pile-product option:selected").text() );

            //工程取得
            CycleGetProcess();

            //工程設定
            idName = "#pile-process-" + graphStart;
            $("#cycle-process").val($(idName).val());

            //作業者設定
            idName = "#pile-worker-button-" + graphStart;
            //ラベル部分に選択値を保持する
            $("#cycle-worker-label").val($(idName).val());
            $("#cycle-worker").val($(idName).text());

            //セットしなおした値で画面更新
            CyclePageUpdate();
        }
        else
        {
            alert("この工程には実績が無いため1サイクル毎の実績を表示することができません。");
            return;
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("サイクル実績チェック処理失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

//#endregion ajax処理

//#region 関数

/**
 *　全更新処理 ※主にカレンダー入力時
 */
function PileAllUpdate() 
{
    //勤帯取得
    var shiftList = PileGetShift();
    if(shiftList.length != 0)
    {
        $("#pile-shift").val(shiftList[0]);
    }
    else
    {
        $("#pile-shift").val("ALL");
    }

    //作業編成取得
    PileGetComposition();

    //品番取得
    PileGetProductType();

    //画面更新
    PilePageUpdate();
}

/**
 * 画面更新処理
 */
function PilePageUpdate()
{
    //上部テーブル更新
    PileGetTopTable();

    if($("#pile-product").val() != "-")
    {
        //下部テーブル更新
        PileGetUnderTable();
    }
    else
    {
        //山積み表初期化処理
        PileInitializePileTable(Number($("#pile-graph-disp-num").val()));

        //推奨戦略チェック更新処理
        PileUpdateStrategy();
    }
    
    //棒グラフ表示
    PileDispGraph();
}

//#region 山積み表描画

/**
 * 山積み表初期化処理
 * @param {number} dataNum データ数(工程+1)
 */
function PileInitializePileTable(dataNum)
{
    //表示件数
    var dispNum = Number($("#pile-graph-disp-num").val());
    if(dispNum > dataNum)
    {
        dispNum = dataNum;
    }

    PileCreateProcess(dataNum, dispNum);
    PileCreateWorker(dataNum, dispNum);
    PileCreateDispOption(dataNum, dispNum);
    PileCreateCTMax(dataNum, dispNum);
    PileCreateCTAverage(dataNum, dispNum);
    PileCreateCTMin(dataNum, dispNum);
    PileCreateCTScattering(dataNum, dispNum);
    PileCreateCTMaxLimit(dataNum, dispNum);
    PileCreateCTMinLimit(dataNum, dispNum);
    PileCreateAncillary(dataNum, dispNum);
    PileCreateSetup(dataNum, dispNum);

    //グラフ最大最小
    $("#pile-graph-max-val").val(0);
    $("#pile-graph-min-val").val(0);

    //ΣCT
    $("#pile-actual-sum-ct").text("");
    $("#pile-plan-sum-ct").text("");

    //編成効率
    $("#pile-actual-com-efficiency").text("");
    $("#pile-plan-com-efficiency").text("");

    //MM比
    $("#pile-actual-mm").text("");
    $("#pile-plan-mm").text("");


    //表示数
    var mainCol = parseInt(dispNum / 2);
    var addCol = dispNum % 2;

    $("#pile-actual-sum-ct").attr("colSpan", mainCol + addCol);
    $("#pile-actual-com-efficiency").attr("colSpan", mainCol + addCol);
    $("#pile-actual-mm").attr("colSpan", mainCol + addCol);
    $("#pile-plan-sum-ct").attr("colSpan", mainCol);
    $("#pile-plan-com-efficiency").attr("colSpan", mainCol);
    $("#pile-plan-mm").attr("colSpan", mainCol);
}

/**
 * 山積み表 工程部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateProcess(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-process-tr").empty();

    //項目名称行
    var clone = '<th class="pile-item-name-th pile-top-header" id="pile-process-top" rowspan="1" colspan="2">⑩工程</th>';
    $('#pile-process-tr').append(clone);

    //テンプレート表示部に追加する
    for (let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var clone = '<th class="pile-th" id="pile-process-' + (i+1).toString() +'" colspan="1" ' + hid + '></th>';
            $('#pile-process-tr').append(clone);
        }
        else
        {
            var clone = '<th class="pile-th" id="pile-process-' + (i+1).toString() +'" colspan="1" ' + hid + '>ネックMCT</th>';
            $('#pile-process-tr').append(clone);
        }
    }

    //テーブル表示情報のセット
    $('#pile-process-top').val(dataNum);
    $('#pile-before-graph').val(1);
    $('#pile-after-graph').val(dispNum);
}

/**
 * 山積み表 作業者部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateWorker(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-worker-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name pile-24-height" rowspan="1" colspan="2">⑪作業者</td>';
    $('#pile-worker-tr').append(clone);
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var addDiv = '<div class="pile-worker-dropdown-div" id="pile-worker-div-' + (i+1).toString() + '"></div>';
            var doropDiv = '<div class="pile-worker-dropdown" id="pile-worker-dropdown-' + (i+1).toString() + '" uk-dropdown="mode: click" value="' + (i+1).toString() + '">' + addDiv + '</div>'
            var button = '<button class="pile-worker-button" id="pile-worker-button-' + (i+1).toString() + '" value=""></button>';
            var clone = '<td class="pile-actual pile-24-height" id="pile-worker-' + (i+1).toString() +'" colspan="1" ' + hid +'>' + button + doropDiv + '</td>';
            $('#pile-worker-tr').append(clone);
        }
        else
        {
            var td1 = '<td class="pile-mct-column-left pile-24-height" colspan="1">⑯MT</td>';
            var td2 = '<td class="pile-mct-column-right"  id="pile-mt-val" colspan="1"></td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-worker-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-worker-tr').append(clone);
        }
    }
}

/**
 * 山積み表　表示設定部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateDispOption(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-disp-option-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" rowspan="1" colspan="2">⑫表示設定</td>';
    $('#pile-disp-option-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            //コンボボックスは工程取得後に作成する
            //var select = '<select class="pile-disp-option" id="pile-disp-option-select-' + ( i + 1 ) + '"><option value="0">実績</option><option value="1">標準</option></select>';
            var clone = '<td class="pile-td" id="pile-disp-option-' + ( i + 1 ) + '" colspan="1" ' + hid + '></td>';
            $('#pile-disp-option-tr').append(clone);
        }
        else
        {
            var td1 = '<td class="pile-mct-column-left" colspan="1">⑰HT</td>';
            var td2 = '<td class="pile-mct-column-right" id="pile-ht-val" colspan="1"></td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-disp-option-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-disp-option-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CT最大部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTMax(dataNum, dispNum)
{
    $("#pile-ct-max-tr").empty();

    var clone = '<td class="pile-td" rowspan="6">⑬CT</td>';
    $('#pile-ct-max-tr').append(clone);
    var clone = '<td class="pile-item-name" colspan="1">最大</td>';
    $('#pile-ct-max-tr').append(clone);
    

    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var td1 = '<td class="pile-ct-actual" id="pile-actual-ct-max-' + (i + 1).toString() + '" colspan="1">&emsp;</td>';
            var td2 = '<td class="pile-ct-plan" id="pile-plan-ct-max-' + (i + 1).toString() + '" colspan="1">&emsp;</td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-ct-max-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-ct-max-tr').append(clone);
        }
        else
        {
            var td1 = '<td class="pile-mct-column-left" colspan="1">⑱MCT</td>';
            var td2 = '<td class="pile-mct-column-right" id="pile-mct-val" colspan="1"></td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-ct-max-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-ct-max-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CT平均部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTAverage(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ct-average-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="1">平均</td>';
    $('#pile-ct-average-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var td1 = '<td class="pile-ct-actual" id="pile-actual-ct-average-' + (i + 1).toString() + '" colspan="1">&emsp;</td>';
            var td2 = '<td class="pile-ct-plan" id="pile-plan-ct-average-' + (i + 1).toString() + '" colspan="1">&emsp;</td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-ct-average-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-ct-average-tr').append(clone);
        }
        else
        {
            var clone = '<td class="pile-td" id="pile-ct-average-' + (i+1).toString() +'" colspan="1" rowspan="7" ' + hid + '></td>';
            $('#pile-ct-average-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CT最小部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTMin(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ct-min-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="1">最小</td>';
    $('#pile-ct-min-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }

        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var td1 = '<td class="pile-ct-actual pile-actual-ct-min" id="pile-actual-ct-min-' + (i + 1) + '" colspan="1">&emsp;</td>';
            var td2 = '<td class="pile-ct-plan" id="pile-plan-ct-min-' + (i + 1) + '" colspan="1">&emsp;</td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-ct-min-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-ct-min-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CTばらつき部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTScattering(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ct-scattering-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="1">バラつき</td>';
    $('#pile-ct-scattering-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }

        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var td1 = '<td class="pile-ct-calcu-left" id="pile-actual-ct-scattering-'+ ( i + 1) +'" colspan="1">&emsp;</td>';
            var td2 = '<td class="pile-ct-calcu-right" id="pile-plan-ct-scattering-'+ ( i + 1) +'" colspan="1">&emsp;</td>';
            var clone = '<td class="pile-td pile-td-include-table" id="pile-ct-scattering-' + (i+1).toString() +'" colspan="1" ' + hid + '><table class="pile-table"><tr>'+ td1 + td2 +'</tr></table></td>';
            $('#pile-ct-scattering-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CT上限カット部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTMaxLimit(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ct-max-limit-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="1">上限カット</td>';
    $('#pile-ct-max-limit-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var clone = '<td class="pile-plan" id="pile-ct-max-limit-' + ( i + 1 ) + '" colspan="1" ' + hid + '></td>';
            $('#pile-ct-max-limit-tr').append(clone);
        }
    }
}

/**
 * 山積み表　CT下限カット部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateCTMinLimit(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ct-min-limit-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="1">下限カット</td>';
    $('#pile-ct-min-limit-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var clone = '<td class="pile-plan" id="pile-ct-min-limit-' + ( i + 1 ) + '" colspan="1" ' + hid + '></td>';
            $('#pile-ct-min-limit-tr').append(clone);
        }
    }
}

/**
 * 山積み表　付帯部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateAncillary(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-ancillary-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="2">⑭付帯</td>';
    $('#pile-ancillary-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var clone = '<td class="pile-plan" id="pile-ancillary-' + ( i + 1 ) + '" colspan="1" ' + hid + '></td>';
            $('#pile-ancillary-tr').append(clone);
        }
    }
}

/**
 * 山積み表　段取り部分描画
 * @param {number} dataNum データ数(工程+1)
 */
function PileCreateSetup(dataNum, dispNum)
{
    //現在表示を消去
    $("#pile-setup-tr").empty();

    //項目名称行
    var clone = '<td class="pile-item-name" colspan="2">⑮段取り</td>';
    $('#pile-setup-tr').append(clone);

    //工程行
    for(let i = 0; i < dataNum; i++)
    {
        var hid = "";
        if(i + 1 >= dispNum + 1)
        {
            hid = "hidden";
        }
        
        //最終行だけネックMCT
        if(i != dataNum - 1)
        {
            var clone = '<td class="pile-plan" id="pile-setup-' + ( i + 1 ) + '" colspan="1" ' + hid + '></td>';
            $('#pile-setup-tr').append(clone);
        }
    }
}

//#endregion 山積み表描画

/**
 * クッキーの情報を取得
 */
function PileGetCookie()
{
    //グラフのチェックボックスの状態を取得して反映する
    var isTTCheck = Cookies.get("pile-tt-check");
    if (isTTCheck == "true") 
    {
        $("#pile-tt-checkbox").prop("checked", true);
        //保持のしなおし
        Cookies.set("pile-tt-check", "true", { expires: 365 });
    }

    var isTTCheck = Cookies.get("pile-pitch-check");
    if (isTTCheck == "true") 
    {
        $("#pile-pitch-checkbox").prop("checked", true);
        //保持のしなおし
        Cookies.set("pile-pitch-check", "true", { expires: 365 });
    }

    var isTTCheck = Cookies.get("pile-weight-check");
    if (isTTCheck == "true") 
    {
        $("#pile-weight-checkbox").prop("checked", true);
        //保持のしなおし
        Cookies.set("pile-weight-check", "true", { expires: 365 });
    }

    var isTTCheck = Cookies.get("pile-single-check");
    if (isTTCheck == "true") 
    {
        $("#pile-single-checkbox").prop("checked", true);
        //保持のしなおし
        Cookies.set("pile-single-check", "true", { expires: 365 });
    }
}

/**
 *　棒グラフ表示
 */
function PileDispGraph() 
{
    //グラフエリアをリセットする
    $("#pile-grapg-area").empty();
    var clone = '<svg class="pile-graph" id="psGraph"></svg>';
    $("#pile-grapg-area").append(clone);

    //ネックMCTのインデックス
    maxIndex = Number($('#pile-process-top').val());

    //グラフ最初のインデックス
    graphStart = Number($('#pile-before-graph').val());

    //棒グラフデータ
    let psdata = [];
    for(var i = Number($("#pile-before-graph").val()); i < $("#pile-before-graph").val() + Number($("#pile-graph-disp-num").val()); i++)
    {
        if(i == maxIndex)
        {
            var ht = Number($("#pile-ht-val").text())
            var mt = Number($("#pile-mt-val").text())
            var data = PileCreateMCTData(ht, mt);
        }
        else
        {
            var idName = "#pile-disp-option-select-" + i;
            //表示設定 0:実績 1:標準
            var isPlan = Number($(idName).val());

            
            if(isPlan == 1)
            {
                //標準値参照
                //CT最大
                idName = "#pile-plan-ct-max-" + i;
                var ctMax = Number($(idName).text());
                //CT平均
                idName = "#pile-plan-ct-average-" + i;
                var ctAverage = Number($(idName).text());
                //CT最小
                idName = "#pile-plan-ct-min-" + i;
                var ctMin = Number($(idName).text());

                //標準
                var isResult = false;
            }
            else
            {
                //実績参照
                //CT最大
                idName = "#pile-actual-ct-max-" + i;
                var ctMax = Number($(idName).text());
                //CT平均
                idName = "#pile-actual-ct-average-" + i;
                var ctAverage = Number($(idName).text());
                //CT最小
                idName = "#pile-actual-ct-min-" + i;
                var ctMin = Number($(idName).text());

                //実績
                var isResult = true;
            }

            //付帯
            idName = "#pile-ancillary-" + i;
            var ancillary = Number($(idName).text());
            //段取り
            idName = "#pile-setup-" + i;
            var setup = Number($(idName).text());

            //背景色が変わっていればネック工程
            idName = "#pile-process-" + i;
            var isNeck = $(idName).hasClass("pile-neck-color");

            var data = PileCreateCTData(ctMax, ctAverage, ctMin, ancillary, setup, isResult, isNeck);
        }
        psdata.push(data);
    }

    let psgraph = new PsGraph('#psGraph');

    //固定
    psgraph.initializeAxis("CT【秒】", 10, 40, 1, 10, 8, 15, 12);
    
    //固定
    psgraph.setGraphArea(460, 260);
    
    //プロット幅を山積み表の数値部に合わせる
    psgraph.setPlotWidth($(".pile-tail-header").width());
    psgraph.setData(psdata);
    
    //縦最大値
    psgraph.setMax(parseInt($("#pile-graph-max-val").val()));
    
    //縦最小値
    psgraph.setMin(parseInt($("#pile-graph-min-val").val()));

    //グラフの本数
    if(maxIndex < Number($("#pile-graph-disp-num").val()))
    {
        psgraph.setArea(0, maxIndex);
    }
    else
    {
        psgraph.setArea(0, Number($("#pile-graph-disp-num").val()));
    }
    
    psgraph.update(graphStart);


    // 指標セット
    //T.T
    if($("#pile-tt-checkbox").prop("checked") && Number($("#pile-tt").text()) != 0)
    {
        psgraph.setIndexLine(3, Number($("#pile-tt").text()), 0);
    }
    //出来高ピッチ
    if($("#pile-pitch-checkbox").prop("checked") && Number($("#pile-pitch").text()) != 0)
    {
        psgraph.setIndexLine(0, Number($("#pile-pitch").text()), 0);
    }
    //ネックCT加重
    if($("#pile-weight-checkbox").prop("checked") && Number($("#pile-neck-weight").text()) != 0)
    {
        psgraph.setIndexLine(1, Number($("#pile-neck-weight").text()), 0);
    }
    //ネックCT個別
    if($("#pile-single-checkbox").prop("checked") && Number($("#pile-neck-single").text()) != 0)
    {
        psgraph.setIndexLine(2, Number($("#pile-neck-single").text()), 0);
    }
}

/**
 * 山積み表CTグラフデータ作成
 * @param {number} max CT最大
 * @param {number} average CT平均
 * @param {number} min CT最小
 * @param {number} incidental 付帯
 * @param {number} setup 段取り
 * @param {boolean} isResult 実績フラグ 1:true
 * @param {boolean} isNeck ネック工程フラグ 1:true
 * @returns 
 */
function PileCreateCTData(max, average, min, incidental, setup, isResult, isNeck)
{
    var data = {};

    data["CTMaxActive"] = max;
    data["CTAvgActive"] = average;
    data["CTMinActive"] = min;
    data["Incidental"] = min + setup;
    data["Setup"] = min + setup + incidental;
    if(isResult)
    {
        data["GraphType"] = 0;
    }
    else
    {
        data["GraphType"] = 1;
    }
    
    if (isNeck) 
    {
        data["Neck"] = 1;
    }
    else 
    {
        data["Neck"] = 0;
    }


    return data;
}

/**
 * 山積み表ネックMCTグラフデータ作成
 * @param {number} ht HT
 * @param {number} mt MT
 * @returns 
 */
function PileCreateMCTData(ht, mt)
{
    var data = {};

    data["NeckHT"] = ht+mt;
    data["NeckMT"] = mt;
    data["GraphType"] = 2;
    data["Neck"] = 0;

    return data;
}

/**
 * 山積み表の列表示切替
 * @param {number} index 行インデックス
 * @param {boolean} isShow 表示フラグ
 */
function PileChangeHideTable(index, isShow)
{
    if(isShow)
    {
        $("#pile-process-" + index, parent.document).show();
        $("#pile-worker-" + index, parent.document).show();
        $("#pile-disp-option-" + index, parent.document).show();
        $("#pile-ct-max-" + index, parent.document).show();
        $("#pile-ct-average-" + index, parent.document).show();
        $("#pile-ct-min-" + index, parent.document).show();
        $("#pile-ct-scattering-" + index, parent.document).show();
        $("#pile-ct-max-limit-" + index, parent.document).show();
        $("#pile-ct-min-limit-" + index, parent.document).show();
        $("#pile-ancillary-" + index, parent.document).show();
        $("#pile-setup-" + index, parent.document).show();
    }
    else
    {
        $("#pile-process-" + index, parent.document).hide();
        $("#pile-worker-" + index, parent.document).hide();
        $("#pile-disp-option-" + index, parent.document).hide();
        $("#pile-ct-max-" + index, parent.document).hide();
        $("#pile-ct-average-" + index, parent.document).hide();
        $("#pile-ct-min-" + index, parent.document).hide();
        $("#pile-ct-scattering-" + index, parent.document).hide();
        $("#pile-ct-max-limit-" + index, parent.document).hide();
        $("#pile-ct-min-limit-" + index, parent.document).hide();
        $("#pile-ancillary-" + index, parent.document).hide();
        $("#pile-setup-" + index, parent.document).hide();
    }

    //表示数
    dispNum = Number($("#pile-graph-disp-num").val());
    if(dispNum > Number($('#pile-process-top').val()))
    {
        dispNum = Number($('#pile-process-top').val());
    }
    var leftCol = parseInt(dispNum / 2);
    var rightCol = dispNum % 2;

    $("#pile-actual-sum-ct").attr("colSpan", leftCol + rightCol);
    $("#pile-actual-com-efficiency").attr("colSpan", leftCol + rightCol);
    $("#pile-actual-mm").attr("colSpan", leftCol + rightCol);
    $("#pile-plan-sum-ct").attr("colSpan", leftCol);
    $("#pile-plan-com-efficiency").attr("colSpan", leftCol);
    $("#pile-plan-mm").attr("colSpan", leftCol);
}

/**
 * 山積み表更新処理
 * @param {List} data 工程データリスト
 */
function PileUpdateUnderTable(data)
{
    //工程数＋ネックMCT分回して山積み表を作成する
    var maxCTVal = 0;
    var resultSumCT = 0;
    var planSumCT = 0;
    var resultMaximumMinCT = 0;
    var planMaximumMinCT = 0;
    var neckMinCT = 0;
    var isResult = false;

    //表示件数
    var processNum = Number($("#pile-graph-disp-num").val());
    if(processNum > data.length)
    {
        processNum = data.length;
    }

    for (let i = 0; i < data.length; i++) 
    {
        //最後はネックMCT
        if(i == data.length - 1)
        {
            $("#pile-mct-val").text(data[i].MachineCycleTime);
            $("#pile-ht-val").text(data[i].HumanTime);
            $("#pile-mt-val").text(data[i].MachineTime);
            break;
        }

        //工程名
        var idName = "#pile-process-" + (i+1);
        $(idName).text(data[i].ProcessName);
        $(idName).val(data[i].ProcessIdx);

        //作業者 ※最初はALL固定
        idName = "#pile-worker-button-" + (i+1);
        var json = JSON.stringify({ Worker: Array("ALL") });
        $(idName).text("ALL");
        $(idName).val(json);

        //表示設定
        idName = "#pile-disp-option-" + (i+1);
        //コンボボックスをつける
        var select = '<select class="pile-disp-option" id="pile-disp-option-select-' + ( i + 1 ) + '"><option value="0">実績</option><option value="1">標準</option></select>';
        $(idName).append(select);

        idName = "#pile-disp-option-select-" + (i+1);
        if(data[i].ResultCTMax != "0.0")
        {
            isResult = true;
            //実績
            $(idName).val(0);

            //山積み表最大値の1.25倍をグラフ最大の初期値とする
            var resultCTMax = Math.ceil(data[i].ResultCTMax * 1.25);
            if(maxCTVal < resultCTMax)
            {
                maxCTVal = resultCTMax;
            }

            if(resultMaximumMinCT < data[i].ResultCTMin)
            {
                resultMaximumMinCT = Number(data[i].ResultCTMin);
            }            
        }
        else
        {
            //標準
            $(idName).val(1);
            $(idName).prop('disabled', true);

            //山積み表最大値の1.25倍をグラフ最大の初期値とする
            var planCTMax = Math.ceil(data[i].CycleTimeMax * 1.25);
            if(maxCTVal < planCTMax)
            {
                maxCTVal = planCTMax;
            }

            if(resultMaximumMinCT < data[i].CycleTimeMin)
            {
                resultMaximumMinCT = Number(data[i].CycleTimeMin);
            }  
        }

        //実績表示の場合は実績値、標準表示の場合は標準値をΣCT実績値に加算する
        if(Number($(idName).val()) == 0)
        {
            resultSumCT += Number(data[i].ResultCTMin);
        }
        else
        {
            resultSumCT += Number(data[i].CycleTimeMin);
        }

        //山積み表値
        //CT最大実績
        idName = "#pile-actual-ct-max-" + (i+1);
        $(idName).text(data[i].ResultCTMax);
        //CT平均実績
        idName = "#pile-actual-ct-average-" + (i+1);
        $(idName).text(data[i].ResultCTAvg);
        //CT最小実績
        idName = "#pile-actual-ct-min-" + (i+1);
        $(idName).text(data[i].ResultCTMin);
        //CTバラつき実績
        idName = "#pile-actual-ct-scattering-" + (i+1);
        var resultScattering = data[i].ResultCTAvg - data[i].ResultCTMin;
        $(idName).text(resultScattering.toFixed(1));
        
        //CT最大標準
        idName = "#pile-plan-ct-max-" + (i+1);
        $(idName).text(data[i].CycleTimeMax);
        //CT平均標準
        idName = "#pile-plan-ct-average-" + (i+1);
        $(idName).text(data[i].CycleTimeAverage);
        //CT最小標準
        idName = "#pile-plan-ct-min-" + (i+1);
        $(idName).text(data[i].CycleTimeMin);
        //CTバラつき標準
        idName = "#pile-plan-ct-scattering-" + (i+1);
        $(idName).text(data[i].CycleTimeDispersion);

        //CT上限カット
        idName = "#pile-ct-max-limit-" + (i+1);
        $(idName).text(data[i].CycleTimeUpper);
        //CT最小カット
        idName = "#pile-ct-min-limit-" + (i+1);
        $(idName).text(data[i].CycleTimeLower);

        //付帯
        idName = "#pile-ancillary-" + (i+1);
        $(idName).text(data[i].Ancillary);
        //段取り
        idName = "#pile-setup-" + (i+1);
        $(idName).text(data[i].Setup);

        //ΣCT標準値
        planSumCT += Number(data[i].CycleTimeMin);

        //最も大きいCT最小実績値
        if(neckMinCT < Number(data[i].ResultCTMin))
        {
            neckMinCT = Number(data[i].ResultCTMin);
        }
        //最も大きいCT最小標準値
        if(planMaximumMinCT < Number(data[i].CycleTimeMin))
        {
            planMaximumMinCT = Number(data[i].CycleTimeMin);
        }
    }

    //データが無いなら他も初期化時点のままとする
    if(data.length == 0)
    {
        return;
    }

    //グラフ最大最小
    $("#pile-graph-max-val").val(maxCTVal);
    $("#pile-graph-min-val").val(0);

    //ΣCT
    $("#pile-actual-sum-ct").text(resultSumCT.toFixed(1));
    $("#pile-plan-sum-ct").text(planSumCT.toFixed(1));

    //編成効率
    //T.T ≧ 工程で最も大きいCT最小値なら
    if(Number($("#pile-tt").text()) >= resultMaximumMinCT)
    {
        // (ΣCT実績値 ÷ (T.T × 工程数)) × 100 ※工程数はMCT列を引いたデータ数
        var resultEfficiency = (resultSumCT / (Number($("#pile-tt").text()) * (data.length - 1))) * 100;
        $("#pile-actual-com-efficiency").text(PileDevaluationSecond(resultEfficiency).toFixed(1));
    }
    else
    {
        // (ΣCT実績値 ÷ (工程で最も大きいCT最小値 × 工程数)) × 100 ※工程数はMCT列を引いたデータ数
        var resultEfficiency = (resultSumCT / (resultMaximumMinCT * (data.length - 1))) * 100;
        $("#pile-actual-com-efficiency").text(PileDevaluationSecond(resultEfficiency).toFixed(1));
    }

    //T.T ≧ 工程で最も大きいCT最小値なら
    if(Number($("#pile-tt").text()) >= planMaximumMinCT)
    {
        // (ΣCT標準値 ÷ (T.T × 工程数)) × 100 ※工程数はMCT列を引いたデータ数
        var planEfficiency = (planSumCT / (Number($("#pile-tt").text()) * (data.length - 1))) * 100;
        $("#pile-plan-com-efficiency").text(PileDevaluationSecond(planEfficiency).toFixed(1));
    }
    else
    {
        // (ΣCT標準値 ÷ (工程で最も大きいCT最小値 × 工程数)) × 100 ※工程数はMCT列を引いたデータ数
        var planEfficiency = (planSumCT / (planMaximumMinCT * (data.length - 1))) * 100;
        $("#pile-plan-com-efficiency").text(PileDevaluationSecond(planEfficiency).toFixed(1));
    }

    //MM比
    var resultMM = PileDevaluationSecond(resultSumCT / Number($("#pile-mct-val").text()))
    $("#pile-actual-mm").text(resultMM.toFixed(1));
    var planMM = PileDevaluationSecond(planSumCT / Number($("#pile-mct-val").text()))
    $("#pile-plan-mm").text(planMM.toFixed(1));

    //実績があればネック工程に色を付ける
    if(isResult)
    {
        var isFirstNeck = true;
        $('.pile-actual-ct-min').each(function() 
        {
            if(Number($(this).text()) == neckMinCT)
            {
                var list = $(this).prop("id").split("-");
                var index = Number(list[list.length - 1]);
                var idName = "#pile-process-" + index;
                $(idName).addClass("pile-neck-color");

                //ネック工程が初期表示に収まるようにする
                if(isFirstNeck)
                {
                    //インデックスが1ではなく、かつ工程数が表示列数より多い場合はネック工程が左端に来るように移動する
                    if(index != 1 && data.length > processNum)
                    {
                        var rightEndNum = processNum - 1;
                        if(index + rightEndNum > data.length)
                        {
                            index = index + rightEndNum - data.length;
                        }

                        //テーブル表示情報のセット
                        $('#pile-before-graph').val(index);
                        $('#pile-after-graph').val(index + rightEndNum);

                        //テーブルの表示非表示を切り替える
                        for(var i = 1; i < data.length + 1; i++)
                        {
                            if(index <= i && i <= index + rightEndNum)
                            {
                                PileChangeHideTable(i, true);
                            }
                            else
                            {
                                PileChangeHideTable(i, false);
                            }
                        }
                    }

                    isFirstNeck = false;
                }
            }
        });
    }
}

/**
 * 山積み表更新処理(作業者更新時)
 * @param {List} data 工程データリスト
 */
function PileUpdateUnderTableWorkerUpdate(data)
{
    //工程数分回して山積み表を作成する
    var resultSumCT = 0;
    var planSumCT = 0;
    var resultMaximumMinCT = 0;
    var planMaximumMinCT = 0;
    for (let i = 0; i < data.length; i++) 
    {
        //工程、作業者、表示設定は変えなくていい

        idName = "#pile-disp-option-select-" + (i+1);
        //実績がある工程で、元々実績が無かった工程だった場合は表示設定を操作可能にする
        if(data[i].ResultCTMax != "0.0" && $(idName).is(":enabled") == false)
        {
            $(idName).prop('disabled', false);
        }

        //実績表示の場合は実績値、標準表示の場合は標準値をΣCT実績値に加算する
        if(Number($(idName).val()) == 0)
        {
            resultSumCT += Number(data[i].ResultCTMin);
        }
        else
        {
            resultSumCT += Number(data[i].CycleTimeMin);
        }

        //山積み表値
        //CT最大実績
        idName = "#pile-actual-ct-max-" + (i+1);
        $(idName).text(data[i].ResultCTMax);
        //CT平均実績
        idName = "#pile-actual-ct-average-" + (i+1);
        $(idName).text(data[i].ResultCTAvg);
        //CT最小実績
        idName = "#pile-actual-ct-min-" + (i+1);
        $(idName).text(data[i].ResultCTMin);
        //CTバラつき実績
        idName = "#pile-actual-ct-scattering-" + (i+1);
        var resultScattering = data[i].ResultCTAvg - data[i].ResultCTMin;
        $(idName).text(resultScattering.toFixed(1));
        
        //CT最大標準
        idName = "#pile-plan-ct-max-" + (i+1);
        $(idName).text(data[i].CycleTimeMax);
        //CT平均標準
        idName = "#pile-plan-ct-average-" + (i+1);
        $(idName).text(data[i].CycleTimeAverage);
        //CT最小標準
        idName = "#pile-plan-ct-min-" + (i+1);
        $(idName).text(data[i].CycleTimeMin);
        //CTバラつき標準
        idName = "#pile-plan-ct-scattering-" + (i+1);
        $(idName).text(data[i].CycleTimeDispersion);

        //CT上限カット
        idName = "#pile-ct-max-limit-" + (i+1);
        $(idName).text(data[i].CycleTimeUpper);
        //CT最小カット
        idName = "#pile-ct-min-limit-" + (i+1);
        $(idName).text(data[i].CycleTimeLower);

        //付帯
        idName = "#pile-ancillary-" + (i+1);
        $(idName).text(data[i].Ancillary);
        //段取り
        idName = "#pile-setup-" + (i+1);
        $(idName).text(data[i].Setup);

        //ΣCT標準値
        planSumCT += Number(data[i].CycleTimeMin);

        //最も大きいCT最小実績値
        if(resultMaximumMinCT < Number(data[i].ResultCTMin))
        {
            resultMaximumMinCT = Number(data[i].ResultCTMin);
        }
        //最も大きいCT最小標準値
        if(planMaximumMinCT < Number(data[i].CycleTimeMin))
        {
            planMaximumMinCT = Number(data[i].CycleTimeMin);
        }
    }

    //ΣCT
    $("#pile-actual-sum-ct").text(resultSumCT.toFixed(1));
    $("#pile-plan-sum-ct").text(planSumCT.toFixed(1));

    //編成効率
    //T.T ≧ 工程で最も大きいCT最小値なら
    if(Number($("#pile-tt").text()) >= resultMaximumMinCT)
    {
        // (ΣCT実績値 ÷ (T.T × 工程数)) × 100
        var resultEfficiency = (resultSumCT / (Number($("#pile-tt").text()) * data.length)) * 100;
        $("#pile-actual-com-efficiency").text(PileDevaluationSecond(resultEfficiency).toFixed(1));
    }
    else
    {
        // (ΣCT実績値 ÷ (工程で最も大きいCT最小値 × 工程数)) × 100
        var resultEfficiency = (resultSumCT / (resultMaximumMinCT * data.length)) * 100;
        $("#pile-actual-com-efficiency").text(PileDevaluationSecond(resultEfficiency).toFixed(1));
    }

    //T.T ≧ 工程で最も大きいCT最小値なら
    if(Number($("#pile-tt").text()) >= planMaximumMinCT)
    {
        // (ΣCT標準値 ÷ (T.T × 工程数)) × 100
        var planEfficiency = (planSumCT / (Number($("#pile-tt").text()) * data.length)) * 100;
        $("#pile-plan-com-efficiency").text(PileDevaluationSecond(planEfficiency).toFixed(1));
    }
    else
    {
        // (ΣCT標準値 ÷ (工程で最も大きいCT最小値 × 工程数)) × 100
        var planEfficiency = (planSumCT / (planMaximumMinCT * data.length)) * 100;
        $("#pile-plan-com-efficiency").text(PileDevaluationSecond(planEfficiency).toFixed(1));
    }

    //MM比
    var resultMM = PileDevaluationSecond(resultSumCT / Number($("#pile-mct-val").text()))
    $("#pile-actual-mm").text(resultMM.toFixed(1));
    var planMM = PileDevaluationSecond(planSumCT / Number($("#pile-mct-val").text()))
    $("#pile-plan-mm").text(planMM.toFixed(1));

    //ネック工程の色付けを全部解除する
    $('.pile-neck-color').each(function() 
    {
        $(this).removeClass("pile-neck-color");
    });

    //実績があればネック工程に色を付ける
    if(resultMaximumMinCT != 0)
    {
        var isFirstNeck = true;
        $('.pile-actual-ct-min').each(function() 
        {
            if(Number($(this).text()) == resultMaximumMinCT)
            {
                var list = $(this).prop("id").split("-");
                var index = Number(list[list.length - 1]);
                var idName = "#pile-process-" + index;
                $(idName).addClass("pile-neck-color");
            }
        });
    }
    
}

/**
 * 推奨戦略チェック更新処理
 */
function PileUpdateStrategy()
{
    //可動率判定
    if($("#pile-table-occupancy-rate").text() != "" && $("#pile-occupancy-rate").text() != "" && Number($("#pile-table-occupancy-rate").text()) < Number($("#pile-occupancy-rate").text()))
    {
        $("#pile-occupancy-rate-check").text("✓");
    }
    else
    {
        $("#pile-occupancy-rate-check").text("―");
    }
    
    //正味改善
    if($("#pile-tt").text() != "" && $("#pile-neck-single").text() && Number($("#pile-tt").text()) < Number($("#pile-neck-single").text()))
    {
        $("#pile-improvement-check").text("✓");
    }
    else
    {
        $("#pile-improvement-check").text("―");
    }
    
    //リバランス
    if($("#pile-actual-com-efficiency").text() != "" && $("#pile-composition-efficiency").text() != "" && Number($("#pile-actual-com-efficiency").text()) < Number($("#pile-composition-efficiency").text()))
    {
        $("#pile-composition-efficiency-check").text("✓");
    }
    else
    {
        $("#pile-composition-efficiency-check").text("―");
    }
}

/**
 * 小数点第二位切り下げ
 * @param {number} number 数値
 * @returns 小数点第二位で切り下げた数値
 */
function PileDevaluationSecond(number)
{
    return Math.floor(Number(number) * 10) / 10;
}

/**
 * 直を文字列から数字に変換して返す
 * @returns string 直を数字に戻したCSV
 */
function PileGetShiftValue()
{
    shift = $('#pile-shift').val().split(",");
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

//#endregion 関数