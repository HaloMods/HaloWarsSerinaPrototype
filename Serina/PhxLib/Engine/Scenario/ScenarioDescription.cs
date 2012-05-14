using System;
using System.Collections.Generic;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BGameType // TODO: move this someplace else
	{
		Skirmish,
		Campaign,
		Scenario,
	};

	public enum BMapType
	{
		Unknown,

		Final,
		Playtest,
		Development,
		Campaign,
	};
}