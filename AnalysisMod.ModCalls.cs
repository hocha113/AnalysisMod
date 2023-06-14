using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisCommon.Systems;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod
{
    // This is a partial class, meaning some of its parts were split into other files. See AnalysisMod.*.cs for other portions.
    // 这是一个部分类，意味着它的一些部分被拆分到其他文件中。请参见AnalysisMod.*.cs以获取其他部分。
    partial class AnalysisMod
	{
        // The following code allows other mods to "call" Analysis Mod data.
        // This allows mod developers to access Analysis Mod's data without having to set it a reference.
        // Mod calls are not exposed by default, so it will be up to you to publish appropriate calls for your mod, and what values they return.

        // 以下代码允许其他模组“调用”Analysis Mod数据。
        // 这使得模组开发者可以在不必设置引用的情况下访问Analysis Mod的数据。
        // 模组调用默认情况下不会公开，因此您需要发布适当的调用来为您的模组提供服务，并返回相应值。
        public override object Call(params object[] args) {
            // Make sure the call doesn't include anything that could potentially cause exceptions.
            // 确保调用不包含任何可能导致异常的内容。
            if (args is null) {
				throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
			}

			if (args.Length == 0) {
				throw new ArgumentException("Arguments cannot be empty!");
			}

            // This check makes sure that the argument is a string using pattern matching.
            // Since we only need one parameter, we'll take only the first item in the array..

            // 此检查确保参数是使用模式匹配确定为字符串类型。
            // 因为我们只需要一个参数，所以我们将仅取数组中的第一项..
            if (args[0] is string content) {
                // ..And treat it as a command type.
                // ..并将其视为命令类型。
                switch (content) {
					case "downedMinionBoss":
                        // Returns the value provided by downedMinionBoss, if the argument calls for it.
                        // 如果参数要求，则返回downedMinionBoss提供的值。

                        return DownedBossSystem.downedMinionBoss;
					case "showMinionCount":
                        // Returns the value provided by showMinionCount, if the argument calls for it.
                        // 如果参数要求，则返回showMinionCount提供的值。

                        return Main.LocalPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount;
					case "setMinionCount":
                        // We need to make sure the call is provided with a value to set the field to.
                        // 我们需要确保调用提供了一个要设置字段值为的值。

                        if (args[1] is not bool minionSet) {
                            // If it's not the type we need, we can't continue
                            // Tell the developer what type we need, and what we got instead.

                            // 如果它不是我们需要的类型，我们无法继续
                            // 告诉开发人员我们需要什么类型以及实际获得了什么类型。
                            throw new Exception($"Expected an argument of type bool when setting minion count, but got type {args[1].GetType().Name} instead.");
						}

                        // We'll set the value to what the argument provided.
                        // Optionally, you can return a value indicating that the assignment was successful.
                        // return true;

                        // 我们将把该值设置为参数提供给定值。
                        // 可选地，您可以返回指示赋值成功或失败状态信息。
                        // 返回true;
                        Main.LocalPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount = minionSet;

                        // Return a 'true' boolean as one of the many ways to tell that the operation succeeded.
                        // 返回'true'布尔型作为操作成功标志之一种方式。
                        return true;
				}
			}

            // We can also do this with different data types.
            // 我们也可以使用不同的数据类型来实现这一点。
            if (args[0] is int contentInt && contentInt == 4) {
				return ModContent.GetInstance<AnalysisBiomeTileCount>().AnalysisBlockCount;
			}

            // If the arguments provided don't match anything we wanted to return a value for, we'll return a 'false' boolean.
            // This value can be anything you would like to provide as a default value.

            // 如果提供的参数与我们想要返回值的任何内容都不匹配，则将返回'false'布尔型。
            // 此值可以是您希望作为默认值提供的任何内容。
            return false;
		}
	}
}