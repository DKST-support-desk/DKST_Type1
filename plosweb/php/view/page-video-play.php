<div class="video-div">
    <!-- 検索設定部 -->
    <div>
        <label>期間</label>
        <input type="text" readonly value="-" class="video-span" id="video-span">
        <!-- uikitを用いたドロップダウン -->
        <div uk-dropdown="mode: click;pos: right-center" id="video-dropdown">
            <div id="video-calendar-div"></div>
        </div>

        <label>選択カメラ</label>
        <select class="video-combo" id="video-camera">
            <option value="-">-</option>
        </select>

        <label>異常のみ</label>
        <button type="button" class="video-menu video-on-off-icon" id="video-on-off-button">OFF</button>

        <button type="button" class="video-menu" id="video-movie-download-button" disabled>
            <!-- DLアイコン -->
            <div class="common-download-icon"></div>
            <!-- アイコンスペースを確保するための空行 -->
            &ensp;&ensp;&ensp;&ensp;
            指定したフォルダーに動画を保存
        </button>

        <!-- 最大表示アイコン -->
        <div class="common-right-icon" id="video-size-change-icon">
            <div class="common-gg-arrows-expand-right"></div>
        </div>

    </div>


    <div>
        <table class="video-table">
            <tr>
                <td class="video-td">
                    <div class="video-movie-area">
                        <video class="video-movie" id="video-movie-player">
                            <source type="video/mp4">
                            <source src="">
                            <p>ブラウザのバージョンが古いため動画を再生することができません。</p>
                        </video>
                    </div>
                </td>
            </tr>

            <tr>
                <td class="video-td">
                    <div>
                        <!-- 動画再生バー -->
                        <input type="range" class="video-movie-scroll" id="video-movie-scroll" min="0" max="30" step="0.1" value="0" disabled>
                    </div>
                </td>
            </tr>

            <tr>
                <td class="video-td">
                    <div>
                        <!-- 再生ボタン -->
                        <button type="button" class="video-movie-button" id="video-play-button">
                            <img src="..\..\img\start.png" class="video-button-icon" id="video-play-button-icon">
                        </button>

                        <!-- 停止ボタン -->
                        <button type="button" class="video-movie-button" id="video-stop-button" hidden>
                            <img src="..\..\img\stop.png" class="video-button-icon">
                        </button>
                        
                        <!-- 音量ボタン -->
                        <button type="button" class="video-movie-button" id="video-volume-button">
                            <img src="..\..\img\volume.png" class="video-button-icon" id="video-volume-icon">
                            <img src="..\..\img\volume-off.png" class="video-button-icon" id="video-volume-off-icon" hidden>
                        </button>
                        <!-- uikitを用いたドロップダウン -->
                        <div uk-dropdown="mode: click">
                            <!-- 音量バー -->
                            <input type="range" id="video-volume-bar" min="0" max="1" step="0.01" value="1">
                        </div>

                        <!-- コマ戻し -->
                        <button type="button" class="video-movie-button" id="video-frame-return-button"><img src="..\..\img\before-frame.png" class="video-button-icon"></button>
                        <!-- コマ送り -->
                        <button type="button" class="video-movie-button" id="video-frame-next-button"><img src="..\..\img\after-frame.png" class="video-button-icon"></button>
                        <!-- コマ送りフレーム数 -->
                        <button type="button" class="video-frame-button" id="video-frame-value">1/30</button>
                        <!-- uikitを用いたドロップダウン -->
                        <div uk-dropdown="mode: click">
                            <!-- コマ送り値バー -->
                            <input type="range" id="video-frame-bar" min="1" max="60" step="1" value="30">
                        </div>

                        <!-- 10秒戻し -->
                        <button type="button" class="video-movie-button" id="video-10sec-return-button"><img src="..\..\img\rewind.png" class="video-button-icon"></button>
                        <!-- 10秒送り -->
                        <button type="button" class="video-movie-button" id="video-10sec-next-button"><img src="..\..\img\fast-forward.png" class="video-button-icon"></button>

                        <label>×</label>
                        <input type="number" class="video-movie-speed-box" id="video-speed-val" min="0.2" max="4.0" step="0.1" value="1.0">

                        <!-- 前の動画 -->
                        <button type="button" class="video-movie-button" id="video-before-movie-button"><img src="..\..\img\before-video.png" class="video-button-icon"></button>
                        <!-- 次の動画 -->
                        <button type="button" class="video-movie-button" id="video-next-movie-button"><img src="..\..\img\next-video.png" class="video-button-icon"></button>
                        <div class="common-right-icon video-play-time-area">
                            <label class="" id="video-movie-time_now">00:00:00</label>
                            <label>/</label>
                            <label id="video-movie-time_max">00:00:00</label>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>
