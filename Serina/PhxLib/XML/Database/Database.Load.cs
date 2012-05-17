using System.Threading.Tasks;
using System.Xml;
using Contracts = System.Diagnostics.Contracts;
using Contract = System.Diagnostics.Contracts.Contract;

using FA = System.IO.FileAccess;

namespace PhxLib.XML
{
	using PhxLib.Engine;

	partial class BDatabaseXmlSerializerBase
	{
		static bool UpdateResultWithTaskResults(ref bool r, Task<bool>[] tasks)
		{
			foreach (var task in tasks)
				r &= task.Result;

			return r;
		}
		void StreamTacticsSync(ref bool r, FA mode)
		{
			var xfi = StreamTacticsGetFileInfo(mode);
			var e = Database.Engine;

			var keys_copy = new System.Collections.Generic.List<string>(TacticsMap.Keys);
			foreach (var name in keys_copy)
			{
				xfi.FileName = name;
				r &= e.TryStreamData(xfi, mode, this, StreamTactic, xfi.FileName, BTacticData.kFileExt);
			}
		}
		void StreamTacticsAsync(ref bool r, FA mode)
		{
			var e = Database.Engine;

			var keys_copy = new System.Collections.Generic.List<string>(TacticsMap.Keys);
			var tasks = new Task<bool>[keys_copy.Count];
			int task_index = 0;
			foreach (var name in keys_copy)
			{
				var xfi = StreamTacticsGetFileInfo(mode, name);
				tasks[task_index++] = Task<bool>.Factory.StartNew((state) =>
					e.TryStreamData(xfi, mode, this, StreamTactic, (state as PhxEngine.XmlFileInfo).FileName, BTacticData.kFileExt), xfi);
			}

			UpdateResultWithTaskResults(ref r, tasks);
		}
		void StreamTactics(FA mode, bool synchronous)
		{
			bool r = true;

			if(!synchronous) StreamTacticsAsync(ref r, mode);
			else StreamTacticsSync(ref r, mode);
		}

