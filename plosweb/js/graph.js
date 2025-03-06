//=====================================================================================================
/**
 * グラフベースクラス
 */
class GraphBase
{	
    
    /**
     * コンストラクタ
     * @param {string} selector  SVG要素のセレクタ文字列
     */
    constructor(selector)
    {
        // paperオブジェクト
        this._paper = Snap(selector);
        // svgエレメント
        this._svgel = $(selector);
 
        //--------------------------------------------------
        // グラフ描画エリア svgのサイズから取得
        //--------------------------------------------------
        // Ⓘグラフエリア縦幅
        this._areaHeight = this._paper.attr('height');
        // Ⓙグラフエリア横幅
        this._areaWidth = this._paper.attr('width');

        //--------------------------------------------------
        // 軸設定 initializeAxisで設定
        //--------------------------------------------------
        // Ⓐy軸単位項目
        this._yName = "CT【秒】";
        // Ⓑx軸線、y軸線の太さ
        this._lineWidth = 1;
        // Ⓑx軸線、y軸線の矢印サイズ
        this._arrowSize = 10;
        // Ⓒ目盛フォントサイズ
        this._fontSize = 15;
        // Ⓓ目盛線幅
        this._scaleLineWidth = 8;
        // Ⓔy軸横幅
        this._yAxisWidth = 20;
        // ⓀX軸縦幅
        this._xAxisHeight = 40;

        //--------------------------------------------------
        // グラフX軸Y軸の幅 setYAxisWidth,setPlotWidthで設定
        //--------------------------------------------------
        // Ⓕy軸縦幅
        this._yAxisHeight = 200;
        // Ⓗプロット幅
        this._xAxisWidth = 400;

        //--------------------------------------------------
        // データ設定 
        //--------------------------------------------------
        // Ⓖ最大目盛数 
        this._maxScaleNum = 12;
        // y軸MAX値
        this._max = undefined;
        // y軸MIN値
        this._min = undefined;
        // 目盛数(MAX,MIN,最大目盛数から計算)
        this._scaleNum = undefined;
        // 目盛間隔(MAX,MIN,最大目盛数から計算)
        this._scaleInterval = undefined;
        // 目盛最小値(MAX,MIN,最大目盛数から計算)
        this._scaleMin = undefined
        // 目盛最大値(MAX,MIN,最大目盛数から計算)
        this._scaleMax = undefined;
        // 1目盛のピクセル数(MAX,MIN,最大目盛数から計算)
        this._pixPerScale = undefined;

        // 原点の座標
        this._basisPoint = new Point(this._areaWidth - this._xAxisWidth - (this._yAxisWidth / 2), this._areaHeight - this._xAxisHeight);
        
        // 表示開始インデックス
        this._plotStart = 0;

        // プロット列数（表示インデックス）
        this._plotDisplayNum = 6;

        // データ配列
        this._data = undefined;
    }

    /**
     * 軸の設定
     * @param {string} yName Y軸単位項目名 
     * @param {number} yAxisWidth Y軸横幅 
     * @param {number} xAxisHeight X軸縦幅
     * @param {number} lineWidth 線の太さ
     * @param {number} arrowSize 矢印サイズ
     * @param {number} scaleLineWidth 目盛線幅
     * @param {number} fontSize フォントサイズ
     * @param {number} maxScaleNum  最大目盛数
     */
    initializeAxis(yName, yAxisWidth, xAxisHeight, lineWidth, arrowSize, scaleLineWidth, fontSize, maxScaleNum)
    {
        this._yName = yName;
        this._yAxisWidth = yAxisWidth;
        this._xAxisHeight = xAxisHeight;
        this._lineWidth = lineWidth;
        this._arrowSize = arrowSize;
        this._scaleLineWidth = scaleLineWidth;
        this._fontSize = fontSize;
        this._maxScaleNum = maxScaleNum;
    }

    /**
     * Y軸縦幅セット
     * @param {number} yAxisHeight Y軸縦幅 
     */
    setYAxisWidth(yAxisHeight)
    {
        this._yAxisHeight = yAxisHeight;
    }

    /**
     * プロット幅
     * @param {number} xAxisWidth プロット幅 
     */
    setPlotWidth(xAxisWidth)
    {
        this._xAxisWidth = xAxisWidth;
    }
    
    /**
     * グラフエリア設定
     * @param {number} areaWidth グラフエリア横幅 
     * @param {number} areaHeight グラフエリア縦幅
     */
     setGraphArea(areaWidth, areaHeight)
     {
        this._areaWidth = areaWidth;
        this._areaHeight = areaHeight;
        // 原点の座標
        this._basisPoint = new Point(this._areaWidth - this._xAxisWidth - (this._yAxisWidth / 2), this._areaHeight - this._xAxisHeight);
     }

