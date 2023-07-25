using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AnalysisMod.AdditionalAnalysis
{
    //泰拉瑞亚的开发中会使用到大量的几何学工具，如果您对此并不是特别了解，那么建议在开始一切之前，优先学习一下几何方面的数学知识，包括计算机图像学相关，以及大学阶段的线性代数。
    //而TmodLoader的官方也有相关的教程，阅读一下或许会有所启发：https://github.com/tModLoader/tModLoader/wiki/Geometry
    internal class TmodLoaderMathHelper
    {
        //而在AnalysisMod中，您可以在'AnalysisContent/Projectiles'与'AnalysisContent/Items/Weapons/AnalysisGun'部分中找到不少对几何进行操纵的例子与方法，
        //下面我们集中讨论一些典型的封装方法

        //在AnalysisGun中的一个方法，实际上是ModItem的一个方法，我们在开发中要做的一般是重写这个方法来达到让该物品精灵拥有独特的偏移量的效果
        //它返回的Vector2二维向量会与香草精灵的偏移量做加法，从而达到独特的精灵偏移效果
        //比如Vector2(0f, 0f)，那么我们得到了一个0向量，这对香草偏移量没有改变
        //如果您阅读过上面的几何加成，您会知道，泰拉瑞亚的的几何坐标系是一个原点在地图左上角的笛卡尔坐标系，其中Y轴向下递增，而X轴向右递增，但希望这不会干扰您对此方法中Vector2的判断
        public /*override*/ Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 0f);
        }

        //所以，如果我们添加Vector2(0f, 200f)的偏移，可以视作对Y增了200个像素单位，也就是让枪的精灵下移了200个像素，这个效果图见:C-001
        public /*override*/ Vector2? HoldoutOffset_C001()
        {
            return new Vector2(0f, 200f);
        }

        //如果我们添加Vector2(0f, -200f)的偏移，可以视作对Y减了200个像素单位，也就是让枪的精灵上移了200个像素，这个效果图见:C-002
        public /*override*/ Vector2? HoldoutOffset_C002()
        {
            return new Vector2(0f, -200f);
        }

        //如果我们添加Vector2(200f, -200f)的偏移，可以视作对Y减了200个像素单位，对X加了200个像素单位，也就是让枪的精灵下移了200个像素，再让枪的精灵右移了200个像素，这个效果图见:C-003
        public /*override*/ Vector2? HoldoutOffset_C003()
        {
            return new Vector2(200f, 200f);
        }

        //如果我们添加Vector2(200f, -200f)的偏移，可以视作对Y减了200个像素单位，对X减了200个像素单位，也就是让枪的精灵下移了200个像素，再让枪的精灵左移了200个像素，这个效果图见:C-004
        public /*override*/ Vector2? HoldoutOffset_C004()
        {
            return new Vector2(-200f, 200f);
        }

        //您应该也意识到了，这些本质是向量的加法

        //下面观察这个AnalysisContent\Items\Weapons\AnalysisGun的一个Shoot方法，它的作用是让枪像吸血鬼飞刀那样发射散射的弹幕
        //想象那会涉及到什么？散射分布发射出的弹幕？这些将涉及到对向量的操纵，比如旋转和归一化！

        /*
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(45);

			position += Vector2.Normalize(velocity) * 45f;

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
                          // 返回false以阻止vanilla调用Projectile.NewProjectile。
		}*/

        //我们可以注意到，Shoot方法使用了Vector2.Normalize(velocity)，它将玩家对准的方向向量归一为一个只含方向的单位向量
        //Vector2.Normalize是一个在计算机图形学和向量运算中常用的函数或方法。它用于将一个二维向量转化为单位向量，即将向量的长度（模）归一化为1，同时保持其方向不变。
        //具体而言，给定一个二维向量(x, y)，调用Vector2.Normalize后，将返回一个新的向量(x', y')，其满足以下条件：
        //向量(x', y') 的长度为1，即 ||(x', y')|| = 1。
        //向量(x', y') 的方向与原始向量(x, y) 的方向相同。
        public Vector2 VectorNormalization(Vector2 vector2)
        {
            return Vector2.Normalize(vector2);
            //Vector2.Normalize方法本身并不能防止零向量（zero vector）的出现。
            //当传入的二维向量是零向量时，即 (0, 0)，归一化操作将导致除以零的情况，这是一个无效的操作，通常会导致错误或异常。
        }

        //velocity.RotatedBy(angle) 是一个用于对向量进行旋转操作的函数或方法。
        //velocity.RotatedBy(angle) 对一个向量 velocity 进行按给定角度 angle(弧度) 的旋转操作。该函数返回一个新的旋转后的向量，而不改变原始向量。
        //旋转操作通过将向量绕原点按给定的角度进行顺时针旋转来实现。角度可以以弧度或度数的形式指定，具体取决于所用的编程语言或库。该函数将原始向量绕原点旋转给定的角度，生成一个新的旋转后的向量。
        public Vector2 VectorRotation(Vector2 vector2,float angle)
        {
            return vector2.RotatedBy(angle);
            //使用此方法操纵向量时，需要注意：
            //旋转方向：通常，再TmodLoader开发中，旋转操作是顺时针方向的。velocity.RotatedBy(MathHelper.ToRadians(45))的效果：见图D-001，可以看到让子弹的发射弹道顺时针转动了45度
            //原始向量不变：velocity.RotatedBy(angle) 函数返回一个新的旋转后的向量，而不会改变原始向量。这意味着在使用函数后，需要将返回的旋转向量赋值给相应的变量或使用它进行进一步的计算和操作。
            //向量的参考点：通常情况下，旋转操作是相对于向量的原点进行的。如果需要绕不同的参考点进行旋转，可能需要先将向量平移，使旋转的参考点成为原点，然后再进行旋转操作。
        }

        //阅读 AnalysisContent\Projectiles\Minions\AnalysisSimpleMinion 的Ai代码部分是学习移动代码和碰撞模拟的好办法

        // Math.Atan2函数是一个数学函数，用于计算给定的y坐标和x坐标之间的反正切值。它接受两个参数：y坐标和x坐标，返回的结果是以弧度表示的角度值。
        // 反正切函数（arctan）是一个三角函数，用于计算直角三角形中一个给定角度的正切值。Math.Atan2函数则是反正切函数的扩展，它能够处理任意坐标系中的角度计算。
        // Math.Atan2函数的参数y和x表示一个点的坐标，通常是相对于原点（0, 0）的偏移量。它会根据y和x的值计算出对应点的角度，返回的角度范围在 -π 到 π 之间。        

        // ToRotation() 用于将一个向量或坐标转换为角度值。在Terraria中，角度用弧度制表示，范围从0到2π（360度）。该函数可以应用于多种情况，例如计算两个点之间的角度、计算对象朝向的角度等。
        // 起点轴（原点）通常是指射击物的发射位置或生成位置，可以是玩家手中的武器、敌对NPC的位置等，具体取决于具体情况。
        // 终点轴（终止射线）是指射击物前进的方向。在Terraria中，射击物通常以直线或抛物线形式飞行，终点轴表示射击物的运动方向。
        public float AnalysisToRotation(Vector2 vector1 ,Vector2 vector2)
        {
            return (vector2 - vector1).ToRotation();
        }
        //阅读AnalysisContent\Projectiles\Minions\AnalysisSimpleMinion 的Ai代码部分是学习移动代码和碰撞模拟的好办法
    }
}
