using System;
using System.Collections.Generic;

namespace ApreTa
{
	/// <summary>
	/// Representa un torneo entre individuos.
	/// </summary>
	public class Torneo
	{
		public class EstructuraIndividuo
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
		}

		List<EstructuraIndividuo> Individuos = new List<EstructuraIndividuo> ();
		public const int MinIndiv = 100;
		public const int MaxIndiv = 200;
		/// <summary>
		/// Número de encuentros por turno.
		/// </summary>
		public const int NumRondas = 10000;
		/// <summary>
		/// Iteraciones de juego por encuentro.
		/// </summary>
		public const int IteracionesPorEncuentro = 10;
		Random r = new Random ();

		public Torneo ()
		{
			// Agregar individuos sin gen hasta llegar al máximo.
			while (Individuos.Count < MaxIndiv) {
				Individuos.Add (new EstructuraIndividuo ());
			}
		}

		/// <summary>
		/// Ejecuta un sólo turno.
		/// </summary>
		public void RunOnce ()
		{
			for (int i = 0; i < NumRondas; i++) {
				// Seleccionar dos individuos.
				EstructuraIndividuo[] I = new EstructuraIndividuo[2];
				I [0] = Individuos [r.Next (Individuos.Count)];
				I [1] = Individuos [r.Next (Individuos.Count)];
				I [0].Juegos++;
				I [1].Juegos++;

				// Hacerlos interactuar.

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
				ReplicarAdaptados ();
				MuestraStats ();
			}
		
		}

		/// <summary>
		/// Replica a los individuos hasta llegar a MaxIndiv
		/// </summary>
		public void ReplicarAdaptados ()
		{
			while (Individuos.Count < MaxIndiv) {
				EstructuraIndividuo I = Individuos [r.Next (Individuos.Count)];
				Individuos.Add (new EstructuraIndividuo (I.Indiv.Replicar ()));
			}
		}

		/// <summary>
		/// Mata a los de puntuación menor, hasta llegar a MinIndiv.
		/// </summary>
		public void MatarMenosAdaptados ()
		{
			Individuos.Sort ((EstructuraIndividuo x, EstructuraIndividuo y) => x.Punt < y.Punt ? -1 : 1);
			Individuos.RemoveRange (MinIndiv, MaxIndiv - MinIndiv);
		}

		/// <summary>
		/// Muestra en consola lo que se debe mostrar entre turnos.
		/// </summary>
		public void MuestraStats ()
		{
			Console.Clear ();
			// Escribir máxima puntuación y mínima.
			//Console.ForegroundColor = Pool[0].Jug.clr;
			Console.WriteLine ("Máxima: {0}", Individuos [0].Punt);   //Pool[0].Jug.Score, Pool[0].Jug.fml);
			// Escribir el pool
			foreach (var x in Individuos) {
				//Console.ForegroundColor = x.Jug.clr;

				Console.Write (string.Format ("{0} ", x.Indiv.ToString ()));
			}

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

			// Ejecutar las rondas
			while (H.Actual < IteracionesPorEncuentro) {
				H.Actual++;

				MemStack MSa = new MemStack ();
				MemStack MSb = new MemStack ();
				int a;
				int b;

				Ind [0].Indiv.Genética.Ejecutar (MSa, H);
				Ind [1].Indiv.Genética.Ejecutar (MSb, H);

				a = MSa.Pop ();
				b = MSb.Pop ();

				a = a == 0 ? 0 : 1;
				b = b == 0 ? 0 : 1;

				// Los jugadores escogen a y b respectivamente.
				//Agrega en el historial las últimas desiciones.
				H.AgregaTurno (a, b);

				// Modificar puntuación.
				if (a == 0) {
					if (b == 0) {
						Ind [0].Punt += -1;
						Ind [0].Punt += -1;
					} else {
						Ind [0].Punt += 5;
						Ind [1].Punt += 0;
					}
				} else { //a == 1
					if (b == 0) {
						Ind [0].Punt += 0;
						Ind [1].Punt += 5;
					} else {
						Ind [0].Punt += 3;
						Ind [1].Punt += 3;
					}
				}
			}
		}
	}
}