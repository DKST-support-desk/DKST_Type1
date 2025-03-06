<!-- ライン名表示部 -->
<div>
    <!-- ライン名 -->
    <label id="pile-line-name"></label>

    <!-- 最大表示アイコン -->
    <div class="common-right-icon" id="pile-size-change-icon">
        <div class="common-gg-arrows-expand-right"></div>
    </div>
</div>

<!-- 検索設定部 -->
<div>
    <label>期間</label>
    <input type="text" class="pile-span" id="pile-span" readonly value="2022/04/26 - 2022/04/26">
    <!-- uikitを用いたドロップダウン -->
    <div uk-dropdown="mode: click" id="pile-dropdown">
        <div id="pile-calendar-div"></div>
    </div>

	<label>勤帯</label>
	<!-- <select class="pile-combo" id="pile-shift"></select> -->
    <input type="text" class="pile-combo" id="pile-shift" value="ALL" readonly>
    <!-- uikitを用いたドロップダウン -->
    <div uk-dropdown="mode: click" id="pile-shift-dropdown">
        <div id="pile-shift-div"></div>
    </div>

	<label>作業編成</label>
	<select class="pile-combo" id="pile-composition"></select>

	<label>品番</label>
	<select class="pile-combo" id="pile-product"></select>
</div>

<!-- 改行 -->
<p></p>

<!-- 表1 -->
<div>
    <table class="pile-table">
		<tr class="pile-plan">
			<td class="pile-head">①必要数</td>
            <td class="pile-tail"></td>
            <td class="pile-tail"></td>
			<td class="pile-first" id="pile-required-num"></td>
			<td class="pile-td">【台】</td>
		</tr>
		<tr class="pile-plan">
			<td class="pile-head">②定時稼働時間</td>
            <td class="pile-tail"></td>
            <td class="pile-tail"></td>
			<td class="pile-td" id="pile-plan-second"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr>
			<td class="pile-head"><label>③T.T</label></td>
            <td class="pile-tail"><hr class="pile-tt-line"></td>
            <td class="pile-tail"><input type="checkbox" id="pile-tt-checkbox"></td>
			<td class="pile-td" id="pile-tt"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr class="pile-actual">
			<td class="pile-head">④良品数</td>
            <td class="pile-tail"></td>
            <td class="pile-tail"></td>
			<td class="pile-td" id="pile-result-num"></td>
			<td class="pile-td">【台】</td>
		</tr>
		<tr class="pile-actual">
			<td class="pile-head">⑤実績稼働時間</td>
            <td class="pile-tail"></td>
            <td class="pile-tail"></td>
			<td class="pile-td" id="pile-result-time"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr>
			<td class="pile-head">出来高ピッチ</td>
			<td class="pile-tail"><hr class="pile-pitch-line"></td>
            <td class="pile-tail"><input type="checkbox" id="pile-pitch-checkbox"></td>
			<td class="pile-td" id="pile-pitch"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr>
			<td class="pile-head">ネックCT(加重)</td>
			<td class="pile-tail"><hr class="pile-weight-line"></td>
            <td class="pile-tail"><input type="checkbox" id="pile-weight-checkbox"></td>
			<td class="pile-td" id="pile-neck-weight"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr>
			<td class="pile-head">ネックCT(個別)</td>
			<td class="pile-tail"><hr class="pile-single-line"></td>
            <td class="pile-tail"><input type="checkbox" id="pile-single-checkbox"></td>
			<td class="pile-td" id="pile-neck-single"></td>
			<td class="pile-td">【秒】</td>
		</tr>
		<tr>
			<td class="pile-head">⑨可動率</td>
            <td class="pile-tail"></td>
            <td class="pile-tail"></td>
			<td class="pile-td" id="pile-table-occupancy-rate"></td>
			<td class="pile-td">【%】</td>
		</tr>
	</table>
</div>

