using System;
using System.Collections.Generic;
using ListasExtra;

namespace ApreTa
{
	/// <summary>
	/// Un gen.
	/// </summary>
	public abstract class Gen
	{
		protected Random r = new Random ();
		protected ConsoleColor clr = ConsoleColor.White;
		/// <summary>
		/// Devuelve o establece si la réplica se hace en modo sexual.
		/// En caso contrario, hace réplica asexial, obviamente.
		/// </summary>
		bool _ReplicaSexual = true;

		public bool ReplicaSexual {
			get {
				return _ReplicaSexual;
			}
			set {
				_ReplicaSexual = value;
			}
		}

		/// <summary>
		/// Réplica asexual
		/// </summary>
		/// <param name="Coef">Coef.</param>
		public abstract Gen Replicar (float Coef = 1);
		//public abstract Gen Replicar (Gen Pareja)
		public abstract void Ejecutar (MemStack Mem, Historial H = null);

		public bool Esbueno ()
		{
			MemStack M = new MemStack ();
			Ejecutar (M, new Historial ());
			if (M.Count > 0 && M.Peek () != 0)
				return true;
			else {
				return false;
			}
		}

		/// <summary>
		/// Revisa si este gen es vengativo
		/// Un gen es vengativo si (!h(1)) = >resp == 0
		/// </summary>
		/// <returns><c>true</c>, si es vengativo, <c>false</c> otherwise.</returns>
		public bool EsVengativo (int MaxLong)
		{
			for (int i = 2; i < MaxLong; i++) {
				if (!_EsVengativo (i, MaxLong))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Revisa si el gen es vengativo en el turno n
		/// </summary>
		/// <returns><c>true</c>, if vengativo was esed, <c>false</c> otherwise.</returns>
		/// <param name="n">Turno donde se revisa si es vengativo</param>
		bool _EsVengativo (int n, int maxn)
		{
			Individuo I = new Individuo ();
			I.Genética = (GrupoGen)this;
			foreach (var H in Historial.ObtenerPosiblesHistorias(n, maxn)) {
				if (H.Data [1, H.Actual] == 0 && I.Ejecutar (H) == 1) // Si confías después de una traición
					return false;
			}
			return true;
		}

		/// <summary>
		/// Devuelve true si este gen es equivalente a otro dado.
		/// </summary>
		/// <returns><c>true</c>, if equivalente was esed, <c>false</c> otherwise.</returns>
		/// <param name="G">Gen a comparar</param>
		public bool EsEquivalente (Gen G)
		{
			return ToString () == G.ToString ();
		}

		/// <summary>
		/// Devuelve el String efectivo del gen (ie, ToString sin paréntesis)
		/// </summary>
		/// <returns>The efectivo.</returns>
		public abstract string StringEfectivo ();

		public static bool EsEquivalente (Gen G1, Gen G2)
		{
			return G1.EsEquivalente (G2);
		}

		public override bool Equals (object obj)
		{
			if (obj is Gen) {
				Gen Obj = (Gen)obj;
				return EsEquivalente (Obj);
			} else
				return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Crea un diccionario con la información cualitativa de sus subgenes.
		/// </summary>
		/// <returns>Un ContadorGen (listapeso) con la información de apariencias de cada subgen.</returns>
		/// <param name="Hereditario">Si es true, la búsqueda se hace recusrivamente. 
		/// Si el false, lo hace sólo un paso.</param>
		public abstract ContadorGen CuentaGen (bool Hereditario = true);
	}

	/// <summary>
	/// Un gen del tipo grupo.
	/// </summary>
	public class GrupoGen:Gen
	{
		List<Gen> _Genes = new List<Gen> ();

		/// <summary>
		/// Devuelve una copia de la lista de genes.
		/// </summary>
		/// <value>The genes.</value>
		public Gen[] Genes {
			get {
				return _Genes.ToArray ();
			}
		}

		/// <summary>
		/// Replica este grupo genético.
		/// </summary>
		public override Gen Replicar (float Coef = 1)
		{
			GrupoGen ret = new GrupoGen ();
			foreach (var x in _Genes) {
				if (r.NextDouble () >= 0.1 * Coef) // La probabilidad de eliminación base es 0.1
					ret._Genes.Add (x.Replicar (Coef * 0.7f)); // Probabilidad recursiva/iterada es de 0.7
			}

			// AgregarInstrucción
			while (r.NextDouble() < Coef * 0.1) {
				int indAgrega = r.Next (ret._Genes.Count + 1); //Índice para agregar
				ret._Genes.Insert (indAgrega, InstrucciónGen.Aleatorio (r));
			}

			// Estado de Replicar
			ret.ReplicaSexual = r.NextDouble () < 0.001 * Coef ? !ReplicaSexual : ReplicaSexual;

			// Dividir gen
			if (r.NextDouble () < 0.01 * Coef) { // La probabilidad de dividir gen base es 0.01// Esta mutación no tiene fenotipo directo.
				int indCorte = r.Next (ret._Genes.Count + 1);
				GrupoGen G0 = new GrupoGen ();
				GrupoGen G1 = new GrupoGen ();
				for (int i = 0; i < ret._Genes.Count; i++) {
					if (i <= indCorte)
						G0._Genes.Add (ret._Genes [i]);
					else
						G1._Genes.Add (ret._Genes [i]);
				}
				ret._Genes = new List<Gen> ();
				ret._Genes.Add (G0);
				ret._Genes.Add (G1);
			}
			// Color (no entra
			if (r.NextDouble () < 0.05)
				clr = (ConsoleColor)r.Next (16);

			return ret;
		}

		/// <summary>
		/// Replica sexualmente un individuo.
		/// Incluye mutaciones.
		/// </summary>
		/// <param name="Pareja">Pareja sexual</param>
		public GrupoGen Replicar (GrupoGen Pareja, float CoefMut = 1)
		{
			return (GrupoGen)GrupoGen.Replicar (this, Pareja).Replicar (CoefMut);
		}

		/// <summary>
		/// Replica sexualmente un GrupoGen, a partir de dos GrupoGen.
		/// Excluye mutación
		/// </summary>
		/// <param name="G1">Un GrupoGen</param>
		/// <param name="G2">Un GrupoGen</param>
		public static GrupoGen Replicar (GrupoGen G1, GrupoGen G2, float prob = 0.5f, Random r = null)
		{
			if (r == null)
				r = new Random ();
			GrupoGen tmp = new GrupoGen ();
			GrupoGen ret = new GrupoGen ();
			// 大体:
			// Agregar cada gen en G1 (con probabilidad prob), luego hacer lo mismo con G2.
			// Finalmente reordenarlos aleatoriamente.

			foreach (var x in G1.Genes) {
				if (r.NextDouble () < prob)
					tmp._Genes.Add (x);
			}

			foreach (var x in G2.Genes) {
				if (r.NextDouble () < prob)
					tmp._Genes.Add (x);
			}

			// Reordenar
			foreach (var x in tmp._Genes) {
				ret._Genes.Insert (r.Next (ret._Genes.Count + 1), x);
			}

			// Note que llegar aquí no implica que ambos sean sexualmente replicables, al menos uno lo es. 
			ret.ReplicaSexual = r.Next (2) == 0 ? G1.ReplicaSexual : G2.ReplicaSexual; 

			return ret;

		}

		public override string ToString ()
		{
			string ret = "(";
			foreach (var x in _Genes) {
				ret += x.ToString ();
			}
			ret += ")";

			return ret;
		}

		public override string StringEfectivo ()
		{
			string ret = "";
			foreach (var x in _Genes) {
				ret += x.StringEfectivo ();
			}
			ret += "";

			return ret;

		}

		public override void Ejecutar (MemStack Mem, Historial H = null)
		{
			foreach (var x in _Genes) {
				x.Ejecutar (Mem, H);
			}

		}

		/// <summary>
		/// Crea un diccionario con la información cualitativa de sus subgenes.
		/// </summary>
		/// <returns>Un ContadorGen (listapeso) con la información de apariencias de cada subgen.</returns>
		/// <param name="Hereditario">Si es true, la búsqueda se hace recusrivamente. 
		/// Si el false, lo hace sólo un paso.</param>
		public override ContadorGen CuentaGen (bool Hereditario = true)
		{
			ContadorGen ret = new ContadorGen ();
			ret [this] += 1; 
			foreach (var x in _Genes) {
				if (Hereditario) {
					foreach (var y in x.CuentaGen(true)) {
						ret [y.Key] += y.Value; 
					}
				} else {
					ret [x] += 1;
				}
			}
			return ret;
		}
	}

	/// <summary>
	/// Un gen activo.
	/// </summary>
	public class InstrucciónGen:Gen
	{
		public override ContadorGen CuentaGen (bool Hereditario)
		{
			return new ContadorGen ();
		}

		static readonly string[] _Símbolos = {
			"!",
			"+",
			"*",
			"-",
			"?",
			"%",
			"<",
			"=",
			"h",
			"i",
			null // Se usa para dar la instrucción de generar entero.
		};
		public string Instrucción;

		/// <summary>
		/// Genera un gen de instrucción aleatorio.
		/// </summary>
		/// <param name="R">R.</param>
		public static InstrucciónGen Aleatorio (Random R)
		{
			InstrucciónGen ret = new InstrucciónGen ();
			ret.Instrucción = _Símbolos [R.Next (_Símbolos.Length)];
			if (ret.Instrucción == null)
				ret.Instrucción = R.Next (10).ToString ();

			return ret;
		}

		public override Gen Replicar (float Coef = 1)
		{
			InstrucciónGen ret = new InstrucciónGen ();
			ret.ReplicaSexual = ReplicaSexual;
			ret.Instrucción = Instrucción;

			return ret;
		}

		public override string ToString ()
		{
			return Instrucción;
		}

		public override string StringEfectivo ()
		{
			return Instrucción;
		}

		public override void Ejecutar (MemStack Mem, Historial H = null)
		{
			if (H == null)
				throw new Exception ("");
			int StackSize = Mem.Count;
			switch (Instrucción) {
			case "!":
				if (StackSize >= 1) {
					int t = Mem.Pop ();
					Mem.Push (t == 0 ? 1 : 0);
				}
				break;
			case "+":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () + Mem.Pop ());
				break;
			case "*":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () * Mem.Pop ());
				break;
			case "-":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () - Mem.Pop ());
				break;
			case "%":
				if (StackSize >= 2) {
					int o1 = Mem.Pop ();
					int o2 = Mem.Pop ();
					if (o2 != 0)
						Mem.Push (o1 % o2);
				}
					
				break;
			case "?":
				if (StackSize >= 3)
					Mem.Push (Mem.Pop () != 0 ? Mem.Pop () : Mem.Pop ());
				break;
			case "<":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () < Mem.Pop () ? 1 : 0);
				break;
			case "=":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () == Mem.Pop () ? 1 : 0);
				break;
			case "h":
				if (StackSize >= 1 && Mem.Peek () < H.Actual && Mem.Peek () >= 0)
					Mem.Push (H.Data [1, H.Actual - Mem.Pop ()]);
				break;
			case "i":
				Mem.Push (H.Actual);
				break;
			default:
				int n;
				if (int.TryParse (Instrucción, out n)) {
					Mem.Push (n);
				}
				break;
			}
		}
	}

	/// <summary>
	/// Clase que cuenta genes.
	/// </summary>
	public class ContadorGen:ListaPeso<Gen, int>
	{
		public ContadorGen ():base((x,y) => x+y, 0)
		{
			Comparador = (x, y) => x.ToString () == y.ToString ();
		}
	}
}