		void StreamDataSync(ref bool r, FA mode)
		{
			var e = Database.Engine;

			r &= e.TryStreamData(BGameData.kXmlFileInfo, mode, this, StreamXmlGameData);
			r &= e.TryStreamData(BDamageType.kXmlFileInfo, mode, this, StreamXmlDamageTypes);
			r &= e.TryStreamData(BWeaponType.kXmlFileInfo, mode, this, StreamXmlWeaponTypes);
			r &= e.TryStreamData(BUserClass.kXmlFileInfo, mode, this, StreamXmlUserClasses);
			r &= e.TryStreamData(BDatabaseBase.kObjectTypesXmlFileInfo, mode, this, StreamXmlObjectTypes);
			r &= e.TryStreamData(BAbility.kXmlFileInfo, mode, this, StreamXmlAbilities);
			r &= e.TryStreamData(BProtoObject.kXmlFileInfo, mode, this, StreamXmlObjects);
			r &= e.TryStreamData(BProtoSquad.kXmlFileInfo, mode, this, StreamXmlSquads);
			r &= e.TryStreamData(BProtoPower.kXmlFileInfo, mode, this, StreamXmlPowers);
			r &= e.TryStreamData(BProtoTech.kXmlFileInfo, mode, this, StreamXmlTechs);
			r &= e.TryStreamData(BCiv.kXmlFileInfo, mode, this, StreamXmlCivs);
			r &= e.TryStreamData(BLeader.kXmlFileInfo, mode, this, StreamXmlLeaders);
		}
		void StreamDataAsync(ref bool r, FA mode)
		{
			var e = Database.Engine;

			Task<bool>[] tasks1 = {
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BGameData.kXmlFileInfo, mode, this, StreamXmlGameData)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BDamageType.kXmlFileInfo, mode, this, StreamXmlDamageTypes)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BWeaponType.kXmlFileInfo, mode, this, StreamXmlWeaponTypes)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BUserClass.kXmlFileInfo, mode, this, StreamXmlUserClasses)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BDatabaseBase.kObjectTypesXmlFileInfo, mode, this, StreamXmlObjectTypes)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BAbility.kXmlFileInfo, mode, this, StreamXmlAbilities)),
			};
			UpdateResultWithTaskResults(ref r, tasks1);

			Task<bool>[] tasks2 = {
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoObject.kXmlFileInfo, mode, this, StreamXmlObjects)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoSquad.kXmlFileInfo, mode, this, StreamXmlSquads)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoPower.kXmlFileInfo, mode, this, StreamXmlPowers)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoTech.kXmlFileInfo, mode, this, StreamXmlTechs)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BCiv.kXmlFileInfo, mode, this, StreamXmlCivs)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BLeader.kXmlFileInfo, mode, this, StreamXmlLeaders)),
			};
			UpdateResultWithTaskResults(ref r, tasks2);
		}
		void StreamData(FA mode, bool synchronous)
		{
			bool r = true;

			if (!synchronous) StreamDataAsync(ref r, mode);
			else StreamDataSync(ref r, mode);
		}

		void PreloadSync(ref bool r)
		{
			var e = Database.Engine;
			const FA k_mode = FA.Read;

			r &= e.TryStreamData(BProtoObject.kXmlFileInfo, k_mode, this, PreloadObjects);
			r &= e.TryStreamData(BProtoSquad.kXmlFileInfo, k_mode, this, PreloadSquads);

			r &= e.TryStreamData(BProtoTech.kXmlFileInfo, k_mode, this, PreloadTechs);
		}
		void PreloadAsync(ref bool r)
		{
			var e = Database.Engine;
			const FA k_mode = FA.Read;

			Task<bool>[] preload_tasks = {
				// only need to preload damage types when async loading
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BDamageType.kXmlFileInfo, k_mode, this, PreloadDamageTypes)),

				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoObject.kXmlFileInfo, k_mode, this, PreloadObjects)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoSquad.kXmlFileInfo, k_mode, this, PreloadSquads)),
				// only need to preload powers when async loading
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoPower.kXmlFileInfo, k_mode, this, PreloadPowers)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoTech.kXmlFileInfo, k_mode, this, PreloadTechs)),
			};
			UpdateResultWithTaskResults(ref r, preload_tasks);
		}
		// objects, objects_update, objectTypes, squads, squads_update, tech, techs_update, leaders, civs, powers, damageTypes
		void Preload(bool synchronous)
		{
			bool r = true;

			if (!synchronous) PreloadAsync(ref r);
			else PreloadSync(ref r);
		}

		void StreamDataUpdatesSync(ref bool r)
		{
			var e = Database.Engine;
			const FA k_mode = FA.Read;

			// In serial mode, we don't need to preload, so don't waste CPU
			r &= e.TryStreamData(BProtoObject.kXmlFileInfoUpdate, k_mode, this, StreamXmlObjectsUpdate);
			r &= e.TryStreamData(BProtoSquad.kXmlFileInfoUpdate, k_mode, this, StreamXmlSquadsUpdate);

			r &= e.TryStreamData(BProtoTech.kXmlFileInfoUpdate, k_mode, this, StreamXmlTechsUpdate);
		}
		void StreamDataUpdatesAsync(ref bool r)
		{
			var e = Database.Engine;
			const FA k_mode = FA.Read;

			Task<bool>[] preload_tasks = {
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoObject.kXmlFileInfoUpdate, k_mode, this, PreloadObjects)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoSquad.kXmlFileInfoUpdate, k_mode, this, PreloadSquads)),

				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoTech.kXmlFileInfoUpdate, k_mode, this, PreloadTechs)),
			};
			UpdateResultWithTaskResults(ref r, preload_tasks);

			Task<bool>[] update_tasks = {
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoObject.kXmlFileInfoUpdate, k_mode, this, StreamXmlObjectsUpdate)),
				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoSquad.kXmlFileInfoUpdate, k_mode, this, StreamXmlSquadsUpdate)),

				Task<bool>.Factory.StartNew(() => e.TryStreamData(BProtoTech.kXmlFileInfoUpdate, k_mode, this, StreamXmlTechsUpdate)),
			};
			UpdateResultWithTaskResults(ref r, update_tasks);
		}
		void StreamDataUpdates(bool synchronous)
		{
			bool r = true;

			if (!synchronous) StreamDataUpdatesAsync(ref r);
			else StreamDataUpdatesSync(ref r);
		}


		protected virtual void LoadCore()
		{
		}
		protected virtual void LoadUpdates(bool synchronous)
		{
			StreamDataUpdates(synchronous);
		}

		void LoadImpl(bool synchronous)
		{
			const FA k_mode = FA.Read;

			Preload(synchronous);

			PreStreamXml(k_mode);

			StreamData(k_mode, synchronous);
			LoadCore();
			StreamTactics(k_mode, synchronous);

			PostStreamXml(k_mode);
		}
		public void Load(BDatabaseXmlSerializerLoadFlags flags = 0)
		{
			AutoIdSerializersInitialize();

			bool synchronous = (flags & BDatabaseXmlSerializerLoadFlags.UseSynchronousLoading) != 0;
			LoadImpl(synchronous);
			if ((flags & BDatabaseXmlSerializerLoadFlags.LoadUpdates) != 0) LoadUpdates(synchronous);

			AutoIdSerializersDispose();
		}
	};
}