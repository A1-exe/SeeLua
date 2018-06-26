using SeeLua.Abstracted;
using System;
using System.Collections.Generic;
using static SeeLua.Abstracted.StaticsData;

namespace SeeLua.Lua51 {
	sealed public class Deserializer51 : Deserializer {
		private LuaProto51 Proto;
		
		protected override void Assert() {
			if (Stream.ReadByte() != Proto.Version) {
				throw new NotSupportedException("Lua 5.1 bytecode expected");
			}

			for (int L = 0; L < LuaProto51.DefaultHeader.Length; L++) {
				if (LuaProto51.DefaultHeader[L] != Stream.ReadByte()) {
					throw new NotSupportedException("Unsupported bytecode stream");
				}
			}
		}

		protected override LuaProto Deser() {
			Proto = new LuaProto51() {
				Header = LuaProto51.DefaultHeader
			};

			ReadHeader();
			ReadInstructions();
			ReadConstants();
			ReadProtos();
			ReadLines();
			ReadLocals();
			ReadUpvalues();

			return Proto;
		}
		
		private void ReadHeader() {
			Proto.Name = Stream.ReadCString();

			Proto.LineBegin = Stream.ReadInt32();
			Proto.LineEnd = Stream.ReadInt32();

			Proto.Nups = Stream.ReadByte();
			Proto.Numparams = Stream.ReadByte();
			Proto.Vararg = Stream.ReadByte();
			Proto.Stack = Stream.ReadByte();
		}

		private void ReadInstructions() {
			int Count = Stream.ReadInt32();
			List<LuaInstruct> Instructs = new List<LuaInstruct>(Count);

			for (int Idx = 0; Idx < Count; Idx++) {
				Instructs.Add(new LuaInstruct(Stream.ReadUInt32()));
			}

			Proto.Instructs = Instructs;
		}

		private void ReadConstants() {
			int Count = Stream.ReadInt32();
			List<LuaConstant> Consts = new List<LuaConstant>(Count);

			for (int Idx = 0; Idx < Count; Idx++) {
				LuaConstantType Type = (LuaConstantType) Stream.ReadByte();
				LuaConstant Const = LuaNil;

				if (Type == LuaConstantType.BOOL) {
					Const = Stream.ReadBoolean();
				}
				else if (Type == LuaConstantType.NUMBER) {
					Const = Stream.ReadDouble();
				}
				else if (Type == LuaConstantType.STRING) {
					Const = Stream.ReadCString();
				}

				Consts.Add(Const);
			}

			Proto.Consts = Consts;
		}

		private void ReadProtos() {
			int Count = Stream.ReadInt32();
			List<LuaProto> Protos = new List<LuaProto>(Count);
			LuaProto51 This = Proto; // Create a reference so we evade a new variable

			for (int Idx = 0; Idx < Count; Idx++) {
				Protos.Add(Deser());
			}

			Proto = This;
			Proto.Protos = Protos;
		}

		private void ReadLines() {
			int Count = Stream.ReadInt32();
			List<int> Lines = new List<int>(Count);
			
			for (int Idx = 0; Idx < Count; Idx++) {
				Lines.Add(Stream.ReadInt32());
			}

			Proto.Lines = Lines;
		}

		private void ReadLocals() {
			int Count = Stream.ReadInt32();
			List<LuaLocal> Locals = new List<LuaLocal>(Count);

			for (int Idx = 0; Idx < Count; Idx++) {
				string Name = Stream.ReadCString();
				int Start = Stream.ReadInt32();
				int End = Stream.ReadInt32();

				Locals.Add(new LuaLocal(Name, Start, End));
			}

			Proto.Locals = Locals;
		}

		private void ReadUpvalues() {
			int Count = Stream.ReadInt32();
			List<string> Upvalues = new List<string>(Count);

			for (int Idx = 0; Idx < Count; Idx++) {
				Upvalues.Add(Stream.ReadCString());
			}

			Proto.Upvalues = Upvalues;
		}

		public Deserializer51(byte[] Byte) : base(Byte) {
			Proto = new LuaProto51();
		}
	}
}
