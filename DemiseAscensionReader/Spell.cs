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
		public static short numspells;
		public static short huk1;

		public short num;
		public short namelen1;
		public short namelen2;
		public string name;
		public short spellid;
		public short type;
		public short lvl;
		public short suk1;
		public short range;
		public short suk2;
		public short mons;
		public short groups;
		public short A;
		public short B;
		public short suk3;
		public short reqstr;
		public short reqint;
		public short reqwis;
		public short reqcon;
		public short reqcha;
		public short reqdex;
		public short suk4;
		public short resist;
		public short suk5;
		public short suk6;
		public byte[] sp1; // 8 bytes
		public short[] OL;
		public byte[] sp2; // 8 bytes
		public short[] OC;
		public byte[] sp3; // 8 bytes
		public short suk7;
		public byte[] sp4; // 12 bytes
	}
}
