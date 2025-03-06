using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("DKSTDeviceConnectionManager")]
[assembly: AssemblyDescription("DENSO DKST手組版デバイス通信モジュール")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DKSTDeviceConnectionManager")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("df9b4d32-199e-4543-9f9f-0538694b50d3")]

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
//// 2022.07.13 Yamasaki 初回納品バージョン
//[assembly: AssemblyVersion("1.0.0.0")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
//// 2022.07.14 Yamasaki QR読込を常時可能に修正、カメラ不使用設定時にサイクル実績を上げていない不具合の修正
//[assembly: AssemblyVersion("1.0.0.1")]
//[assembly: AssemblyFileVersion("1.0.0.1")]
//// 2022.07.14 Yamasaki 作業者情報がない(完全初期立ち上げ)時、QR読込したときにエラーが発生する不具合の修正
//[assembly: AssemblyVersion("1.0.0.2")]
//[assembly: AssemblyFileVersion("1.0.0.2")]
// 2022.07.20 Yamasaki 作業登録テーブルに編成はあるが品番タイプの登録がない時にアプリが立ち上がらない不具合の修正(詳細SVNログ参照のこと)
//[assembly: AssemblyVersion("1.0.0.3")]
//[assembly: AssemblyFileVersion("1.0.0.3")]
// 2022.08.22 Yamasaki 撮影時の動作を、カメラ非同期に修正(カメラの取込とファイルストリームへの絵積みを別処理化)
// 2023.03.31 Yamasaki 現地導入に合わせる形での更新(動画エンコード1スレッドのみ、コア割り当ての中止)
[assembly: AssemblyVersion("1.0.0.4")]
[assembly: AssemblyFileVersion("1.0.0.4")]

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
