using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DemiseAscensionReader {
	class Monster {
		public static string sorted = "";
		public static string[] types = new string[] {
			"Humanoid", "Slime", "Demon", "Devil", "Elemental", "Reptile", "Dragon", "Animal", "Insect",
			"Undead", "Water", "Giant", "Mythical", "Lycanthrope", "Thief", "Mage", "Warrior", "Indigni"};
		public static string header;
		public static short nummon;
		public static short huk1;
		public static short huk2;

		public short num;
		public short namelen1, namelen2;
		public string name;
		public short att, def;
		public short monid;
		public short hp;
		public string uk;
		public byte findlvl;
		public byte ukb;
		public short[] res;
		public float[] abil;
		public short[] spells;
		public float sizef;
		public string uk2;
		public short[] stats;
		public short type;
		public string uk3;
		public string uk4;
		public byte size;
		public string uk5;
	}
}
