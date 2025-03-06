using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLOS.Core
{

	/// <summary>
	/// 
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public delegate List<String> DriveDataHeaderCollectorDelegate(String filename);

	/// <summary>
	/// 
	/// </summary>
	public delegate ListSelectionItem ItemSelecteDelegate(System.Collections.ArrayList selections);

}
