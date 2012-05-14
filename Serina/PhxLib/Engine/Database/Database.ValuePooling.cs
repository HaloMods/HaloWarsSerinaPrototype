using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib.Engine
{
	using BCost = Collections.BTypeValuesSingle;
	using BPops = Collections.BTypeValues<BPopulation>;
	using BProtoObjectVeterancyList = Collections.BListExplicitIndex<BProtoObjectVeterancy>;

	partial class BDatabaseBase
	{
		HashSet<BCost> m_poolCosts;
		//HashSet<BPops> m_poolPops;
		HashSet<BProtoObjectVeterancyList> m_poolVeterancies;

		void InitializeValuePools()
		{
			m_poolCosts = new HashSet<BCost>();

			m_poolVeterancies = new HashSet<BProtoObjectVeterancyList>();
		}

		public bool InternTypeValues<T>(ref Collections.BTypeValuesBase<T> values)
		{
			return false;
		}
	};
}