    /**
     * MAX値セット
     * @param {number} max MAX値
     */
    setMax(max)
    {
        this._max = max;
    }

    /**
     * MIN値セット
     * @param {number} min MIN値 
     */
    setMin(min)
    {
        this._min = min;
    }


     /**
      * 表示範囲設定
      *
      * @param {number} plotStart   開始インデックス
      * @param {number} plotDisplayNum  表示数
      */
     setArea(plotStart, plotDisplayNum)
     {
        this._plotStart = plotStart;
        this._plotDisplayNum = plotDisplayNum;
     }


    /**
     * 入力データセット
     *
     * @param {*} data  データ配列
     */
    setData(data)
    {
        this._data = data;
    }                                                                                                                                                   

    /**
     * グラフ更新
     */
    update()
    {
        // 2021
        this.setScale();
        // Y軸描画
        this._YAxis();
        // X軸描画
        this._XAxis();
    }

    /**
     * 目盛設定
     * @returns 設定結果(true/false)
     */
    setScale()
    {
        // MAX,MIN判定
        if(this._max <= this._min)
        {
            return false;
        }
        
        // 目盛間隔 5未満は5固定
        if( ((this._max - this._min) / this._maxScaleNum) < 5 )
        {
            this._scaleInterval = 5;
        }
        else
        {
            this._scaleInterval = Math.ceil((this._max - this._min) / (this._maxScaleNum + 1) / 10) * 10;
        }

        // 目盛最小値
        if(this._min >= 0)
        {
            this._scaleMin = Math.floor(this._min / this._scaleInterval) * this._scaleInterval;
        }
        else
        {
            this._scaleMin = Math.ceil(this._min / this._scaleInterval) * this._scaleInterval;
        }
        // 目盛数
        this._scaleNum = Math.ceil((this._max - this._scaleMin) / this._scaleInterval);
        // 目盛最大値
        this._scaleMax = this._scaleMin + (this._scaleInterval * this._scaleNum);
        // 1目盛のピクセル数
        this._pixPerScale = this._yAxisHeight / (this._scaleMax - this._scaleMin);

        return true;
    }

    /**
     * y軸
     *
     * @memberof GraphBase
     */
    _YAxis()
    {
        // 軸
        this._paper.line(this._basisPoint.x, this._basisPoint.y, this._basisPoint.x, this._basisPoint.y - this._yAxisHeight).attr({stroke: 'rgb(0,0,0)', strokeWidth: this._lineWidth});
        // 矢印
        this._arrow(this._basisPoint.x, this._basisPoint.y - this._yAxisHeight, this._arrowSize, 'y');
        // 項目名
        this._paper.text(this._basisPoint.x, this._basisPoint.y - this._yAxisHeight, this._yName).attr({fill: "black", fontSize: this._fontSize, textAnchor:'middle', dominantBaseline:'ideographic', class:"id"});

        // 目盛
        for(let plotIndex = 0; plotIndex < this._scaleNum; plotIndex++)
        {
            // 目盛描画y座標
            let ypos = this._basisPoint.y - (this._pixPerScale * this._scaleInterval * plotIndex);
            // 目盛値            
            let label = this._scaleMin + (this._scaleInterval * plotIndex);

            // 目盛のラベル
            this._paper.text(this._basisPoint.x - (this._yAxisWidth / 2), ypos, String(label)).attr({fontSize: this._fontSize, textAnchor:'end', dominantBaseline:'middle'});
            // 目盛線(y軸基点値の目盛線は不要のため)
            if(plotIndex != 0)
            {
                this._paper.line(this._basisPoint.x - (this._scaleLineWidth / 2), ypos, this._basisPoint.x + (this._scaleLineWidth / 2), ypos).attr({stroke: 'rgb(0,0,0)', strokeWidth: 1});
            }
        }
    }

    /**
     * x軸
     *
     * @memberof GraphBase
     */
     _XAxis()
     {
        // 軸
        this._paper.line(this._basisPoint.x, this._basisPoint.y, this._basisPoint.x + this._xAxisWidth, this._basisPoint.y).attr({stroke: 'rgb(0,0,0)', strokeWidth: this._lineWidth});
        // 矢印
        this._arrow(this._basisPoint.x + this._xAxisWidth, this._basisPoint.y, this._arrowSize, 'x');
     }



