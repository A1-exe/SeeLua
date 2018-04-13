using System.Collections.Generic;

namespace SeeLua.Abstracted {
	// Note that Deserializer and Serializer use different patterns
	// in how they handle things
	abstract public class Serializer {
		protected List<byte> Bytecode;

		abstract protected void SetHeader(LuaProto Pr);
		abstract protected void Ser(LuaProto Pr);

		public byte[] GetBytecode(LuaProto Proto) {
			Bytecode = new List<byte>();

			SetHeader(Proto);
			Ser(Proto);

			return Bytecode.ToArray();
		}
	}
}
