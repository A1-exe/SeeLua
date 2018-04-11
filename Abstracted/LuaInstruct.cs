using System;
using System.Runtime.CompilerServices;
using static SeeLua.Abstracted.LuaInstruct.LuaOpType;

namespace SeeLua.Abstracted {
	struct Field { // Easier handling for masks
		private uint Mask;
		private int Offset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Get(uint O) {
			return (O & Mask) >> Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Set(uint O, int V) {
			return (O & ~Mask) | (uint) V << Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Field(int Start, int Length) { // 0 based
			Mask = (uint) (~((~0) << Length)) << Start;
			Offset = Start;
		}
	}

	sealed class LuaInstruct {
		static Field I_Mask = new Field(0, 6);
		static Field A_Mask = new Field(6, 8);
		static Field B_Mask = new Field(23, 18);
		static Field Bx_Mask = new Field(14, 18);
		static Field C_Mask = new Field(14, 9);

		[Flags] public enum LuaOpType : byte {
			NOFLAG = 0,
			OA = 1, OB = 2,
			OBx = 4, OsBx = 8,
			OC = 16
		}

		static LuaOpType[] Types = {
			OA | OB, OA | OBx, OA | OB | OC, OA | OB, // move...
			OA | OBx, OA | OBx, OA | OB | OC, // getupval...
			OA | OBx, OA | OBx, OA | OB | OC, // setglobal...
			OA | OB | OC, OA | OB | OC, // newtable...
			OA | OB | OC, OA | OB | OC, OA | OB | OC, OA | OB | OC, OA | OB | OC, OA | OB | OC, // add...
			OA | OB, OA | OB, OA | OB, OA | OB | OC, OsBx, // umn...
			OA | OB | OC, OA | OB | OC, OA | OB | OC, OA | OB | OC, OA | OB | OC, // eq...
			OA | OB | OC, OA | OB, OA | OB | OC, // call...
			OA | OsBx, OA | OsBx, OA | OB | OC, // forloop...
			OA | OB | OC, OA | OB, OA | OBx, OA | OB, // setlist...
			NOFLAG
		};

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

		public LuaOpType Type {
			get => Types[(int) Op];
		}

		public uint Instr;
		public LuaOpcode Op {
			get {
				LuaOpcode O = LuaOpcode.NULL;
				byte I = (byte) I_Mask.Get(Instr);

				if (Enum.IsDefined(typeof(LuaOpcode), I)) {
					O = (LuaOpcode) I;
				}

				return O;
			}
			set => Instr = I_Mask.Set(Instr, (int) value);
		}

		public int A {
			get {
				uint Res = 0;
				
				if (Type.HasFlag(OA)) {
					Res = A_Mask.Get(Instr);
				};

				return (int) Res;
			}
			set {
				if (Type.HasFlag(OA)) {
					Instr = A_Mask.Set(Instr, value);
				}
			}
		}

		public int B {
			get {
				LuaOpType T = Type;
				uint Res = 0;

				if (T.HasFlag(OB)) { // B
					Res = B_Mask.Get(Instr);
				}
				else if (T.HasFlag(OBx)) { // Bx
					Res = Bx_Mask.Get(Instr);
				}
				else if (T.HasFlag(OsBx)) { // sBx
					Res = Bx_Mask.Get(Instr) - 131071;
				}

				return (int) Res;
			}
			set {
				LuaOpType T = Type;

				if (T.HasFlag(OB)) {
					Instr = B_Mask.Set(Instr, value);
				}
				else if (T.HasFlag(OBx)) { // Bx
					Instr = Bx_Mask.Set(Instr, value);
				}
				else if (T.HasFlag(OsBx)) { // sBx
					Instr = Bx_Mask.Set(Instr, value + 131071);
				}
			}
		}

		public int C {
			get {
				uint Res = 0;

				if (Type.HasFlag(OC)) { // C
					Res = C_Mask.Get(Instr);
				}

				return (int) Res;
			}
			set {
				if (Type.HasFlag(OC)) { // C
					Instr = C_Mask.Set(Instr, value);
				}
			}
		}

		public override string ToString() =>
			String.Format("{0} ({1}, {2}, {3})", Op.ToString(), A, B, C);

		public LuaInstruct(uint Int) {
			Instr = Int;
		}

		public LuaInstruct(int Int) {
			Instr = (uint) Int;
		}
	}
}
