using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;

namespace DemiseAscensionReader {
	public partial class Form1 : Form {
#region Variables
	#region Graphics
		Graphics gf, gb; Bitmap gi;
		//int fx = 1536, fy = 1024, fx2 = 768, fy2 = 512;
		int fx = 1920, fy = 1080, fx2 = 960, fy2 = 540;
		bool nomouse = true;
		Brush[] br = new Brush[] {
			Brushes.Black, Brushes.Blue, Brushes.Green, Brushes.Teal,
			Brushes.Red, Brushes.Purple, Brushes.Brown, Brushes.DarkGray,
			Brushes.White, Brushes.SkyBlue, Brushes.LimeGreen, Brushes.Cyan,
			Brushes.Pink, Brushes.Magenta, Brushes.Yellow, Brushes.White};


		#endregion Graphics
		#region Files
		bool Rev = false;
		string fyl = "", dir = "", mode = ""; byte[] dat; int pos = 0;
		System.IO.FileStream io; bool changed = false;
		Dungeon map; int lv; Dungeon.Sqr csq; Dungeon.Grp cgp; Info[] mapinfo;
		Monster[] mons; int page = 0; Monster cmon; Info[] moninfo;
		Item[] items; Item citem; Info[] iteminfo;
		Spell[] spells; Spell cspell; Info[] spellinfo;
		int hexwidth = 0x40, hexheight = 0x50;
	#endregion Files

#endregion Variables
#region Events
		public Form1() {InitializeComponent();}
		private void Form1_Load(object sender, EventArgs e) {
			gi = new Bitmap(fx, fy); gb = Graphics.FromImage(gi); gf = CreateGraphics();

			gb.Clear(Color.Black);
			gb.DrawString(
				"Press O to open a file\n" + 
				"Press D to De/Encrypt a file\n\n" +
				"In dungeon mode:\n" +
				"Home/End to level 1/45\n" +
				"Page Up/Down to change levels",
				Font, Brushes.White, 0, 0);
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
				default:
					switch(mode) {
						case "": break;
						case "DEMISEDungeon":
							switch(e.KeyCode) {
								case Keys.Home: DrawMap(0); break;
								case Keys.End: DrawMap(44); break;
								case Keys.PageUp: if(lv == 0) lv = 45; DrawMap(--lv); break;
								case Keys.PageDown: if(lv == 44) lv = -1; DrawMap(++lv); break;
							} break;
						case "DEMISEMonsters":
							switch(e.KeyCode) {
								case Keys.Home: ShowMonsters(0, page); break;
								case Keys.End: ShowMonsters(Monster.nummon - 75, page); break;
								case Keys.PageUp: ShowMonsters(lv - 75, page); break;
								case Keys.PageDown: ShowMonsters(lv + 75, page); break;
								case Keys.Up: ShowMonsters(--lv, page); break;
								case Keys.Down: ShowMonsters(++lv, page); break;
								case Keys.Left: ShowMonsters(lv, --page); break;
								case Keys.Right: ShowMonsters(lv, ++page); break;
								case Keys.E: CsVMonsters(); break;
							} break;
						case "DEMISEItems":
							switch(e.KeyCode) {
								case Keys.Home: ShowItems(0, page); break;
								case Keys.End: ShowItems(Item.numitems - 75, page); break;
								case Keys.PageUp: ShowItems(lv - 75, page); break;
								case Keys.PageDown: ShowItems(lv + 75, page); break;
								case Keys.Up: ShowItems(--lv, page); break;
								case Keys.Down: ShowItems(++lv, page); break;
								case Keys.Left: ShowItems(lv, --page); break;
								case Keys.Right: ShowItems(lv, ++page); break;
							} break;
						case "DEMISESpells":
							switch(e.KeyCode) {
								case Keys.Home: ShowSpells(0, page); break;
								case Keys.End: ShowSpells(Spell.numspells - 75, page); break;
								case Keys.PageUp: ShowSpells(lv - 75, page); break;
								case Keys.PageDown: ShowSpells(lv + 75, page); break;
								case Keys.Up: ShowSpells(--lv, page); break;
								case Keys.Down: ShowSpells(++lv, page); break;
								case Keys.Left: ShowSpells(lv, --page); break;
								case Keys.Right: ShowSpells(lv, ++page); break;
							} break;
						default: // Unknown Mode
							switch(e.KeyCode) {
								case Keys.Home: ShowHex(0); break;
								case Keys.End: ShowHex(-1); break;
								case Keys.PageUp: ShowHex(lv - hexwidth * hexheight); break;
								case Keys.PageDown: ShowHex(lv + hexwidth * hexheight); break;
								case Keys.Up: ShowHex(lv - hexwidth); break;
								case Keys.Down: ShowHex(lv + hexwidth); break;
								case Keys.Left: ShowHex(lv - 0x1); break;
								case Keys.Right: ShowHex(lv + 0x1); break;
								case Keys.Oemcomma: hexwidth--; ShowHex(lv); break;
								case Keys.OemPeriod: hexwidth++; ShowHex(lv); break;
							} break;
					} break;


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
					//gf.FillRectangle(Brushes.Black, 0, 0, 990, 24);
					gf.FillRectangle(Brushes.Black, 1032, 22, 888, 1058);

					gf.DrawString(
						"mXY: (" + e.X + ", " + e.Y + ")" +
						"\nXYZ: (" + (x+1) + "," + (y+1) + "," + (lv+1) + ")" +
						"\nTE: " + csq.te +
						"\nTW: " + csq.tw +
						"\nTN: " + csq.tn +
						"\nTS: " + csq.ts +
						"\nTF: " + csq.tf +
						"\nTC: " + csq.tc +
						"\nM: " + csq.m +
						"\nG: " + csq.g +
						"\nR: " + csq.r
						, Font, Brushes.White, 1050, 40);
					string laired = cgp.lm.ToString();
					if(laired == "-1") {
						laired = "No lair";
					} else {
						if(!(mons is null)) {
							for(int i = 0; i < mons.Length; i++) {
								if(mons[i].monid == cgp.lm) {
									laired = mons[i].name; break;
								}
							}
						}
					}
							gf.DrawString(
						//"Types: " + cgp.type.ToString("X8") +
						"Types: " + abandc(BitList(cgp.type, Monster.types)) +
						"\nGID: " + cgp.id +
						"\nLF: " + cgp.lf +
						"\nLM: " + laired +
						"\nGF: " + cgp.gf +
						"\nSM: " + cgp.sm +
						"\nMV: " + cgp.mv +
						"\nT: " + cgp.t +
						"\nR: " + cgp.r +
						"\nS: " + cgp.s +
						"\nUK: " + HexStr(cgp.uk) +
						"\nSQ: " + cgp.sq
						, Font, Brushes.White, 1050, 540  );
					//gf.DrawString(
					//	"00        01        02        03        04        " +
					//	"05        06        07        08        09        " +
					//	"10        11        12        13        14        " +
					//	"15        16        17        18        19        " +
					//	"20        21        22        23        24        " +
					//	"25        26        27        28        29        " +
					//	"30        31        32        33        34        "
					//	, Font, Brushes.White, 0, 20);

					break;

			}
		}
#endregion Events
#region Functions
		public void Open(bool c = true) {
			bool cancel = false;
			string InsD = @"C:\Games\Ascension Reader Files";
			OpenFileDialog ofd = new OpenFileDialog() {
				Filter = "Demise files (*.DED)|*.DED|Server files (*.MIS)|*.MIS",
				InitialDirectory = InsD };
			switch(ofd.ShowDialog()) {
				case System.Windows.Forms.DialogResult.OK:
					if(fyl != "") CloseFyl(); fyl = ofd.FileName;
					io = new System.IO.FileStream(
						fyl,System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite);
					dat = new byte[io.Length]; io.Read(dat, 0, (int)io.Length);
					break;
				case System.Windows.Forms.DialogResult.Cancel:
					cancel = true;
					MessageBox.Show("You hit cancel! " + fyl + " remains open!"); break;
			}
			ofd.Dispose(); if(cancel || fyl == "") return;

			// Datafile selector
			dir = fyl.Substring(0, fyl.LastIndexOf('\\') + 1);
			mode = fyl.Substring(fyl.LastIndexOf('\\') + 1, fyl.Length - fyl.LastIndexOf('\\') - 5);
			Rev = mode.StartsWith("REV", true, System.Globalization.CultureInfo.CurrentCulture);
			MessageBox.Show("Rev " + Rev);
			if(c) Crypt(); else { MessageBox.Show("You 'crypted " + fyl + "!"); CloseFyl(false);}
			if(c) MessageBox.Show("You opened " + fyl + "!");
			//mode += "Hex";
			switch(mode) {
				case "RevDungeon": mode = "DEMISEDungeon"; break;
				case "RevMonsters": mode = "DEMISEMonsters"; break;
				case "RevItems": mode = "DEMISEItems"; break;
				case "RevSpells": mode = "DEMISESpells"; break;
				case "RevInfoSpell": mode = "DEMISEInfoSpell"; break;

			}
			switch(mode) {
				case "DEMISEDungeon": MessageBox.Show("Loading the map!"); LoadMap(); break;
				case "DEMISEMonsters": MessageBox.Show("Loading the monsters!"); LoadMonsters(); break;
				case "DEMISEItems": MessageBox.Show("Loading the items!"); LoadItems(); break;
				case "DEMISESpells": MessageBox.Show("Loading the spells!"); LoadSpells(); break;
				case "DEMISEInfoSpell": if(spellinfo is null) {
						MessageBox.Show("Spells not found! Load Spells first!");
						mode += "hex"; ShowHex(0); break; } mode = "DEMISESpells";
					MessageBox.Show("Loading the spell info!"); LoadSpellInfo(); break;
				default: MessageBox.Show(mode); ShowHex(0); break;
			}
		}
		public void CloseFyl(bool c = true) {
			if(fyl == "") return; nomouse = true;
			switch(mode) {
				case "DEMISEMonsters":
					if(Monster.sorted != "num") { Monster.sorted = "num";
						mons = mons.OrderBy(x => x.num).ToArray();
					}
					break;
				case "DEMISEItems":
					if(Item.sorted != "num") { Item.sorted = "num";
						items = items.OrderBy(x => x.num).ToArray();
					}
					break;
				case "DEMISESpells":
					if(Spell.sorted != "num") { Spell.sorted = "num";
						spells = spells.OrderBy(x => x.num).ToArray();
					} break;
			}
			if(changed || !c) {
				io.Position = 0; if(dat.Length != io.Length) { MessageBox.Show("File length changed!"); } else { if(!c)Crypt(); io.Write(dat, 0, dat.Length); }
			} else MessageBox.Show(fyl + " not changed!");
			io.Close(); dat = null; changed = false; mode = "";
			if(c) MessageBox.Show("You closed " + fyl + "!"); fyl = "";

		}
		byte[] kiiAsc = new byte[] { 0x1E, 0x2E, 0x9D, 0xF4, 0xCE, 0x38, 0xB0, 0xC6 };
		byte[] kiiRev = new byte[] { 0xE7, 0xED, 0x4F, 0x43, 0xDf, 0x3D,
			0xEC, 0xE0, 0xEF, 0x88, 0x7E, 0xC9, 0x26 };
		//byte[] kiiRev = new byte[] { 0x00 };
		public void Crypt() {
			byte[] kii; if(Rev) kii = kiiRev; else kii = kiiAsc;
			if(fyl == "") return;
			for(int q=0 ; q < dat.Length ; q++) {
				dat[q] = (byte)(dat[q] ^ kii[q % kii.Length]);
			}
		}
#endregion Functions
		#region IO
		public byte[] ReadBytes(int l) {
			byte[] o = new byte[l]; for(int q = 0 ; q < l ; q++) o[q] = dat[pos + q]; pos += l; return o;
		}
		public byte ReadByte() {
			return (dat[pos++]);// == 0) ? false : true;
		}
		public short ReadShort() {
			return (short)(dat[pos++] + ((1 << 8) * dat[pos++]));
		}
		public int ReadInt() {
			return dat[pos++] + ((1 << 8) * dat[pos++]) + ((1 << 16) * dat[pos++]) + ((1 << 24) * dat[pos++]);
		}
		public string ReadString(int l) {
			char[] o = new char[l];
			for(int q = 0; q < l; q++)
				o[q] = (char)dat[pos + q];
			pos += l; return new string(o);
		}
		public float ReadFloat() {
			//byte[] b = { dat[pos+3], dat[pos+2], dat[pos+1], dat[pos] };
			//Single f = BitConverter.ToSingle(b, 0);
			Single f = BitConverter.ToSingle(dat, pos);
			pos += 4; return f;
		}

