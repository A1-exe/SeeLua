using SeeLua.Abstracted;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SeeLua.Lua51 {
	class Serializer51 : Serializer {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DumpByte(byte Byte) => // I'm lazy, tell me about it
			Bytecode.Add(Byte);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DumpBytes(byte[] Bytes) =>
			Bytecode.AddRange(Bytes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DumpInt(int Int) =>
			DumpBytes(BitConverter.GetBytes(Int));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DumpString(string Str) {
			if (Str.Length == 0) {
				DumpInt(0);
			}
			else {
				DumpInt(Str.Length + 1);
				DumpBytes(StaticsData.EightBit.GetBytes(Str));

				DumpByte(0);
			}
		}

		private void DumpInstructions(List<LuaInstruct> Instructs) {
			DumpInt(Instructs.Count); // Number instructions

			for (int Idx = 0; Idx < Instructs.Count; Idx++) {
				DumpInt((int) Instructs[Idx].Instr);
			}
		}

		private void DumpConstants(List<LuaConstant> Constants) {
			DumpInt(Constants.Count);

			for (int Idx = 0; Idx < Constants.Count; Idx++) {
				LuaConstant Const = Constants[Idx];

				DumpByte((byte) Const.Type);

				switch (Const.Type) {
					case LuaConstant.LuaConstantType.BOOL:
						DumpBytes(BitConverter.GetBytes(Const.Boolean));

						break;
					case LuaConstant.LuaConstantType.NUMBER:
						DumpBytes(BitConverter.GetBytes(Const.Number));

						break;
					case LuaConstant.LuaConstantType.STRING:
						DumpString(Const.String);

						break;
				}
			}
		}

		private void DumpProtos(List<LuaProto> Protos) {
			DumpInt(Protos.Count);

			for (int Idx = 0; Idx < Protos.Count; Idx++) {
				Ser(Protos[Idx]);
			}
		}

		private void DumpLines(List<int> Lines) {
			DumpInt(Lines.Count);

			for (int Idx = 0; Idx < Lines.Count; Idx++) {
				DumpInt(Lines[Idx]);
			}
		}

		private void DumpLocals(List<LuaLocal> Locals) {
			DumpInt(Locals.Count);

			for (int Idx = 0; Idx < Locals.Count; Idx++) {
				LuaLocal Local = Locals[Idx];

				DumpString(Local.Name);
				DumpInt(Local.Startpc);
				DumpInt(Local.Endpc);
			}
		}

		private void DumpUpvalues(List<string> Upvalues) {
			DumpInt(Upvalues.Count);

			for (int Idx = 0; Idx < Upvalues.Count; Idx++) {
				DumpString(Upvalues[Idx]);
			}
		}

		protected override void SetHeader(LuaProto Pr) {
			for (int S = 0; S < StaticsData.LuaSignature.Length; S++) {
				DumpByte(StaticsData.LuaSignature[S]);
			}

			DumpByte(Pr.Version);

			for (int S = 0; S < Pr.Header.Length; S++) {
				DumpByte(Pr.Header[S]);
			}
		}

		protected override void Ser(LuaProto Pr) {
			DumpString(Pr.Name);
			DumpInt(Pr.LineBegin); // Start
			DumpInt(Pr.LineEnd); // End

			DumpByte(Pr.Nups);
			DumpByte(Pr.Numparams);
			DumpByte(Pr.Vararg);
			DumpByte(Pr.Stack);

			DumpInstructions(Pr.Instructs);
			DumpConstants(Pr.Consts);
			DumpProtos(Pr.Protos);
			DumpLines(Pr.Lines);
			DumpLocals(Pr.Locals);
			DumpUpvalues(Pr.Upvalues);
		}
	}
}
