//#region イベント処理
/**
 * イベント処理
 */
$(function()
{
    /**
     * 動画再生速度
     */
    var videoPlaySpeed = 1.0;
//#region カレンダーイベント

    /**
     * カレンダー表示イベント
     */
    UIkit.util.on('#video-dropdown', 'show', function () 
    {
        try
        {
            if($('#video-span').val() != "-")
            {
                //入力期間取得
                spanArray = $('#video-span').val().split(' - ');
                startDate = moment(spanArray[0]);
                endDate = moment(spanArray[1]);
            }
            else
            {
                startDate = moment();
                endDate = moment();
            }
            
            nowMonth = moment();

            var span = VideoGetOperatingPlan();
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
            $('#video-calendar-div').append(clone);

            //カレンダー初期表示
            CalendarCreate(startDate, endDate, startSpan, endSpan, nowMonth, true, "video", allSpan);
        } 
        catch(e) 
        {
            alert("カレンダー表示イベントエラー:" + e.message );
        }
    });

    /**
     * カレンダー非表示イベント
     */
    UIkit.util.on('#video-dropdown', 'hide', function () 
    {
        //カレンダー部分を消す
        $("#video-calendar-div").empty();
    });

//#endregion カレンダーイベント

    /**
     * 編成変更イベント
     */
    $("#video-camera").change(function () 
    {
        try
        {
            //画面更新
            VideoPageUpdate();
        } 
        catch(e) 
        {
            alert("編成変更イベントエラー:" + e.message );
        }
    });

    /**
     * 異常のみONOFFボタンクリックイベント
     */
    $("#video-on-off-button").click(function()
    {
        try
        {
            if($("#video-on-off-button").text() == "OFF")
            {
                $("#video-on-off-button").text("ON");
            }
            else
            {
                $("#video-on-off-button").text("OFF");
            }

            //画面更新
            VideoPageUpdate();
        } 
        catch(e) 
        {
            alert("異常のみONOFFボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 動画保存ボタンクリックイベント
     */
    $("#video-movie-download-button").click(function()
    {
        try
        {
            var fileName = VideoCreateZipFile();
            if(fileName != "")
            {
                //タイマーのセット
                setTimeout(VideoCheckZipFile, 1000, fileName);
            }
        } 
        catch(e) 
        {
            alert("動画保存ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
      * サイズ変更アイコンクリックイベント
      */
    $("#video-size-change-icon").click(function() 
    { 
        try
        {
            $('#index-frame-1').toggle();
            $('#index-frame-2').toggle();

            $('#video-movie-player').toggleClass("video-movie");
            $('#video-movie-player').toggleClass("video-movie-big");

            //プロット再描画
            CycleDispPlot();

            //山積み表再描画
            PileDispGraph();
        } 
        catch(e) 
        {
            alert("サイズ変更アイコンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 動画読み込み時イベント
     */
    $("#video-movie-player").get(0).addEventListener("canplay", function()
    {
        try
        {
            //動画時間ミリ秒
            var movieTime = $("#video-movie-player").get(0).duration;

            //動画時間 ミリ秒切り捨て
            var movieDispTime = VideoConvertSecondToTime(movieTime);
            $("#video-movie-time_max").text(movieDispTime);

            //再生バー ミリ秒第一位まで
            $("#video-movie-scroll").attr("max", Math.floor((movieTime * 10) / 10));

            //現在再生時間を初期化する
            $("#video-movie-now").text("00:00:00");
        } 
        catch(e) 
        {
            alert("動画読み込み時イベントエラー:" + e.message );
        }
	}, false);

    /**
     * 動画再生中イベント
     */
    $("#video-movie-player").get(0).addEventListener("timeupdate", function()
    {
        try
        {
            //再生時間表示の変更
            VideoChangeMovieNowTime();
        } 
        catch(e) 
        {
            alert("動画再生中イベントエラー:" + e.message );
        }
    }, false);
    
    /**
     * 動画終了イベント
     */
    $("#video-movie-player").get(0).addEventListener("ended", function()
    {
        try
        {
            //動画リストを取得
            var json = JSON.parse($("#video-play-button").val());
            
            //次の動画へ遷移
            if(json.NowMovieIndex == json.MoviePath.length )
            {
                //すでに最後の動画なら停止ボタンを切り替える

                //停止ボタンが表示されていれば
                if($("#video-stop-button").is(':visible'))
                {
                    //停止ボタン非表示
                    $("#video-stop-button").hide();
                    //再生ボタン表示
                    $("#video-play-button").show();
                }
            }
            else
            {
                //次の動画
                json.NowMovieIndex++;
                $("#video-movie-player").get(0).src = json.MoviePath[json.NowMovieIndex - 1];
                $("#video-play-button").val(JSON.stringify(json));

                //動画再生
                VideoPlayMovie(videoPlaySpeed);
            }
        } 
        catch(e) 
        {
            alert("動画終了イベントエラー:" + e.message );
        }
    }, false);
    
    /**
     * 動画クリックイベント
     */
    $("#video-movie-player").click(function()
    {
        try
        {
            //動画が表示されていないなら何もしない
            if($("#video-movie-player").get(0).src == "")
            {
                return;
            }

            //再生ボタンが表示されていれば
            if($("#video-play-button").is(":visible"))
            {
                //動画再生
                VideoPlayMovie(videoPlaySpeed);

                //再生ボタン非表示
                $("#video-play-button").hide();
                //停止ボタン表示
                $("#video-stop-button").show();
            }
            else
            {
                //動画停止
                $("#video-movie-player").get(0).pause();

                //停止ボタン非表示
                $("#video-stop-button").hide();
                //再生ボタン表示
                $("#video-play-button").show();
            }
        } 
        catch(e) 
        {
            alert("動画クリックイベントエラー:" + e.message );
        }
    });

    /**
     * 再生バー変更イベント
     */
    $("#video-movie-scroll").on("input", function()
    {
        try
        {
            //動画時間 ミリ秒切り捨て
            var movieDispTime = VideoConvertSecondToTime($("#video-movie-scroll").val());
            //動画の再生時間変更
            $("#video-movie-player").get(0).currentTime = $("#video-movie-scroll").val();
            //再生時間ラベル更新
            $("#video-movie-time_now").text(movieDispTime);
        } 
        catch(e) 
        {
            alert("再生バー変更イベントエラー:" + e.message );
        }
    });

    /**
     * 再生ボタンクリックイベント
     */
    $("#video-play-button").click(function()
    {
        try
        {
            //動画が表示されていないなら何もしない
            if($("#video-movie-player").get(0).src == "")
            {
                return;
            }

            //動画再生
            VideoPlayMovie(videoPlaySpeed);

            //再生ボタン非表示
            $("#video-play-button").hide();
            //停止ボタン表示
            $("#video-stop-button").show();
        } 
        catch(e) 
        {
            alert("再生ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 停止ボタンクリックイベント
     */
    $("#video-stop-button").click(function()
    {
        try
        {
            //動画が表示されていないなら何もしない
            if($("#video-movie-player").get(0).src == "")
            {
                return;
            }
            
            //動画停止
            $("#video-movie-player").get(0).pause();

            //停止ボタン非表示
            $("#video-stop-button").hide();
            //再生ボタン表示
            $("#video-play-button").show();
        } 
        catch(e) 
        {
            alert("停止ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 音量バー変更イベント
     */
    $("#video-volume-bar").on("input", function()
    {
        try
        {
            $("#video-movie-player").get(0).volume = $("#video-volume-bar").val();

            //音量アイコンの変更
            if($("#video-movie-player").get(0).volume != 0)
            {
                $("#video-volume-off-icon").hide();
                $("#video-volume-icon").show();
            }
            else
            {
                $("#video-volume-icon").hide();
                $("#video-volume-off-icon").show();
            }
        } 
        catch(e) 
        {
            alert("音量バー変更イベントエラー:" + e.message );
        }
    });

    /**
     * コマ戻しボタンクリックイベント
     */
    $("#video-frame-return-button").click(function()
    {
        try
        {
            var video = $("#video-movie-player").get(0);
            //現在再生時間-10秒　※0秒を下回っていたら0秒にする
            var frame = 1 / $("#video-frame-bar").val();
            video.currentTime = Math.max(0, video.currentTime - frame);

            //再生時間表示の変更
            VideoChangeMovieNowTime();
        } 
        catch(e) 
        {
            alert("コマ戻しボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * コマ送りボタンクリックイベント
     */
    $("#video-frame-next-button").click(function()
    {
        try
        {
            var video = $("#video-movie-player").get(0);
            //現在再生時間＋10秒　※最大再生時間を上回っていたら最大再生時間にする
            var frame = 1 / $("#video-frame-bar").val();
            video.currentTime = Math.min(video.duration, video.currentTime + frame);

            //再生時間表示の変更
            VideoChangeMovieNowTime();
        } 
        catch(e) 
        {
            alert("コマ送りボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * コマ送りフレーム数バー変更イベント
     */
    $("#video-frame-bar").on("input", function()
    {
        try
        {
            var frame = $(this).val();
            $("#video-frame-value").text("1/" + frame.toString());
        } 
        catch(e) 
        {
            alert("コマ送りフレーム数バー変更イベントエラー:" + e.message );
        }
    });

    /**
     * 10秒戻るボタンクリックイベント
     */
    $("#video-10sec-return-button").click(function()
    {
        try
        {
            var video = $("#video-movie-player").get(0);
            //現在再生時間-10秒　※0秒を下回っていたら0秒にする
            video.currentTime = Math.max(0, video.currentTime - 10);

            //再生時間表示の変更
            VideoChangeMovieNowTime();
        } 
        catch(e) 
        {
            alert("10秒戻るボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 10秒進むボタンクリックイベント
     */
    $("#video-10sec-next-button").click(function()
    {
        try
        {
            var video = $("#video-movie-player").get(0);
            //現在再生時間＋10秒　※最大再生時間を上回っていたら最大再生時間にする
            video.currentTime = Math.min(video.duration, video.currentTime + 10);

            //再生時間表示の変更
            VideoChangeMovieNowTime();
        } 
        catch(e) 
        {
            alert("10秒進むボタンクリックイベントエラー:" + e.message );
        }
    });



    /**
     * 再生速度の変更
     */
    $("#video-speed-val").on("blur", function()
    {
        try
        {
            var val = Number($(this).val());
            var valMax = Number($(this).attr("max"));
            var valMin = Number($(this).attr("min"));
            if(val >= valMax)
            { 
                $(this).val(valMax.toFixed(1)); 
                //再生時間の設定
                $("#video-movie-player").get(0).playbackRate = $("#video-speed-val").val();
                videoPlaySpeed = $("#video-speed-val").val();
                return;
            }
            if(val < valMin)
            { 
                $(this).val(1.0); 
                //再生時間の設定
                $("#video-movie-player").get(0).playbackRate = $("#video-speed-val").val();
                videoPlaySpeed = $("#video-speed-val").val();
                return;
            }
            if(isNaN(val))
            { 
                $(this).val(1.0); 
                //再生時間の設定
                $("#video-movie-player").get(0).playbackRate = $("#video-speed-val").val();
                videoPlaySpeed = $("#video-speed-val").val();
                return;
            }

            $(this).val(val.toFixed(1));

            //再生時間の設定
            $("#video-movie-player").get(0).playbackRate = $("#video-speed-val").val();
            videoPlaySpeed = $("#video-speed-val").val();
        } 
        catch(e) 
        {
            alert("再生速度変更イベントエラー:" + e.message );
        }
    });

    /**
     * 再生速度テキストボックスキーダウンイベント
     */
    $("#video-speed-val").keydown(function(event)
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
            else if(keyCode == 13)
            {
                //Enterキーが押下されていたら
                $("#video-speed-val").blur();
            }
        } 
        catch(e) 
        {
            alert("再生速度テキストボックスキーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * 前動画ボタンクリックイベント
     */
    $("#video-before-movie-button").click(function()
    {
        try
        {
            //動画リストを取得
            var json = JSON.parse($("#video-play-button").val());
            
            //すでに最初の動画ならファイルの最初の時間に遷移する
            if(json.NowMovieIndex == 1)
            {
                $("#video-movie-player").get(0).currentTime = 0;
            }
            else
            {
                //次の動画
                json.NowMovieIndex--;
                $("#video-movie-player").get(0).src = json.MoviePath[json.NowMovieIndex - 1];
                $("#video-play-button").val(JSON.stringify(json));

                //停止ボタンが表示されていれば再生状態だったということ
                if($("#video-stop-button").is(':visible'))
                {
                    //動画再生
                    VideoPlayMovie(videoPlaySpeed);
                }
            }
        } 
        catch(e) 
        {
            alert("前動画ボタンクリックイベントエラー:" + e.message );
        }
    });

    /**
     * 次動画ボタンクリックイベント
     */
    $("#video-next-movie-button").click(function()
    {
        try
        {
            //動画リストを取得
            var json = JSON.parse($("#video-play-button").val());
            
            //すでに最後の動画ならファイルの最後の時間に遷移する
            if(json.NowMovieIndex == json.MoviePath.length )
            {
                $("#video-movie-player").get(0).currentTime = $("#video-movie-player").get(0).duration;
            }
            else
            {
                //次の動画
                json.NowMovieIndex++;
                $("#video-movie-player").get(0).src = json.MoviePath[json.NowMovieIndex - 1];
                $("#video-play-button").val(JSON.stringify(json));

                //停止ボタンが表示されていれば再生状態だったということ
                if($("#video-stop-button").is(':visible'))
                {
                    //動画再生
                    VideoPlayMovie(videoPlaySpeed);
                }
            }
        } 
        catch(e) 
        {
            alert("次動画クリックイベントエラー:" + e.message );
        }
    });
});
//#endregion イベント処理

//#region ajax処理

/**
 * 稼働予定日取得
 */
function VideoGetOperatingPlan() 
{
    //関数インデックス
    funIndex = 0;
 
    var startSpan = moment();
    var endSpan = moment();
    var allData = null;
    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
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
 * 選択カメラ取得
 */
function VideoGetCamera() 
{
    //関数インデックス
    funIndex = 1;
 
    //クリア
    $("#video-camera").children().remove();

    if($("#video-span").val() == "-")
    {
        $('#video-camera').append($('<option>').html("-").val("-"));
        return;
    }

    //期間を開始日と終了日に分ける
    spanArray = $("#video-span").val().split(" - ");
    startDate = spanArray[0];
    endDate = spanArray[1];

    //工程はサイクルプロット部から取得する
    if($("#cycle-process").val() == "-")
    {
        $('#video-camera').append($('<option>').html("-").val("-"));
        return;
    }
    processIdx = $("#cycle-process").val();

    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "processIdx": processIdx },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        for (let i = 0; i < data.length; i++) 
        {
            $('#video-camera').append($('<option>').html(data[i].CameraName).val(data[i].CameraIdx));
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("選択カメラ取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {
        if ($('#video-camera').children("option").length == 0) 
        {
            $('#video-camera').append($('<option>').html("-").val("-"));
        }
    });
}

/**
 * 動画ファイル取得
 */
function VideoGetMovie() 
{
    //関数インデックス
    funIndex = 2;

    //期間を開始日と終了日に分ける
    spanArray = $("#video-span").val().split(" - ");
    startDate = moment(spanArray[0]).add(1, "s").format("YYYY-MM-DD HH:mm:ss");
    endDate = moment(spanArray[1]).subtract(1, "s").format("YYYY-MM-DD HH:mm:ss");

    processIdx = $("#cycle-process").val();
    cameraIdx = $("#video-camera").val();

    //異常のみボタン
    isErrorFlag = false;
    if($("#video-on-off-button").text() == "ON")
    {
        isErrorFlag = true;
    }

    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
        data: { "funIndex": funIndex, "startDate": startDate, "endDate": endDate, "processIdx": processIdx, "cameraIdx": cameraIdx, "isErrorFlag":isErrorFlag},
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        var moviePathArray = Array();
        for(var i = 0; i < data.length; i++)
        {
            moviePathArray.push(data[i].FilePath);
        }
        //再生ボタンに動画パスの情報を保持する
        var json = JSON.stringify({ MoviePath: moviePathArray, NowMovieIndex: 1 });
        $("#video-play-button").val(json);

        if(data.length != 0)
        {
            //動画保存ボタンEnable変更
            VileChangeDownloadButtonEnabled(true);

            //動画をセットする
            $("#video-movie-player").get(0).src = data[0].FilePath;
        }
        else
        {
            //動画保存ボタンEnable変更
            VileChangeDownloadButtonEnabled(false);
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        //動画保存ボタンEnable変更
        VileChangeDownloadButtonEnabled(false);
        alert("動画ファイル取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

/**
 * zipファイル取得
 */
function VideoCreateZipFile() 
{
    //関数インデックス
    funIndex = 3;
 
    //動画リストを取得
    var json = JSON.parse($("#video-play-button").val());
        
    if(json.MoviePath.length == 0)
    {
        return "";
    }

    var ret = "";

    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
        data: { "funIndex": funIndex, "moviePath": json.MoviePath },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        ret = data.ZipFileName;
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("zipファイル取得失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });

    return ret;
}

/**
 * zipファイル存在チェック
 */
function VideoCheckZipFile(zipFileName) 
{
    //関数インデックス
    funIndex = 4;

    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
        data: { "funIndex": funIndex, "zipFileName": zipFileName },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        if(data.ZipFilePath != "")
        {
            const a = document.createElement("a");
            document.body.appendChild(a);
            var path = data.ZipFilePath.split('\\');
            var fileName = path[path.length - 1];
            a.download = fileName;
            a.href = data.ZipFilePath;
            a.click();
            a.remove();
            URL.revokeObjectURL(data.ZipFilePath);

            //zipファイル削除処理
            VideoDeleteZipFile(zipFileName);
        }
        else
        {
            setTimeout(VideoCheckZipFile, 1000, zipFileName);
        }
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        alert("zipファイル存在チェック失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

/**
 * zipファイル削除処理
 */
function VideoDeleteZipFile(zipFileName) 
{
    //関数インデックス
    funIndex = 5;
 
    $.ajax(
    {
        type: "POST",
        url: "../method/method-video-plot.php",
        data: { "funIndex": funIndex, "zipFileName": zipFileName },
        dataType: "json",
        async: false //同期通信
    })
    //正常時
    .done(function (data) 
    {
        //特に何もしない
    })
    //失敗時
    .fail(function (XMLHttpRequest, textStatus, error) 
    {
        //特に何もしない
        //alert("zipファイル存在チェック失敗, " + textStatus + ", " + XMLHttpRequest.status + ", " + error);
    })
    //必ず実行
    .always(function () 
    {

    });
}

//#endregion ajax処理

//#region 関数処理

/**
 *　全更新処理 ※主にカレンダー入力時
 */
function VideoAllUpdate() 
{
    //カメラ取得
    VideoGetCamera();
 
    //画面更新
    VideoPageUpdate();
}

function VideoPageUpdate()
{
    if($("#video-camera").val() == "-")
    {
        //動画保存ボタンEnable変更
        VileChangeDownloadButtonEnabled(false);
        return;
    }
    
    //工程はサイクルプロット部から取得する
    if($("#cycle-process").val() == "-")
    {
        //動画保存ボタンEnable変更
        VileChangeDownloadButtonEnabled(false);
        return;
    }

    //動画ファイル取得
    VideoGetMovie();
}

/**
 * 動画保存ボタンの有効無効切替
 * @param {bool} isEnabled true:Enable false:disable
 */
function VileChangeDownloadButtonEnabled(isEnabled)
{
    //停止ボタン非表示
    $("#video-stop-button").hide();
    //再生ボタン表示
    $("#video-play-button").show();
    if(isEnabled)
    {
        //動画DLボタン
        $("#video-movie-download-button").prop('disabled', false);
        //動画再生バー
        $("#video-movie-scroll").prop('disabled', false);
        $("#video-movie-scroll").val(0);
    }
    else
    {
        //動画DLボタン
        $("#video-movie-download-button").prop('disabled', true);
        //動画再生バー
        $("#video-movie-scroll").prop('disabled', true);
        $("#video-movie-scroll").val(0);
        //動画クリア
        $("#video-movie-time_now").text("00:00:00");
        $("#video-movie-time_max").text("00:00:00");
        $("#video-movie-player").get(0).src = "";
    }
}

/**
 * 動画を再生する
 */
function VideoPlayMovie(videoPlaySpeed)
{
    //再生速度設定の反映
    $("#video-movie-player").get(0).playbackRate = videoPlaySpeed;

    //音量設定の反映
    $("#video-movie-player").get(0).volume = $("#video-volume-bar").val();

    //動画再生
    $("#video-movie-player").get(0).play();
}

/**
 * 動画現在再生時間表示と再生バーの変更
 */
function VideoChangeMovieNowTime()
{
    //動画ミリ秒
    var movieTime = $("#video-movie-player").get(0).currentTime;

    //動画時間 ミリ秒切り捨て
    var movieDispTime = VideoConvertSecondToTime(movieTime);
    $("#video-movie-time_now").text(movieDispTime);

    //再生バー ミリ秒第一位まで
    $("#video-movie-scroll").val(Math.floor((movieTime * 10) / 10));
}

/**
 * 秒を時間に変換する
 */
function VideoConvertSecondToTime(time) 
{
    //ミリ秒を切り捨て
    time = Math.floor(time);
    var sec = time % 60;
    var min = Math.floor(time / 60) % 60;
    var hour = Math.floor(time / 3600);

    return hour.toString().padStart( 2, '0') + ":" + min.toString().padStart( 2, '0') + ":" + sec.toString().padStart( 2, '0');
}

//#endregion 関数処理
  