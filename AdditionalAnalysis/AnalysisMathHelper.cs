using System;
//这是对MathHelper的拆解，此类对本模组无作用，浏览完成后请注释
//MathHelper的MicrosoftC#文档页面：https://learn.microsoft.com/zh-cn/dotnet/api/opentk.mathhelper?view=xamarin-mac-sdk-14
//涉及知识：计算机图形学:https://en.wikipedia.org/wiki/Computer_graphics_(computer_science)
namespace AnalysisMod.AdditionalAnalysis
{
    public static class AnalysisMathHelper
    {   //以下是定义了一些常用的常量
        public const float E = MathF.E;

        public const float Log10E = 0.4342945f;

        public const float Log2E = 1.442695f;

        public const float Pi = MathF.PI;

        public const float PiOver2 = MathF.PI / 2f;

        public const float PiOver4 = MathF.PI / 4f;

        public const float TwoPi = MathF.PI * 2f;

        internal static readonly float Analysis_MachineEpsilonFloat = Analysis_GetMachineEpsilonFloat();//定义一个静态的、只读的、只在此类可见的浮点值 Analysis_MachineEpsilonFloat 通过 GetMachineEpsilonFloat() 方法获取值
        //这个方法是用来计算三角形内插值的，输入的参数包括三个三角形顶点的值以及两个插值因子。
        //在三角形内部取一点时，通常使用插值法来计算这一点的属性值，其中插值因子表示该点与三角形各个顶点之间的距离比例。
        //这个方法使用巴瑞心坐标系（Barycentric coordinates）来计算插值值，
        //具体来说，就是使用了这个公式：
        // P = v1 + a(v2-v1) + b(v3-v1)
        // 其中，v1、v2和v3分别表示三角形的三个顶点，a和b是两个插值因子，P是计算出的插值点
        public static float Analysis_Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        //贝塞尔曲线
        //这个函数是实现了Catmull-Rom插值算法，用于在给定四个点上进行插值计算，并返回在指定位置上插值得到的数值。
        //Catmull-Rom插值算法可以用于计算在一条曲线上某点的插值值，相对于其他的插值方法，Catmull-Rom插值算法有较好的平滑性，
        //且保持了给定曲线的特征，所以在计算曲线上的点时，比较常用
        //给定四个点value1、value2、value3和value4，Catmull-Rom插值算法通过对这些点进行插值，生成一个新的值。
        //amount参数指定要插值的位置在value2和value3之间，使用该算法可以得到一个新值
        public static float Analysis_CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            double num = amount * amount;
            double num2 = num * (double)amount;
            return (float)(0.5 * (2.0 * (double)value2 + (double)((value3 - value1) * amount) + (2.0 * (double)value1 - 5.0 * (double)value2 + 4.0 * (double)value3 - (double)value4) * num + (3.0 * (double)value2 - (double)value1 - 3.0 * (double)value3 + (double)value4) * num2));
        }

        //这个函数是用来将一个数值限制在一定范围内的函数。它有三个参数，分别是要进行限制的数值、最小值和最大值。
        //函数会首先判断 value 是否大于 max，如果大于，则将其赋值为 max；
        //然后再判断 value 是否小于 min，如果小于，则将其赋值为 min。
        //最后返回限制后的 value 值。
        //这个函数在游戏中经常被用来确保数值不超过一定范围，例如防止玩家输入过大或过小的数值，或者确保一些计算结果不会超出有效范围。
        public static float Analysis_Clamp(float value, float min, float max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        //这是一个计算两个浮点数之间距离的静态方法，可以用于计算数值之间的差异或误差。
        //该方法使用Math.Abs()函数计算两个数的绝对值之差。
        //例如，如果value1是3.5，value2是4.2，则该方法将返回它们之间的距离为0.7
        public static float Analysis_Distance(float value1, float value2)
        {   //Math.Abs:返回单精度浮点数的绝对值
            return Math.Abs(value1 - value2);
        }

        //埃尔米特插值法
        //这个方法实现了 Hermite 插值，它可以用于在两个点之间进行平滑的插值，产生连续变化的效果。
        //Hermite 插值的结果取决于四个参数：value1 和 value2 分别为起始点和结束点的值；
        //tangent1 和 tangent2 为起始点和结束点的切线（或斜率），它们控制了曲线的形状.
        //amount 参数表示当前要插值的点的位置，可以是 0 到 1 之间的任何值。
        //如果 amount 为 0，则返回 value1；如果 amount 为 1，则返回 value2。
        //如果 tangent1 和 tangent2 相等，则 Hermite 插值就变成了线性插值.
        //该方法的实现使用了多项式函数计算 Hermite 插值，具体公式可以参考 Hermite 插值的相关资料
        public static float Analysis_Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            double num = value1;
            double num2 = value2;
            double num3 = tangent1;
            double num4 = tangent2;
            double num5 = amount;
            double num6 = num5 * num5 * num5;
            double num7 = num5 * num5;
            double num8 = Analysis_WithinEpsilon(amount, 0f) ? (double)value1 : !Analysis_WithinEpsilon(amount, 1f) ? (2.0 * num - 2.0 * num2 + num4 + num3) * num6 + (3.0 * num2 - 3.0 * num - 2.0 * num3 - num4) * num7 + num3 * num5 + num : (double)value2;
            return (float)num8;
        }

        //插值函数，amount为间距系数，value1与value2分别为起点与终点
        public static float Analysis_Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        //返回最大值
        public static float Analysis_Max(float value1, float value2)
        {
            if (!(value1 > value2))
            {
                return value2;
            }

            return value1;
        }

