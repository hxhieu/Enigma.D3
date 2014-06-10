public class RecipeIngredient : MemoryObject
{
	// 2.0.5.24017
	public const int SizeOf = 0x08; // 8

	public RecipeIngredient(ProcessMemory memory, int address)
		: base(memory, address) { }

	public DT_GBID x00_ItemsGameBalanceId { get { return Field<DT_GBID>(0x00); } }
	public int x04 { get { return Field<int>(0x04); } }
}