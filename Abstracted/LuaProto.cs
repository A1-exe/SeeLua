using System;
using System.Collections.Generic;

namespace SeeLua.Abstracted {
	abstract public class LuaProto {
		// Proto info
		public LuaProto Parent; // Special field for testing
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
		
		// Virtual methods for descendants
		virtual public byte[] Serialize() => throw new NotSupportedException("Can not serialize proto");
		virtual public void StripDebug() => throw new NotSupportedException("Can not strip debug data");
		virtual public void Cascade() => throw new NotSupportedException("Can not cascade data");
		virtual public void Repair() => throw new NotSupportedException("Can not repair proto");
	}
}
