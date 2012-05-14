using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib.HaloWars
{
	public enum BScenarioWorld
	{
		Undefined,

		Harvest,
		Arcadia,
		/// <summary>Shield World Interior</summary>
		SWI,
		/// <summary>Shield World Exterior</summary>
		SWE,
	};

	public enum BScenarioTeam // Not really an official enum...just documentation as I've seen the TeamIDs used
	{
		None,

		UNSC,
		Covenant,
		Flood,
		Forerunner,
	};
}