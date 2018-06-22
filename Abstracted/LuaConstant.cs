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
				case LuaConstantType.NONE:
				case LuaConstantType.NIL:
					Ret = "nil";
					break;
				case LuaConstantType.BOOL:
					Ret = Boolean.ToString().ToLower();
					break;
				case LuaConstantType.NUMBER:
					if (Double.IsPositiveInfinity(Number)) {
						Ret = Double.MaxValue.ToString();
					}
					else if (Double.IsNegativeInfinity(Number)) {
						Ret = Double.MinValue.ToString();
					}
					else {
						Ret = Number.ToString();
					}
					break;
				case LuaConstantType.STRING:
					Ret = String;
					break;
				default:
					throw new InvalidOperationException("Constant type not supported");
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
