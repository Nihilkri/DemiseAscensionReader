using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemiseAscensionReader {
	class Dungeon {
		public short xm, ym, zm; public int gm; public int[] offset;// = new int[45];
		public struct Sqr {
			public short te, tw, tn, ts, tf, tc, m, g; public byte r;
		}
		public struct Lev {
			public byte[] uk, uk2;
			public short sqx, sqy, l, g; public Sqr[,] sq;
		} public Lev[] lvs;// = new Lev[45];
		public struct Grp {
			public int type; public short id, lf, lm, gf, sm, mv, t, r, s, sq; public byte[] uk;
		} public Grp[] gps;// = new Grp[9172];
	}
}
 