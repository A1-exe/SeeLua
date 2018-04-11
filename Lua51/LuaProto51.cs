using SeeLua.Abstracted;

namespace SeeLua.Lua51 {
	class LuaProto51 : LuaProto {
		static public byte[] Signature = { 0, 1, 4, 4, 4, 8, 0 };

		public override byte Version { get => 0x51; }
		public override byte[] Serialize() {
			return new Serializer51().GetBytecode(this);
		}
	}
}
