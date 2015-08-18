using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemiseAscensionReader {
	class Dungeon {
		public short xm, ym, zm; public int gm; public int[] offset = new int[45];
		public struct sqr {
			public short te, tw, tn, ts, tf, tc, m, g; public bool r;
		}
		public struct lev {
			public byte[] uk, uk2;
			public short sqx, sqy, l, g; public sqr[,] sq;
		} public lev[] lvs = new lev[45];
		public struct grp {
			public int type; public short id, lf, lm, gf, sm, mv, t, r, s, sq; public byte[] uk;
		} public grp[] gps = new grp[9172];
	}
}
 