using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemiseAscensionReader {
	class Item {
		public static string[] types = new string[] {};

		public static byte[] header;
		public static short numitems;
		public static short huk1;
		public static short huk2;


		public short namelen1, namelen2;
		public string name;
		public short itemid;
		public short att, def;
		public int buk1;
		public short findlvl;
		public short suk1;
		public float[] abil;
		public short swings;
		public short suk2;
		public byte[] buk2;
		public byte[] buk3;
		public byte[] buk4;


	}
}
