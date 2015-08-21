using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemiseAscensionReader {
	public partial class Form1 : Form {
#region Variables
	#region Graphics
		Graphics gf, gb; Bitmap gi;
		int fx = 1024, fy = 1024, fx2 = 512, fy2 = 512;
		bool nomouse = true;


	#endregion Graphics
	#region Files
		string fyl = "", mode = ""; byte[] dat; int pos = 0;
		System.IO.FileStream io; bool changed = false;
		Dungeon map; int lv; Dungeon.sqr csq; Dungeon.grp cgp;
	#endregion Files

#endregion Variables
#region Events
		public Form1() {InitializeComponent();}
		private void Form1_Load(object sender, EventArgs e) {
			gi = new Bitmap(fx, fy); gb = Graphics.FromImage(gi); gf = CreateGraphics();

			gb.DrawString("Press O to open a file\nPress D to De/Encrypt a file\n\nIn dungeon mode:\nHome/End to level 1/45\nPage Up/Down to change levels", Font, Brushes.Black, 0, 0);
		}

		private void Form1_Paint(object sender, PaintEventArgs e) {
			gf.DrawImage(gi, 0, 0);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {

		}

		private void Form1_KeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Escape: CloseFyl(); Close(); break;
				case Keys.O: Open(); break;
				case Keys.D: Open(false); break;

				case Keys.Home: if(mode == "DEMISEDungeon") DrawMap(0); break;
				case Keys.PageUp: if(mode == "DEMISEDungeon") { if(lv == 0) lv = 45; DrawMap(--lv); } break;
				case Keys.PageDown: if(mode == "DEMISEDungeon") { if(lv == 44) lv = -1; DrawMap(++lv); } break;
				case Keys.End: if(mode == "DEMISEDungeon") DrawMap(44); break;


			}

		}

		private void Form1_MouseClick(object sender, MouseEventArgs e) {
			int x=0, y=0; if(nomouse) return;
			switch(mode) {
				case "DEMISEDungeon":
					x = e.X - 990; y = (e.Y - 34) / 21;
					if(x < 0 || y < 0 || x > 34 || y > 44) return;
					DrawMap(y);
					break;

			}
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e) {
			int x=0, y=0; if(nomouse) return;
			switch(mode) {
				case "DEMISEDungeon":
					x = e.X / 11; y = 89-(e.Y-34)/11; 
					if(x < 0 || y < 0 || x > 89 || y > 89) return;
					csq = map.lvs[lv].sq[x, y]; cgp = map.gps[csq.g];
					gf.FillRectangle(Brushes.Black, 0, 0, 500, 30);

					gf.DrawString("XYZ: (" + (x+1) + "," + (y+1) + "," + (lv+1) + "); TE: " + csq.te + "; TW: " + csq.tw + "; TN: " + csq.tn + "; TS: " + csq.ts + 
						"; TF: " + csq.tf + "; TC: " + csq.tc + "; M: " + csq.m + "; G: " + csq.g + "; R: " + csq.r, Font, Brushes.White, 0, 0);
					gf.DrawString("Types: " + cgp.type.ToString("X8") + "; GID: " + cgp.id + "; LF: " + cgp.lf + "; LM: " + cgp.lm + "; GF: " + cgp.gf + 
						"; SM: " + cgp.sm + "; MV: " + cgp.mv + "; T: " + cgp.t + "; R: " + cgp.r + "; S: " + cgp.s + "; SQ: " + cgp.sq


						, Font, Brushes.White, 0, 10);

					break;

			}
		}
#endregion Events
#region Functions
		public void Open(bool c = true) {
			string InsD = @"E:\Shared\Demise\Ascension\";
			OpenFileDialog ofd = new OpenFileDialog() {
				Filter = "Demise files (*.DED)|*.DED|Server files (*.MIS)|*.MIS",
				InitialDirectory = InsD };
			switch(ofd.ShowDialog()) {
				case System.Windows.Forms.DialogResult.OK:
					if(fyl != "") CloseFyl();
					fyl = ofd.FileName;	io = new System.IO.FileStream(fyl,System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite);
					dat = new byte[io.Length]; io.Read(dat, 0, (int)io.Length);
					if(c) Crypt(); else { MessageBox.Show("You 'crypted " + fyl + "!"); CloseFyl(false);}
					if(c) MessageBox.Show("You opened " + fyl + "!");
					break;
				case System.Windows.Forms.DialogResult.Cancel:	
					MessageBox.Show("You hit cancel! " + fyl + " remains open!"); break;
			}
			ofd.Dispose(); if(fyl == "") return;

			// Datafile selector
			mode = fyl.Substring(fyl.LastIndexOf('\\') + 1, fyl.Length - fyl.LastIndexOf('\\') - 5);
			switch(mode) {
				case "DEMISEDungeon": MessageBox.Show("Loading the map!"); LoadMap(); break;
				default: MessageBox.Show(mode); break;
			}
		}
		public void CloseFyl(bool c = true) {
			if(fyl == "") return; nomouse = true;
			if(changed || !c) {
				io.Position = 0; if(dat.Length != io.Length) { MessageBox.Show("File length changed!"); } else { if(!c)Crypt(); io.Write(dat, 0, dat.Length); }
			} else MessageBox.Show(fyl + " not changed!");
			io.Close(); dat = null; changed = false; mode = "";
			if(c) MessageBox.Show("You closed " + fyl + "!"); fyl = "";

		}
		byte[] kii = new byte[] { 0x1E, 0x2E, 0x9D, 0xF4, 0xCE, 0x38, 0xB0, 0xC6 };
		public void Crypt() {
			//return;
			if(fyl == "") return; for(int q=0 ; q < dat.Length ; q++) { dat[q] = (byte)(dat[q] ^ kii[q % kii.Length]); }
		}
#endregion Functions
		#region IO
		public byte[] ReadBytes(int l) {
			byte[] o = new byte[l]; for(int q = 0 ; q < l ; q++) o[q] = dat[pos + q]; pos += l; return o;
		}
		public bool ReadBool() {
			return (dat[pos++] == 0) ? false : true;
		}
		public short ReadShort() {
			return (short)(dat[pos++] + ((1 << 8) * dat[pos++]));
		}
		public int ReadInt() {
			return dat[pos++] + ((1 << 8) * dat[pos++]) + ((1 << 16) * dat[pos++]) + ((1 << 24) * dat[pos++]);
		}

		#endregion IO
		#region Dungeon
		public void LoadMap() {
			ByteConverter bc = new ByteConverter(); pos = 20;
			map = new Dungeon();
			map.xm = ReadShort();
			map.ym = ReadShort();
			map.gm = ReadInt();
			for(int q = 0 ; q < 45 ; q++)
				map.offset[q] = ReadInt() + 11;
			for(int q = 0 ; q < 45 ; q++) {
				if(pos != map.offset[q]) MessageBox.Show(q + ": " + pos + ", " + map.offset[q]);
				map.lvs[q].uk = ReadBytes(8);
				map.lvs[q].sqx = ReadShort();
				map.lvs[q].sqy = ReadShort();
				map.lvs[q].l = ReadShort();
				map.lvs[q].g = ReadShort();
				map.lvs[q].uk2 = ReadBytes(170);
				map.lvs[q].sq = new Dungeon.sqr[map.lvs[q].sqx, map.lvs[q].sqy];
					for(int y = 0 ; y < map.lvs[q].sqy ; y++) {
				for(int x = 0 ; x < map.lvs[q].sqx ; x++) {
						map.lvs[q].sq[x, y].te = ReadShort();
						map.lvs[q].sq[x, y].tw = ReadShort();
						map.lvs[q].sq[x, y].tn = ReadShort();
						map.lvs[q].sq[x, y].ts = ReadShort();
						map.lvs[q].sq[x, y].tf = ReadShort();
						map.lvs[q].sq[x, y].tc = ReadShort();
						map.lvs[q].sq[x, y].m = ReadShort();
						map.lvs[q].sq[x, y].g = ReadShort();
						map.lvs[q].sq[x, y].r = ReadBool();
					}
				}
			}
			map.gps = new Dungeon.grp[map.gm];
			for(int q = 0 ; q < map.gm ; q++) {
				map.gps[q].type = ReadInt();
				map.gps[q].id = ReadShort();
				map.gps[q].lf = ReadShort();
				map.gps[q].lm = ReadShort();
				map.gps[q].gf = ReadShort();
				map.gps[q].sm = ReadShort();
				map.gps[q].mv = ReadShort();
				map.gps[q].t = ReadShort();
				map.gps[q].r = ReadShort();
				map.gps[q].s = ReadShort();
				map.gps[q].uk = ReadBytes(13);
				map.gps[q].sq = ReadShort();
			}
			MessageBox.Show("Door? " + map.lvs[0].sq[3, 0].te);
			MessageBox.Show("Map loaded! Drawing the map!"); 
			//MessageBox.Show(map.xm + "\n" + map.ym + "\n" + map.gm + "\n" + map.offset[0] + "\n" + map.offset[1] + "\n" + map.offset[2]);
			DrawMap(0); nomouse = false;
		}
		public void DrawMap(int nv) {
			lv = nv;
			gb.Clear(Color.Gray); int x1, y1, x2, y2;
			int em=0, wm=0, nm=0, sm=0;
			Brush[] br = new Brush[] { Brushes.Blue, Brushes.Green, Brushes.Cyan, Brushes.Red, Brushes.Purple, Brushes.Yellow, Brushes.DarkGray, Brushes.White, Brushes.SkyBlue, Brushes.LimeGreen, Brushes.Pink };
			for(int x = 0 ; x < map.lvs[lv].sqx ; x++) {
				for(int y = 0 ; y < map.lvs[lv].sqy ; y++) {
					x1 = x * 11; x2 = x1 + 10; y1 = 34 + ((89 - y) * 11); y2 = y1 + 10; csq = map.lvs[lv].sq[x, y]; cgp = map.gps[csq.g];

					// Floor
					if(csq.tf == 0) {
						gb.FillRectangle(Brushes.Black, x1, y1, 11, 11);
						if(lv < 40 && map.lvs[lv + 1].sq[x, y].tf == 0 && 
							map.lvs[lv + 2].sq[x, y].tf == 0 && map.lvs[lv + 3].sq[x, y].tf == 0)
							gb.FillRectangle(Brushes.Red, x1 + 3, y1 + 3, 7, 7);
					}

					// Rock?
					if(csq.r) gb.FillRectangle(Brushes.White, x1, y1, 11, 11);

					if(cgp.sm > sm) sm = cgp.sm;
					if(cgp.sm > 0) gb.FillRectangle(br[cgp.sm-1], x1 + 3, y1 + 3, 7, 7);
					if(x < 89 && (csq.g != map.lvs[lv].sq[x + 1, y].g)) gb.DrawLine(Pens.Violet, x2, y1, x2, y2);
					if(x > 0 && (csq.g != map.lvs[lv].sq[x - 1, y].g)) gb.DrawLine(Pens.Violet, x1, y1, x1, y2);
					if(y < 89 && (csq.g != map.lvs[lv].sq[x, y + 1].g)) gb.DrawLine(Pens.Violet, x1, y1, x2, y1);
					if(y > 0 && (csq.g != map.lvs[lv].sq[x, y - 1].g)) gb.DrawLine(Pens.Violet, x1, y2, x2, y2);

					if(cgp.type != 0) {
						if((cgp.type & 0x8000) != 0) gb.FillRectangle(Brushes.Blue, x1 + 4, y1 + 4, 3, 3);
						if((cgp.type & 0x0040) != 0) gb.FillRectangle(Brushes.Red, x1 + 4, y1 + 4, 3, 3);
						if((cgp.type | 0x8040) == 0x8040) gb.FillRectangle(Brushes.Green, x1 + 4, y1 + 4, 3, 3);
					}

					// Wals exist
					if(csq.te > 0) gb.DrawLine(Pens.Blue, x2, y1, x2, y2); if(csq.te > em) em = csq.te;
					if(csq.tw > 0) gb.DrawLine(Pens.Blue, x1, y1, x1, y2); if(csq.tw > wm) wm = csq.tw;
					if(csq.tn > 0) gb.DrawLine(Pens.Blue, x1, y1, x2, y1); if(csq.tn > nm) nm = csq.tn;
					if(csq.ts > 0) gb.DrawLine(Pens.Blue, x1, y2, x2, y2); //if(csq.ts > sm) sm = csq.ts;
					//Invisible doors?
					if(csq.te == 107) gb.DrawLine(Pens.Green, x2, y1, x2, y2);
					if(csq.tw == 107) gb.DrawLine(Pens.Green, x1, y1, x1, y2);
					if(csq.tn == 107) gb.DrawLine(Pens.Green, x1, y1, x2, y1);
					if(csq.ts == 107) gb.DrawLine(Pens.Green, x1, y2, x2, y2);
					// Doors?
					if(csq.te == 98) gb.DrawLine(Pens.Yellow, x2, y1, x2, y2);
					if(csq.tw == 98) gb.DrawLine(Pens.Yellow, x1, y1, x1, y2);
					if(csq.tn == 98) gb.DrawLine(Pens.Yellow, x1, y1, x2, y1);
					if(csq.ts == 98) gb.DrawLine(Pens.Yellow, x1, y2, x2, y2);
					// Negative textures, maybe invisible doors?
					if(csq.te < 0) gb.DrawLine(Pens.Red, x2, y1, x2, y2);
					if(csq.tw < 0) gb.DrawLine(Pens.Red, x1, y1, x1, y2);
					if(csq.tn < 0) gb.DrawLine(Pens.Red, x1, y1, x2, y1);
					if(csq.ts < 0) gb.DrawLine(Pens.Red, x1, y2, x2, y2);
				}
			}
			//MessageBox.Show(em + ", " + wm + ", " + nm + ", " + sm);
			gb.DrawString(em + ", " + wm + ", " + nm + ", " + sm, Font, Brushes.Black, 650, 0);

			for(int q = 0 ; q < 45 ; q++) {
				gb.DrawString("Lv " + (q + 1), Font, (q == lv) ? Brushes.White : Brushes.Black, 990, q * 21 + 5 + 34);
				gb.DrawLine(Pens.Black, 990, q * 21 + 34, fx, q * 21 + 34);

			}


				gf.DrawImage(gi, 0, 0);
		}

#endregion Dungeon

	}
}

// Modes: Map, char, etc
// Map Mode: dead end viewer, show a redder overlay the closer to a dead end a room is
// ie, the slave master room would be full red, it's a dead end. The slaver room, one away, will be slightly lesser red.
