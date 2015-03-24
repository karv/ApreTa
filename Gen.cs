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

		public abstract Gen Replicar (float Coef = 1);

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
		/// Devuelve true si este gen es equivalente a otro dado.
		/// </summary>
		/// <returns><c>true</c>, if equivalente was esed, <c>false</c> otherwise.</returns>
		/// <param name="G">Gen a comparar</param>
		public bool EsEquivalente (Gen G)
		{
			return ToString () == G.ToString ();
		}

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
		List<Gen> Genes = new List<Gen> ();

		/// <summary>
		/// Replica este grupo genético.
		/// </summary>
		public override Gen Replicar (float Coef = 1)
		{
			GrupoGen ret = new GrupoGen ();
			foreach (var x in Genes) {
				if (r.NextDouble () >= 0.1 * Coef) // La probabilidad de eliminación base es 0.1
					ret.Genes.Add (x.Replicar (Coef * 0.7f)); // Probabilidad recursiva/iterada es de 0.7
			}

			// AgregarInstrucción
			while (r.NextDouble() < Coef * 0.1) {
				int indAgrega = r.Next (ret.Genes.Count + 1); //Índice para agregar
				ret.Genes.Insert (indAgrega, InstrucciónGen.Aleatorio (r));
			}

			// Dividir gen
			if (r.NextDouble () < 0.01 * Coef) { // La probabilidad de dividir gen base es 0.01// Esta mutación no tiene fenotipo directo.
				int indCorte = r.Next (ret.Genes.Count + 1);
				GrupoGen G0 = new GrupoGen ();
				GrupoGen G1 = new GrupoGen ();
				for (int i = 0; i < ret.Genes.Count; i++) {
					if (i <= indCorte)
						G0.Genes.Add (ret.Genes [i]);
					else
						G1.Genes.Add (ret.Genes [i]);
				}
				ret.Genes = new List<Gen> ();
				ret.Genes.Add (G0);
				ret.Genes.Add (G1);
			}
			// Color (no entra
			if (r.NextDouble () < 0.05)
				clr = (ConsoleColor)r.Next (16);
			return ret;
		}

		public override string ToString ()
		{
			string ret = "(";
			foreach (var x in Genes) {
				ret += x.ToString ();
			}
			ret += ")";

			return ret;
		}

		public override void Ejecutar (MemStack Mem, Historial H = null)
		{
			foreach (var x in Genes) {
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
			ret [this.ToString ()] += 1; 
			foreach (var x in Genes) {
				if (Hereditario) {
					foreach (var y in x.CuentaGen(true).Data) {
						ret [y.Key] += y.Value; 
					}
				} else {
					ret [x.ToString ()] += 1;
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
			ret.Instrucción = Instrucción;

			return ret;
		}

		public override string ToString ()
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
				if (StackSize >= 1)
					Mem.Push (1 - Mem.Pop ());
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
	public class ContadorGen:ListaPeso<string, int>
	{
		public ContadorGen ():base((x,y) => x+y, 0)
		{
		}
	}
}