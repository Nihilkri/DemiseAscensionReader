using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemiseAscensionReader {
	class Monster {
		public static byte[] header;
		public static short nummon;
		public static short huk1;
		public static short huk2;


		public short namelen1, namelen2;
		public string name;
		public short att, def;
		public short monid;
		public short hp;
		public byte[] uk;
		public short[] res;
		public float[] abil;
		public byte[] uk2;
		public byte[] uk3;
	}
}
