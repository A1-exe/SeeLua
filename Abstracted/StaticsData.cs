using System;
using System.IO;
using System.Text;
using SeeLua.Lua51;

namespace SeeLua.Abstracted {
	static public class StaticsData {
		static public byte[] LuaSignature = { 27, 76, 117, 97 };
		static public Encoding EightBit = Encoding.GetEncoding(28591);
		public static LuaConstant LuaNil = new LuaConstant(LuaConstantType.NIL, null);

		public enum LuaOpcode : byte {
			MOVE, LOADK, LOADBOOL, LOADNIL,
			GETUPVAL, GETGLOBAL, GETTABLE,
			SETGLOBAL, SETUPVAL, SETTABLE,
			NEWTABLE, SELF,
			ADD, SUB, MUL, DIV, MOD, POW,
			UNM, NOT, LEN, CONCAT, JMP,
			EQ, LT, LE, TEST, TESTSET,
			CALL, TAILCALL, RETURN,
			FORLOOP, FORPREP, TFORLOOP,
			SETLIST, CLOSE, CLOSURE, VARARG,
			NULL // Not a real opcode
		}

		[Flags]
		public enum LuaOpType : byte {
			NOFLAG = 0,
			OA = 1, OB = 2,
			OBx = 4, OsBx = 8,
			OC = 16
		}

		public enum LuaConstantType : byte {
			NIL, BOOL, NONE, NUMBER, STRING
		}
		
		static public string ReadCString(this BinaryReader Stream) {
			string Result = string.Empty;
			int Length = Stream.ReadInt32();

			if (Length != 0) { // hmmMMMMm
				Result = EightBit.GetString(Stream.ReadBytes(Length - 1));

				Stream.ReadByte(); // null terminator
			}

			return Result;
		}

		public static bool IsRegist(int P) =>
			(P & 0x100) == 0;

		public static string Sanitize(this string Dirty) {
			StringBuilder Result = new StringBuilder();
			string Repl;

			foreach (byte Byte in EightBit.GetBytes(Dirty)) {
				switch (Byte) {
					case ((byte) '"'):
						Repl = "\\\"";

						break;
					case ((byte) '\\'):
						Repl = "\\\\";

						break;
					case ((byte) '\b'):
						Repl = "\\b";

						break;
					case ((byte) '\n'):
						Repl = "\\n";

						break;
					case ((byte) '\r'):
						Repl = "\\r";

						break;
					case ((byte) '\t'):
						Repl = "\\t";

						break;
					default:
						char Real = (char) Byte;

						if (!Char.IsWhiteSpace(Real) && ((Byte < 33) || (Byte > 126)))
							Repl = $"\\{Byte:D3}";
						else
							Repl = Real.ToString();

						break;
				}

				Result.Append(Repl);
			}

			return Result.ToString();
		}
	}
}
