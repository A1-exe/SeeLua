using System;
using System.IO;
using System.Text;

namespace SeeLua.Abstracted {
	static class StaticsData {
		static public byte[] LuaSignature = { 27, 76, 117, 97 };
		static public Encoding EightBit = Encoding.GetEncoding(28591);
		
		static public string ReadCString(this BinaryReader Stream) {
			string Result = string.Empty;
			int Length = Stream.ReadInt32();

			if (Length != 0) { // hmmMMMMm
				StringBuilder New = new StringBuilder(--Length);
				byte[] Chars = Stream.ReadBytes(Length);
				
				for (int C = 0; C < Length; C++) {
					New.Append((char) Chars[C]);
				}

				Stream.ReadByte(); // null terminator
				Result = New.ToString();
			}

			return Result;
		}
	}

	// Light-weight-ish deserializer base class
	abstract class Deserializer {
		protected BinaryReader Stream;
		private byte[] Bytecode;

		abstract protected void Assert(); // Should ensure bytecode matches
		abstract protected LuaProto Deser(); // Should return a new proto

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
