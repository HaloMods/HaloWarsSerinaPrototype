using System;
using System.Collections.Generic;
using System.IO;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

namespace PhxLib.Engine
{
	public enum ContentStorage
	{
		UpdateOrGame,

		Game, // game://
		Update, // update://
		Images, // img://
	};
	public enum GameDirectory
	{
		Art,

		Data,
		AbilityScripts,
		AIData,
		PowerScripts,
		Tactics,
		TriggerScripts,
	};
	public class GameDirectories
	{
		#region Art
		protected const string kArtPath = @"art\";
		#endregion
		#region Data
		protected const string kDataPath = @"data\";
		protected const string kAbilitiesPath = @"abilities\";
		protected const string kAIPath = @"ai\";
		protected const string kPowersPath = @"powers\";
		protected const string kTacticsPath = @"tactics\";
		protected const string kTriggerScriptsPath = @"triggerscripts\";
		#endregion

		/*public*/ string RootDirectory { get; /*private*/ set; }
		/*public*/ string UpdateDirectory { get; /*private*/ set; }
		public bool UseTitleUpdates { get; set; }

		public GameDirectories(string root, string update_root)
		{
			RootDirectory = root;
			UpdateDirectory = update_root;
			UseTitleUpdates = true;

			Art = kArtPath;//Path.Combine(RootDirectory, kArtPath);

			Data = kDataPath;//Path.Combine(RootDirectory, kDataPath);
			Abilities = Path.Combine(Data, kAbilitiesPath);
			AI = Path.Combine(Data, kAIPath);
			Powers = Path.Combine(Data, kPowersPath);
			Tactics = Path.Combine(Data, kTacticsPath);
			TriggerScripts = Path.Combine(Data, kTriggerScriptsPath);
		}

		#region Art
		public virtual string Art { get; protected set; }
		#endregion
		#region Data
		public virtual string Data { get; protected set; }
		public virtual string Abilities { get; protected set; }
		public virtual string AI { get; protected set; }
		public virtual string Powers { get; protected set; }
		public virtual string Tactics { get; protected set; }
		public virtual string TriggerScripts { get; protected set; }
		#endregion

		public string GetContentLocation(ContentStorage location)
		{
			switch (location)
			{
				case ContentStorage.Game: return RootDirectory;
				case ContentStorage.Update: return UpdateDirectory;

				default: throw new NotImplementedException();
			}
		}
		public string GetDirectory(GameDirectory dir)
		{
			switch (dir)
			{
				#region Art
				case GameDirectory.Art: return Art;
				#endregion
				#region Data
				case GameDirectory.Data: return Data;
				case GameDirectory.AbilityScripts: return Abilities;
				case GameDirectory.AIData: return AI;
				case GameDirectory.PowerScripts: return Powers;
				case GameDirectory.Tactics: return Tactics;
				case GameDirectory.TriggerScripts: return TriggerScripts;
				#endregion

				default: throw new NotImplementedException();
			}
		}

		bool TryGetFileImpl(ContentStorage loc, GameDirectory game_dir, string filename, out FileInfo file, string ext = null)
		{
			file = null;

			string root = GetContentLocation(loc);
			string dir = GetDirectory(game_dir);
			string file_path = Path.Combine(root, dir, filename);
			if (!string.IsNullOrEmpty(ext)) file_path += ext;

			return (file = new FileInfo(file_path)).Exists;
		}
		bool TryGetFileFromUpdateOrGame(GameDirectory game_dir, string filename, out FileInfo file, string ext = null)
		{
			file = null;

			if (!UseTitleUpdates)
				return TryGetFileImpl(ContentStorage.Game, game_dir, filename, out file, ext);

			//////////////////////////////////////////////////////////////////////////
			// Try to get the file from the TU storage first
			string dir = GetDirectory(game_dir);
			string file_path = Path.Combine(dir, filename);
			if (!string.IsNullOrEmpty(ext)) file_path += ext;

			string full_path = Path.Combine(UpdateDirectory, file_path);
			file = new FileInfo(full_path);

			//////////////////////////////////////////////////////////////////////////
			// No update file exists, fall back to regular game storage
			if (!file.Exists)
			{
				full_path = Path.Combine(RootDirectory, file_path);
				file = new FileInfo(full_path);
				return file.Exists;
			}
			return true;
		}
		public bool TryGetFile(ContentStorage loc, GameDirectory game_dir, string filename, out FileInfo file, string ext = null)
		{
			Contract.Requires(!string.IsNullOrEmpty(filename));
			file = null;

			if (loc == ContentStorage.UpdateOrGame)
				return TryGetFileFromUpdateOrGame(game_dir, filename, out file, ext);
			else
				return TryGetFileImpl(loc, game_dir, filename, out file, ext);
		}
	};
}