        //返回最小值
        public static float Analysis_Min(float value1, float value2)
        {
            if (!(value1 < value2))
            {
                return value2;
            }

            return value1;
        }

        //SmoothStep 是一个常用的插值函数，用于实现平滑过渡的效果。
        //它的返回值在输入值 value1 和 value2 之间插值，插值位置由 amount 指定。
        //这个函数主要的实现原理就是使用了 Hermite 插值函数，
        //并且对其进行了一定的调整.SmoothStep 函数首先会将 amount 限制在 [0, 1] 的范围内，然后调用 Hermite 函数进行插值计算。
        //这里的 tangen1 和 tangent2 都是 0，因此 Hermite 函数只有前两个参数 value1 和 value2 是有意义的。
        //实际上，SmoothStep 函数和 Hermite 函数最大的区别就在于它将 tangen1 和 tangent2 都设置为了 0
        //SmoothStep 函数可以用于实现一些平滑过渡的效果，例如平滑移动、颜色渐变等。
        //在游戏中，我们常常需要让一些动画效果看起来更加平滑自然，这时候 SmoothStep 就可以派上用场了
        public static float Analysis_SmoothStep(float value1, float value2, float amount)
        {
            float amount2 = Analysis_Clamp(amount, 0f, 1f);//这里使用了 Clamp 来建立amount参数的区间为[0,1]
            return Analysis_Hermite(value1, 0f, value2, 0f, amount2);//接着使用Hermite插值，但你只需要输入两个 value 参数
        }

        //弧度制转角度制
        public static float Analysis_ToDegrees(float radians)
        {
            return (float)((double)radians * (180.0 / Math.PI));//把弧度值乘以180分之PI
        }

        //角度制转弧度制
        public static float Analysis_ToRadians(float degrees)
        {
            return (float)((double)degrees * (Math.PI / 180.0));//如果对弧度与角度有所疑问，请查阅:https://baike.baidu.com/item/%E5%BC%A7%E5%BA%A6%E5%88%B6/3315973
        }

        //这个函数的作用是将角度值限制在 -π 到 π 之间，因为有时候在计算角度值时可能会超出这个范围，但是有些计算需要角度在这个范围内。
        //这个函数的具体实现是：如果角度在 -π 和 π 之间，则返回该角度；否则，将角度对 2π 取模，再根据角度所在的范围做出相应调整。最终将调整后的角度返回
        public static float Analysis_WrapAngle(float angle)
        {
            if (angle > -MathF.PI && angle <= MathF.PI)//设置angle大于-PI同时也要满足小于等于PI，即angle属于区间(-PI,PI]
            {
                return angle;//若angle满足区间，则直接返回angle
            }

            angle %= MathF.PI * 2f;//第二种情况，angle在区间之外，此时需要对其取2PI的模并求余
            if (angle <= -MathF.PI)
            {
                return angle + MathF.PI * 2f;
            }

            if (angle > MathF.PI)
            {
                return angle - MathF.PI * 2f;
            }

            return angle;//进行调整，确保angle在一个周角内，上面的所有变换皆是与2PI进行加减，所以是恒等变换
        }

        //与上面的Clamp一样，这里参数使用int型
        internal static int Analysis_Clamp(int value, int min, int max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        //
        internal static bool Analysis_WithinEpsilon(float floatA, float floatB)
        {
            return Math.Abs(floatA - floatB) < Analysis_MachineEpsilonFloat;
        }

        //这个函数用于计算最接近给定值的2的次方数，并且这个2的次方数可以作为多重采样抗锯齿（MSAA）的样本数量
        //效果是输出给定数最接近的2^n数，且2^n<=给定数
        internal static int Analysis_ClosestMSAAPower(int value)
        {
            if (value == 1)//如果给定值是1，则返回0，这意味着不使用MSAA采样
            {
                return 0;
            }

            int num = value - 1;
            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;
            num++;//对于给定值减1的结果，执行一个或操作，将结果与右移1位后的结果执行或操作，以此类推，执行6次或操作，得到一个最近的2的次方数
            if (num == value)//如果得到的结果等于给定值，则返回该结果
            {
                return num;
            }

            return num >> 1; //否则，将结果右移一位并返回。这是因为MSAA样本数量必须是2的次方数，而该函数返回的结果可能比给定值小一些，因此将其减半以获得最接近给定值的2的次方数
        }

        //这个函数是用来获取机器浮点数的精度（Machine Epsilon）。
        //机器浮点数精度指的是能表示的两个相邻的浮点数之间的差值（即eps），它是一个浮点数的二分之一，即机器能够区分的最小浮点数。
        //因为计算机在计算浮点数时有精度限制，所以能够表示的浮点数是有限的，而机器精度就是表示有限的浮点数所能达到的最高精度。
        //这个函数的实现方式是通过不断除以2来计算能够被表示的最小的浮点数与1之间的差值
        //当加上1后仍然能够被表示时，说明此时的num已经达到了机器浮点数的精度，因此返回此时的num
        private static float Analysis_GetMachineEpsilonFloat()
        {
            float num = 1f;
            float num2;
            do
            {
                num *= 0.5f;
                num2 = 1f + num;
            }
            while (num2 > 1f);//判断什么时候num2=num，此时num为计算机眼中的不可运算浮点数，即num到达最小表达极限(想一想，为什么用do..while而不是用for)
            return num;
        }
    }
}