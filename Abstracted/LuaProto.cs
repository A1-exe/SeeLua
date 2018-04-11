using System;
using System.Collections.Generic;

namespace SeeLua.Abstracted {
	abstract class LuaProto {
		// Proto info
		public byte[] Header; // For rebuilding chunks
		public string Name;

		public List<LuaInstruct> Instructs;
		public List<LuaConstant> Consts;
		public List<LuaProto> Protos;

		public byte Nups;
		public byte Numparams;
		public byte Vararg;
		public byte Stack;

		// Proto debug info
		public List<LuaLocal> Locals;
		public List<String> Upvalues;
		public List<Int32> Lines;

		public int LineBegin;
		public int LineEnd;

		abstract public byte Version { get; }
		virtual public byte[] Serialize() => throw new NotSupportedException("Type can not be serialized");
	}
}
