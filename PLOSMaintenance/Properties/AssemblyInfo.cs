using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("DKSTMaintenance")]
[assembly: AssemblyDescription("DENSO DKST手組版マスタメンテナンスモジュール")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DKSTMaintenance")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("57edb357-d727-46e3-a894-4359c958668e")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      リビジョン
//
// すべての値を指定するか、次を使用してビルド番号とリビジョン番号を既定に設定できます
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
// 2022.07.13 初回納品バージョン
//[assembly: AssemblyVersion("1.0.0.0")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
//// 2022.08.23 カメラの撮影設定上限数を設定ファイルに保持するよう修正
//[assembly: AssemblyVersion("1.0.0.1")]
//[assembly: AssemblyFileVersion("1.0.0.1")]
// 2022.08.30 実績生産数が、センサーが上げてきた数と同じ時に登録できない不具合の修正
[assembly: AssemblyVersion("1.0.0.2")]
[assembly: AssemblyFileVersion("1.0.0.2")]
[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
