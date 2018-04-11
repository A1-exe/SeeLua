using System;

namespace SeeLua.Abstracted {
	sealed class LuaLocal {
		public string Name;
		public int Startpc;
		public int Endpc;

		public override string ToString() {
			return String.Format("{0} ({1}, {2})", Name, Startpc, Endpc);
		}

		public LuaLocal(string N, int S, int E) {
			Name = N;
			Startpc = S;
			Endpc = E;
		}
	}
}
