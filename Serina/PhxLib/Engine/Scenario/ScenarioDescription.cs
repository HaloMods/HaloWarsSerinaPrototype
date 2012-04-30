using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum GameType // TODO: move this someplace else
	{
		Skirmish,
		Campaign,
		Scenario,
	};

	public enum MapType
	{
		Unknown,

		Final,
		Playtest,
		Development,
		Campaign,
	};
}