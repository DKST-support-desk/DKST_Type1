/**
 * イベント処理
 */
$(function () 
{
    /**
     * ウインドウサイズ変更イベント
     */
    $(window).resize(function() 
    {
        try
        {
            //山積み表部が表示されていたら
            if($("#index-frame-1").is(":hidden") == false)
            {
                //グラフ再描画
                PileDispGraph();
            }
            
            //折れ線グラフ部が表示されていたら
            if($("#index-frame-2").is(":hidden") == false)
            {
                //折れ線グラフ描画
                CycleDispPlot();
            }
        } 
        catch(e) 
        {
            alert("ウィンドウサイズ変更イベントエラー:" + e.message );
        }
    });

    /**
     * 左クリックイベント（プロット以外のクリック検知）
     */
    $(document).on('click',function(e) 
    {
        try
        {
            //ダイアログ以外のクリックなら
            if(!$(e.target).closest('.cycle-popup').length) 
            {
                //ダイアログを閉じる
                $(".cycle-popup").removeClass("active");
            }
            else
            {
                //ダイアログのクリックならFromTo指定を解除しない
                return;
            }

            //選択プロット以外のクリックなら
            if(!$(e.target).closest('.click-plot').length) 
            {
                //From入力を解除する
                $(".click-plot").removeClass("click-plot");
                $(".right-click-plot").removeClass("right-click-plot");
            }
        } 
        catch(e) 
        {
            alert("左クリックイベントエラー:" + e.message );
        }
    });

    /**
     * キーダウンイベント（Shiftキー押下検知）
     */
    $(document).on('keydown',function(e) 
    {
        try
        {
            var keyCode = e.keyCode;

            //shiftが押下されていたら
            if(keyCode == 16)
            {
                $("#cycle-plot-area").val(true);
            }
        } 
        catch(e) 
        {
            alert("キーダウンイベントエラー:" + e.message );
        }
    });

    /**
     * キーアップイベント（Shiftキーを離したのを検知）
     */
    $(document).on('keyup',function(e) 
    {
        try
        {
            var keyCode = e.keyCode;

            //shiftが押下されていたら
            if(keyCode == 16)
            {
                $("#cycle-plot-area").val(false);
            }
        } 
        catch(e) 
        {
            alert("キーアップイベントエラー:" + e.message );
        }
    });
});