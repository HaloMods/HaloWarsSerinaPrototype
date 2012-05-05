using System.Xml;

using PhxLib.Engine;

namespace PhxLib.HaloWars
{
	partial class BDatabase
	{
		protected override void FixWeaponTypes()
		{
			WeaponTypes.DynamicAdd(new BWeaponType(), "Cannon");
			WeaponTypes.DynamicAdd(new BWeaponType(), "needler");
			WeaponTypes.DynamicAdd(new BWeaponType(), "HeavyNeedler");
			WeaponTypes.DynamicAdd(new BWeaponType(), "Plasma");
			WeaponTypes.DynamicAdd(new BWeaponType(), "HeavyPlasma");
		}

		// Fix float values which are in an invalid format for .NET's parsing
		static void FixObjectsXmlInvalidSingles(KSoft.IO.XmlElementStream s)
		{
			var node = XPathSelectNodeByName(s, BProtoObject.kBListParams, "unsc_veh_cobra_01");
			if (node == null) return;

			var element = node[BProtoObject.kXmlElementAttackGradeDPS] as XmlElement;

			string txt = element.InnerText;
			int idx = txt.IndexOf('.');
			if (idx != -1 && (idx = txt.IndexOf('.', idx)) != -1)
				element.InnerText = txt.Remove(idx, txt.Length - idx);
		}
		static void FixObjectsXmlInvalidFlags(KSoft.IO.XmlElementStream s)
		{
			var node = XPathSelectNodeByName(s, BProtoObject.kBListParams, "fx_proj_fldbomb_01");
			if (node == null) return;

			var nodes = node.ChildNodes;
			foreach (XmlNode element in nodes)
				if (element.Name == "Flag" && element.InnerText == "NonCollidable")
				{
					var fc = element.FirstChild;
					fc.Value = "NonCollideable";
				}
		}
		protected override void FixObjectsXml(KSoft.IO.XmlElementStream s)
		{
			FixObjectsXmlInvalidSingles(s);
			FixObjectsXmlInvalidFlags(s);
		}

		// Rename the SubType attribute to be all lowercase (subType is only uppercase for 'TurretYawRate'...)
		static void FixTechsXmlEffectsDataSubType(XmlDocument doc, XmlNode n)
		{
			const string kAttrNameOld = "subType";
			const string kAttrName = "subtype";

			string xpath = "Effects/Effect[@" + kAttrNameOld + "]";

			var elements = n.SelectNodes(xpath);

			foreach (XmlElement e in elements)
			{
				var attr_old = e.Attributes[kAttrNameOld];
				var attr = doc.CreateAttribute(kAttrName);
				attr.Value = attr_old.Value;
				e.Attributes.InsertBefore(attr, attr_old);
				e.RemoveAttribute(kAttrNameOld);
			}
		}
		// Remove non-existent ProtoTechs that are referenced by effects
		static void FixTechsXmlEffectsInvalid(KSoft.IO.XmlElementStream s, Collections.BListParams op)
		{
			string xpath = string.Format(
				"/{0}/{1}/Effects/Effect/Target",
				op.RootName, op.ElementName);

			var elements = s.Document.SelectNodes(xpath);

			foreach(XmlElement e in elements)
			{
				if (e.InnerText != "unsc_turret_upgrade3") continue;

				var p = e.ParentNode;
				p.ParentNode.RemoveChild(p);
			}
		}
		protected override void FixTechsXml(KSoft.IO.XmlElementStream s)
		{
			var node = XPathSelectNodeByName(s, BProtoTech.kBListParams, "unsc_scorpion_upgrade3");
			if (node != null) FixTechsXmlEffectsDataSubType(s.Document, node);

			node = XPathSelectNodeByName(s, BProtoTech.kBListParams, "unsc_grizzly_upgrade0");
			if (node != null) FixTechsXmlEffectsDataSubType(s.Document, node);

			FixTechsXmlEffectsInvalid(s, BProtoTech.kBListParams);
		}