		public string HexStr(byte[] b) {
			string s = "";
			for (int i = 0; i < b.Length; i++) { s += b[i].ToString("X2"); }
			return s;
		}

		public string[] BitList(int bits, string[] list) {
			List<string> col = new List<string>();
			for(int i = 0; i < list.Length; i++) {
				if ((bits & 1 << i) != 0) col.Add(list[i]);
			}
			return col.ToArray();
		}

		public string abandc(string[] a) {
			string s = "";
			switch(a.Length) {
				case 0:
					s = ""; break;
				case 1:
					s = a[0]; break;
				case 2:
					s = a[0] + " and " + a[1]; break;
				default:
					for (int i = 0; i < a.Length - 1; i++) {
						s += a[i] + ", ";
					} s += "and " + a[a.Length - 1];
					break;
			}
			return s;
		}

		#endregion IO
		#region Dungeon
		public void LoadMap() {
			ByteConverter bc = new ByteConverter(); pos = 0;
			map = new Dungeon();
			map.header = ReadBytes(18);
			map.zm = ReadShort(); // Max z coordinate of level, always 45
			map.xm = ReadShort(); // Max x coordinate of level, always 90
			map.ym = ReadShort(); // Max y coordinate of level, always 90
			map.gm = ReadInt(); // Max encounter groups of dungeon, always 9172
			MessageBox.Show("map.gm = " + map.gm);
			map.lvs = new Dungeon.Lev[map.zm];
			map.offset = new int[map.zm];
			for (int q = 0 ; q < map.zm; q++)
				map.offset[q] = ReadInt() + 11;
			for(int q = 0 ; q < map.zm; q++) {
				if(pos != map.offset[q]) MessageBox.Show(q + ": " + pos + ", " + map.offset[q]);
				map.lvs[q].uk = ReadBytes(8); // Unknown1, always 0s
				map.lvs[q].sqx = ReadShort(); // The width of the level, always 90
				map.lvs[q].sqy = ReadShort(); // The height of the level, always 90
				map.lvs[q].l = ReadShort();   // The number of the level
				map.lvs[q].g = ReadShort();   // The number of monster encounter groups on this level
				map.lvs[q].uk2 = ReadBytes(170); // Unknown2, always 0s
				for (int i = 0; i < map.lvs[q].uk2.Length; i++)
					if (map.lvs[q].uk2[i] != 0)
						MessageBox.Show("map.lvs[" + q + "].uk2[" + i + "] is " + map.lvs[q].uk2[i]);
				map.lvs[q].sq = new Dungeon.Sqr[map.lvs[q].sqx, map.lvs[q].sqy];
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
						map.lvs[q].sq[x, y].r = ReadByte();
					}
				}
			}
			map.gps = new Dungeon.Grp[map.gm];
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
				map.gps[q].sq = ReadShort();   // Number of map squares in the encounter
				// Nonrock?
				if (map.gps[q].sm != 0 && map.gps[q].sm != 11) {
					MessageBox.Show("Non0,11 sm found! Group " + q);
				}

			}
			//MessageBox.Show("Door? " + map.lvs[0].sq[3, 0].te);
			MessageBox.Show("Map loaded! Drawing the map!"); 
			//MessageBox.Show(map.xm + "\n" + map.ym + "\n" + map.gm + "\n" + map.offset[0] + "\n" + map.offset[1] + "\n" + map.offset[2]);
			DrawMap(0); nomouse = false;
		}
		public void DrawMap(int nv) {
			lv = nv;
			gb.Clear(Color.Gray); int x1, y1, x2, y2;
			int em=0, wm=0, nm=0, sm=0, cm=0, fm=0;
			for(int x = 0 ; x < map.lvs[lv].sqx ; x++) {
				for(int y = 0 ; y < map.lvs[lv].sqy ; y++) {
					x1 = x * 11; x2 = x1 + 10; y1 = 34 + ((89 - y) * 11); y2 = y1 + 10;
					csq = map.lvs[lv].sq[x, y]; cgp = map.gps[csq.g];

					// Floor
					if(csq.tf == 0) {
						gb.FillRectangle(Brushes.Black, x1, y1, 11, 11);
						//if(lv < 40 && map.lvs[lv + 1].sq[x, y].tf == 0 && 
						//	map.lvs[lv + 2].sq[x, y].tf == 0 && (true || map.lvs[lv + 3].sq[x, y].tf == 0))
						//	gb.FillRectangle(Brushes.Red, x1 + 3, y1 + 3, 7, 7);
					}

					// Rock?
					if(csq.r>0) gb.FillRectangle(Brushes.White, x1, y1, 11, 11);
					// Lava?
					if(csq.tf == 224) gb.FillRectangle(Brushes.DarkRed, x1, y1, 11, 11);

					//if (cgp.sm > sm) sm = cgp.sm;
					//if(cgp.sm > 0) gb.FillRectangle(br[cgp.sm-1], x1 + 3, y1 + 3, 7, 7);

					if(true) {
						// Lairs
						if (cgp.lm != -1) gb.DrawRectangle(Pens.Magenta, x1 + 1, y1 + 1, 9, 9);
						// Borders between encounter groups
						if (x < 89 && (csq.g != map.lvs[lv].sq[x + 1, y].g)) gb.DrawLine(Pens.Violet, x2, y1, x2, y2);
						if (x > 0 && (csq.g != map.lvs[lv].sq[x - 1, y].g)) gb.DrawLine(Pens.Violet, x1, y1, x1, y2);
						if (y < 89 && (csq.g != map.lvs[lv].sq[x, y + 1].g)) gb.DrawLine(Pens.Violet, x1, y1, x2, y1);
						if (y > 0 && (csq.g != map.lvs[lv].sq[x, y - 1].g)) gb.DrawLine(Pens.Violet, x1, y2, x2, y2);
					}
					// Encounter Group types
					if(false && cgp.type == 0) {
						if((cgp.type & 0x8000) != 0) gb.FillRectangle(Brushes.Blue, x1 + 4, y1 + 4, 3, 3);
						if((cgp.type & 0x0040) != 0) gb.FillRectangle(Brushes.Red, x1 + 4, y1 + 4, 3, 3);
						if((cgp.type | 0x8040) == 0x8040) gb.FillRectangle(Brushes.Green, x1 + 4, y1 + 4, 3, 3);
					}

					int wcheck = 0;
					// Negative textures, maybe invisible doors?
					if (csq.te < 0) gb.DrawLine(Pens.Red, x2, y1, x2, y2);
					if (csq.tw < 0) gb.DrawLine(Pens.Red, x1, y1, x1, y2);
					if (csq.tn < 0) gb.DrawLine(Pens.Red, x1, y1, x2, y1);
					if (csq.ts < 0) gb.DrawLine(Pens.Red, x1, y2, x2, y2);
					if (csq.tf < 0) gb.FillRectangle(Brushes.Red, x1 + 2, y1 + 2, 7, 7);
					if (csq.tc < 0) gb.FillRectangle(Brushes.LightPink, x1 + 4, y1 + 4, 3, 3);
					// Walls exist
					if (csq.te > 0) gb.DrawLine(Pens.Blue, x2, y1, x2, y2); if(csq.te > em) em = csq.te;
					if (csq.tw > 0) gb.DrawLine(Pens.Blue, x1, y1, x1, y2); if(csq.tw > wm) wm = csq.tw;
					if (csq.tn > 0) gb.DrawLine(Pens.Blue, x1, y1, x2, y1); if(csq.tn > nm) nm = csq.tn;
					if (csq.ts > 0) gb.DrawLine(Pens.Blue, x1, y2, x2, y2); if (csq.ts > sm) sm = csq.ts;
					//if (csq.tf > 0) gb.FillRectangle(Brushes.Blue, x1 + 2, y1 + 2, 7, 7);
					if (csq.ts > fm) fm = csq.tf;
					//if (csq.tc > 0) gb.FillRectangle(Brushes.SkyBlue, x1 + 4, y1 + 4, 3, 3);
					if (csq.ts > cm) cm = csq.tc;
					if (true) {
						//Invisible doors?
						if (csq.te == 107) gb.DrawLine(Pens.Green, x2, y1, x2, y2);
						if (csq.tw == 107) gb.DrawLine(Pens.Green, x1, y1, x1, y2);
						if (csq.tn == 107) gb.DrawLine(Pens.Green, x1, y1, x2, y1);
						if (csq.ts == 107) gb.DrawLine(Pens.Green, x1, y2, x2, y2);
						// Doors?
						if (csq.te == 98) gb.DrawLine(Pens.Yellow, x2, y1, x2, y2);
						if (csq.tw == 98) gb.DrawLine(Pens.Yellow, x1, y1, x1, y2);
						if (csq.tn == 98) gb.DrawLine(Pens.Yellow, x1, y1, x2, y1);
						if (csq.ts == 98) gb.DrawLine(Pens.Yellow, x1, y2, x2, y2);
						//Pits?
						if (csq.tf == 106) {
							gb.DrawLine(Pens.White, x1, y1, x2, y2);
							gb.DrawLine(Pens.White, x1, y2, x2, y1);
						}
						// Nonrock?
						if (csq.r > 1)
						{
							gb.FillRectangle(Brushes.Cyan, x1, y1, 11, 11);
							MessageBox.Show("Nonrock found! " + x + ", " + y + ", " + lv);
						}
					}
				}
			}
			//MessageBox.Show(em + ", " + wm + ", " + nm + ", " + sm);
			gb.DrawString(em + ", " + wm + ", " + nm + ", " + sm, Font, Brushes.Black, 1032, 0);
			//gb.DrawString("uk: " + HexStr(map.lvs[lv].uk) + "    uk2: " + HexStr(map.lvs[lv].uk2),
			gb.DrawString(
				"sqx: " + map.lvs[lv].sqx +
				" sqy: " + map.lvs[lv].sqx +
				" l: " + map.lvs[lv].l +
				" g: " + map.lvs[lv].g,
				Font, Brushes.Black, 1032, 11);

			for(int q = 0 ; q < 45 ; q++) {
				gb.DrawString("Lv " + (q < 9 ? "0" : "") + (q + 1), Font,
					(q == lv) ? Brushes.White : Brushes.Black, 990, q * 21 + 5 + 34);
				gb.DrawLine(Pens.Black, 990, q * 21 + 34, 1032, q * 21 + 34);
			}
			gb.DrawLine(Pens.Black, 1032, 34, 1032, 1024);


			gf.DrawImage(gi, 0, 0);
		}

		#endregion Dungeon
		#region Monsters
		public void LoadMonsters() {
			int maxinfo = -1;
			ByteConverter bc = new ByteConverter(); pos = 0;
			Monster.header = HexStr(ReadBytes(18));
			Monster.huk1 = ReadShort();
			Monster.huk2 = ReadShort();
			Monster.nummon = ReadShort(); // Max number of monsters
			mons = new Monster[Monster.nummon];
			for(short mon = 0; mon < Monster.nummon; mon++) {
				mons[mon] = new Monster();
				mons[mon].num = mon;
				mons[mon].namelen1 = ReadShort();
				mons[mon].namelen2 = ReadShort();
				mons[mon].name = ReadString(mons[mon].namelen1);
				mons[mon].att = ReadShort();
				mons[mon].def = ReadShort();
				mons[mon].monid = ReadShort();
				mons[mon].hp = ReadShort();
				mons[mon].uk = HexStr(ReadBytes(6));
				mons[mon].findlvl = ReadByte();
				mons[mon].ukb = ReadByte();
				mons[mon].res = new short[12];
				for(int i = 0; i < 12; i++) mons[mon].res[i] = ReadShort();
				mons[mon].abil = new float[23];
				for(int i = 0; i < 23; i++) mons[mon].abil[i] = ReadFloat();
				mons[mon].spells = new short[24];
				for(int i = 0; i < 24; i++) mons[mon].spells[i] = ReadShort();
				mons[mon].sizef = ReadFloat();
				mons[mon].uk2 = HexStr(ReadBytes(22));
				mons[mon].stats = new short[7];
				for(int i = 0; i < 7; i++) mons[mon].stats[i] = ReadShort();
				mons[mon].type = ReadShort();
				mons[mon].uk3 = HexStr(ReadBytes(10));
				mons[mon].uk4 = HexStr(ReadBytes(26));
				mons[mon].size = ReadByte(); 
				mons[mon].uk5 = HexStr(ReadBytes(73));
			}
			//if(moninfo is null) moninfo = new Info[maxinfo];
			Monster.sorted = "num";
			//mons = mons.OrderBy(x => x.size).ToArray();
			MessageBox.Show("Monsters loaded! Printing the monsters!");
			ShowMonsters(0, 0); nomouse = false;

		}
		public void ShowMonsters(int nv, int np) {
			lv = (nv + Monster.nummon) % Monster.nummon;
			page = (np + 5) % 5;
			gb.Clear(Color.Black); Monster mon; int num; String fmt = "", s = "";
			switch(page) {
				case 0:
					fmt = "{0,3} {1,24} {2,4}/{3,4} {4,5} {5,5} " +
						"{6,3} {7,3} {8,3} {9,3} {10,3} {11,3} {12,3} {13,11} {14,3} {15,3}  " +
						"{16,16} " +
						"{17,3} {18,3} {19,3} {20,3} {21,3} {22,3} " + 
						"{23,3} {24,3} {25,3} {26,3} {27,3} {28,3}\n";
					s = String.Format(fmt, "Num", "Name", "Att", "Def", "MonID", "HP", 
						"Str", "Int", "Wis", "Con", "Cha", "Dex", "   ", "Type", "Lvl", "Ukb",
						"Unknown",
						"Fir", "Col", "Ele", "Min", "Dis", "Poi",
						"Mag", "Sto", "Par", "Dra", "Aci", "Age");
					break;
				case 1:
					fmt = "{0,3} {1,24} " +
						"{02,7} {03,7} {04,7} {05,7} {06,7}  {07,7} {08,7} {09,7} {10,7} {11,7}  " +
						"{12,7} {13,7} {14,7} {15,7} {16,7}  {17,7} {18,7} {19,7} {20,7} {21,7}  " +
						"{22,7} {23,7} {24,7}\n";
					s = String.Format(fmt, "Num", "Name",
						" SeeInv", "  Invis", " MagRes", "ChrmRes", "WeapRes",
						"ComplWR", " Unused", " Poison", "Disease", "Paralyz",
						"BrthFir", "BrthCol", "SpitAcd", "Electro", "  Drain",
						"Stone", "    Age", "CritHit", "BckStab", "DstrItm",
						"  Steal", " Behead", "Unused");
					break;
				case 2:
					fmt = "{0,3} {1,24} " +
						"{02,7} {03,7} {04,7} {05,7} {06,7}  {07,7} {08,7} {09,7} {10,7} {11,7}  " +
						"{12,7} {13,7} {14,7} {15,7} {16,7}  {17,7} {18,7} {19,7} {20,7} {21,7}  " +
						"{22,7} {23,7} {24,7} {25,7}\n";
					s = String.Format(fmt, "Num", "Name",
						"   Fire", "   Cold", " Electr", "   Mind", " Damage",
						"Element", "   Kill", "  Charm", "   Bind", "   Heal",
						"Movemnt", " Banish", " Dispel", " Resist", " Visual",
						"Magical", "Locaton", "Protect", "MDamage", " MDeath",
						"MAlchem", "  MHeal", "  MMove", "Unobtan");
					break;
				case 3:
					fmt = "{0,3} {1,24} {2,5} {3,4} {4,44} {5,28}\n";
					s = String.Format(fmt, "Num", "Name", "Sizef", "Size", "Unknown2", "Unknown3");
					break;
				case 4:
					fmt = "{0,3} {1,24} {2,52} {3,146}\n";
					s = String.Format(fmt, "Num", "Name", "Unknown4", "Unknown5");
					break;
				default:
					fmt = ""; s = "";
					break;
			}
			gb.DrawString(s, Font, Brushes.White, 0, 0);

			int fnum = lv; bool filter;
			for (int q = 0; q < 75; q++) {
        do {
					filter = true;
					num = fnum % Monster.nummon; mon = mons[num];
					for (int i = 0; i < 23; i++) {
						if((i == 2 && mon.abil[i] > 0) )// || (i != 2 && mon.abil[i] == 0))
							filter &= true; else filter &= false;
					}
					fnum++;
        } while (false && filter == false);
				if(q > 0 && fnum == lv) break;
                
				switch(page) {
					case 0:
						s = String.Format(fmt,
							num, mon.name, mon.att, mon.def, mon.monid, mon.hp,
							mon.stats[0], mon.stats[1], mon.stats[2], mon.stats[3], mon.stats[4], mon.stats[5],
							mon.stats[6], Monster.types[mon.type], mon.findlvl, mon.ukb, mon.uk,
							mon.res[0], mon.res[1], mon.res[2], mon.res[3], mon.res[4], mon.res[5], mon.res[6],
							mon.res[7], mon.res[8], mon.res[9], mon.res[10], mon.res[11]);
						break;
					case 1:
						s = String.Format(fmt,
							num, mon.name,
							mon.abil[0], mon.abil[1], mon.abil[2], mon.abil[3], mon.abil[4],
							mon.abil[5], mon.abil[6], mon.abil[7], mon.abil[8], mon.abil[9],
							mon.abil[10], mon.abil[11], mon.abil[12], mon.abil[13], mon.abil[14],
							mon.abil[15], mon.abil[16], mon.abil[17], mon.abil[18], mon.abil[19],
							mon.abil[20], mon.abil[21], mon.abil[22]);
						break;
					case 2:
						s = String.Format(fmt,
							num, mon.name,
							mon.spells[0], mon.spells[1], mon.spells[2], mon.spells[3], mon.spells[4],
							mon.spells[5], mon.spells[6], mon.spells[7], mon.spells[8], mon.spells[9],
							mon.spells[10], mon.spells[11], mon.spells[12], mon.spells[13], mon.spells[14],
							mon.spells[15], mon.spells[16], mon.spells[17], mon.spells[18], mon.spells[19],
							mon.spells[20], mon.spells[21], mon.spells[22], mon.spells[23]);
						break;
					case 3:
						s = String.Format(fmt,
							num, mon.name, mon.sizef, mon.size, mon.uk2, mon.uk3);
						break;
					case 4:
						s = String.Format(fmt,
							num, mon.name, mon.uk4, mon.uk5);
						break;
					default:
						break;
				}

				gb.DrawString(s, Font, q%2==0 ? Brushes.White : Brushes.Gray, 0, q*13+13);
			}
			gf.DrawImage(gi, 0, 0);

		}
		public void CsVMonsters() {
			string csvfyl = dir + mode + ".csv";
			using(StreamWriter writer = new StreamWriter(csvfyl))
			using(CsvWriter csv = new CsvWriter(writer,
				System.Globalization.CultureInfo.InvariantCulture)) {
				Object[] fields;
				String[] header = {
					"Num", "Name", "Att", "Def", "MonID", "HP",
						"Str", "Int", "Wis", "Con", "Cha", "Dex", "   ", "Type",
						"Lvl", "Ukb", "Unknown",
						"Fir", "Col", "Ele", "Min", "Dis", "Poi",
						"Mag", "Sto", "Par", "Dra", "Aci", "Age",
					" SeeInv", "  Invis", " MagRes", "ChrmRes", "WeapRes",
						"ComplWR", " Unused", " Poison", "Disease", "Paralyz",
						"BrthFir", "BrthCol", "SpitAcd", "Electro", "  Drain",
						"Stone", "    Age", "CritHit", "BckStab", "DstrItm",
						"  Steal", " Behead", "Unused",
					"   Fire", "   Cold", " Electr", "   Mind", " Damage",
						"Element", "   Kill", "  Charm", "   Bind", "   Heal",
						"Movemnt", " Banish", " Dispel", " Resist", " Visual",
						"Magical", "Locaton", "Protect", "MDamage", " MDeath",
						"MAlchem", "  MHeal", "  MMove", "Unobtan",
						"Unknown2", "Unknown3", "Unknown4" };
				foreach (String headerItem in header) csv.WriteField(headerItem); csv.NextRecord();
				for(int num = 0; num < Monster.nummon; num++) {
					Monster mon = mons[num];
					fields = new object[] {
						num, mon.name, mon.att, mon.def, mon.monid, mon.hp,
						mon.stats[0], mon.stats[1], mon.stats[2], mon.stats[3], mon.stats[4], mon.stats[5],
						mon.stats[6], Monster.types[mon.type], mon.findlvl, mon.ukb, mon.uk,
						mon.res[0], mon.res[1], mon.res[2], mon.res[3], mon.res[4], mon.res[5], mon.res[6],
						mon.res[7], mon.res[8], mon.res[9], mon.res[10], mon.res[11],
						mon.abil[0], mon.abil[1], mon.abil[2], mon.abil[3], mon.abil[4],
						mon.abil[5], mon.abil[6], mon.abil[7], mon.abil[8], mon.abil[9],
						mon.abil[10], mon.abil[11], mon.abil[12], mon.abil[13], mon.abil[14],
						mon.abil[15], mon.abil[16], mon.abil[17], mon.abil[18], mon.abil[19],
						mon.abil[20], mon.abil[21], mon.abil[22],
						mon.spells[0], mon.spells[1], mon.spells[2], mon.spells[3], mon.spells[4],
						mon.spells[5], mon.spells[6], mon.spells[7], mon.spells[8], mon.spells[9],
						mon.spells[10], mon.spells[11], mon.spells[12], mon.spells[13], mon.spells[14],
						mon.spells[15], mon.spells[16], mon.spells[17], mon.spells[18], mon.spells[19],
						mon.spells[20], mon.spells[21], mon.spells[22], mon.spells[23],
						mon.uk2, mon.uk3, mon.uk4 };
					foreach(Object fieldItem in fields) csv.WriteField(fieldItem); csv.NextRecord();
				}
			};
			MessageBox.Show("Monsters exported to " + csvfyl);
		}
		#endregion Monsters
		#region Items
		public void LoadItems() {
			int maxinfo = -1;
			ByteConverter bc = new ByteConverter(); pos = 0;
			Item.header = ReadBytes(18);
			MessageBox.Show("Header = " + HexStr(Item.header));
			Item.huk1 = ReadShort();
			Item.huk2 = ReadShort();
			MessageBox.Show("huk1 = " + Item.huk1 + ", huk2 = " + Item.huk2);
			Item.numitems = ReadShort(); // Max number of items
			items = new Item[Item.numitems];
			for(short item = 0; item < Item.numitems; item++) {
				items[item] = new Item();
				items[item].num = item;
				items[item].namelen1 = ReadShort();
				items[item].namelen2 = ReadShort();
				items[item].name = ReadString(items[item].namelen1);
				items[item].itemid = ReadShort();
				items[item].att = ReadShort();
				items[item].def = ReadShort();
				items[item].buk1 = ReadInt();
				items[item].findlvl = ReadShort();
				items[item].suk1 = ReadShort();
				items[item].abil = new float[17];
				for (int i = 0; i < items[item].abil.Length; i++)
					items[item].abil[i] = ReadFloat();
				items[item].swings = ReadShort();
				items[item].suk2 = ReadShort();
				items[item].spellnum = ReadShort();
				items[item].spellID = ReadShort();
				items[item].charges = ReadInt();
				items[item].guilds = ReadInt();
				items[item].uselvl = ReadShort();
				items[item].dmg = ReadFloat();
				items[item].suk3 = ReadShort();

				items[item].sp1 = ReadShort();
				items[item].hands = ReadShort();
				items[item].type = ReadShort();
				items[item].res = new short[12];
				for(int i = 0; i < items[item].res.Length; i++)
					items[item].res[i] = ReadShort();

				items[item].req = new short[7];
				for(int i = 0; i < items[item].req.Length; i++)
					items[item].req[i] = ReadShort();
				items[item].mod = new short[7];
				for(int i = 0; i < items[item].mod.Length; i++)
					items[item].mod[i] = ReadShort();
				items[item].cursed = ReadShort();
				items[item].SL = ReadShort();
				items[item].CR = ReadShort();

				items[item].dmgmult = new float[21];
				for(int i = 0; i < items[item].dmgmult.Length; i++)
					items[item].dmgmult[i] = ReadFloat();
				items[item].questitem = ReadShort();

				items[item].infoindex = ReadShort();
				if(items[item].infoindex > maxinfo)
					maxinfo = items[item].infoindex;
				items[item].codexspell = ReadShort();
				items[item].buk4 = ReadBytes(44);
			}
			if(iteminfo is null) iteminfo = new Info[maxinfo];
			Item.sorted = "num";
			//items = items.OrderBy(x => x.dmg).ToArray();
			MessageBox.Show("Items loaded! Printing the items!");
			ShowItems(0, 0); nomouse = false;

		}
		public void ShowItems(int nv, int np) {
			lv = (nv + Item.numitems) % Item.numitems;
			page = (np + 4) % 4;
			gb.Clear(Color.Black); Item item; int num; String fmt = "", s = "", codexspellname = "";
			switch(page) {
				case 0:
					fmt = "{0,3} {1,30} {2,3} {3,3} {4,4}/{5,4} " +
						"{6,8} {7,3} {8,4} " +
						"{9,4} {10,4} {11,4} {12,4} {13,4} {14,4} {15,4} {16,4} " +
						"{17,4} {18,4} {19,4} {20,4} {21,4} {22,4} {23,4} {24,4} {25,4} " +
						"{26,4} {27,4} \n";
					s = String.Format(fmt, "  #", "Name", "Num", "ID", "Att", "Def",
						"Value", "Lvl", "suk1",
						"Levi", "Invs", "Prot", "SeeI", "Crit", "Stab", "Burn", "Frez",
						"Pois", "    ", "Elec", "Ston", "Dcap", "HPrg", "SPrg", "Spel", "    ",
						"Swng", "suk2");
					break;
				case 1:
					fmt = "{0,3} {1,30} " +
						"{2,8} {3,7} {4,21} " +
						"{5,7} {6,48} {7,6} {8,8} {9,4}" +
						"{10,3} {11,5} {12,13} " +
						"{13,3} {14,3} {15,3} {16,3} {17,3} {18,3} " +
						"{19,3} {20,3} {21,3} {22,3} {23,3} {24,3} " +
						"\n";
					s = String.Format(fmt, "  #", "Name",
						"SpellNum", "SpellID", "SpellName",
						"Charges", "Guilds", "Uselvl", "Dmg", "suk3",
						"sp1", "Hands", "Type",
						"Fir", "Col", "Ele", "Min", "Dis", "Poi",
						"Mag", "Sto", "Par", "Dra", "Aci", "Age");
					break;
				case 2:
					fmt = "{0,3} {1,30} " +
						"{02,3}{03,2} {04,3}{05,2} " +
						"{06,3}{07,2} {08,3}{09,2} " +
						"{10,3}{11,-3} {12,3}{13,2}" +
						"{14,2}{15,2} " +
						"{16,6} {17,3} {18,3} {19,12} " +
						"{20,3} {21,3} {22,3} {23,3} {24,3} " +
						"{25,3} {26,3} {27,3} {28,3} {29,3} " +
						"{30,3} {31,3} {32,3} {33,3} {34,3} " +
						"{35,3} {36,3} {37,3} {38,3} {39,3} " +
						"{40,5} \n";
					s = String.Format(fmt, "  #", "Name",
						"Str", "  ", "Int", "  ",
						"Wis", "  ", "Con", "  ",
						"Cha", "   ", "Dex", "  ",
						"-", "",
						"Cursed", "SL", " CR", "DmgMult: Hum",
						"Sli", "Dem", "Dev", "Ele", "Rep",
						"Dra", "Ani", "Ins", "Und", "Wat",
						"Gia", "Myt", "Lyc", "Thi", "Mag",
						"War", "Ind", "f18", "f19", "f20",
						"Quest");
					break;
				case 3:
					fmt = "{0,3} {1,30} {2,4} {3,21} {4,88}\n";
					s = String.Format(fmt, "  #", "Name", "Info", "Codex Spell", "Unknown4");
					break;

				default:
					break;
			}
			for(int q = lv; q < lv + 75; q++) {
				num = q % Item.numitems; item = items[num];
				switch(page) {
					case 0:
						s += String.Format(fmt,
							num, item.name, item.num, item.itemid, item.att, item.def, item.buk1, item.findlvl, 
							item.suk1,
							item.abil[00], item.abil[01], item.abil[02], item.abil[03],
							item.abil[04], item.abil[05], item.abil[06], item.abil[07],
							item.abil[08], item.abil[09], item.abil[10], item.abil[11],
							item.abil[12], item.abil[13], item.abil[14], item.abil[15], item.abil[16],
							item.swings, item.suk2);
						break;
					case 1:
						string gs = "";
						for(int i = 0; i < 12; i++) {
							gs += (((item.guilds & (1 << i)) != 0) ? Item.guildnames[i] : "   ") + " ";
						}
						s += String.Format(fmt,
							num, item.name, 
							item.spellnum, item.spellID,
							(item.spellID == -1 ? "" :
								((spells is null) ? "No Spells Loaded" :
									spells[item.spellnum].name)), 
							item.charges, gs, item.uselvl, item.dmg, item.suk3,
							item.sp1, item.hands, Item.types[item.type],
							item.res[0], item.res[1], item.res[2], item.res[3], item.res[4], item.res[5],
							item.res[6], item.res[7], item.res[8], item.res[9], item.res[10], item.res[11]
							);
						break;
					case 2:
						s += String.Format(fmt,
							num, item.name,
							item.req[0], (item.mod[0] == 0 ? "  " : (item.mod[0] > 0 ? "+" : "") + item.mod[0]),
							item.req[1], (item.mod[1] == 0 ? "  " : (item.mod[1] > 0 ? "+" : "") + item.mod[1]),
							item.req[2], (item.mod[2] == 0 ? "  " : (item.mod[2] > 0 ? "+" : "") + item.mod[2]),
							item.req[3], (item.mod[3] == 0 ? "  " : (item.mod[3] > 0 ? "+" : "") + item.mod[3]),
							item.req[4], (item.mod[4] == 0 ? "  " : (item.mod[4] > 0 ? "+" : "") + item.mod[4]),
							item.req[5], (item.mod[5] == 0 ? "  " : (item.mod[5] > 0 ? "+" : "") + item.mod[5]),
							item.req[6], (item.mod[6] == 0 ? "  " : (item.mod[6] > 0 ? "+" : "") + item.mod[6]),
							item.cursed, item.SL, item.CR == 0 ? "NCR" : " CR",
							item.dmgmult[00] == 1.0 ? "_" : "" + item.dmgmult[00],
							item.dmgmult[01] == 1.0 ? "_" : "" + item.dmgmult[01],
							item.dmgmult[02] == 1.0 ? "_" : "" + item.dmgmult[02],
							item.dmgmult[03] == 1.0 ? "_" : "" + item.dmgmult[03],
							item.dmgmult[04] == 1.0 ? "_" : "" + item.dmgmult[04],
							item.dmgmult[05] == 1.0 ? "_" : "" + item.dmgmult[05],
							item.dmgmult[06] == 1.0 ? "_" : "" + item.dmgmult[06],
							item.dmgmult[07] == 1.0 ? "_" : "" + item.dmgmult[07],
							item.dmgmult[08] == 1.0 ? "_" : "" + item.dmgmult[08],
							item.dmgmult[09] == 1.0 ? "_" : "" + item.dmgmult[09],
							item.dmgmult[10] == 1.0 ? "_" : "" + item.dmgmult[10],
							item.dmgmult[11] == 1.0 ? "_" : "" + item.dmgmult[11],
							item.dmgmult[12] == 1.0 ? "_" : "" + item.dmgmult[12],
							item.dmgmult[13] == 1.0 ? "_" : "" + item.dmgmult[13],
							item.dmgmult[14] == 1.0 ? "_" : "" + item.dmgmult[14],
							item.dmgmult[15] == 1.0 ? "_" : "" + item.dmgmult[15],
							item.dmgmult[16] == 1.0 ? "_" : "" + item.dmgmult[16],
							item.dmgmult[17] == 1.0 ? "_" : "" + item.dmgmult[17],
							item.dmgmult[18] == 1.0 ? "_" : "" + item.dmgmult[18],
							item.dmgmult[19] == 1.0 ? "_" : "" + item.dmgmult[19],
							item.dmgmult[20] == 1.0 ? "_" : "" + item.dmgmult[20],
							item.questitem);
						break;
					case 3:
						if(item.codexspell == 0) {
							codexspellname = "";
						} else {
							if(spells is null) {
								codexspellname = "No Spells Loaded";
							} else {
								codexspellname = "Spell not found!";
								for(int i = 0; i < spells.Length; i++) {
									if(spells[i].spellid == item.codexspell) {
										codexspellname = spells[i].name; break;
									}
								}
							}
						}
						s += String.Format(fmt,
							num, item.name, item.infoindex, codexspellname, HexStr(item.buk4));
						break;
					default:
						break;
				}

			}
			gb.DrawString(s, Font, Brushes.White, 0, 0);
			gf.DrawImage(gi, 0, 0);

		}
		#endregion Items
		#region Spells
		public void LoadSpells() {
			int maxinfo = -1;
			ByteConverter bc = new ByteConverter(); pos = 0;
			Spell.header = ReadBytes(18);
			Spell.huk1 = ReadShort();
			Spell.numspells = ReadShort(); // Max number of spells
			//MessageBox.Show(Spell.numspells + " spells!");
			spells = new Spell[Spell.numspells];
			for(short spell = 0; spell < Spell.numspells; spell++) {
				spells[spell] = new Spell();
				spells[spell].num = spell;
				spells[spell].namelen1 = ReadShort();
				spells[spell].namelen2 = ReadShort();
				spells[spell].name = ReadString(spells[spell].namelen1);
				spells[spell].spellid = ReadShort();
				spells[spell].type = ReadShort();
				spells[spell].lvl = ReadShort();
				spells[spell].suk1 = ReadShort();
				spells[spell].range = ReadShort();
				spells[spell].suk2 = ReadShort();
				spells[spell].mons = ReadShort();
				spells[spell].groups = ReadShort();
				spells[spell].A = ReadShort();
				spells[spell].B = ReadShort();
				spells[spell].suk3 = ReadShort();
				spells[spell].reqstr = ReadShort();
				spells[spell].reqint = ReadShort();
				spells[spell].reqwis = ReadShort();
				spells[spell].reqcon = ReadShort();
				spells[spell].reqcha = ReadShort();
				spells[spell].reqdex = ReadShort();
				spells[spell].suk4 = ReadShort();
				spells[spell].resist = ReadShort();
				spells[spell].infoindex = ReadShort();
				if(spells[spell].infoindex > maxinfo)
					maxinfo = spells[spell].infoindex;
				spells[spell].suk6 = ReadShort();
				spells[spell].sp1 = ReadBytes(6);
				spells[spell].OL = new short[12];
				for (int i = 0; i < 12; i++)
					spells[spell].OL[i] = ReadShort();
				spells[spell].sp2 = ReadBytes(8);
				spells[spell].OC = new short[12];
				for(int i = 0; i < 12; i++)
					spells[spell].OC[i] = ReadShort();
				spells[spell].sp3 = ReadBytes(8);
				spells[spell].suk7 = ReadShort();
				spells[spell].sp4 = ReadBytes(12);
			}
			if(spellinfo is null) spellinfo = new Info[maxinfo + 1];
			Spell.sorted = "spellid";
			spells = spells.OrderBy(x => x.spellid).ToArray();
			MessageBox.Show("Spells loaded! Printing the spells!");
			ShowSpells(0, 0); nomouse = false;

		}
		public void LoadSpellInfo() {
			int maxinfo = -1, maxnum = -1, maxinfospell = -1;
			ByteConverter bc = new ByteConverter(); pos = 0;
			Info.header = ReadBytes(14);
			for(int info = 0; info < spellinfo.Length; info++) {
				spellinfo[info] = new Info();
				spellinfo[info].loc = ReadInt();
			}
			for(int info = 0; info < spellinfo.Length; info++) {
				spellinfo[info].infolen = ReadShort();
				if(spellinfo[info].infolen > maxinfo) {
					maxinfo = spellinfo[info].infolen;
					maxnum = info;
				}
				spellinfo[info].info = ReadString(spellinfo[info].infolen);
			}
			for(int spell = 0; spell < spells.Length; spell++) {
				if(spells[spell].infoindex == maxnum) { maxinfospell = spell; break; }
			}
			MessageBox.Show("The longest spell info is " + spells[maxinfospell].name + 
				" at " + maxinfo + " long!");
			MessageBox.Show("Spell Infos loaded! Printing the spells!");
			ShowSpells(0, 0); nomouse = false;
		}
		public void ShowSpells(int nv, int np) {
			lv = (nv + Spell.numspells) % Spell.numspells;
			page = (np + 4) % 4;
			gb.Clear(Color.Black); Spell spell; int num; String fmt = "", s = "";
			switch(page) {
				case 0:
					fmt = "{0,3} {1,30} {2,3} {3,3} {4,15} " +
						"{5,3} {6,3} {7,2} {8,2} {9,3} {10,2} " +
						"{11,3} {12,3} {13,3} {14,3} {15,3} {16,3} " +
						"{17,3} \n";
					s = String.Format(fmt, "  #", "Name", "Num", " ID", "Type",
						"lvl", "rng", "#m", "#g", "  A", " B",
						"str", "int", "wis", "con", "cha", "dex",
						"res");
					break;
				case 1:
					fmt = "{0,3} {1,30} " +
						"{02,8} {03,4} {04,4} {05,4} {06,4} {07,4} " +
						"{08,4} {09,4} {10,4} {11,4} {12,4} {13,4} " +
						"{14,12} {15,4} {16,4} {17,4} {18,4} {19,4} " +
						"{20,4} {21,4} {22,4} {23,4} {24,4} {25,4} \n";
					s = String.Format(fmt, "  #", "Name",
						"OL: Art", "War", "Pal", "Nin", "Vil", "Exp",
						"Thi", "Bar", "Mag", "Sor", "Wlk", "Cle",
						"OC: Art", "War", "Pal", "Nin", "Vil", "Exp",
						"Thi", "Bar", "Mag", "Sor", "Wlk", "Cle");
					break;
				case 2:
					fmt = "{0,3} {1,30} {2,4} {3,-466} \n";
					s = String.Format(fmt, "  #", "Name", "Info", "Informational Text");
					break;
				case 3:
					fmt = "{0,3} {1,30} {2,4} {3,4} " +
						"{4,4} {5,4} {6,4} {7,4} \n";
					s = String.Format(fmt, "  #", "Name", "suk1", "suk2",
						"suk3", "suk4", "suk6", "suk7");
					break;
				case 4:
					fmt = "{0,3} {1,30} {2,12} {3,16} {4,16} {5,24} \n";
					s = String.Format(fmt, "  #", "Name", "sp1", "sp2", "sp3", "sp4");
					break;

				default:
					break;
			}
			for(int q = lv; q < lv + 75; q++) {
				num = q % Spell.numspells; spell = spells[num];
				switch(page) {
					case 0:
						s += String.Format(fmt,
							num, spell.name, spell.num, spell.spellid, Spell.types[spell.type],
							spell.lvl, spell.range, spell.mons, spell.groups, spell.A, spell.B,
							spell.reqstr, spell.reqint, spell.reqwis, spell.reqcon, spell.reqcha, spell.reqdex,
							spell.resist);
						break;
					case 1:
						s += String.Format(fmt, num, spell.name,
							spell.OL[0], spell.OL[1], spell.OL[2], spell.OL[3], spell.OL[4], spell.OL[5],
							spell.OL[6], spell.OL[7], spell.OL[8], spell.OL[9], spell.OL[10], spell.OL[11],
							spell.OC[0], spell.OC[1], spell.OC[2], spell.OC[3], spell.OC[4], spell.OC[5],
							spell.OC[6], spell.OC[7], spell.OC[8], spell.OC[9], spell.OC[10], spell.OC[11]);
						break;
					case 2:
						s += String.Format(fmt,
							num, spell.name, spell.infoindex,
							(spellinfo[spell.infoindex] is null ? "No Info Loaded" : spellinfo[spell.infoindex].info));
						break;
					case 3:
						s += String.Format(fmt,
							num, spell.name, spell.suk1, spell.suk2,
							spell.suk3, spell.suk4, spell.suk6, spell.suk7);
						break;
					case 4:
						s += String.Format(fmt,
							num, spell.name, HexStr(spell.sp1), HexStr(spell.sp2),
							HexStr(spell.sp3), HexStr(spell.sp4));
						break;
					default:
						break;
				}

			}
			gb.DrawString(s, Font, Brushes.White, 0, 0);
			gf.DrawImage(gi, 0, 0);

		}

		#endregion Spells
		#region Unknown
		public void ShowHex(int nv) {
			lv = nv; // if(lv < 0) lv = 0;
			if(lv < 0 || lv > dat.Length - (hexwidth * hexheight))
				lv = (int)(dat.Length - (hexwidth * hexheight));
			gb.Clear(Color.Black);
			String str = "";
			byte v = 0;
			Char[] uni = new Char[hexwidth];
			byte[] bytes = new byte[hexwidth];
			for (int y = 0; y < hexheight; y++) {
				if(dat.Length < lv + y * hexwidth) break;
				str += String.Format("{0:X2} {1:X8} ", y, lv + y * hexwidth);
				for(int x = 0; x < hexwidth; x++) {
					if(dat.Length <= lv + y * hexwidth + x) {
						v = bytes[x] = (byte)0; uni[x] = ' ';
					} else {
						v = bytes[x] = dat[lv + y * hexwidth + x];
						if(v >= 0x20 && v <= 0xFF && v != 0x84) uni[x] = (char)v; else uni[x] = '.';
					}
				}
				str += HexStr(bytes) + " " + new string(uni) + "\n";
			}
			gb.DrawString(str, Font, Brushes.White, 0, 0);

			gf.DrawImage(gi, 0, 0);
		}
		#endregion Unknown

	}
}

// Modes: Map, char, etc
// Map Mode: dead end viewer, show a redder overlay the closer to a dead end a room is
// ie, the slave master room would be full red, it's a dead end. The slaver room, one away, will be slightly lesser red.
