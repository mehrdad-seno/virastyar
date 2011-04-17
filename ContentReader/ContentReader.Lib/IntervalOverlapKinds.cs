using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.Microsoft.Office.Word.ContentReader
{
    /// <summary>
    /// Enumerates all possible ways two intervals may overlap
    /// </summary>
    public enum IntervalOverlapKinds
    {
        /// <summary>
        /// (--)
        /// </summary>
        FirstInsideSecond,

        /// <summary>
        /// -()-
        /// </summary>
        FirstIncludesSecond,

        /// <summary>
        /// -(-)
        /// </summary>
        FirstBeforeAndInsideSecond,

        /// <summary>
        /// (-)-
        /// </summary>
        FirstInsideAndAfterSecond,

        /// <summary>
        /// --()
        /// </summary>
        FirstBeforeSecond,

        /// <summary>
        /// ()--
        /// </summary>
        FirstAfterSecond
    }
}
