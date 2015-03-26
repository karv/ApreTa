using System;
using System.Linq;
using System.Collections.Generic;

namespace ApreTa
{
	/// <summary>
	/// Representa un torneo entre individuos.
	/// </summary>
	public class Torneo
	{
		/// <summary>
		/// Obtiene la puntuación si se juega con i, y el otro jugador juega con j
		/// </summary>
		public static float Puntuación (int a, int b)
		{
			if (a == 0) {
				if (b == 0) {
					return -1;
				} else {
					return 5;
				}
			} else { //a == 1
				if (b == 0) {
					return  0;
				} else {
					return 3;
				}
			}
		}

		public class EstructuraIndividuo: IComparable<EstructuraIndividuo>
		{
			/// <summary>
			/// Puntuación.
			/// </summary>
			public float Punt = 0;
			/// <summary>
			/// Juegos jugados en este turno.
			/// </summary>
			public int Juegos = 0;
			public Individuo Indiv;

			public EstructuraIndividuo (Individuo I)
			{
				Indiv = I;
			}

			public EstructuraIndividuo ()
			{
				Indiv = new Individuo ();
			}

			public override string ToString ()
			{
				return string.Format ("{0}\t {1}", Punt, Indiv);
			}
			// Comparador
			int IComparable<EstructuraIndividuo>.CompareTo (EstructuraIndividuo cmp)
			{

				return cmp != null && Punt < cmp.Punt ? 1 : -1;
			}
		}

		/// <summary>
		/// Devuelve la genpool con peso.
		/// </summary>
		/// <returns>The gen.</returns>
		public ContadorGen CuentaGen ()
		{
			ContadorGen ret = new ContadorGen ();
			foreach (var x in Individuos) {
				foreach (var y in x.Indiv.CuentaGen()) {
					ret [y.Key] += y.Value;
				}
			}
			return ret;
		}

		public List<EstructuraIndividuo> Individuos = new List<EstructuraIndividuo> ();
		public int MinIndiv = 100;
		public int MaxIndiv = 1000;
		/// <summary>
		/// Número de encuentros por turno.
		/// </summary>
		public int NumRondas = 10000;
		/// <summary>
		/// Iteraciones de juego por encuentro.
		/// </summary>
		public const int IteracionesPorEncuentro = 10;
		Random r = new Random ();
		// Variables de opciones
		/// <summary>
		/// ¿El mejor resultado muestra si es vengativo?
		/// Si es true, puede ralentizar el proceso.
		/// </summary>
		public bool MostrarVengativo = false;

		public void InicializaTorneo ()
		{
			// Agregar individuos sin gen hasta llegar al máximo.
			while (Individuos.Count < MaxIndiv) {
				Individuos.Add (new EstructuraIndividuo ());
			}
		}

		/// <summary>
		/// Ejecuta un sólo turno.
		/// </summary>
		public void RunOnce (bool ForzarDiferente = false)
		{
			for (int i = 0; i < NumRondas; i++) {
				// Seleccionar dos individuos.
				EstructuraIndividuo[] I = new EstructuraIndividuo[2];
				I [0] = Individuos [r.Next (Individuos.Count)];

				do {
					I [1] = Individuos [r.Next (Individuos.Count)];
				} while (ForzarDiferente && I[0] == I[1]);

				I [0].Juegos++;
				I [1].Juegos++;
				// Hacerlos interactuar.
				Encuentro (I [0], I [1]);
			}
		}

		/// <summary>
		/// Entra al ciclo principal.
		/// </summary>
		public void Run ()
		{
			while (true) {
				RunOnce ();
				MatarMenosAdaptados ();
				MuestraStats ();
				ReplicarAdaptados ();
				ResetScore ();

				while (Console.KeyAvailable) {
					ConsoleKeyInfo kp = Console.ReadKey ();
					if (kp.KeyChar == ' ') {
						Torneo.Encuentro (new IndividuoHumano (), Individuos [r.Next (Individuos.Count)].Indiv);
					}
				}
			}
		}

		public void DespuntuarLargos ()
		{
			foreach (var x in Individuos) {
				x.Punt -= x.ToString ().Length / 10;
			}
		}

		public void ResetScore ()
		{
			foreach (var x in Individuos) {
				x.Juegos = 0;
				x.Punt = 0;
			}
		}

		/// <summary>
		/// Replica a los individuos hasta llegar a MaxIndiv
		/// </summary>
		public void ReplicarAdaptados ()
		{
			EstructuraIndividuo I1, I2;
			while (Individuos.Count < MaxIndiv) {
				I1 = Individuos [r.Next (Individuos.Count)];
				I2 = Individuos [r.Next (Individuos.Count)];
				Individuos.Add (new EstructuraIndividuo (I1.Indiv.Replicar (I2.Indiv)));
			}
		}

		/// <summary>
		/// Mata a los de puntuación menor, hasta llegar a MinIndiv.
		/// </summary>
		public void MatarMenosAdaptados ()
		{
			Individuos.Sort ();
			Individuos.RemoveRange (MinIndiv, MaxIndiv - MinIndiv);
		}

