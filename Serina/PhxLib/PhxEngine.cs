﻿using System;
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
			Action<KSoft.IO.XmlElementStream, FA, TContext> stream_proc, TContext ctxt,
			string ext = null)
		{
			Contract.Requires(xfi != null);
			Contract.Requires(stream_proc != null);

			System.IO.FileInfo file;
			bool result = Directories.TryGetFile(xfi.Location, xfi.Directory, xfi.FileName, out file, ext);

			if (mode == FA.Read)
			{
				if (!result) return false;

				using (var s = new KSoft.IO.XmlElementStream(file.FullName, mode, this))
				{
					SetupStream(s);
					stream_proc(s, mode, ctxt);
				}
			}
			else if (mode == FA.Write)
			{
				if(xfi.Writable) using (var s = KSoft.IO.XmlElementStream.CreateForWrite(xfi.RootName, this))
				{
					SetupStream(s);
					stream_proc(s, mode, ctxt);
					s.Document.Save(file.FullName);
				}
			}

			return true;
		}
		public bool TryStreamData(XmlFileInfo xfi, FA mode,
			Action<KSoft.IO.XmlElementStream, FA> stream_proc, string ext = null)
		{
			Contract.Requires(xfi != null);
			Contract.Requires(stream_proc != null);

			System.IO.FileInfo file;
			bool result = Directories.TryGetFile(xfi.Location, xfi.Directory, xfi.FileName, out file, ext);

			if (mode == FA.Read)
			{
				if (!result) return false;

				using (var s = new KSoft.IO.XmlElementStream(file.FullName, mode, this))
				{
					SetupStream(s);
					stream_proc(s, mode);
				}
			}
			else if (mode == FA.Write)
			{
				if(xfi.Writable) using (var s = KSoft.IO.XmlElementStream.CreateForWrite(xfi.RootName, this))
				{
					SetupStream(s);
					stream_proc(s, mode);
					s.Document.Save(file.FullName);
				}
			}

			return true;
		}
	};
}