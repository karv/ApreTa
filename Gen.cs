using System;
using System.Collections.Generic;

namespace ApreTa
{
	/// <summary>
	/// Un gen.
	/// </summary>
	public abstract class Gen
	{
		public abstract Gen Replicar ();

		public abstract void Ejecutar (MemStack Mem, Historial H = null);
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
		public override Gen Replicar ()
		{
			GrupoGen ret = new GrupoGen ();
			foreach (var x in Genes) {
				ret.Genes.Add (x.Replicar ());
			}
			return ret;
		}

		public override void Ejecutar (MemStack Mem, Historial H = null)
		{
			foreach (var x in Genes) {
				x.Ejecutar (Mem);
			}
		}
	}

	/// <summary>
	/// Un gen activo.
	/// </summary>
	public class InstrucciónGen:Gen
	{
		public string Instrucción;

		public override Gen Replicar ()
		{
			InstrucciónGen ret = new InstrucciónGen ();
			ret.Instrucción = Instrucción;

			return ret;
		}

		public override void Ejecutar (MemStack Mem, Historial H = null)
		{
			int StackSize = Mem.Count;
			switch (Instrucción) {
			case "!":
				Mem.Push (1 - Mem.Pop ());
				break;
			case "+":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () + Mem.Pop ());
				break;
			case "*":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () * Mem.Pop ());
				break;
			case "-":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () - Mem.Pop ());
				break;
			case "%":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () % Mem.Pop ());
				break;
			case "?":
				if (StackSize >= 2)
					Mem.Push (Mem.Pop () != 0 ? Mem.Pop () : Mem.Pop ());
				break;
			case "<":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () < Mem.Pop () ? 1 : 0);
				break;
			case "=":
				if (StackSize >= 1)
					Mem.Push (Mem.Pop () == Mem.Pop () ? 1 : 0);
				break;
			case "h":
				if (Mem.Peek () < H.Actual && Mem.Peek () >= 0 && StackSize >= 1)
					Mem.Push (H.Data [1, Mem.Pop () - H.Actual]);
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
}