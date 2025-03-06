<!-- 検索設定部 -->
<div class="cycle-height-correction">
    <div>
        <label>期間</label>
        <input type="text" readonly value="2022/99/99 99:99:99 - 2022/99/99 99:99:99" class="cycle-span" id="cycle-span">
        <!-- uikitを用いたドロップダウン -->
        <div uk-dropdown="mode: click" id="cycle-dropdown">
            <div id="cycle-calendar-div"></div>
        </div>

        <label>勤帯</label>
        <input type="text" class="cycle-combo" id="cycle-shift" value="ALL" readonly>
        <!-- uikitを用いたドロップダウン -->
        <div uk-dropdown="mode: click" id="cycle-shift-dropdown">
            <div id="cycle-shift-div"></div>
        </div>

        <label>作業編成</label>
        <select class="cycle-combo" id="cycle-composition"></select>

        <label id="cycle-product-label">
            品番
            <input type="text" class="cycle-combo" id="cycle-product" value="-" readonly>
        </label>
        <!-- DBから値取得時ドロップダウンを追加する -->
        <!-- uikitを用いたドロップダウン -->
        <!-- <div uk-dropdown="mode: click" id="cycle-product-dropdown"><div id="cycle-product-div"></div></div> -->

        <label>工程</label>
        <select class="cycle-combo" id="cycle-process"></select>

        <!-- 最大表示アイコン -->
        <div class="common-right-icon" id="cycle-size-change-icon">
            <div class="common-gg-arrows-expand-right"></div>
        </div>
    </div>

    <p></p>

    <div class="cycle-div-menu-line">
        <label id="cycle-worker-label">
            作業者
            <input type="text" class="cycle-combo" id="cycle-worker" value="-" readonly>
        </label>
        <!-- DBから値取得時ドロップダウンを追加する -->
        <!-- uikitを用いたドロップダウン -->
        <!-- <div uk-dropdown="mode: click" id="cycle-worker-dropdown"><div id="cycle-worker-div"></div></div> -->


        <label>異常のみ</label>
        <button type="button" class="cycle-menu cycle-on-off-button" id="cycle-on-off-button" value="OFF">OFF</button>
                            
        <button type="button" class="cycle-menu" id="cycle-csv-download" title="《自由にサイクルタイムデータを分析してもらう機能》">
            <!-- DLアイコン -->
            <div class="common-download-icon"></div>
            <!-- アイコンスペースを確保するための空行 -->
            &ensp;&ensp;&ensp;&ensp;
            CT CSV出力
        </button>
    </div>

    <p></p>

    <div>
        <div>
            <button type="button" class="cycle-regist" id="cycle-min-button" title="《画面上でマスタ登録をするための機能》※品番タイプにALL、異常のみにONが選択されている場合は使えません。">最小値を登録</button>
            <input type="number" class="cycle-input" id="cycle-min-val" min="0" max="9999"></input>

            <button type="button" class="cycle-regist" id="cycle-average-button" title="《画面上でマスタ登録をするための機能》※品番タイプにALL、異常のみにONが選択されている場合は使えません。">平均値を登録</button>
            <input type="number" class="cycle-input" id="cycle-average-val" min="0" max="9999"></input>

            <button type="button" class="cycle-regist" id="cycle-max-button" title="《画面上でマスタ登録をするための機能》※品番タイプにALL、異常のみにONが選択されている場合は使えません。">最大値を登録</button>
            <input type="number" class="cycle-input" id="cycle-max-val" min="0" max="9999"></input>

            <button type="button" class="cycle-regist" id="cycle-upper-button" title="《画面上でマスタ登録をするための機能》※品番タイプにALL、異常のみにONが選択されている場合は使えません。">上限値を登録</button>
            <input type="number" class="cycle-input" id="cycle-upper-val" min="0" max="9999"></input>

            <button type="button" class="cycle-regist" id="cycle-lower-button" title="《画面上でマスタ登録をするための機能》※品番タイプにALL、異常のみにONが選択されている場合は使えません。">下限値を登録</button>
            <input type="number" class="cycle-input" id="cycle-lower-val" min="0" max="9999"></input>
        </div>
    </div>

    <!-- 一括除外ポップアップ -->
    <div class="cycle-popup">
    <table class="cycle-popup-table">
        <tr>
            <td class="cycle-popup-msg-line" id="cycle-popup-msg" colspan="3"></td>
        </tr>
        <tr>
            <td class="cycle-popup-button-line"><button id="cycle-all-error-button">全除外</button></td>
            <td class="cycle-popup-button-line"><button id="cycle-all-release-button">全除外解除</button></td>
            <td class="cycle-popup-button-line"><button id="cycle-cancel-button">キャンセル</button></td>
        </tr>
    </table>  
    </div>

    <!-- 折れ線グラフ -->
    <div class="cycle-height-correction">
        <table class="cycle-table">
            <tr>
                <td class="cycle-plot-left">
                    <div class="cycle-center">最大</div>
                    <input type="number" class="cycle-plot-input" id="cycle-graph-max-val" min="1" max="99999" value="0"></input>
                    <p></p>
                    <div class="cycle-center">最小</div>
                    <input type="number" class="cycle-plot-input" id="cycle-graph-min-val" min="0" max="99998" value="0"></input>
                </td>

                <td class="cycle-plot-right">
                    <div class="cycle-center cycle-height-correction">
                        <label>1サイクル毎の実績</label>
                        <label class="common-help-button common-right-icon"></label>
                        <div uk-dropdown="mode: click">
                            <img src="..\..\img\sample-cycle-graph.png">
                        </div>

                        <div class="cycle-plot-parent">
                            <!-- サイクルプロット -->
                            <div class="cycle-plot-area" id="cycle-plot-area">
                                <svg class="cycle-plot" id="cycle-plot"></svg> 
                            </div>

                            <!-- プログレスダイアログ -->
                            <div class="cycle-progress-circle" id="cycle-progress" hidden></div>
                        </div>

                        <p></p>

                        <!-- プロット描画数スライドバー -->
                        <div class="cycle-center">
                            <input type="range" class="cycle-slider" id="cycle-plot-disp-bar" step="1" min="1" value="1">
                            <input type="number" class="cycle-plot-disp-num" id="cycle-plot-disp-num" min="1" max="99999" value="1">
                        </div>

                        <p></p>
                        
                        <div class="cycle-center">
                            <label>合計サイクル数</label>
                            <input type="text" class="cycle-input" id="cycle-sum-cycle-num" value="0" readonly>
                            
                            <label>&ensp;</label>
                                                        
                            <label>異常発生件数</label>
                            <input type="text" class="cycle-input" id="cycle-error-num" value="0" readonly>
                            
                            <label>=</label>
                            
                            <label>上限カット数</label>
                            <input type="text" class="cycle-input" id="cycle-upper-num" value="0" readonly>
                            
                            <label>+</label>
                            
                            <label>下限カット数</label>
                            <input type="text" class="cycle-input" id="cycle-lower-num" value="0" readonly>
                            
                            <label>+</label>
                            
                            <label>手動除外数</label>
                            <input type="text" class="cycle-input" id="cycle-manual-error-num" value="0" readonly>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>