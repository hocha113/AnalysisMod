using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;

// This file contains fake ModConfig class that showcase creating config section
// by using fields with various data types.
// 这个文件包含了一个虚假的 ModConfig 类，展示了如何使用不同数据类型的字段创建配置部分。

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(144, 252, 249)]
	public class ModConfigShowcaseDataTypes : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        // Value Types
        // 值类型
        public bool SomeBool;
		public int SomeInt;
		public float SomeFloat;
		public string SomeString;
		public EquipType SomeEnum;
		public byte SomeByte;
		public uint SomeUInt;

        // Structs - These require special code. We've implemented Color and Vector2 so far.
        // 结构体 - 这些需要特殊的代码。我们目前已经实现了Color和Vector2。
        public Color SomeColor;
		public Vector2 SomeVector2;
		public Point SomePoint; // notice the not implemented message.
                                // 注意未实现的消息。

        // Data Structures (Reference Types)
        // 数据结构（引用类型）
        public int[] SomeArray = new int[] { 25, 70, 12 }; // Arrays have a specific length and need a default value specified.
														   // 数组具有特定的长度，需要指定默认值。
        public List<int> SomeList = new List<int>() { 1, 3, 5 }; // Initializers can be used to declare defaults for data structures.
																 // 初始化器可用于为数据结构声明默认值。
        public Dictionary<string, int> SomeDictionary = new Dictionary<string, int>();
		public HashSet<string> SomeSet = new HashSet<string>();

        // Classes (Reference Types) - Classes are automatically implemented in the UI.
        // 类（引用类型）- 类在 UI 中自动实现。
        public SimpleData SomeClassA;
        // EntityDefinition classes store the identity of an Entity (Item, NPC, Projectile, etc) added by a mod or vanilla. Only the identity is preserved, not other mod data or stack.
        // When using XDefinition classes, you can the .Type property to get the ID of the item. You can use .IsUnloaded to check if the item in question is loaded.
        // EntityDefinition类存储由模组或原版添加的实体（物品、NPC、弹射物等）的标识。仅保留标识，不包括其他模组数据或堆栈。
        // 使用XDefinition类时，您可以使用.Type属性获取物品ID。您可以使用.IsUnloaded检查所讨论的项目是否已加载。
        public ItemDefinition itemDefinitionAnalysis;
		public NPCDefinition npcDefinitionAnalysis = new NPCDefinition(NPCID.Bunny);
        //public ProjectileDefinition projectileDefinitionAnalysis = new ProjectileDefinition("AnalysisMod", nameof(Content.Projectiles.AnalysisHomingProjectile));

        // Data Structures of reference types
        // 引用类型的数据结构
        /* TODO: Add this back in once AnalysisMod adds a ModPrefix Analysis.
         * 待办事项：在AnalysisMod添加ModPrefix Analysis后再将其添加回来。
		public Dictionary<PrefixDefinition, float> SomeClassE = new Dictionary<PrefixDefinition, float>() {
			[new PrefixDefinition("AnalysisMod", "Awesome")] = 0.5f,
			[new PrefixDefinition("AnalysisMod", "ReallyAwesome")] = 0.8f
		};
		*/

        // TODO: Not working at the moment.
        // Using a custom class as a key in a Dictionary. When used as a Dictionary Key, special code must be used.
        // TODO: 目前无法工作。
        // 在字典中使用自定义类作为键时，必须使用特殊代码。 
        public Dictionary<ClassUsedAsKey, Color> CustomKey = new Dictionary<ClassUsedAsKey, Color>();

		public ModConfigShowcaseDataTypes() {
            // Doing the initialization of defaults for reference types in a constructor is also acceptable.
            // 在构造函数中对引用类型进行默认值初始化也是可以接受的。
            SomeClassA = new SimpleData() {
				percent = .85f
			};

			CustomKey.Add(new ClassUsedAsKey() {
				SomeBool = true,
				SomeNumber = 42
			},
			new Color(1, 2, 3, 4));

			itemDefinitionAnalysis = new ItemDefinition("Terraria/GoldOre"); // EntityDefinition uses ItemID field names rather than the numbers themselves for readability.
                                                                             // EntityDefinition使用ItemID字段名称而不是数字本身，以提高可读性。
        }
	}
}
