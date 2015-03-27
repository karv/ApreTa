using System;
using System.Collections.Generic;

namespace ApreTa
{
	/// <summary>
	/// Representa un individio.
	/// </summary>
	public class Individuo
	{
		/// <summary>
		/// Genotipo del individuo.
		/// </summary>
		public GrupoGen Genética = new GrupoGen ();

		public Individuo ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ApreTa.Individuo"/> class.
		/// </summary>
		/// <param name="s">Secuencia de instrucciones genéticas.</param>
		public Individuo (string s)
		{
			InstrucciónGen G;
			foreach (var x in s) {
				G = new InstrucciónGen ();
				G.Instrucción = x.ToString ();
				Genética.AddGen (G);
			}
		}

		/// <summary>
		/// Crea una réplica genética -mutada- de este individuo.
		/// </summary>
		public Individuo Replicar ()
		{
			Individuo ret = new Individuo ();
			ret.Genética = (GrupoGen)Genética.Replicar ();

			return ret;
		}

		/// <summary>
		/// Realiza una réplica sexual de un individuo.
		/// </summary>
		/// <param name="I">Pareja sexual</param>
		public Individuo Replicar (Individuo I)
		{
			Individuo ret = new Individuo ();
			ret.Genética = Genética.Replicar (I.Genética);

			return ret;
		}

		public override string ToString ()
		{
			return (Genética.ReplicaSexual ? "" : "a") + Genética.ToString ();
		}

		/// <summary>
		/// Devuelve la lista de genes con sus respectivas apariciones.
		/// </summary>
		/// <returns>La lista de genes.</returns>
		public ContadorGen CuentaGen ()
		{
			return Genética.CuentaGen ();
		}

		public virtual int Ejecutar (Historial H)
		{
			MemStack Mem = new MemStack ();
			int ret;
			Genética.Ejecutar (Mem, H);
			if (Mem.Count > 0) {
				ret = Mem.Pop ();
				return ret == 0 ? 0 : 1; 
			}
			return 0;
		}
	}

	/// <summary>
	/// Stack de ints para ejecución genética.
	/// </summary>
	public class MemStack:System.Collections.Generic.Stack<int>
	{
	}

	/// <summary>
	/// Representa un jugador humano
	/// </summary>
	public class IndividuoHumano:Individuo
	{
		/// <summary>
		/// Pide al usuario un movimiento (númerao natural: 0 ó 1) con qué jugar.
		/// 1 es cooperar.
		/// </summary>
		/// <param name="H">Lista de movimientos pasados.</param>
		public override int Ejecutar (Historial H)
		{
			Console.Clear ();
			//Console.WriteLine ();
			DibujaTablero (H);
			return PedirJugada (H);
		}

		void DibujaTablero (Historial H)
		{
			Console.Write ("I |");
			for (int i = 0; i < H.Actual; i++) {
				Console.Write (H [0, i]);
			}
			Console.WriteLine ("  - " + H.ObtenerPuntuación (0) + " || Nombre: " + H.Ind [0].Genética.StringEfectivo ());
			Console.Write ("II|");
			for (int i = 0; i < H.Actual; i++) {
				Console.Write (H [1, i]);
			}
			Console.WriteLine ("  - " + H.ObtenerPuntuación (1) + " || Nombre: " + H.Ind [1].Genética.StringEfectivo ());
		}

		int PedirJugada (Historial H)
		{
			bool Validado = false;
			int ret = 0;
			char c;
			while (Console.KeyAvailable) // Limpiar el buffer.
				Console.ReadKey ();
			do {
				Console.SetCursorPosition (3 + H.Actual, 0);
				c = Console.ReadKey ().KeyChar;
				if (c == '0' || c == '1')
					Validado = true;
			} while (!Validado);
			ret = int.Parse (c.ToString ());

			return ret;
		}

		public override string ToString ()
		{
			return string.Format ("[IndividuoHumano]");
		}
	}
}