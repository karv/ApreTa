using System;
using System.Collections.Generic;

namespace ApreTa
{
	public static class MainClass
	{
		static MainClass ()
		{
			Console.SetWindowSize (ColSize * NumCol, 24);
		}

		public const int NumCol = 2;
		public const int ColSize = 40;

		public static void Main (string[] args)
		{

			// Inicializar la pantalla
			Console.SetWindowSize (ColSize * NumCol, 24);
			Console.CursorVisible = false;
			//Console.BufferWidth = ColSize * NumCol;
			//Console.SetBufferSize (ColSize * NumCol, 10);
			//JRápido Jue = new JRápido();
			//Evol Ev = new Evol ();


			//Ev.Run ();

			Torneo T = new Torneo ();
			T.Run ();

		}
	}

	public class Jugador
	{
		public string Estr;
	}

	public abstract class Juego
	{

	}

	public class JRápido:Juego
	{
		public class EstadoJug
		{
			public JJuegoRápido Jug;

			public float Score {
				get {
					return Jug.Score;
				}
				set {
					Jug.Score = value;
				}
			}

			public int Juegos = 0;

			public float LS {
				get {
					return Jug.LastScore;
				}
				set {
					Jug.LastScore = value;
				}
			}
		}

		public JRápido (JJuegoRápido J1, JJuegoRápido J2)
		{
			Jug = new EstadoJug[2];
			Jug [0] = new EstadoJug ();
			Jug [1] = new EstadoJug ();
			Jug [0].Jug = J1;
			Jug [1].Jug = J2;

			H.Data = new int[2, Iteraciones];		
		}

		public EstadoJug[] Jug = new EstadoJug[2];
		public int Iteraciones = 10;
		public JJuegoRápido Ganador;
		public JJuegoRápido Perdedor;
		Historial H = new Historial ();
		// int RondaContador = 0;
		public void Run (bool Show = false)
		{
			for (int RondaContador = 0; RondaContador < Iteraciones; RondaContador++) {
				Ronda (RondaContador);
			}

			Jug [0].Score += Jug [0].LS;
			Jug [1].Score += Jug [1].LS;

			int iGan;
			if (Jug [0].Score > Jug [1].Score)
				iGan = 0;
			else
				iGan = 1;

			Ganador = Jug [iGan].Jug;
			Perdedor = Jug [1 - iGan].Jug;

			if (Show) {
				Console.WriteLine ("0: " + Jugador (0).fml);
				Console.WriteLine ("1: " + Jugador (1).fml);
				for (int j = 0; j < 2; j++) {
					Console.Write (string.Format ("{0}:\t", j));
					for (int i = 0; i < H.Data.GetLength(1); i++) {
						Console.Write (H.Data [j, i] + " | ");
					}

					Console.WriteLine (" ---> " + Jug [j].LS + "(" + Jug [j].Score + ")");
				}
				// Console.ReadLine();
			}

			Jug [0].LS = 0;
			Jug [1].LS = 0;


		}

		public JJuegoRápido Jugador (int i)
		{
			return Jug [i].Jug;
		}

		void Ronda (int RondaContador)
		{
			int a;
			int b;

			a = Jugador (0).JugarEstrategia (H, RondaContador);
			b = Jugador (1).JugarEstrategia (H.Invertir (), RondaContador);

			a = a == 0 ? 0 : 1;
			b = b == 0 ? 0 : 1;

			H.Data [0, RondaContador] = a;
			H.Data [1, RondaContador] = b;

			if (a == 0) {
				if (b == 0) {
					Jug [0].LS += -1;
					Jug [1].LS += -1;
				} else {
					Jug [0].LS += 5;
					Jug [1].LS += 0;
				}
			} else { //a == 1
				if (b == 0) {
					Jug [0].LS += 0;
					Jug [1].LS += 5;
				} else {
					Jug [0].LS += 3;
					Jug [1].LS += 3;
				}
			}
		}
	}

	public class Historial
	{
		public int[,] Data;
		/// <summary>
		/// Devuelve el "turno" actual.
		/// </summary>
		public int Actual = 0;

		public Historial Invertir ()
		{
			Historial ret = new Historial ();
			ret.Data = new int[2, Data.GetLength (1)];
			for (int i = 0; i < Data.GetLength(1); i++) {
				ret.Data [0, i] = Data [1, i];
				ret.Data [1, i] = Data [0, i];
			}
			return ret;
		}

