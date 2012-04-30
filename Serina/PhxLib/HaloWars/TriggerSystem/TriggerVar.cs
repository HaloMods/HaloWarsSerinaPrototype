using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhxLib.HaloWars
{
	public enum BChatSpeaker
	{
		Serena,
		Forge,
		Cutter,
		Voice_of_God,
		Generic_Soldiers,
		Arcadian_Police,
		Civilians,
		Anders,
		RhinoCommander,
		Spartan1,
		Spartan2,
		SpartanSniper,
		SpartanRockeLauncher,

		Covenant = Anders,
		Arbiter = RhinoCommander,

		ODST = SpartanRockeLauncher+1+1 + 1,
	};

	public enum BHUDItem
	{
		Minimap,
		Resources,
		Time,
		PowerStatus,
		Units,
		DpadHelp,
		ButtonHelp,
		Reticle,
		Score,
		UnitStats,
		CircleMenuExtraInfo,
	};

	public enum BFlashableUIItem // aka FlashableItems
	{
		Minimap,
		CircleMenu0,
		CircleMenu1,
		CircleMenu2,
		CircleMenu3,
		CircleMenu4,
		CircleMenu5,
		CircleMenu6,
		CircleMenu7,
		CircleMenuPop,
		CircleMenuPower,
		CircleMenuSupply,
		Dpad,
		DpadUp,
		DpadDown,
		DpadLeft,
		DpadRight,
		ResourcePanel,
		ResourcePanelPop,
		ResourcePanelPower,
		ResourcePanelSupply,
	};
}