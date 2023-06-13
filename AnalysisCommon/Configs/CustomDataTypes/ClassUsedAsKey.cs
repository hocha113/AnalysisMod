using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

// This file defines a custom data type that can be used as a key in dictionary.
// 这个文件定义了一个自定义数据类型，可以用作字典中的键。
namespace AnalysisMod.AnalysisCommon.Configs.CustomDataTypes
{
    //这是一个类型转换器(TypeConverter)的属性，表示将特定类型(ClassUsedAsKey)转换为字符串，
    //以便在代码中进行存储和传输。通过在类型声明中添加这个属性，
    //可以告诉编译器在需要将该类型转换为字符串时使用指定的转换器(ToFromStringConverter)。
    //这可以帮助我们更轻松地处理某些复杂数据类型，尤其是在序列化和反序列化数据时。
    [TypeConverter(typeof(ToFromStringConverter<ClassUsedAsKey>))]
    //使用TypeConverter将某个类型转换为字符串可以方便我们在代码中进行保存和传输。
    //例如，在将对象序列化为XML或JSON格式时，需要将对象中的属性转换为字符串。
    //通过使用TypeConverter，我们可以自定义对象在进行序列化和反序列化时的转换逻辑，
    //使得整个过程更加灵活和可控。此外，TypeConverter还可以用于在属性窗格中编辑和显示对象属性时进行转换。
    public class ClassUsedAsKey
	{
        // When you save data from a dictionary into a file (json), you need to represent the key as a string
        // But to get the object back, you need a TypeConverter, and this Analysis shows how to implement one

        // You start with the [TypeConverter(typeof(ToFromStringConverter<NameOfClassHere>))] attribute above the class
        // For this to work, you need the usual Equals and GetHashCode overrides as explained in the other Analysiss,
        // plus ToString and FromString, which are used to transform your object into a string and back

        // 当你将字典中的数据保存到文件（json）中时，需要将键表示为字符串
        // 但是要获取对象，则需要一个TypeConverter，本分析展示了如何实现

        // 首先，在类上方添加[TypeConverter(typeof(ToFromStringConverter<NameOfClassHere>))]属性
        // 为使其正常工作，您需要像其他分析所述那样重写Equals和GetHashCode，
        // 还需编写ToString和FromString方法，用于将对象转换为字符串并进行反向转换

        public bool SomeBool { get; set; }
		public int SomeNumber { get; set; }

        //在C#中，如果我们要比较两个自定义类型的对象是否相等，就需要重载Equals和GetHashCode方法。
        //在重载Equals方法时，我们需要比较对象的属性值是否相等，而在重载GetHashCode方法时，
        //我们需要将对象的属性值转换为一个哈希码，以便快速判断两个对象是否相等。
        public override bool Equals(object obj) {
			if (obj is ClassUsedAsKey other)
				return SomeBool == other.SomeBool && SomeNumber == other.SomeNumber;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { SomeBool, SomeNumber }.GetHashCode();
		}

        // Here you need to write how the string representation of your object will look like so it is easy to reconstruct again
        // Inside the json file, it will look something like this: "True, 5"
        // 在此处编写对象的字符串表示形式，以便于再次重建
        // 在 JSON 文件中，它看起来像这样："True, 5"
        public override string ToString() {
			return $"{SomeBool}, {SomeNumber}";
		}

        // Here you need to create an object from the given string (reverting ToString basically)
        // This has to be static and it must be named FromString
        // 在这里，您需要从给定的字符串创建一个对象（基本上是反转ToString）
        // 这必须是静态的，并且必须命名为FromString
        public static ClassUsedAsKey FromString(string s) {
            // This following code depends on your implementation of ToString, here we just have two values separated by a ','
            // 以下代码取决于您对ToString的实现，这里我们只有两个由逗号分隔的值

            //这段代码的作用是将字符串s按照逗号分隔符进行拆分，并将拆分后的结果放入string数组vars中。
            //其中，new char[] { ',' }表示分隔符是逗号；2表示最多分隔成两个字符串元素；
            //StringSplitOptions.RemoveEmptyEntries表示在分隔过程中忽略空白项。
            //具体来说，Split方法是用于将一个字符串按照指定的分隔符分割成多个子字符串的方法。
            //它的第一个参数指定字符串的分隔符，这里使用逗号作为分隔符。
            //第二个参数指定最多分隔成的字符串数量，这里设置为2，表示最多分隔成两个字符串。
            //第三个参数指定如何处理空白项，这里使用StringSplitOptions.RemoveEmptyEntries表示在分隔过程中忽略空白项
            string[] vars = s.Split(new char[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
            // The System.Convert class provides methods to transform data types between each other, here using the string overload
            // System.Convert类提供了将数据类型相互转换的方法，这里使用字符串重载
            return new ClassUsedAsKey {
				SomeBool = Convert.ToBoolean(vars[0]),
				SomeNumber = Convert.ToInt32(vars[1])
			};
		}
	}
}