    /**
     * 矢印の設定
     *
     * @param {number} pointX　x座標
     * @param {number} pointY　y座標
     * @param {number} size　　矢印サイズ
     * @memberof GraphBase
     */
    _arrow(pointX, pointY, size, axis)
    {
        let arrowPoint = [];
        if(axis == 'x')
        {
            // x軸横線が矢印先端からつき出るため矢印位置を1px右にセット
            pointX = pointX + 1;
            arrowPoint.push([pointX, pointY]);
            arrowPoint.push([pointX - size, pointY + Math.floor(size / 2)]);
            arrowPoint.push([pointX - size, pointY - Math.floor(size / 2)]);
            arrowPoint.push([pointX, pointY]);
        }
        else if(axis == 'y')
        {
            // y軸縦線が矢印先端からつき出るため矢印位置を1px上にセット
            pointY = pointY - 1;
            arrowPoint.push([pointX, pointY]);
            arrowPoint.push([pointX - Math.floor(size / 2), pointY + size]);
            arrowPoint.push([pointX + Math.floor(size / 2), pointY + size]);
            arrowPoint.push([pointX, pointY]);
        }
        this._paper.polygon(arrowPoint).attr({fill: 'rgb(0,0,0)'});
    }
}

//=====================================================================================================
/**
 * 折れ線グラフクラス
 *
 * @extends {GraphBase} グラフベースクラス
 */
class CtGraph extends GraphBase
{
    /**
     * コンストラクタ
     * @param {string} selector  SVG要素のセレクタ文字列
     */
    constructor(selector)
    {
        super(selector);

        // 20211129
        //--------------------------------------------------
        // 指標 setXAxisSettingで設定
        //--------------------------------------------------
        this._angle = undefined;
        this._align = undefined;
        this._skipStep = undefined;
        this._initialStep = undefined;
        // 20211129
    }

    /**
     * グラフ更新 
     *
     */
    update()
    {
        // 目盛設定
        if(super.setScale() == false)
        {
            return;
        }
        // グラフ描画
        this._drowGraph();
        // ベース描画
        super.update();
    }

    // 20211129
    /**
     * 指標セット
     * @param {Number} angle    // 角度
     * @param {Number} align    // 配置
     * @param {Number} skipStep // ラベル間隔
     * @param {Number} initialStep // 初期ラベル値
     */
    setXAxisSetting(angle, align, skipStep, initialStep)
    {
        // 角度
        this._angle = angle;
        // 配置
        this._align = align;
        // ラベル間隔
        this._skipStep = skipStep;
        // 初期ラベル値
        this._initialStep = initialStep;
    }
    // 20211129

    /**
     * 折れ線グラフの描画
     *
     */
    _drowGraph()
    {
        // 20211129
        // **************************
        // x軸ラベル配置設定
        // **************************
        var alignAttr;
        switch(this._align)
        {
            case 0:
                alignAttr = 'start'; // 左寄せ
                break;
            case 1:
                alignAttr = 'End';   // 右寄せ
                break;
            case 2:
                alignAttr = 'middle';   // 中央
                break;
            default:
                break;
        }
        // **************************
        // x軸ラベル角度設定
        // **************************
        // 範囲外の場合は角度０度でセット
        if(180 < this._angle || this._angle < -180)
        {
            this._angle = 0;
        }
        // 20211129

        // サイクルタイム格納
        let cycleTime = [];
        // プロット列幅に均等割
        let plotColumnWidth = this._xAxisWidth / this._plotDisplayNum;

        // **************************
        // プロット
        // **************************
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // プロット列幅
            let labelPoint = this._basisPoint.x +  (plotColumnWidth / 2) + (plotColumnWidth * plotIndex);

            // 20211129
            // 初期ラベル値
            if(this._initialStep <= this._data[this._plotStart + plotIndex]["Number"])
            {
                // ラベル間隔   (Number - ラベル初期値)　% ラベル間隔値
                if((this._data[this._plotStart + plotIndex]["Number"] - this._initialStep) % this._skipStep == 0)
                {
                    // ラベル(動画有無)
                    if(this._data[this._plotStart + plotIndex]["HasVideo"] == 1)
                    {
                        // 動画あり（黒文字）
                        //this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"])).attr({fill: 'rgb(0,0,0)',fontSize: this._fontSize, textAnchor:'middle', dominantBaseline:'middle'});   
                        this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"]))
                        .attr({transform: `rotate(${this._angle},${labelPoint},${this._basisPoint.y + (this._xAxisHeight / 2)})`, fill: 'rgb(0,0,0)',fontSize: this._fontSize, textAnchor:alignAttr, dominantBaseline:'middle'});
                    }
                    else
                    {
                        // 動画無し（灰色）
                        //this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"])).attr({fill: 'rgb(166,166,166)',fontSize: this._fontSize, textAnchor:'middle', dominantBaseline:'middle'});
                        this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"]))
                        .attr({transform: `rotate(${this._angle},${labelPoint},${this._basisPoint.y + (this._xAxisHeight / 2)})`, fill: 'rgb(166,166,166)',fontSize: this._fontSize, textAnchor:alignAttr, dominantBaseline:'middle'});
                    }
                }
            }
            // 20211129

            // サイクルタイムデータ
            cycleTime.push(labelPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale));

