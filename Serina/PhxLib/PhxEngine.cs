using System;
using System.Collections.Generic;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;
using PhxLib.Engine;

namespace PhxLib
{
	public enum PhxEngineBuild
	{
		Alpha,
		Release,
	};
	public partial class PhxEngine
	{
		public sealed class XmlFileInfo
		{
			public ContentStorage Location { get; set; }
			public GameDirectory Directory { get; set; }
			public string FileName { get; set; }
			public string RootName { get; set; }

			public bool Writable { get; set; }
			public bool NonrequiredFile { get; set; }
		};

		public PhxEngineBuild Build { get; private set; }

		public GameDirectories Directories { get; private set; }

		public BDatabaseBase Database { get; private set; }

		public TriggerDatabase TriggerDb { get; private set; }

		public virtual void Load()
		{
			Database.Load();
		}

		static void SetupStream(KSoft.IO.XmlElementStream s)
		{
			s.IgnoreCaseOnEnums = true;
			s.ExceptionOnEnumParseFail = true;
			s.InitializeAtRootElement();
		}
		public bool TryStreamData<TContext>(XmlFileInfo xfi, FA mode,
			Action<KSoft.IO.XmlElementStream, FA, TContext> streamProc, TContext ctxt,
			string ext = null)
		{
			Contract.Requires(xfi != null);
			Contract.Requires(streamProc != null);

			System.IO.FileInfo file;
			bool result = Directories.TryGetFile(xfi.Location, xfi.Directory, xfi.FileName, out file, ext);

			if (mode == FA.Read)
			{
				if (!result) return false;

				using (var s = new KSoft.IO.XmlElementStream(file.FullName, mode, this))
				{
					SetupStream(s);
					streamProc(s, mode, ctxt);
				}
			}
			else if (mode == FA.Write)
			{
				if(xfi.Writable) using (var s = KSoft.IO.XmlElementStream.CreateForWrite(xfi.RootName, this))
				{
					SetupStream(s);
					streamProc(s, mode, ctxt);
					s.Document.Save(file.FullName);
				}
			}

			return true;
		}
		public bool TryStreamData(XmlFileInfo xfi, FA mode,
			Action<KSoft.IO.XmlElementStream, FA> streamProc, string ext = null)
		{
			Contract.Requires(xfi != null);
			Contract.Requires(streamProc != null);

			System.IO.FileInfo file;
			bool result = Directories.TryGetFile(xfi.Location, xfi.Directory, xfi.FileName, out file, ext);

			if (mode == FA.Read)
			{
				if (!result) return false;

				using (var s = new KSoft.IO.XmlElementStream(file.FullName, mode, this))
				{
					SetupStream(s);
					streamProc(s, mode);
				}
			}
			else if (mode == FA.Write)
			{
				if(xfi.Writable) using (var s = KSoft.IO.XmlElementStream.CreateForWrite(xfi.RootName, this))
				{
					SetupStream(s);
					streamProc(s, mode);
					s.Document.Save(file.FullName);
				}
			}

			return true;
		}

		public void ReadDataFilesAsync<TContext>(ContentStorage loc, GameDirectory gameDir, string searchPattern,
			Action<KSoft.IO.XmlElementStream, FA, TContext> streamProc, TContext ctxt,
			out System.Threading.Tasks.ParallelLoopResult result)
		{
			Contract.Requires(!string.IsNullOrEmpty(searchPattern));
			Contract.Requires(streamProc != null);

			result = System.Threading.Tasks.Parallel.ForEach(Directories.GetFiles(loc, gameDir, searchPattern), (filename) =>
			{
				const FA k_mode = FA.Read;

				using (var s = new KSoft.IO.XmlElementStream(filename, k_mode, this))
				{
					SetupStream(s);
					streamProc(s, k_mode, ctxt);
				}
			});
		}
		public void ReadDataFilesAsync(ContentStorage loc, GameDirectory gameDir, string searchPattern,
			Action<KSoft.IO.XmlElementStream, FA> streamProc,
			out System.Threading.Tasks.ParallelLoopResult result)
		{
			Contract.Requires(!string.IsNullOrEmpty(searchPattern));

			result = System.Threading.Tasks.Parallel.ForEach(Directories.GetFiles(loc, gameDir, searchPattern), (filename) =>
			{
				const FA k_mode = FA.Read;

				using (var s = new KSoft.IO.XmlElementStream(filename, k_mode, this))
				{
					SetupStream(s);
					streamProc(s, k_mode);
				}
			});
		}
	};
}