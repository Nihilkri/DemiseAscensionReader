using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Xml.Linq;

namespace DemiseAscensionReader {
	class Spell {
		public static string[] types = new string[] {
			"Fire", "Cold", "Electrical", "Mind", "Damage",
			"Element", "Kill", "Charm", "Bind", "Heal",
			"Movement", "Banish", "Dispel", "Resistant", "Visual",
			"Magical", "Location", "Protection", "Morkal Damage", "Morkal Death",
			"Morkal Alchemy", "Morkal Healing", "Morkal Movement", "Unobtainable"};
		public static byte[] header;
		public static short numspell;
		public static short huk1;
		public static short huk2;


		public short namelen1, namelen2;
		public string name;

		public byte[] uk;
	}
}
