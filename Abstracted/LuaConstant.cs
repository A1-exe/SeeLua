using System;
using static SeeLua.Abstracted.StaticsData;

namespace SeeLua.Abstracted {
	sealed public class LuaConstant {
		private object Value;
		public LuaConstantType Type;

		public bool Boolean => (bool) Value;
		public int Integer => (int) Value;
		public double Number => (double) Value;
		public string String => (string) Value;

		public override string ToString() {
			string Ret = string.Empty;

			switch (Type) {
				case LuaConstantType.NIL:
					Ret = "nil";
					break;
				case LuaConstantType.BOOL:
					Ret = Boolean.ToString();
					break;
				case LuaConstantType.NUMBER:
					Ret = Number.ToString();
					break;
				case LuaConstantType.STRING:
					Ret = String;
					break;
			}

			return Ret;
		}

		static public implicit operator LuaConstant(bool O) =>
			new LuaConstant(LuaConstantType.BOOL, O);

		static public implicit operator LuaConstant(double O) =>
			new LuaConstant(LuaConstantType.NUMBER, O);

		static public implicit operator LuaConstant(string O) =>
			new LuaConstant(LuaConstantType.STRING, O);

		public LuaConstant(LuaConstantType T, object Val = null) {
			Type = T;
			Value = Val;
		}
	}
}
