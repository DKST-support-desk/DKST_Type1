<!-- ドロップダウン全体テンプレート -->
<template id="calendar-template">
    <table class="calendar-main-table">
        <tr>
            <td class="calendar-main-td">
                <!-- カレンダー表示部分 -->
                <div class="calendar-area" id="calendar-bodys"></div>
            </td>
        </tr>
        <tr>
            <td class="calendar-main-td calendar-text-left">
                <div>
                    <!-- 開始日時 -->
                    <p>
                        <label><input type="radio" name="calendar-select" id="calendar-start-radio" value="start">開始</label>
                        <input type="date" readonly id="calendar-start-date">
                        <input type="time" step=1 id="calendar-start-time">
                    </p>

                    <!-- 終了日時 -->
                    <p>
                        <label><input type="radio" name="calendar-select" id="calendar-end-radio" value="end">終了</label>
                        <input type="date" readonly id="calendar-end-date">
                        <input type="time" step=1 id="calendar-end-time">
                    </p>
                </div>

                <div class="calendar-button-area">
                    <button type="button" class="calendar-button" id="calendar-ok-button">OK</button>
                    <button type="button" class="calendar-button" id="calendar-cancel-button">キャンセル</button>
                </div>
            </td>
        </tr>
    </table>
</template>

<!-- カレンダー部分テンプレート -->
<template id="calendar-template-body">
    <div>
        <div class="calendar-title">
            <label id="calendar-year"></label>
            <label id="calendar-month"></label>
        </div>

        <table class="calendar-table" id="calendar-table" year month>
            <thead>
                <tr>
                    <th class="calendar-th">日</th>
                    <th class="calendar-th">月</th>
                    <th class="calendar-th">火</th>
                    <th class="calendar-th">水</th>
                    <th class="calendar-th">木</th>
                    <th class="calendar-th">金</th>
                    <th class="calendar-th">土</th>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
                <tr>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                    <td class="calendar-td"></td>
                </tr>
            </tbody>
        </table>

        <!-- 改行 -->
        <p class="new-line"></p>
    </div>
</template>