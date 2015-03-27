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
					return 4;
				}
			}
		}

		public class EstructuraIndividuo: IComparable<EstructuraIndividuo>
		{
			/// <summary>
			/// Determina si el usuario seguirá a este individuo.
			/// </summary>
			public bool Siguiendo = false;
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
		public int MaxIndiv = 150;
		/// <summary>
		/// Número de encuentros por turno.
		/// </summary>
		public int NumRondas = 10000;
		/// <summary>
		/// Iteraciones de juego por encuentro.
		/// </summary>
		public int IteracionesPorEncuentro = 10;
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
				Individuos.Add (new EstructuraIndividuo (new Individuo ("")));
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

		float MinSobrevivir;

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
					ConsoleKeyInfo kp = Console.ReadKey (true);
					if (kp.KeyChar == ' ') {// Humano vs individuo al azar
						Torneo.Encuentro (new IndividuoHumano (), Individuos [r.Next (Individuos.Count)].Indiv);
					}
					if (kp.KeyChar == '<') {// Agrega un individuo en observacion
						Individuo I = new Individuo (Console.ReadLine ());
						EstructuraIndividuo J = new EstructuraIndividuo (I);
						J.Siguiendo = true;
						Individuos.Add (J);
					}
					if (kp.KeyChar == 'z') {// Peleas contra 1h
						Individuo I = new Individuo ("1h0i0=?");

						Torneo.Encuentro (new IndividuoHumano (), I);
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
			EstructuraIndividuo Agrega;
			while (Individuos.Count < MaxIndiv) {
				I1 = Individuos [r.Next (Individuos.Count)];
				if (I1.Indiv.Genética.ReplicaSexual) {
					I2 = Individuos [r.Next (Individuos.Count)];
					Agrega = new EstructuraIndividuo (I1.Indiv.Replicar (I2.Indiv));
				} else {
					Agrega = new EstructuraIndividuo (I1.Indiv.Replicar ());
				}
				Individuos.Add (Agrega);
			}
		}

		/// <summary>
		/// Mata a los de puntuación menor, hasta llegar a MinIndiv.
		/// </summary>
		public void MatarMenosAdaptados ()
		{
			// Normalizar puntuación
			foreach (var x in Individuos) {
				if (x.Juegos > 0)
					x.Punt /= x.Juegos;
			}

			Individuos.Sort ();
			while (Individuos.Count > MinIndiv) {
				EstructuraIndividuo rem = Individuos [Individuos.Count - 1];
				if (rem.Siguiendo) {
					Console.WriteLine (string.Format ("Se extingue {0} con puntuación de {1}", rem.Indiv, rem.Punt));
					Console.ReadLine ();
				}
				Individuos.Remove (rem);
			}
			MinSobrevivir = Individuos [Individuos.Count - 1].Punt;
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
			Console.WriteLine ("Máxima: {0} - {1}\nSobrevivir: {2}", Individuos [0].Punt, Individuos [0].Indiv, MinSobrevivir);
			if (Individuos [0].Indiv.Genética.Esbueno ())
				Console.Write ("  Bueno");
			if (MostrarVengativo && Individuos [0].Indiv.Genética.EsVengativo (IteracionesPorEncuentro))
				Console.Write ("  Vengativo");
			Console.WriteLine ();
			Console.WriteLine ("% buenos: " + BuenoPct);
			Console.WriteLine ("% sexuales: " + Individuos.FindAll (x => x.Indiv.Genética.ReplicaSexual).Count / Individuos.Count);
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

			if (r.Next (2) == 0) {
				Ind [0] = I;
				Ind [1] = J;
			} else {
				Ind [0] = J;
				Ind [1] = I;
			}
			H.Ind [0] = Ind [0].Indiv;
			H.Ind [1] = Ind [1].Indiv;

			if (Ind [0].Siguiendo || Ind [1].Siguiendo) {
				Console.Write ("");
			}

			// Ejecutar las rondas
			while (H.Actual < IteracionesPorEncuentro) {
				// H.Actual++;

				int a;
				int b;

				a = Ind [0].Indiv.Ejecutar (H);
				b = Ind [1].Indiv.Ejecutar (H.Invertir ());

				// Los jugadores escogen a y b respectivamente.
				//Agrega en el historial las últimas desiciones.
				H.AgregaTurno (a, b);

				// Modificar la puntuación
				Ind [0].Punt += Torneo.Puntuación (a, b) / IteracionesPorEncuentro;
				Ind [1].Punt += Torneo.Puntuación (b, a) / IteracionesPorEncuentro;
			}
			if (Ind [0].Siguiendo || Ind [1].Siguiendo) {
				Console.WriteLine (string.Format ("{0}:{1}\n{2}:{3}", Ind [0].Indiv, H.ObtenerPuntuación (0), Ind [1].Indiv, H.ObtenerPuntuación (1)));
				if (Console.ReadLine () != "") {
					// Mostrar el historial
					for (int i = 0; i < 2; i++) {
						for (int j = 0; j < H.Count; j++) {
							Console.Write (H [i, j]);
						}
						Console.WriteLine (" - " + H.Ind [i]);
					}
				}
			}
		}
	}

	/// <summary>
	/// Representa el historial de un juego.
	/// </summary>
	public class Historial : List<Tuple<int, int>>
	{
		public int this [int Player, int Inning] {
			get {
				return Player == 0 ? base [Inning].Item1 : base [Inning].Item2;
			}
		}

		/// <summary>
		/// Devuelve el "turno" actual.
		/// </summary>
		public int Actual {
			get {
				return Count;
			}
		}

		/// <summary>
		/// Los individuos en juego.
		/// </summary>
		public Individuo[] Ind = new Individuo[2];

		public Historial Invertir ()
		{
			Historial ret = new Historial ();
			for (int i = 0; i < Actual; i++) {
				ret.Add (new Tuple<int, int> (this [1, i], this [0, i]));
			}
			ret.Ind [0] = Ind [1];
			ret.Ind [1] = Ind [0];
			return ret;
		}

		public float ObtenerPuntuación (int i)
		{
			if (i < 0 || i > 1) {
				throw new IndexOutOfRangeException ();
			}
			float ret = 0;
			for (int j = 0; j < Actual; j++) {
				ret += Torneo.Puntuación (this [i, j], this [1 - i, j]);
			}
			return ret;
		}

		public void AgregaTurno (int a, int b)
		{
			Add (new Tuple<int, int> (a, b));
		}

		/// <summary>
		/// Enlista todos los posibles historiales, variando al jugador II.
		/// El jugador I siempre jugará 0's
		/// </summary>
		/// <returns>Un arreglo enumerando a todos los Historiales.</returns>
		/// <param name="Long">Longitud de las instancias de Historial a mostrar.</param>
		public static Historial[] ObtenerPosiblesHistorias (int Long)
			// TODO: Para evitar iteración, enumerar las historias con 2^Long (las que empiecen con 1 en expansión decimal, y convertirlos a Historiales vía función de Cantor.
		{
			List<Historial> ret = new List<Historial> ();
			if (Long == 0) {
				Historial H = new Historial ();
				ret.Add (H);
				return ret.ToArray ();
			} else {
				// Paso recursivo
				Historial[] Iterador = ObtenerPosiblesHistorias (Long - 1);
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