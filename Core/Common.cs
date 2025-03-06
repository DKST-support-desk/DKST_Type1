using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PLOS.Core
{
    public class Common
    {
        public static string GetCommonApplicationBaseFolder()
        {
            string path = Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					PLOS.Core.ConstDef.C_SystemCompanySubFolder);

            return path;
        }

        public static string GetCommonApplicationDataFolder()
        {
            string path = Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					PLOS.Core.ConstDef.C_SystemCompanySubFolder,
					string.Format("{0} Ver.{1}", PLOS.Core.ConstDef.C_ApplicationName, PLOS.Core.ConstDef.C_SystemVersion_Sub));

            return path;
        }

		///// <summary>
		///// データの以降を保障する最後にインストールされた過去バージョンのフォルダを取得
		///// </summary>
		///// <returns></returns>
		//public static string GetLastInstallCommonApplicationDataFolder()
		//{
		//	string lastVerPath = string.Empty;

		//	foreach (string verSub in PLOS.Core.ConstDef.C_MoveSupportSystemVersion_Sub)
		//	{
		//		string path = Path.Combine(
		//				System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
		//				PLOS.Core.ConstDef.C_SystemCompanySubFolder,
		//				string.Format("{0} Ver.{1}", PLOS.Core.ConstDef.C_ApplicationName, verSub));

		//		if (Directory.Exists(path))
		//		{
		//			lastVerPath = path;
		//		}
		//	}

		//	return lastVerPath;
		//}
    }
}
