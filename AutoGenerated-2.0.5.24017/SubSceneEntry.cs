public class SubSceneEntry : MemoryObject
{
	// 2.0.5.24017
	public const int SizeOf = 0x20; // 32

	public SubSceneEntry(ProcessMemory memory, int address)
		: base(memory, address) { }

	public int x00_SceneSnoId { get { return Field<int>(0x00); } }
	public int x04 { get { return Field<int>(0x04); } }
	public int x08 { get { return Field<int>(0x08); } }
	public int x10_Count { get { return Field<int>(0x10); } }
	public SubSceneLabel[] x14_PtrArray { get { return Dereference<SubSceneLabel>(0x14, x10_Count); } }
	public SerializeData x18 { get { return Field<SerializeData>(0x18); } }
}