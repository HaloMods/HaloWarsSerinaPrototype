using System;
using System.Collections.Generic;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.Collections
{
	public class BListOfIDsParams<TContext> : BListParams
		where TContext : class
	{
	};
	public class BListOfIDsParams : BListOfIDsParams<object>
	{
	};

	public class BListOfIDs<TContext> : BListBase<int>
		where TContext : class
	{
// 		BListOfIDsParams<TContext> IDsParams { get { return Params as BListOfIDsParams<TContext>; } }
// 
// 		public BListOfIDs(BListOfIDsParams<TContext> @params) : base(@params)
// 		{
// 			Contract.Requires<ArgumentNullException>(@params != null);
// 		}
	};
	public class BListOfIDs : BListOfIDs<object>
	{
// 		public BListOfIDs(BListOfIDsParams @params) : base(@params)
// 		{
// 			Contract.Requires<ArgumentNullException>(@params != null);
// 		}
	};
}