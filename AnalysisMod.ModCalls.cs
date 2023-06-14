using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisCommon.Systems;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod
{
    // This is a partial class, meaning some of its parts were split into other files. See AnalysisMod.*.cs for other portions.
    // ����һ�������࣬��ζ������һЩ���ֱ���ֵ������ļ��С���μ�AnalysisMod.*.cs�Ի�ȡ�������֡�
    partial class AnalysisMod
	{
        // The following code allows other mods to "call" Analysis Mod data.
        // This allows mod developers to access Analysis Mod's data without having to set it a reference.
        // Mod calls are not exposed by default, so it will be up to you to publish appropriate calls for your mod, and what values they return.

        // ���´�����������ģ�顰���á�Analysis Mod���ݡ�
        // ��ʹ��ģ�鿪���߿����ڲ����������õ�����·���Analysis Mod�����ݡ�
        // ģ�����Ĭ������²��ṫ�����������Ҫ�����ʵ��ĵ�����Ϊ����ģ���ṩ���񣬲�������Ӧֵ��
        public override object Call(params object[] args) {
            // Make sure the call doesn't include anything that could potentially cause exceptions.
            // ȷ�����ò������κο��ܵ����쳣�����ݡ�
            if (args is null) {
				throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
			}

			if (args.Length == 0) {
				throw new ArgumentException("Arguments cannot be empty!");
			}

            // This check makes sure that the argument is a string using pattern matching.
            // Since we only need one parameter, we'll take only the first item in the array..

            // �˼��ȷ��������ʹ��ģʽƥ��ȷ��Ϊ�ַ������͡�
            // ��Ϊ����ֻ��Ҫһ���������������ǽ���ȡ�����еĵ�һ��..
            if (args[0] is string content) {
                // ..And treat it as a command type.
                // ..��������Ϊ�������͡�
                switch (content) {
					case "downedMinionBoss":
                        // Returns the value provided by downedMinionBoss, if the argument calls for it.
                        // �������Ҫ���򷵻�downedMinionBoss�ṩ��ֵ��

                        return DownedBossSystem.downedMinionBoss;
					case "showMinionCount":
                        // Returns the value provided by showMinionCount, if the argument calls for it.
                        // �������Ҫ���򷵻�showMinionCount�ṩ��ֵ��

                        return Main.LocalPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount;
					case "setMinionCount":
                        // We need to make sure the call is provided with a value to set the field to.
                        // ������Ҫȷ�������ṩ��һ��Ҫ�����ֶ�ֵΪ��ֵ��

                        if (args[1] is not bool minionSet) {
                            // If it's not the type we need, we can't continue
                            // Tell the developer what type we need, and what we got instead.

                            // ���������������Ҫ�����ͣ������޷�����
                            // ���߿�����Ա������Ҫʲô�����Լ�ʵ�ʻ����ʲô���͡�
                            throw new Exception($"Expected an argument of type bool when setting minion count, but got type {args[1].GetType().Name} instead.");
						}

                        // We'll set the value to what the argument provided.
                        // Optionally, you can return a value indicating that the assignment was successful.
                        // return true;

                        // ���ǽ��Ѹ�ֵ����Ϊ�����ṩ����ֵ��
                        // ��ѡ�أ������Է���ָʾ��ֵ�ɹ���ʧ��״̬��Ϣ��
                        // ����true;
                        Main.LocalPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount = minionSet;

                        // Return a 'true' boolean as one of the many ways to tell that the operation succeeded.
                        // ����'true'��������Ϊ�����ɹ���־֮һ�ַ�ʽ��
                        return true;
				}
			}

            // We can also do this with different data types.
            // ����Ҳ����ʹ�ò�ͬ������������ʵ����һ�㡣
            if (args[0] is int contentInt && contentInt == 4) {
				return ModContent.GetInstance<AnalysisBiomeTileCount>().AnalysisBlockCount;
			}

            // If the arguments provided don't match anything we wanted to return a value for, we'll return a 'false' boolean.
            // This value can be anything you would like to provide as a default value.

            // ����ṩ�Ĳ�����������Ҫ����ֵ���κ����ݶ���ƥ�䣬�򽫷���'false'�����͡�
            // ��ֵ��������ϣ����ΪĬ��ֵ�ṩ���κ����ݡ�
            return false;
		}
	}
}