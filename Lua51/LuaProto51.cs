using SeeLua.Abstracted;
using System.Text;

namespace SeeLua.Lua51 {
	sealed public class LuaProto51 : LuaProto {
		static public byte[] Signature = { 0, 1, 4, 4, 4, 8, 0 };

		public override byte Version { get => 0x51; }
		public override byte[] Serialize() {
			return new Serializer51().GetBytecode(this);
		}

		public override void StripDebug(bool Recursive) {
			Name = string.Empty;
			Locals.Clear();
			Upvalues.Clear();
			Lines.Clear();
			LineBegin = 0;
			LineEnd = 0;

			if (Recursive) {
				for (int Idx = 0; Idx < Protos.Count; Idx++) {
					Protos[Idx].StripDebug(true);
				}
			}
		}

		public override string ToString() {
			StringBuilder S = new StringBuilder($"\"{Name}\"");
			S.Append(" (");

			for (int Idx = 0; Idx < Numparams; Idx++) {
				if (Idx != 0) {
					S.Append(", ");
				}

				if (Idx < Locals.Count) {
					S.Append(Locals[Idx].Name);
				}
				else {
					S.Append("?");
				}
			}

			S.Append(")");
			return S.ToString();
		}
	}
}
