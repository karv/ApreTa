using System;

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
		public Gen Genética = new GrupoGen ();

		/// <summary>
		/// Crea una réplica genética -mutada- de este individuo.
		/// </summary>
		public Individuo Replicar ()
		{
			Individuo ret = new Individuo ();
			ret.Genética = Genética.Replicar ();

			return ret;
		}

		public override string ToString ()
		{
			return Genética.ToString ();
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
				Console.Write (H.Data [0, i]);
			}
			Console.WriteLine ();
			Console.Write ("II|");
			for (int i = 0; i < H.Actual; i++) {
				Console.Write (H.Data [1, i]);
			}
			Console.WriteLine ();
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