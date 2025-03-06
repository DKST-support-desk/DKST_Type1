//#region イベント処理
/**
 * イベント処理
 */
$(function()
{
    /**
     * 日付クリックイベント
     */
    $("body").on("click", ".calendar-td", function() 
    { 
        try
        {
            //日付のないセルのクリックは無視する
            if($(this).val() == "")
            {
                return;
            }

            //ラジオボタンがついている側のテキストを変更する
            if($("#calendar-start-radio").prop("checked"))
            {
                //開始日付変更
                $("#calendar-start-date").val(moment($(this).val()).format("YYYY-MM-DD"));
            }
            else
            {
                //終了日付変更
                $("#calendar-end-date").val(moment($(this).val()).format("YYYY-MM-DD"));
            }

            //今選択されているセルの色を戻す
            var onCell = $(".calendar-radio-on-color");
            onCell.removeClass("calendar-radio-on-color");

            //クリックしたセルの色を変える
            $(this).addClass("calendar-radio-on-color");

            //ラジオボタンがついている側のテキストを変更する
            if($("#calendar-start-radio").prop("checked") && $('#calendar-start-time').is(':hidden'))
            {
                //終了ラジオボタンにチェックを入れる
                $("input[name='calendar-select']").val(["end"]);
                CalendarRadioChange();
            }
        } 
        catch(e) 
        {
            alert("日付クリックイベントエラー:" + e.message );
        }
    });

    /**
     * 開始ラジオボタンチェックイベント
     */
    $("body").on("change", "#calendar-start-radio", function() 
    { 
        try
        {
            CalendarRadioChange();
        } 
        catch(e) 
        {
            alert("開始ラジオボタンチェックイベントエラー:" + e.message );
        }
    });

    /**
     * 終了ラジオボタンチェックイベント
     */
    $("body").on("change", "#calendar-end-radio", function() 
    { 
        try
            {
            CalendarRadioChange();
        } 
        catch(e) 
        {
            alert("終了ラジオボタンチェックイベントエラー:" + e.message );
        }
    });

    /**
     * 開始時刻入力完了イベント
     */
    $("body").on("focusout", "#calendar-start-time", function() 
    { 
        try
        {
            //終了ラジオボタンにチェックを入れる
            $("input[name='calendar-select']").val(["end"]);

            //ラジオボタン変更時処理
            CalendarRadioChange();
        } 
        catch(e) 
        {
            alert("開始時刻入力完了イベントエラー:" + e.message );
        }
    });

    /**
     * 開始時刻キーダウンイベント
     */
    $("body").on("keydown", "#calendar-start-time", function(e) 
    {
        try
        {
            var keyCode = e.keyCode;

            //backspaceが押下されていたら無視する
            if(keyCode == 8)
            {
                return false;
            }
        } 
        catch(e) 
        {
            alert("キーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * 終了時刻キーダウンイベント
     */
    $("body").on("keydown", "#calendar-end-time", function(e) 
    {
        try
        {
            var keyCode = e.keyCode;

            //backspaceが押下されていたら無視する
            if(keyCode == 8)
            {
                return false;
            }
        } 
        catch(e) 
        {
            alert("キーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * OKボタンクリックイベント
     */
    $("body").on("click", "#calendar-ok-button", function()
    {
        try
        {
            //入力チェック
            var startDateTime = "";
            var endDateTime = "";
            var returnValue = "";

            if($("#calendar-start-time").is(":hidden"))
            {
                startDateTime = moment($("#calendar-start-date").val());
                endDateTime = moment($("#calendar-end-date").val());
                returnValue = startDateTime.format("YYYY/MM/DD") + " - " + endDateTime.format("YYYY/MM/DD");
            }
            else
            {
                startDateTime = moment($("#calendar-start-date").val() + " " + $("#calendar-start-time").val());
                endDateTime = moment($("#calendar-end-date").val() + " " + $("#calendar-end-time").val());
                returnValue = startDateTime.format("YYYY/MM/DD HH:mm:ss") + " - " + endDateTime.format("YYYY/MM/DD HH:mm:ss");
            }

            if(startDateTime > endDateTime)
            {
                alert("開始は終了以前の日時を選択してください。");
                return;
            }
            
            //テンプレート生成時に親ページのクラス名 命名規則にのっとった判別項を入れるので、それを取得する
            var pageName = $("#calendar-template-body").val();
            //期間に値を入れる
            $("#" + pageName + "-span", parent.document).val(returnValue);
            //ドロップダウンを閉じる
            UIkit.dropdown($("#" + pageName + "-dropdown", parent.document)).hide(0);
            //カレンダー部分を消す
            $("#" + pageName + "-calendar-div").empty();

            //元画面のほうで更新処理を実行する
            switch(pageName)
            {
                case "pile":
                    PileAllUpdate();
                    break;

                case "cycle":
                    CycleAllUpdate();
                    break;

                case "video":
                    VideoAllUpdate();
                    break;
            }
        } 
        catch(e) 
        {
            alert("OKボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * キャンセルボタンクリックイベント
     */
     $("body").on("click", "#calendar-cancel-button", function()
     {
        try
        {
            //テンプレート生成時に親ページのクラス名 命名規則にのっとった判別項を入れるので、それを取得する
            var pageName = $("#calendar-template-body").val();
            //ドロップダウンを閉じる
            UIkit.dropdown($("#" + pageName + "-dropdown", parent.document)).hide(0);
            //カレンダー部分を消す
            $("#" + pageName + "-calendar-div").empty();
        } 
        catch(e) 
        {
            alert("キャンセルボタンクリックイベントエラー:" + e.message );
        }
     });
});

//#endregion イベント処理


//#region 関数

/**
 * カレンダー初期表示
 * @param {moment} startDate 開始日時
 * @param {moment} endDate 終了日時
 * @param {moment} startSpan データ範囲の開始日
 * @param {moment} endSpan データ範囲の終了日
 * @param {moment} nowMonth 当月
 * @param {boolean} timeFlag 時刻表示フラグ
 * @param {string} pageName 親ページ識別文字
 * @param {json} allSpan 計画のある稼働日
 */
function CalendarCreate(startDate, endDate, startSpan, endSpan, nowMonth, timeFlag, pageName, allSpan)
{
    //テンプレート生成時に親ページのクラス名 命名規則にのっとった判別項を入れておく
    $("#calendar-template-body").val(pageName);

    //月の数を取得する
    yearCount = endSpan.year() - startSpan.year();
    monthCount = endSpan.month() - startSpan.month() + 1 + yearCount * 12;

    var spanList = Array();
    if(allSpan != null)
    {
        for(var i = 0; i < allSpan.length; i++)
        {
            spanList.push(moment(allSpan[i].OperationDate).format("YYYY-MM-DD"));
        }
    }
    

    //スクロールバーの初期位置
    var scrollTop = 0;

    if ('content' in document.createElement('template')) 
    {
        //ループ内で使用する月
        calMonth = moment(startSpan.year() + "-" + (startSpan.month() + 1) + "-" + startSpan.date());

        var scrollCount = 0

        for(var count = 0; count < monthCount; count++)
        {
            var year = calMonth.year();
            var month = calMonth.month() + 1; //月は取得の時のみ1月が0で取得される

            //当月ならスクロール位置を記録しておく
            if(nowMonth.year() == year && (nowMonth.month() + 1) == month)
            {
                scrollTop = scrollCount;
            }

            //テンプレート表示部に追加する
            var clone = $($('#calendar-template-body').html());

            //年月タイトル部
            $('#calendar-year', clone).text(year + "年");
            $('#calendar-month', clone).text(month + "月");

            //月初めの曜日を取得
            calStartDate = moment(year + "-" + month + "-" + 1);
            startDay = calStartDate.day();

            //月末の日を取得
            calEndDate = moment(calStartDate.endOf('month').format("YYYY-MM-DD"));
            endDayCount = calEndDate.date();

            //カレンダー部生成
            for(var i = 0; i < endDayCount; i++)
            {
                $("td", clone).eq(startDay + i).text(i + 1);
                $("td", clone).eq(startDay + i).val(moment(year + "-" + month + "-" + (i + 1)).format("YYYY-MM-DD"));
            }

            //1カレンダー分の縦幅
            scrollCount += 181;

            //第六週が不要なら非表示にする
            if((startDay + i) < 36)
            {
                for(var i = 0; i < 7; i++)
                {
                    $("td", clone).eq(35 + i).hide();
                }
            }
            else
            {
                //6行目のセルがある場合はその分縦幅を足す
                scrollCount += 27;
            }

            //カレンダー部の追加
            $('#calendar-bodys').append(clone);

            //次の月
            calMonth.add(1, "month");
        }
    }

    //開始終了日付
    $("#calendar-start-date").val(startDate.format("YYYY-MM-DD"));
    $("#calendar-end-date").val(endDate.format("YYYY-MM-DD"));

    var startVal = $("#calendar-start-date").val();
    var endVal = $("#calendar-end-date").val();

    //カレンダーのセルを全て回して色を変更する
    $('.calendar-td').each(function() 
    {
        //全セルに基本色をセット
        $(this).addClass('calendar-no-target-color');
        var eachVal = $(this).val();
        if(spanList.includes(eachVal))
        {
            $(this).addClass('calendar-plan-exists-color');
        }
        
        //終了日にはラジオ未選択セル色をセット
        if(eachVal == endVal)
        {
            $(this).addClass('calendar-radio-off-color');
        }

        //開始日にはラジオ選択セル色をセット
        if(eachVal == startVal)
        {
            $(this).addClass('calendar-radio-on-color');
        }
    });

    //開始終了時刻
    if(timeFlag)
    {
        $("#calendar-start-time").val(startDate.format("HH:mm:ss"));
        $("#calendar-end-time").val(endDate.format("HH:mm:ss"));
    }
    else
    {
        $("#calendar-start-time").hide();
        $("#calendar-end-time").hide();
    }

    //開始ラジオボタンにチェックを入れる
    $("input[name='calendar-select']").val(["start"]);

    //スクロール移動
    $(".calendar-area").scrollTop(scrollTop);
}

/**
 * ラジオボタン変更時処理
 */
function CalendarRadioChange()
{
    var onCell = $(".calendar-radio-on-color");
    var offCell = $(".calendar-radio-off-color");

    //日付選択が同一の場合は何もしない
    if(onCell.val() == offCell.val())
    {
        return;
    }

    //ラジオ選択セルをラジオ未選択セルの色にする
    onCell.removeClass("calendar-radio-on-color");
    onCell.addClass("calendar-radio-off-color");

    //ラジオ未選択セルをラジオ選択セルの色にする
    offCell.removeClass("calendar-radio-off-color");
    offCell.addClass("calendar-radio-on-color");
}

//#endregion 関数