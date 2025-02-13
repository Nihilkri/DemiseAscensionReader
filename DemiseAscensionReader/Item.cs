using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DemiseAscensionReader {
	class Item {
		public static string sorted = "";
		public static string[] guildnames = new string[] { 
			"Art", "War", "Pal", "Nin", "Vil", "Exp", "Thi", "Bar", "Mag", "Sor", "Wlk", "Cle" };
		public static string[] types = new string[] {
			"Te-Waza", "Dagger", "Cross", "Sword", "Staff",
			"Mace", "Axe", "Hammer", "Leather Armor", 
			"Chain Armor", "Plate Armor", "Shield", "Cap", "Helmet",
			"Gloves", "Gauntlets", "Cloak", "Bracers", "Sash", 
			"Belt", "Boots", "Ring", "Amulet", "Potion", 
			"Scroll", "Tome", "Dust", "Crystal", "Rod", 
			"Stone", "Sphere", "Cube", "Artifact", "Miscellaneous", 
			"Guild Crest", "Treatise"};

		public static byte[] header;
		public static short numitems;
		public static short huk1;
		public static short huk2;

		public short num;
		public short namelen1, namelen2;
		public string name;
		public short itemid;
		public short att, def;
		public int val;
		public short findlvl;
		public short suk1;
		public float[] abil;
		public short swings;
		public short suk2;

		public short spellnum;
		public short spellID;
		public int charges;

		public int guilds;
		public string gs;
		public short uselvl;
		public float dmg;
		public short suk3;

		public short sp1;
		public short hands;
		public short type;
		public short[] res;

		public short[] req;
		public short[] mod;
		public short cursed;
		public short SL;
		public short CR;

		public float[] dmgmult;
		public short questitem;

		public short infoindex;

		public short codexspellindex;
		public string codexspellname;

		public int sp2;
		public short unique;
		public short suk4;
		public short sp3;
		public byte[] buk;
		public short sp4;


	}
	class Info {
		public static byte[] header;
		public int loc;
		public short infolen;
		public string info;
	}
}