<!-- 棒グラフ -->
<div>
    <table class="pile-table">
        <tr>
            <td class="pile-graph-left">
                <div class="pile-centor-div">最大</div>
                <input type="number" class="pile-grapf-input" id="pile-graph-max-val" min="1" max="99999" value="0"></input>
                <p></p>
                <div class="pile-centor-div">最小</div>
                <input type="number" class="pile-grapf-input" id="pile-graph-min-val" min="0" max="99998" value="0"></input>
            </td>
            <td class="pile-grapf-right">
                <div class="pile-centor-div">
                    <label>山積み表(戦略立案)</label>
                    <label class="common-help-button common-right-icon"></label>
                    <!-- uikitを用いたドロップダウン -->
                    <div uk-dropdown="mode: click">
                    <img src="..\..\img\sample-pile-graph.png">
                    </div>
                </div>

                <!-- グラフ -->
                <div id="pile-grapg-area">
                    <svg class="pile-graph" id="psGraph"></svg>
                </div>
            </td>
        </tr>
    </table>

    <table class="pile-table">
        <tr>
            <th class="pile-top-header">
                <Label>表示列数</Label>
                <input type="number" class="pile-graph-disp-num" id="pile-graph-disp-num" min="2" max="30" value="6"></input>
            </th>
            <th class="pile-tail-header">
                <button type="button" class="pile-movie-button" id="pile-before-graph"><img src="..\..\img\before-frame.png" width="15px" height="15px"></button>
                <button type="button" class="pile-movie-button common-right-icon" id="pile-after-graph"><img src="..\..\img\after-frame.png" width="15px" height="15px"></button>
            </th>
        </tr>
    </table>
</div>

<!-- 表2 -->
<div class="pile-test">
    <table class="pile-table">
        <tbody>
            <tr id="pile-process-tr">
                <!-- PileCreateProcess参照 -->
            </tr>
            <tr id="pile-worker-tr">
                <!-- PileCreateWorker参照 -->
            </tr>
            <tr id="pile-disp-option-tr">
                <!-- PileCreateDispOption参照 -->
            </tr>
            <tr id="pile-ct-max-tr">
                <!-- PileCreateCTMax参照 -->
            </tr>
            <tr id="pile-ct-average-tr">
                <!-- PileCreateCTAverage参照 -->
            </tr>
            <tr id="pile-ct-min-tr">
                <!-- PileCreateCTMin参照 -->
            </tr>
            <tr id="pile-ct-scattering-tr">
                <!-- PileCreateCTScattering参照 -->
            </tr>
            <tr id="pile-ct-max-limit-tr">
                <!-- PileCreateCTMaxLimit参照 -->
            </tr>
            <tr id="pile-ct-min-limit-tr">
                <!-- PileCreateCTMinLimit参照 -->
            </tr>
            <tr id="pile-ancillary-tr">
                <!-- PileCreateAncillary参照 -->
            </tr>
            <tr id="pile-setup-tr">
                <!-- PileCreateSetup参照 -->
            </tr>
            <tr>
                <td class="pile-item-name" colspan="2">⑲ΣCT</td>
                <td class="pile-calcu" id="pile-actual-sum-ct" colspan="3"></td>
                <td class="pile-plan" id="pile-plan-sum-ct" colspan="3"></td>
            </tr>
            <tr>
                <td class="pile-item-name" colspan="2">⑳編成効率</td>
                <td class="pile-calcu" id="pile-actual-com-efficiency" colspan="3"></td>
                <td class="pile-plan" id="pile-plan-com-efficiency" colspan="3"></td>
            </tr>
            <tr>
                <td class="pile-item-name" colspan="2">㉑MM比</td>
                <td class="pile-calcu" id="pile-actual-mm" colspan="3"></td>
                <td class="pile-plan" id="pile-plan-mm" colspan="3"></td>
            </tr>
        </tbody>
    </table>
</div>

<p></p>

<!-- 表3 -->
<div>
    <table class="pile-table">
        <tr>
            <td class="pile-td" rowspan="2">推奨<br>戦略</td>
            <td class="pile-td" colspan="3">㉒可動率向上</td>
            <td class="pile-td" id="pile-occupancy-rate-check" rowspan="2">―</td>
            <td class="pile-td">㉓正味改善</td>
            <td class="pile-td" id="pile-improvement-check" rowspan="2">―</td>
            <td class="pile-td" colspan="3">㉔リバランス</td>
            <td class="pile-td" id="pile-composition-efficiency-check" rowspan="2">―</td>
        </tr>
        <tr>
            <td class="pile-td">可動率</td>
            <td class="pile-plan" id="pile-occupancy-rate">0.0</td>
            <td class="pile-td">%以下</td>
            <td class="pile-td">ネックCT>T.T</td>
            <td class="pile-td">編成効率</td>
            <td class="pile-plan" id="pile-composition-efficiency">0.0</td>
            <td class="pile-td">%以下</td>
        </tr>
    </table>
</div>