            // 上限カット、下限カット
            let plotPoint = this._basisPoint.x + (plotColumnWidth * plotIndex);
            // プロット塗りつぶし
            this._paper.rect(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotColumnWidth, (this._data[this._plotStart + plotIndex]["Upper"] - this._data[this._plotStart + plotIndex]["Lower"]) * this._pixPerScale).attr({fill: 'rgb(231,230,230)', stroke: 'none'});
        }

        // **************************
        // 上限カット、下限カット、標準値
        // **************************
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // 上限カット、下限カット
            let plotPoint = this._basisPoint.x + (plotColumnWidth * plotIndex);
            // 上限ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
            // 下限ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
            // 標準ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale)).attr({stroke: 'rgb(0,0,0)',strokeWidth: 1});
            if(this._plotStart + plotIndex < this._data.length - 1)
            {
                // 上限ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["Upper"] != this._data[this._plotStart + plotIndex + 1]["Upper"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["Upper"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
                }
                // 下限ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["Lower"] != this._data[this._plotStart + plotIndex + 1]["Lower"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["Lower"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
                }
                // 標準ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["DefValue"] != this._data[this._plotStart + plotIndex + 1]["DefValue"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["DefValue"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(0,0,0)',strokeWidth: 1});
                }
            }
        }

        // **************************
        // 折れ線、サイクルポイント
        // **************************
        // 折れ線
        this._paper.polyline(cycleTime).attr({fill: 'none', stroke: 'rgb(0,0,0)', strokeWidth: 1, class: 'ct-c ct-pos'});

        // ポイント
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            let circleObj;
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // プロット列幅
            let plotPoint = this._basisPoint.x +  (plotColumnWidth / 2) + (plotColumnWidth * plotIndex);
            // ポイント
            if(this._data[this._plotStart + plotIndex]["IsException"] == 1)
            {
                // 異常
                circleObj = this._paper.circle(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale), 8).attr({fill: 'rgb(255,255,255)', stroke: 'rgb(255,0,0)', class: 'ct-b ct-pos', id:'ct-pos-' + String(this._data[this._plotStart + plotIndex]["Number"])});
            }
            else
            {
                // 正常
                circleObj = this._paper.circle(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale), 8).attr({fill: 'rgb(231,230,230)', stroke: 'rgb(0,0,0)', class: 'ct-b ct-pos', id:'ct-pos-' + String(this._data[this._plotStart + plotIndex]["Number"])});
            }
            // ツールチップ
            let title = Snap.parse('<title >' + this._data[this._plotStart + plotIndex]["Title"] + '</title>');
            circleObj.append(title);
        }
    }

