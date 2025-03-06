using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PLOS.Core.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// コレクションオブジェクトがNULLもしくは0の時にtrueを返す
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsEmpy(this ICollection list)
        {
            if (list == null)
                return true;

            if (list.Count == 0)
                return true;

            return false;
        }

    }
}
