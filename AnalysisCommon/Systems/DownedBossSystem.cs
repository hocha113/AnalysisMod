using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisCommon.Systems
{
    // Acts as a container for "downed boss" flags.
    // Set a flag like this in your bosses OnKill hook:

    // 用于容纳“已击败的Boss”标志。
    // 在你的Boss OnKill钩子中设置一个标志，如下所示：

    //    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

    // Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    // 保存和加载这些标志需要使用TagCompounds，有一份指南可以在维基上找到：https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    public class DownedBossSystem : ModSystem
    {
        public static bool downedMinionBoss = false;
        // public static bool downedOtherBoss = false;

        public override void ClearWorld()
        {
            downedMinionBoss = false;
            // downedOtherBoss = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.

        //我们使用TagCompounds保存数据集。
        //注意：此处提供的标签实例默认为空。
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedMinionBoss)
            {
                tag["downedMinionBoss"] = true;
            }

            // if (downedOtherBoss) {
            //	tag["downedOtherBoss"] = true;
            // }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedMinionBoss = tag.ContainsKey("downedMinionBoss");
            // downedOtherBoss = tag.ContainsKey("downedOtherBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            //操作顺序很重要，必须与NetReceive的操作顺序相匹配
            var flags = new BitsByte();
            flags[0] = downedMinionBoss;
            // flags[1] = downedOtherBoss;
            writer.Write(flags);

            /*
			Remember that Bytes/BitsByte only have up to 8 entries. If you have more than 8 flags you want to sync, use multiple BitsByte:
            请记住，Bytes / BitsByte最多只有8个条目。如果您想同步超过8个标志，请使用多个BitsByte：

				This is wrong:
			flags[8] = downed9thBoss; // an index of 8 is nonsense.
                                      // 索引为8是无意义的。

				This is correct:
			flags[7] = downed8thBoss;
			writer.Write(flags);
			BitsByte flags2 = new BitsByte(); // create another BitsByte
                                              // 创建另一个BitsByte

			flags2[0] = downed9thBoss; // start again from 0
                                       // 从0开始再次

			// up to 7 more flags here
			writer.Write(flags2); // write this byte
                                  // 写入这个字节
			*/

            // If you prefer, you can use the BitsByte constructor approach as well.
            //如果您愿意，也可以使用BitsByte构造函数方法。

            // BitsByte flags = new BitsByte(downedMinionBoss, downedOtherBoss);
            // writer.Write(flags);

            // This is another way to do the same thing, but with bitmasks and the bitwise OR assignment operator (the |=)
            // Note that 1 and 2 here are bit masks. The next values in the pattern are 4,8,16,32,64,128. If you require more than 8 flags, make another byte.

            //这是另一种用位掩码和按位或赋值运算符（| =）完成相同事情的方法
            //请注意，这里的1和2是位掩码。模式中下一个值为4,8,16,32,64,128。如果需要更多于8个标志，请再建立一个字节。

            // byte flags = 0;
            // if (downedMinionBoss)
            // {
            //	flags |= 1;
            // }
            // if (downedOtherBoss)
            // {
            //	flags |= 2;
            // }
            // writer.Write(flags);

            // If you plan on having more than 8 of these flags and don't want to use multiple BitsByte, an alternative is using a System.Collections.BitArray
            //如果您计划拥有超过8个这些标志，并且不想使用多个BitsByte，则替代方案是使用System.Collections.BitArray
            /*
			bool[] flags = new bool[] {
				downedMinionBoss,
				downedOtherBoss,
			};
			BitArray bitArray = new BitArray(flags);
			byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1]; // Calculation for correct length of the byte array
			bitArray.CopyTo(bytes, 0);

			writer.Write(bytes.Length);
			writer.Write(bytes);
			*/
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            // 操作顺序很重要，必须与NetSend的匹配相同
            BitsByte flags = reader.ReadByte();
            downedMinionBoss = flags[0];
            // downedOtherBoss = flags[1];

            // As mentioned in NetSend, BitBytes can contain up to 8 values. If you have more, be sure to read the additional data:
            // 如在NetSend中提到的那样，BitBytes可以包含多达8个值。如果您有更多，请确保阅读附加数据：

            // BitsByte flags2 = reader.ReadByte();
            // downed9thBoss = flags2[0];

            // System.Collections.BitArray 方法:
            /*
			int length = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(length);

			BitArray bitArray = new BitArray(bytes);
			downedMinionBoss = bitArray[0];
			downedOtherBoss = bitArray[1];
			*/
        }
    }
}