//#region 2022/05/23 西部追加

    /**
     * グラフ更新 
     *
     */
    update2(dispNum)
    {
        // 目盛設定
        if(super.setScale() == false)
        {
            return;
        }
        // グラフ描画
        var newWidth = this._drowGraph2(dispNum);
        this._xAxisWidth = newWidth;
        // ベース描画
        super.update();

        return newWidth;
    }

    /**
     * 折れ線グラフの描画
     *
     */
    _drowGraph2(dispNum)
    {
        // 20211129
        // **************************
        // x軸ラベル配置設定
        // **************************
        var alignAttr;
        switch(this._align)
        {
            case 0:
                alignAttr = 'start'; // 左寄せ
                break;
            case 1:
                alignAttr = 'End';   // 右寄せ
                break;
            case 2:
                alignAttr = 'middle';   // 中央
                break;
            default:
                break;
        }
        // **************************
        // x軸ラベル角度設定
        // **************************
        // 範囲外の場合は角度０度でセット
        if(180 < this._angle || this._angle < -180)
        {
            this._angle = 0;
        }
        // 20211129

        // サイクルタイム格納
        let cycleTime = [];
        // プロット列幅に均等割
        // let plotColumnWidth = this._xAxisWidth / this._plotDisplayNum;
        let plotColumnWidth = this._xAxisWidth / dispNum;

        // **************************
        // プロット
        // **************************
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // プロット列幅
            let labelPoint = this._basisPoint.x +  (plotColumnWidth / 2) + (plotColumnWidth * plotIndex);

            // 20211129
            // 初期ラベル値
            if(this._initialStep <= this._data[this._plotStart + plotIndex]["Number"])
            {
                // ラベル間隔   (Number - ラベル初期値)　% ラベル間隔値
                if((this._data[this._plotStart + plotIndex]["Number"] - this._initialStep) % this._skipStep == 0)
                {
                    // ラベル(動画有無)
                    if(this._data[this._plotStart + plotIndex]["HasVideo"] == 1)
                    {
                        // 動画あり（黒文字）
                        //this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"])).attr({fill: 'rgb(0,0,0)',fontSize: this._fontSize, textAnchor:'middle', dominantBaseline:'middle'});   
                        this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2) + 10, String(this._data[this._plotStart + plotIndex]["Number"]))
                        .attr({transform: `rotate(${this._angle},${labelPoint},${this._basisPoint.y + (this._xAxisHeight / 2)})`, fill: 'rgb(0,0,0)',fontSize: this._fontSize, textAnchor:alignAttr, dominantBaseline:'middle'});
                    }
                    else
                    {
                        // 動画無し（灰色）
                        //this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2), String(this._data[this._plotStart + plotIndex]["Number"])).attr({fill: 'rgb(166,166,166)',fontSize: this._fontSize, textAnchor:'middle', dominantBaseline:'middle'});
                        this._paper.text(labelPoint, this._basisPoint.y + (this._xAxisHeight / 2) + 10, String(this._data[this._plotStart + plotIndex]["Number"]))
                        .attr({transform: `rotate(${this._angle},${labelPoint},${this._basisPoint.y + (this._xAxisHeight / 2) + 10})`, fill: 'rgb(166,166,166)',fontSize: this._fontSize, textAnchor:alignAttr, dominantBaseline:'middle'});
                    }
                }
            }
            // 20211129

            // サイクルタイムデータ
            cycleTime.push(labelPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale));

            // 上限カット、下限カット
            let plotPoint = this._basisPoint.x + (plotColumnWidth * plotIndex);
            // プロット塗りつぶし
            this._paper.rect(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotColumnWidth, (this._data[this._plotStart + plotIndex]["Upper"] - this._data[this._plotStart + plotIndex]["Lower"]) * this._pixPerScale).attr({fill: 'rgb(231,230,230)', stroke: 'none'});
        }

        // **************************
        // 上限カット、下限カット、標準値
        // **************************
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // 上限カット、下限カット
            let plotPoint = this._basisPoint.x + (plotColumnWidth * plotIndex);
            // 上限ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
            // 下限ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
            // 標準ライン（横線）
            this._paper.line(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale)).attr({stroke: 'rgb(0,0,0)',strokeWidth: 1});
            if(this._plotStart + plotIndex < this._data.length - 1)
            {
                // 上限ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["Upper"] != this._data[this._plotStart + plotIndex + 1]["Upper"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Upper"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["Upper"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
                }
                // 下限ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["Lower"] != this._data[this._plotStart + plotIndex + 1]["Lower"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["Lower"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["Lower"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(166,166,166)',strokeWidth: 1});
                }
                // 標準ライン（縦線）
                if(this._data[this._plotStart + plotIndex]["DefValue"] != this._data[this._plotStart + plotIndex + 1]["DefValue"])
                {
                    this._paper.line(plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["DefValue"] - this._scaleMin) * this._pixPerScale), plotPoint + plotColumnWidth, this._basisPoint.y - ((this._data[this._plotStart + plotIndex + 1]["DefValue"] - this._scaleMin) * this._pixPerScale)).attr({fill: 'none', stroke: 'rgb(0,0,0)',strokeWidth: 1});
                }
            }
        }

        // **************************
        // 折れ線、サイクルポイント
        // **************************
        // 折れ線
        this._paper.polyline(cycleTime).attr({fill: 'none', stroke: 'rgb(0,0,0)', strokeWidth: 1, class: 'ct-c ct-pos'});

        // ポイント
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            let circleObj;
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }
            // プロット列幅
            let plotPoint = this._basisPoint.x +  (plotColumnWidth / 2) + (plotColumnWidth * plotIndex);
            // ポイント
            if(this._data[this._plotStart + plotIndex]["IsException"] == 1)
            {
                // 異常
                circleObj = this._paper.circle(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale), 8).attr({fill: 'rgb(255,255,255)', stroke: 'rgb(255,0,0)', class: 'ct-b ct-pos cycle-plot-point', id:'ct-pos-' + String(this._data[this._plotStart + plotIndex]["Number"]), value:JSON.stringify(this._data[this._plotStart + plotIndex])});
            }
            else
            {
                // 正常
                circleObj = this._paper.circle(plotPoint, this._basisPoint.y - ((this._data[this._plotStart + plotIndex]["CycleTime"] - this._scaleMin) * this._pixPerScale), 8).attr({fill: 'rgb(231,230,230)', stroke: 'rgb(0,0,0)', class: 'ct-b ct-pos cycle-plot-point', id:'ct-pos-' + String(this._data[this._plotStart + plotIndex]["Number"]), value:JSON.stringify(this._data[this._plotStart + plotIndex])});
            }
            // ツールチップ
            let title = Snap.parse('<title >' + this._data[this._plotStart + plotIndex]["Title"] + '</title>');
            circleObj.append(title);
        }

        return plotColumnWidth * this._plotDisplayNum;
    }
