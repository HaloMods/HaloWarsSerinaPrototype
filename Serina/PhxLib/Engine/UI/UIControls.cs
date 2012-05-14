using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using FA = System.IO.FileAccess;

namespace PhxLib.Engine
{
	public enum BGameCommand // TODO: find a better place for this (UIGame related)
	{
		GlobalRallyPoint,
		SelectPower,
		Repair,
	};

	public enum BUIControlBaseType
	{
		X,
		Y,
		Z,
		RX,
		RY,
		RZ,
		POV,
		Slider,
		Button,
	};

	public enum BUIControlPovDir
	{
		Up,				Right,			Down,			Left,
	};

	public enum BUIControlType
	{
		StickLeftUp,	StickLeftDown,	StickLeftLeft,	StickLeftRight,
		StickRightUp,	StickRightDown,	StickRightLeft,	StickRightRight,
		DpadUp,			DpadDown,		DpadLeft,		DpadRight,
		
		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,
		ButtonStart,
		ButtonBack,
		ButtonShoulderRight,
		ButtonShoulderLeft,
		ButtonThumbLeft,
		ButtonThumbRight,
		TriggerLeft,
		TriggerRight,
		StickLeft,
		StickRight,
		Dpad,
		Triggers,
		GamepadInsert,
		GamepadRemove,
		
		KeyBackSpace,	KeyTab,		KeyEnter,		KeyShift,	KeyCtrl,	KeyAlt,		KeyPause,
		KeyEscape,		KeySpace,	KeyPageUp,		KeyPageDown,KeyEnd,		KeyHome,	KeyLeft,
		KeyUp,			KeyRight,	KeyDown,		KeyPrtSc,	KeyInsert,	KeyDelete,	Key0,
		Key1,			Key2,		Key3,			Key4,		Key5,		Key6,		Key7,
		Key8,			Key9,		KeyA,			KeyB,		KeyC,		KeyD,		KeyE,
		KeyF,			KeyG,		KeyH,			KeyI,		KeyJ,		KeyK,		KeyL,
		KeyM,			KeyN,		KeyO,			KeyP,		KeyQ,		KeyR,		KeyS,
		KeyT,			KeyU,		KeyV,			KeyW,		KeyX,		KeyY,		KeyZ,
		KeyAccent,		KeyAdd,		KeySeparator,	KeyDecimal,	KeyDivide,	KeyF1,		KeyF2,	
		KeyF3,			KeyF4,		KeyF5,			KeyF6,		KeyF7,		KeyF8,		KeyF9,
		KeyF10,			KeyF11,		KeyF12,			KeyF16,

		KeyShiftLeft,	KeyShiftRight,	KeyCtrlLeft,		KeyCtrlRight,
		KeyAlltLeft,	KeyAltRight,	KeyGreenModifier,	KeyOrangeModifier,
	};

	public enum BUIControlFunctionType
	{
		Flare,		SpeedModifier,	Start,		Back,				Translation,		Pan,
		Tilt,		Zoom,			Selection,	DoubleClickSelect,	MultiSelect,		Clear,
		DoWork,		DoWorkQueue,	Ability,	AbilityQueue,		Powers,				ResetCamera,

		AssignGroup1,	AssignGroup2,	AssignGroup3,	AssignGroup4,
		SelectGroup1,	SelectGroup2,	SelectGroup3,	SelectGroup4,
		GotoGroup1,		GotoGroup2,		GotoGroup3,		GotoGroup4,
		
		GotoBase,	GotoAlert,	GotoScout,	GotoArmy,	GotoNode,	GotoHero,	GotoRally,	GotoSelected,

		ScreenSelect,		GlobalSelect,
		ScreenSelectPrev,	ScreenSelectNext,
		GlobalSelectPrev,	GlobalSelectNext,
		ScreenCyclePrev,	ScreenCycleNext,
		TargetPrev,			TargetNext,
		SubSelectPrev,		SubSelectNext,		SubSelectSquad,		SubSelectType,	SubSelectTag,
		SubSelectSelect,	SubSelectGoto,
		ModeGoto,			ModeSubSelectRight,	ModeSubSelectLeft,	ModeGroup,		ModeFlare,
		GroupAdd,			GroupNext,			GroupPrev,			GroupGoto,
		FlareLook,			FlareHelp,			FlareMeet,			FlareAttack,	MapZoom,
		
		AttackMove,
		SetRally,
		NoAction1,			NoAction2,
		ExtraInfo,
	};
}