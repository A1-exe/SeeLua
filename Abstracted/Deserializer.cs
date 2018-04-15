using System;
using System.IO;
using SeeLua.Lua51;

namespace SeeLua.Abstracted {
	// Light-weight-ish deserializer base class
	abstract public class Deserializer {
		protected BinaryReader Stream;
		private byte[] Bytecode;

		abstract protected void Assert(); // Should ensure bytecode matches
		abstract protected LuaProto Deser(); // Should return a new proto

		static public LuaProto ResolveProto(byte Version) {
			switch (Version) {
				case 0x51:
					return new LuaProto51();
				default:
					throw new NotSupportedException("Lua version could not be resolved");
			}
		}

		static public Deserializer Resolve(byte[] Byte) {
			if (Byte.Length > 4) {
				byte Version = Byte[4];

				switch (Version) {
					case 0x51:
						return new Deserializer51(Byte);
				}
			}

			throw new NotSupportedException("Lua version could not be resolved");
		}

		private void NewStream() {
			MemoryStream Code = new MemoryStream(Bytecode, false);

			if (Stream != null) {
				Stream.Dispose();
			}

			Stream = new BinaryReader(Code, StaticsData.EightBit, false);
		}

		public void SetChunk(byte[] Byte) {
			if ((Byte == null) || (Byte.Length == 0)) {
				throw new ArgumentNullException("The reader does not have a bytecode stream");
			}

			Bytecode = Byte;

			NewStream();
		}
		
		public LuaProto GetProto() {
			Stream.BaseStream.Position = 0; // Re-start the stream

			for (int Idx = 0; Idx < StaticsData.LuaSignature.Length; Idx++) {
				if (Stream.ReadByte() != StaticsData.LuaSignature[Idx]) {
					throw new NotSupportedException("Malformed Lua header");
				}
			}

			Assert(); // Read the Lua info

			return Deser();
		}

		public Deserializer(byte[] Byte) {
			SetChunk(Byte);
		}
	}
}