		static void FixTacticsXmlBadWeapons(KSoft.IO.XmlElementStream s, string name)
		{
			string xpath;
			XmlNodeList elements;

			xpath = "Weapon[WeaponType='plasma']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["WeaponType"].FirstChild;
					fc.Value = "Plasma";
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: fx_proj_beam_01
			xpath = "Weapon[WeaponType='ForunnerBeam']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["WeaponType"].FirstChild;
					fc.Value = "Beam";
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: pow_proj_cleansing_01, pow_proj_wave_explode_01, pow_proj_wave_lightning_01,
			// cov_inf_brutechief_01
			xpath = "Weapon[WeaponType='leaderPower']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["WeaponType"].FirstChild;
					fc.Value = "LeaderPower";
				}
				FixTacticsTraceFixEvent(name, xpath);
			}

			// see: pow_gp_orbitalbombardment
			xpath = "Weapon/DamageRatingOverride[@type='TurretBuilding']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: unsc_veh_cobra_01, cpgn_scn10_warthog_01
			xpath = "Weapon/DamageRatingOverride[@type='Unarmored']"
				+ " | Weapon/DamageRatingOverride[@type='Air']"
				+ " | Weapon/DamageRatingOverride[@type='Vehicle']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
			}

			// see: unsc_air_vulture_01
			xpath = "Weapon[AirBurstSpan='25.0f']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["AirBurstSpan"].FirstChild;
					fc.Value = fc.Value.Substring(0, fc.Value.Length-1);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: cov_inf_brute_01
			xpath = "Weapon[contains(Name, 'IncoverBrutegun')]";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["Name"].FirstChild;
					fc.Value = fc.Value.Replace("IncoverBrutegun", "InCoverBrutegun");
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: cov_inf_brutechief_01
			xpath = "Weapon[Name='stunHammer']"
				+ " | Weapon[Name='stunPullHammer']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["Name"].FirstChild;
					fc.Value = fc.Value.Replace("stun", "Stun");
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}
		}
		static void FixTacticsXmlBadActionWeaponNames(KSoft.IO.XmlElementStream s, string name)
		{
			string xpath;
			XmlNodeList elements;

			// see: fx_proj_rocket_01,02,03
			xpath = "Action[contains(Weapon, '>')]";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["Weapon"].FirstChild;
					fc.Value = fc.Value.Substring(1);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: pow_gp_rage_impact
			xpath = "Action[Weapon='RageShockwave']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: unsc_inf_cyclops_01
			xpath = "Action[Weapon='Buildingjackhammer']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["Weapon"].FirstChild;
					fc.Value = "BuildingJackhammer";
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: unsc_veh_elephant_02
			xpath = "Action[Weapon='lightCannon']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					var fc = e["Weapon"].FirstChild;
					fc.Value = "LightCannon";
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}
		}
		static void FixTacticsXmlBadActions(KSoft.IO.XmlElementStream s, string name)
		{
			FixTacticsXmlBadActionWeaponNames(s, name);
			string xpath;
			XmlNodeList elements;

			// see: cov_inf_grunt_01, cov_inf_needlergrunt_01, cov_inf_elite_01,
			// creep_inf_grunt_01, creep_inf_needlergrunt_01
			xpath = "Action[BaseDPSWeapon='InCoverPlasmaPistolAttackAction']"
				+ " | Action[BaseDPSWeapon='InCoverNeedlerAttackAction']"
				+ " | Action[BaseDPSWeapon='IcCoverPlasmaRifleAttackAction']"
				+ " | Action[BaseDPSWeapon='PlasmaPistolAttackAction']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 0)
			{
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: cov_inf_elitecommando_01, cpgn_scn10_warthog_01
			xpath = "Action[Name='GatherSupplies']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 1)
			{
				// I'm going to assume the first Action supersedes all proceeding Actions with the same name
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}

			// see: cpgn_scn10_warthog_01
			xpath = "Action[Name='Capture']";
			elements = s.Cursor.SelectNodes(xpath);
			if (elements.Count > 1)
			{
				// I'm going to assume the first Action supersedes all proceeding Actions with the same name
				foreach (XmlElement e in elements)
				{
					e.ParentNode.RemoveChild(e);
				}
				FixTacticsTraceFixEvent(name, xpath);
				return;
			}
		}
		protected override void FixTacticsXml(KSoft.IO.XmlElementStream s, string name)
		{
			FixTacticsXmlBadWeapons(s, name);
			FixTacticsXmlBadActions(s, name);
		}
	};
}