		/// <summary>
		/// Muestra en consola lo que se debe mostrar entre turnos.
		/// </summary>
		public void MuestraStats ()
		{
			Console.Clear ();
			Console.ForegroundColor = ConsoleColor.White;
			float BuenoPct = GetPctBuenos ();
			// Escribir máxima puntuación y mínima.
			//Console.ForegroundColor = Pool[0].Jug.clr;
			Console.Write ("Máxima: {0} - {1}", Individuos [0].Punt, Individuos [0].Indiv);
			if (Individuos [0].Indiv.Genética.Esbueno ())
				Console.Write ("  Bueno");
			if (MostrarVengativo && Individuos [0].Indiv.Genética.EsVengativo (IteracionesPorEncuentro))
				Console.Write ("  Vengativo");
			Console.WriteLine ();
			Console.Write ("% buenos: " + BuenoPct);
			Console.WriteLine ();
			// Escribir el pool
			foreach (var x in Individuos) {
				//Console.ForegroundColor = x.Jug.clr;
				if (x.Indiv.Genética.Esbueno ()) {
					Console.ForegroundColor = ConsoleColor.Blue;
				} else {
					Console.ForegroundColor = ConsoleColor.White;
				}
				Console.Write (string.Format ("{0} ", x.Indiv.ToString ()));
			}
			Console.WriteLine ();
			Console.WriteLine ("La Genepool:");
			ContadorGen Pool = CuentaGen ();
			

			int MaxGen = 5;
			foreach (var x in Pool.OrderByDescending(x=> x.Value)) {
				MaxGen--;
				if (MaxGen >= 0) {
					Console.ForegroundColor = x.Key.Esbueno () ? ConsoleColor.Blue : ConsoleColor.White;
					Console.WriteLine (string.Format ("{0}\t {1}", x.Key, x.Value));
				}					
			}
		}

		/// <summary>
		/// Devuelve el % de Individuos buenos.
		/// </summary>
		/// <returns>The pct buenos.</returns>
		public float GetPctBuenos ()
		{
			int ctr = 0;
			foreach (var x in Individuos) {
				if (x.Indiv.Genética.Esbueno ())
					ctr++;
			}
			return (float)ctr / Individuos.Count;
		}

		public static int Encuentro (Individuo I, Individuo J)
		{
			Torneo Trn = new Torneo ();
			EstructuraIndividuo[] EI = new EstructuraIndividuo[2];
			EI [0] = new EstructuraIndividuo (I);
			EI [1] = new EstructuraIndividuo (J);
			Trn.Individuos.Add (new EstructuraIndividuo (I));
			Trn.Individuos.Add (new EstructuraIndividuo (J));
			Trn.NumRondas = 1;
			Trn.MaxIndiv = 2;
			Trn.MinIndiv = 2;
			Trn.RunOnce (true);

			return Trn.Individuos [0].Punt < Trn.Individuos [1].Punt ? 0 : 1;
		}

		/// <summary>
		/// Ejecuta un encuentro entre dos individuos.
		/// </summary>
		public void Encuentro (EstructuraIndividuo I, EstructuraIndividuo J)
		{
			EstructuraIndividuo[] Ind = new EstructuraIndividuo[2];
			Historial H = new Historial ();

			H.Data = new int[2, IteracionesPorEncuentro];

			if (r.Next (2) == 0) {
				Ind [0] = I;
				Ind [1] = J;
			} else {
				Ind [0] = J;
				Ind [1] = I;
			}
			H.Ind [0] = Ind [0].Indiv;
			H.Ind [1] = Ind [1].Indiv;

			// Ejecutar las rondas
			while (H.Actual < IteracionesPorEncuentro) {
				// H.Actual++;

				int a;
				int b;

				a = Ind [0].Indiv.Ejecutar (H);
				b = Ind [1].Indiv.Ejecutar (H);

				// Los jugadores escogen a y b respectivamente.
				//Agrega en el historial las últimas desiciones.
				H.AgregaTurno (a, b);

				// Modificar la puntuación
				Ind [0].Punt += Torneo.Puntuación (a, b);
				Ind [1].Punt += Torneo.Puntuación (b, a);
			}
		}
	}

	/// <summary>
	/// Representa el historial de un juego.
	/// </summary>
	public class Historial
	{
		public int[,] Data;
		/// <summary>
		/// Devuelve el "turno" actual.
		/// </summary>
		public int Actual = 0;
		/// <summary>
		/// Los individuos en juego.
		/// </summary>
		public Individuo[] Ind = new Individuo[2];

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

		public float ObtenerPuntuación (int i)
		{
			if (i < 0 || i > 1) {
				throw new IndexOutOfRangeException ();
			}
			float ret = 0;
			for (int j = 0; j < Actual; j++) {
				ret += Torneo.Puntuación (Data [i, j], Data [1 - i, j]);
			}
			return ret;
		}

		public void AgregaTurno (int a, int b)
		{
			Data [0, Actual] = a;
			Data [1, Actual] = b;
			Actual++;
		}

		/// <summary>
		/// Enlista todos los posibles historiales, variando al jugador II.
		/// El jugador I siempre jugará 0's
		/// </summary>
		/// <returns>Un arreglo enumerando a todos los Historiales.</returns>
		/// <param name="Long">Longitud de las instancias de Historial a mostrar.</param>
		public static Historial[] ObtenerPosiblesHistorias (int Long, int MaxLong)
			// TODO: Para evitar iteración, enumerar las historias con 2^Long (las que empiecen con 1 en expansión decimal, y convertirlos a Historiales vía función de Cantor.
		{
			List<Historial> ret = new List<Historial> ();
			if (Long == 0) {
				Historial H = new Historial ();
				H.Data = new int[2, MaxLong];
				H.Actual = 0;
				ret.Add (H);
				return ret.ToArray ();
			} else {
				// Paso recursivo
				Historial[] Iterador = ObtenerPosiblesHistorias (Long - 1, MaxLong);
				foreach (var H in Iterador) { // Crear una copia para iterar
					Historial Hi;
					Hi = (Historial)H.MemberwiseClone (); // Si no funciona, hacer a Historial IClonable.
					Hi.AgregaTurno (0, 0);
					ret.Add (Hi);
					Hi = (Historial)H.MemberwiseClone (); // idem
					Hi.AgregaTurno (0, 1);
					ret.Add (Hi);
				}
				return ret.ToArray ();
			}
		}
	}
}