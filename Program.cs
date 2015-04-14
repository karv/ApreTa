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

			Torneo[] tors = new Torneo[TorneoDinámico.NumTorneos];
			for (int i = 0; i < TorneoDinámico.NumTorneos; i++) {
				tors [i] = new Torneo ();
			}

			TorneoDinámico T = new TorneoDinámico (tors);
			T.Run ();
			/*

			T.IteracionesPorEncuentro = 100;
			T.NumRondas = 1000;
			T.InicializaTorneo ();
			T.Run ();
			*/

		}
	}
}