		public void AgregaTurno (int a, int b)
		{
			Data [0, Actual] = a;
			Data [1, Actual] = b;
			Actual++;
		}
	}

	public class JJuegoRápido
	{
		public float _score = 0;

		public float Score {
			get {
				return _score;
			}
			set {
				_score = value;
			}
		}

		public System.ConsoleColor clr = ConsoleColor.White;
		public float LastScore = 0;

		public int JugarEstrategia (Historial H, int i)
		{
			// Evaluar fml
			int[] MemStack = new int[256];
			int stackptr = 0;

			string[] splilFml;
			splilFml = fmll.ToArray ();

			foreach (var m in splilFml) {
				switch (m) {
				case "!":
					MemStack [stackptr] = 1 - MemStack [stackptr];
					break;
				case "+":
					if (stackptr >= 1) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] + MemStack [stackptr];
						stackptr--;
					}
					break;
				case "*":
					if (stackptr >= 1) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] * MemStack [stackptr];
						stackptr--;
					}
					break;
				case "-":
					if (stackptr >= 1) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] - MemStack [stackptr];
						stackptr--;
					}
					break;
				case "%":
					if (stackptr >= 1 && MemStack [stackptr] > 0) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] % MemStack [stackptr];
						stackptr--;
					}
					break;
				case "<":
					if (stackptr > 1) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] < MemStack [stackptr] ? 1 : 0;
						stackptr--;
					}
					break;
				case "?":
					if (stackptr >= 2) {
						MemStack [stackptr - 2] = MemStack [stackptr] != 0 ? MemStack [stackptr - 1] : MemStack [stackptr - 2];
						stackptr -= 2;
					}
					break;
				case "=":
					if (stackptr > 0) {
						MemStack [stackptr - 1] = MemStack [stackptr - 1] == MemStack [stackptr] ? 1 : 0;
						stackptr--;
					}
					break;
				case "h":
					if (MemStack [stackptr] < H.Data.GetLength (1) && MemStack [stackptr] >= 0)
						MemStack [stackptr] = H.Data [1, MemStack [stackptr]];
					break;
				case "i":
					stackptr++;
					MemStack [stackptr] = i;
					break;
				default:
					int n;
					if (int.TryParse (m, out n)) {
						stackptr++;
						MemStack [stackptr] = n;
					}
					break;
				}
			}
			return MemStack [stackptr];
		}

		public JJuegoRápido (string f)
		{
			string[] a = f.Split (splitChar);
			fmll = new List<string> (a);
			//pct = Strat;
		}

		public JJuegoRápido (string[] f)
		{
			fmll = new List<string> (f);
		}
		//public string fml;
		public List<string> fmll = new List<string> ();

		public string fml {
			get {
				string ret = "";
				foreach (var x in fmll) {
					ret += x + " ";
				}
				return ret.TrimEnd (' ');
			}
		}

		char[] splitChar = {' '};
		// public float pct;
		// Random r = new Random();
	}

	public class Evol
	{
		public class EstadoJugador: IComparable<EstadoJugador>
		{
			public override string ToString ()
			{
				return string.Format ("({0}) : {1}", Jug.fml, Jug.Score);
			}

			public int Juegos = 0;
			public JJuegoRápido Jug;
			// public int Lives = 3;
			//public int ReprScore = 0;
			int IComparable<EstadoJugador>.CompareTo (EstadoJugador comp)
			{
				if (comp == null)
					return -1;
				return Jug.Score < comp.Jug.Score ? 1 : -1;
			}
		}

		public EstadoJugador EncuentraEstado (JJuegoRápido J)
		{
			return Pool.Find (x => x.Jug == J);
		}

		const int PoolSizeInicial = 300;
		const int PoolSizeMerge = 220;
		const int PoolSizeCompete = 300;

		public int PoolSize {
			get {
				return Pool.Count;
			}
		}

		List<EstadoJugador> Pool = new List<EstadoJugador> ();

		public void AgregaJugador (JJuegoRápido J)
		{
			EstadoJugador t = new EstadoJugador ();
			t.Jug = J;
			Pool.Add (t);
		}

		Random r = new Random ();

		public Evol ()
		{
			for (int i = 0; i < PoolSizeInicial; i++) {
				AgregaJugador (new JJuegoRápido ("")); //i = 0 ? 1 : h (i - 1)
			}
		}

		const int Rondas = 10000;

		public void RunOnce ()
		{
			for (int i = 0; i < Rondas; i++) {
				int a = r.Next (PoolSize);
				int b = r.Next (PoolSize);
				Pool [a].Juegos ++;
				Pool [b].Juegos ++;
				JRápido J = new JRápido (Pool [a].Jug, Pool [b].Jug);
				J.Run (false);
			}
/*
			for (int a = 0; a < PoolSize; a++)
			{
				for (int b = 0; b < PoolSize; b++)
				{
					JRápido J = new JRápido(Pool[a].Jug, Pool[b].Jug);
					J.Run(a == 0);
				}
			}
*/

			//EncuentraEstado(J.Ganador).ReprScore += ;
			//EncuentraEstado(J.Perdedor).Lives -= 1;

			//if (EncuentraEstado(J.Perdedor).Lives <= 0)
			//	Pool.RemoveAll(x => x.Jug == J.Perdedor);
		}

		public void EntreRuns ()
		{
		}

		public void Run ()
		{
			while (true) {
				RunOnce ();

				// Duplicarme

				// ADN largo tiene penalización


				foreach (var x in Pool) {
					if (x.Juegos > 0) {
						x.Jug.Score = x.Jug.Score / x.Juegos;
						x.Juegos = 0;
					} else {
						x.Jug.Score = 0;
					}
					x.Jug.Score -= (x.Jug.fml.Length / 10);
				}

				try
				{
					if (Pool.Exists(x => x.Jug == null))
						Console.WriteLine("Pfft");
					Pool.Sort();
					//Pool.Sort((x, y) => x.Jug.Score < y.Jug.Score ? 1 : -1); // Ordenados por score.
				}
				catch (NullReferenceException C)
				{					
					throw C;
				}

				Pool.RemoveRange (PoolSizeMerge, PoolSize - PoolSizeMerge);

				int Adders = PoolSizeCompete - PoolSize;

				for (int i = 0; i < Adders; i++) {
					JJuegoRápido Padre = Pool [i].Jug;
					JJuegoRápido dupe = new JJuegoRápido (Padre.fmll.ToArray());

					if (r.NextDouble() < 0.05)
					{
						dupe.clr = (ConsoleColor)r.Next (16);
					} else {
						dupe.clr = Padre.clr;
					}

					while (r.NextDouble() < 0.1) {
						bool Borrar = (r.NextDouble () < 0.75);
						switch (Borrar) {
						case true:
							if (dupe.fmll.Count > 0)
							{
								int m = r.Next (dupe.fmll.Count);
								dupe.fmll.RemoveAt (m);
							}
							break;

						case false:
							string[] Sym = {
								"+",
								"h",
								"i",
								"-",
								"%",
								"=",
								"?",
								"<",
								"*",
								"!"
							};
							int k = r.Next (Sym.Length + 1);
							if (k == Sym.Length) {
								dupe.fmll.Insert (r.Next (dupe.fmll.Count + 1), " " + (r.Next (21) - 10).ToString () + " ");
							} else {
								dupe.fmll.Insert (r.Next (dupe.fmll.Count + 1), Sym [k]);
							}
							break;

						default:
							break;
						}
					}				
					//dupe.fml = dupe.fml.Replace ("  ", " ");
					//dupe.fml = dupe.fml.Trim ();
					AgregaJugador (dupe);
				}

				//Console.WriteLine (Pool[0]);
				//Random r = new Random();


				Pool.Sort();

				// Borrar pantalla
				Console.Clear ();
				// Escribir máxima puntuación y mínima.
				Console.ForegroundColor = Pool[0].Jug.clr;
				Console.WriteLine("Máxima: {0}, por ({1})", Pool[0].Jug.Score, Pool[0].Jug.fml);
				// Escribir el pool
				foreach (var x in Pool) {
					Console.ForegroundColor = x.Jug.clr;
					if (x.Jug.fmll.Count > 0)
						Console.Write(string.Format("{0} ", x.Jug.fml));
					x.Jug.Score = 0;
				}
			}
		}
	}
}