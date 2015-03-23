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
	}

	/// <summary>
	/// Stack de ints para ejecución genética.
	/// </summary>
	public class MemStack:System.Collections.Generic.Stack<int>
	{
	}
}

