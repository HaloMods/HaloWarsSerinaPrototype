using System;
using System.Collections.Generic;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BScenarioPlayerPlacementType
	{
		Invalid,

		Grouped,
		Consecutive, // int "Spacing" attribute
		Random,
	};

	// starting Resource KV values are the 'left over' attributes of Player...what do?
}