//#endregion 2022/05/23 西部追加
}

//=====================================================================================================
/**
 * 棒グラフクラス
 *
 * @extends {GraphBase} グラフベースクラス
 */
class PsGraph extends GraphBase
{
    /**
     * コンストラクタ
     * @param {string} selector  SVG要素のセレクタ文字列
     */
    constructor(selector)
    {
        super(selector);

        // 公開メソッド
        // プロット列幅の端から棒グラフの端までのサイズ
        this.spaceWidth = 25;

        //--------------------------------------------------
        // 指標 setIndexLineで設定
        //--------------------------------------------------
        // 指標タイプ
        this._type = undefined;
        // 値
        this._value = undefined;
        // 表示/非表示
        this._display = undefined;

        //--------------------------------------------------
        // 棒グラフパターン
        //--------------------------------------------------
        // パターン：付帯
        this._paper.image().attr({href: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAPklEQVQoU4XQ0QoAIAhD0ev/f3RhUJQ59XWHDTRgAIY+z0vg+UIdPMgL1eSDVOOHMpiiCCW6YYk2bFGE5dMntcgMCLmUuPAAAAAASUVORK5CYII="})
                    .pattern(0, 0, 10, 10).attr({id:"pattern-futai"}).toDefs();
        // パターン：段取り
        this._paper.image().attr({href: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAKElEQVQoU2NkwAT/GRgYGNGE/6MLgOTpqBBkFUEwwG5Edx/O4CHKMwCalBIB0b7ALQAAAABJRU5ErkJggg=="})
                    .pattern(0, 0, 10, 10).attr({id:"pattern-dandori"}).toDefs();
        // パターン：HT
        this._paper.image().attr({href: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAKElEQVQoU2NkYGD4z4AHMCLJwRTCxMB8EIegCVh1QjUTZwLMGTRyAwAmQAwDuicqNwAAAABJRU5ErkJggg=="})
                    .pattern(0, 0, 8, 8).attr({id:"pattern-ht"}).toDefs();
    }

    /**
     * グラフの更新
     *
     */
    update(startIndex)
    {
        // 目盛設定
        if(super.setScale() == false)
        {
            return;
        }

        // グラフ描画
        this._drowGraph(startIndex);

        // ベース描画
        super.update();
    }

    /**
     * 指標セット
     *
     * @param {number} type      指標タイプ
     * @param {number} value     値
     * @param {number} display   表示/非表示
     * @memberof PsGraph
     */
    setIndexLine(type, value, display)
    {
        // 指標タイプ
        this._type = type;
        // 値
        this._value = value;
        // 表示/非表示
        this._display = display;

        // 指標の表示/非表示
        if(this._display == 0)
        {
            this._indexLine();
        }
    }

    /**
     * 棒グラフの描画
     *
     */
    _drowGraph(startIndex)
    {
        // プロット列幅に均等割
        let plotColumnWidth = (this._xAxisWidth - (this._yAxisWidth / 2)) / this._plotDisplayNum;
        // **************************
        // プロット
        // **************************
        for(let plotIndex = 0; plotIndex < this._plotDisplayNum; plotIndex++)
        {
            // 表示データがない時、処理終了
            if(this._data.length <= this._plotStart + plotIndex)
            {
                break;
            }

            // グラフタイプ確認
            if(!(this._data[this._plotStart + plotIndex]["GraphType"] == 0) && !(this._data[this._plotStart + plotIndex]["GraphType"] == 1) && !(this._data[this._plotStart + plotIndex]["GraphType"] == 2))
            {
                break;
            }

            // プロットのグループ化
            let idName = 'ps-' + plotIndex;
            let graphGroup = this._paper.g().attr({class: "ps-bar-class", id: idName});

            // プロット列幅ポイント
            let plotWidthPoint = this._basisPoint.x + (this._yAxisWidth / 2) +  (plotColumnWidth * plotIndex);

            // **************************
            // ネック(背景色)
            // **************************
            if(this._data[this._plotStart + plotIndex]["Neck"] == 1)
            {
                // プロット塗りつぶし(ネック)
                graphGroup.rect(plotWidthPoint, this._basisPoint.y - this._yAxisHeight, plotColumnWidth, this._yAxisHeight).attr({fill: 'rgb(255,235,255)', stroke: 'none'});
            }
            else
            {
                // プロット塗りつぶし(通常)
                graphGroup.rect(plotWidthPoint, this._basisPoint.y - this._yAxisHeight, plotColumnWidth, this._yAxisHeight).attr({fill: 'rgb(255,255,255)', stroke: 'none'});
            }

            if(this._data[this._plotStart + plotIndex]["GraphType"] == 0 || this._data[this._plotStart + plotIndex]["GraphType"] == 1)
            {
                // **************************
                // 実績・計画
                // ************************** 
                this._planResultGraph(this._plotStart + plotIndex, plotWidthPoint, plotColumnWidth, this._basisPoint.y, graphGroup);
                var widthPoint = plotWidthPoint + (plotColumnWidth / 2);
                this._paper.text(widthPoint-5, 240, startIndex).attr({fill: 'rgb(0,0,0)',fontSize: this._fontSize});
                graphGroup.attr({class: "pile-ps-bar-class ps-bar-class", id: idName});
                startIndex++;
            }
            else if(this._data[this._plotStart + plotIndex]["GraphType"] == 2)
            {
                // **************************
                // ネックMCT
                // **************************
                this._neckMctGraph(this._plotStart + plotIndex, plotWidthPoint, plotColumnWidth, this._basisPoint.y, graphGroup)
                graphGroup.attr({class: "ps-mct-bar-class"});
            }
            
            //　西部変更
            // // **************************
            // // トリガー設定
            // // **************************
            // $('#'+ idName).on('click', '*', {key: this._plotStart}, function(e)
            // {
            //     $(this).trigger('psclick', e.data.key + plotIndex);
            // });
            // **************************
            // トリガー設定
            // **************************
            $('#'+ idName).on('click', '*', {key: this._plotStart}, function(e)
            {
                $(this).trigger('psclick', ".ps-bar-class");
                // $(this).trigger('psclick', e.data.key + plotIndex);
            });
        }
    }

    /**
     * 棒グラフタイプ「計画」「実績」の描画
     *
     * @param {number} plotIndex        プロットインデックス
     * @param {number} plotWidthPoint   プロット列幅ポイント
     * @param {number} plotColumnWidth  プロット列幅
     * @param {number} stackedBerYPoint 積み上げ棒グラフのy軸起点値
     * @param {String} className        クラス名（配列）
     * @param {object} graphGroup       グラフのグループ化
     */
    _planResultGraph(plotIndex, plotWidthPoint, plotColumnWidth, stackedBerYPoint, graphGroup)
    {
        // 棒グラフのX座標左側
        let xpt1 = plotWidthPoint + this.spaceWidth;
        // 棒グラフのX座標真ん中
        let xpt2 = xpt1 + ((plotColumnWidth - (this.spaceWidth * 2)) /2);
        // 棒グラフの横幅
        let stackedWidth = plotColumnWidth - (this.spaceWidth * 2)

        let dataXPoint; // x座標
        let dataYPoint; // y座標
        let rectWidth; // 横幅
        let rectHeight; // 縦幅
        let difValue; // 差分縦幅

        // 最小の棒グラフ描画
        if(this._data[plotIndex]["CTMinActive"] - this._scaleMin > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["CTMinActive"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt1;
            rectWidth  = stackedWidth;
            rectHeight = stackedBerYPoint - dataYPoint;
            if(this._data[plotIndex]["GraphType"] == 0)
            {
                // 実績
                graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-b', fill: 'rgb(255,255,204)', stroke: 'rgb(0,0,0)'});
            }
            else if(this._data[plotIndex]["GraphType"] == 1)
            {
                // 計画
                graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-a', fill: 'rgb(166,166,166)', stroke: 'rgb(0,0,0)'});
            }
        }
        
        // 最大の棒グラフ描画
        difValue = this._data[plotIndex]["CTMaxActive"] - this._data[plotIndex]["CTMinActive"];
        if(difValue > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["CTMaxActive"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt2;
            rectWidth  = stackedWidth / 2;
            rectHeight = difValue * this._pixPerScale;
            // 原点を超えた場合は、原点に合わせる
            if(dataYPoint + rectHeight > stackedBerYPoint)
            {
                rectHeight = stackedBerYPoint - dataYPoint;
            }
            graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-d', fill: 'rgb(255,255,255)', stroke: 'rgb(0,0,0)', "stroke-dasharray": '8 8'});
        }

        // 付帯の棒グラフ描画
        difValue = this._data[plotIndex]["Incidental"] - this._data[plotIndex]["CTMinActive"]
        if(difValue > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["Incidental"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt1;
            rectWidth  = stackedWidth / 2;
            rectHeight = difValue * this._pixPerScale;
            // 原点を超えた場合は、原点に合わせる
            if(dataYPoint + rectHeight > stackedBerYPoint)
            {
                rectHeight = stackedBerYPoint - dataYPoint;
            }
            graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-e', fill: 'url(#pattern-futai)', stroke: 'rgb(0,0,0)'});
        }

        // 段取りの棒グラフ描画
        difValue = this._data[plotIndex]["Setup"] - this._data[plotIndex]["Incidental"];

        if(difValue > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["Setup"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt1;
            rectWidth  = stackedWidth / 2;
            rectHeight = difValue * this._pixPerScale;
            // 原点を超えた場合は、原点に合わせる
            if(dataYPoint + rectHeight > stackedBerYPoint)
            {
                rectHeight = stackedBerYPoint - dataYPoint;
            }
            graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-f', fill: 'url(#pattern-dandori)', stroke: 'rgb(0,0,0)'});
        }

        // 平均の棒グラフ描画
        if(this._data[plotIndex]["CTAvgActive"] - this._scaleMin > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["CTAvgActive"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt2 + (stackedWidth / 4);
            graphGroup.circle(dataXPoint, dataYPoint, 4).attr({class: 'ps-c', fill: 'rgb(0,0,0)'});
        }
    }

    /**
     * 棒グラフタイプ　「ネックMCT」の描画
     * 
     * @param {number} plotIndex        プロットインデックス
     * @param {number} plotWidthPoint   プロット列幅ポイント
     * @param {number} plotColumnWidth  プロット列幅
     * @param {number} stackedBerYPoint 積み上げ棒グラフのy軸起点値
     * @param {String} className        クラス名（配列）
     * @param {object} graphGroup       グラフのグループ化 
     */
    _neckMctGraph(plotIndex, plotWidthPoint, plotColumnWidth, stackedBerYPoint, graphGroup)
    {
        // 棒グラフのX座標左側
        let xpt1 = plotWidthPoint + this.spaceWidth;
        // 棒グラフの横幅
        let stackedWidth = plotColumnWidth - (this.spaceWidth * 2)

        let dataXPoint; // x座標
        let dataYPoint; // y座標
        let rectWidth; // 横幅
        let rectHeight; // 縦幅
        let difValue; // 差分縦幅

        // MTの棒グラフ描画
        if(this._data[plotIndex]["NeckMT"] - this._scaleMin > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["NeckMT"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt1;
            rectWidth  = stackedWidth;
            rectHeight = stackedBerYPoint - dataYPoint;
            graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-h', fill: 'rgb(0,0,0)', stroke: 'rgb(0,0,0)'});
        }

        // HTの棒グラフ描画
        difValue = this._data[plotIndex]["NeckHT"] - this._data[plotIndex]["NeckMT"];
        if(difValue > 0)
        {
            dataYPoint = stackedBerYPoint - (this._data[plotIndex]["NeckHT"] - this._scaleMin) * this._pixPerScale;
            dataXPoint = xpt1;
            rectWidth  = stackedWidth;
            rectHeight = difValue * this._pixPerScale;
            // 原点を超えた場合は、原点に合わせる
            if(dataYPoint + rectHeight > stackedBerYPoint)
            {
                rectHeight = stackedBerYPoint - dataYPoint;
            }
            graphGroup.rect(dataXPoint, dataYPoint, rectWidth, rectHeight).attr({class: 'ps-g', fill: 'url(#pattern-ht)', stroke: 'rgb(0,0,0)'});
        }
    }

    /**
     * 指標値の描画
     *
     */
    _indexLine()
    {
        // 指標タイプ
        var _color;
        switch(this._type)
        {
             // 出来高ピッチ
            case 0:
                _color = 'rgb(0,176,80)';
                break;
            // ネックCT（加重）
            case 1:
                _color = 'rgb(0,0,255)';
                break;
            // ネックCT（個別）
            case 2:
                _color = 'rgb(0,204,255)';
                break;
            // T.T
            case 3:
                _color = 'rgb(255,0,0)';
                break;
            default:
                break;
        }
        // 指標値ポイント
        let linePosition = this._basisPoint.y - ((this._value - this._scaleMin) * this._pixPerScale)
        // 指標値の描画
        this._paper.line(this._basisPoint.x, linePosition, this._basisPoint.x + this._xAxisWidth, linePosition).attr({stroke: _color, strokeWidth: 1, class:'ps-annotation'});
    }
}

//=====================================================================================================
/**
 * ポイント(x座標とy座標のペア)クラス
 */
class Point
{
    /**
      * コンストラクタ
      * @param {numner} x X軸 
      * @param {number} y y軸
      */
     constructor(x, y)
     {
         this._x = x;
         this._y = y;
     }
 
     /**
      * x軸参照
      * @return {number} X軸
      */
     get x()
     {
         return this._x;
     }
 
     /**
      * X軸代入
      * @param {numner} arg X軸
      */
     set x(arg)
     {
         this._x = x;
     }
 
     /**
      * y軸参照
      * @return {number} y軸
      */
     get y()
     {
         return this._y;
     }
 
     /**
      * y軸代入
      * @param {numner} arg y軸
      */    
     set y(arg)
     {
         this._y = y;
     }

}