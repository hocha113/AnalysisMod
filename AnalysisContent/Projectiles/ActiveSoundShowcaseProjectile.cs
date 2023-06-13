using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    /// <summary>
    /// Showcases ActiveSounds. The various styles when studied in order serve to teach important concepts, please experiment with them in-game.
    /// Do note that the earlier Analysiss aren't useful to copy, if you are just looking for an Analysis to copy, consult SoundUpdateCallbackApproach and LoopedSound, as they are the most suitable Analysiss.
    /// This Analysis serves as a companion to the Active Sounds section of the Sounds wiki page, please study them both together: https://github.com/tModLoader/tModLoader/wiki/Basic-Sounds#active-sounds
    /// <br/>
    /// 展示了ActiveSounds。按顺序学习各种样式可以教授重要的概念，请在游戏中进行实验。
    /// 请注意，早期的分析案例并不适合复制。如果您只是想要复制一个分析案例，请参考SoundUpdateCallbackApproach和LoopedSound，因为它们是最适合的分析案例。
    /// 这个分析作为声音wiki页面上活动声音部分的附加演示，请一起学习：https://github.com/tModLoader/tModLoader/wiki/Basic-Sounds#active-sounds
    /// </summary>
    public class ActiveSoundShowcaseProjectile : ModProjectile
    {
        internal enum ActiveSoundShowcaseStyle
        {
            // This Analysis plays a long sound (12 seconds) and never attempts to change it. Notice how the sound plays without any location, the player and projectile can move left or right and the sound panning and volume do not change. Also note that the sound keeps playing after the projectile dies.
            // 这个Analysis播放一个长时间（12秒）的声音，并且从不尝试改变它。请注意，声音在没有任何位置时播放，玩家和抛射物可以向左或向右移动而声道和音量不会改变。还请注意，在抛射物死亡后，声音仍然继续播放。
            FireAndForget,
            // This Analysis improves on FireAndForget. The Projectile position is passed into PlaySound. The sound still does not update location, but the player can move around the initial spawn location and the sound pans and volume adjusts accordingly.
            // 这个Analysis对FireAndForget进行了改进。Projectile位置被传入PlaySound中。该声音仍然不更新位置，但玩家可以围绕初始生成位置移动，并且声道和音量会相应地调整。
            FireAndForgetPlusInitialPosition,
            // Further improving on FireAndForgetPlusInitialPosition, this Analysis updates the sound location in AI and stops the sound when the projectile is killed in Kill.
            // 在FireAndForgetPlusInitialPosition基础上进一步改进，这个Analysis在AI中更新了声源位置，并在Kill中停止了该项目ile。
            SyncSoundToProjectilePosition,
            // Further improving on SyncSoundToProjectilePosition, this Analysis uses the SoundUpdateCallback parameter to keep all sound logic organized in a single place instead of spread between different methods.
            // 在SyncSoundToProjectilePosition基础上进一步改进，这个Analysis使用SoundUpdateCallback参数，将所有声音逻辑组织在一个单独的位置，而不是分散在不同的方法之间。
            SoundUpdateCallbackApproach,
            // LoopedSound shows using SoundUpdateCallback once again to adjust sound position. The SoundStyle used is looped, so SoundUpdateCallback is necessary in case Projectile.Kill doesn't get called for some exceptional reason.
            // LoopedSound再次显示了使用SoundUpdateCallback来调整声源位置。所使用的SoundStyle为looped，因此如果由于某种异常原因Projectile.Kill没有被调用，则需要使用SoundUpdateCallback。
            LoopedSound,
            // LoopedSoundAdvanced adjusts pitch and volume dynamically in the SoundUpdateCallback, in addition to the usual sound position.
            // LoopedSoundAdvanced动态地调整音高和音量，在通常的声源位置之外。
            LoopedSoundAdvanced,
        }

        private ActiveSoundShowcaseStyle Style
        {
            get => (ActiveSoundShowcaseStyle)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        SlotId soundSlot;
        bool played = false;

        SoundStyle soundStyleTwister = new SoundStyle("Terraria/Sounds/Custom/dd2_book_staff_twister_loop");

        SoundStyle soundStyleIgniteLoop = new SoundStyle("Terraria/Sounds/Custom/dd2_kobold_ignite_loop")
        {
            IsLooped = true,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            // Note that MaxInstances defaults to 1.
            // 请注意，默认情况下MaxInstances为1。
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.penetrate = 4; // Can bounce 3 times and dies on 4th tile collide
                                      // 可以反弹3次，并在第4个方块碰撞后死亡

            Projectile.timeLeft = 300; // Despawns after 5 seconds
                                       // 5秒后消失
        }

        public override void OnSpawn(IEntitySource source)
        {
            Main.NewText($"{Style}");
        }

        public override void AI()
        {
            Projectile.frame = (int)Style;

            // Sounds are paused when the game loses focus (Player switches to another program). In some situations the modder might want to restart a sound when the game is focused again, in other situations that might not be desired. Some of these Analysiss use a bool, "played", to track if the sound has been played since the projectile spawned, while others do not and will attempt to restart the sound if it is not currently playing.
            // 当游戏失去焦点（玩家切换到另一个程序）时，声音会暂停。在某些情况下，modder可能希望当游戏重新获得焦点时重新启动声音，在其他情况下可能不希望这样做。其中一些Analysis使用布尔值“played”来跟踪自抛射物生成以来是否已播放该声音，而其他Analysis则没有，并且将尝试重新启动该声音（如果当前未播放）。

            // Also note that in this Analysis the SoundStyle all have "MaxInstances = 1" and "SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest" by default, so if 2 projectiles attempt to play the same sound, they'll constantly interrupt each other every AI update, making a horrible sound.
            // In a real mod, the modder should design the SoundStyle properties and PlaySound logic to meet their needs. For Analysis, the modder might decide that 3 overlapping sounds is too chaotic and adjust MaxInstances accordingly. The modder might also decide that the sound should not restart when the game is re-focused and use logic to only attempt to play the sound once.

            // 还请注意，在这个Analysis中，“MaxInstances = 1”和“SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest”的所有SoundStyle都是默认设置的，因此如果2个抛射物尝试播放相同的声音，则它们将每次AI更新都相互打断并发出可怕的噪音。
            // 在真正的mod中，modder应该设计SoundStyle属性和PlaySound逻辑以满足他们的需求。对于Analysis，modder可能会决定3个重叠声音太混乱了，并相应地调整MaxInstances。Modder还可以决定在游戏重新获得焦点时不重新启动声音，并使用逻辑仅尝试播放一次声音。
            switch (Style)
            {
                case ActiveSoundShowcaseStyle.FireAndForget:
                    if (!played)
                    {
                        played = true;
                        SoundEngine.PlaySound(soundStyleTwister);
                    }
                    break;
                case ActiveSoundShowcaseStyle.FireAndForgetPlusInitialPosition:
                    if (!played)
                    {
                        played = true;
                        SoundEngine.PlaySound(soundStyleTwister, Projectile.position);
                    }
                    break;
                case ActiveSoundShowcaseStyle.SyncSoundToProjectilePosition:
                    if (!SoundEngine.TryGetActiveSound(soundSlot, out var activeSoundTwister))
                    {
                        soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position);
                    }
                    else
                    {
                        // If the sound is playing, update the sound's position to match the current position of the projectile.
                        // 如果声音正在播放，则更新声音的位置以匹配弹丸的当前位置。
                        activeSoundTwister.Position = Projectile.position;
                    }
                    break;
                case ActiveSoundShowcaseStyle.SoundUpdateCallbackApproach:
                    if (!SoundEngine.TryGetActiveSound(soundSlot, out var _))
                    {
                        var tracker = new ProjectileAudioTracker(Projectile);
                        soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position, soundInstance => BasicSoundUpdateCallback(tracker, soundInstance));

                        // If only the sound stopping when the projectile is killed is required, this simpler code can be used:
                        // 如果仅需要在杀死弹丸时停止声音，则可以使用这个更简单的代码：

                        //soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position, soundInstance => tracker.IsActiveAndInGame());

                        // Do NOT make this mistake, the ProjectileAudioTracker object must be initialized outside the callback:
                        // 不要犯这个错误，ProjectileAudioTracker对象必须在回调外部初始化：

                        // soundSlot = SoundEngine.PlaySound(soundStyleTwister, Projectile.position, soundInstance => new ProjectileAudioTracker(Projectile).IsActiveAndInGame()); // WRONG
                    }
                    break;
                case ActiveSoundShowcaseStyle.LoopedSound:
                    if (!SoundEngine.TryGetActiveSound(soundSlot, out var _))
                    {
                        var tracker = new ProjectileAudioTracker(Projectile);
                        soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, soundInstance =>
                        {
                            // The SoundUpdateCallback can be inlined if desired, such as in this Analysis. Otherwise, LoopedSoundAdvanced shows the other approach
                            // 如果需要，SoundUpdateCallback可以内联，例如在此分析中。否则，LoopedSoundAdvanced显示了另一种方法
                            soundInstance.Position = Projectile.position;
                            return tracker.IsActiveAndInGame();
                        });
                    }

                    // SlotId can be stored as a float, such as in Projectile.localAI entries. This can be an alternative to making a SlotId field in the class.
                    // Don't use ai slots for SlotId, since those will sync and sounds and sound slots are completelly local and are not synced

                    // SlotId可以存储为浮点数，例如在Projectile.localAI条目中。这是一个替代方案，而不是在类中创建SlotId字段。
                    // 不要使用ai槽来存储SlotId, 因为那些将同步和声音和声音插槽完全本地并且不会同步

                    // SlotId soundSlot = SlotId.FromFloat(Projectile.localAI[0]);
                    // Projectile.localAI[0] = soundSlot.ToFloat();

                    // As an alternate approach to TryGetActiveSound, we could use FindActiveSound. The difference is that FindActiveSound will find any ActiveSound matching the given SoundStyle,
                    // so if 2 projectile instances spawn the same SoundStyle, the ActiveSound retrieved isn't necessarily the sound spawned by this instance. This can be useful,
                    // but in this situation we want the ActiveSound spawned by this projectile.

                    // 作为尝试获取活动声音的另一种方法，我们可以使用FindActiveSound。区别在于FindActiveSound将查找与给定SoundStyle匹配的任何ActiveSound，
                    // 因此如果2个projectile实例生成相同的SoundStyle，则检索到的ActiveSound不一定是由该实例生成的声音。 这可能很有用，
                    //但在这种情况下我们想要由此projectile生成的ActiveSound。
                    /* 
					var activeSoundB = SoundEngine.FindActiveSound(soundStyleIgniteLoop);
					if (activeSoundB == null) {
						SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, updateIgniteLoop);
					}
					*/
                    break;
                case ActiveSoundShowcaseStyle.LoopedSoundAdvanced:
                    if (!SoundEngine.TryGetActiveSound(soundSlot, out var _))
                    {
                        var tracker = new ProjectileAudioTracker(Projectile);
                        soundSlot = SoundEngine.PlaySound(soundStyleIgniteLoop, Projectile.position, soundInstance => AdvancedSoundUpdateCallback(tracker, soundInstance));
                    }
                    break;
            }
        }

        private bool BasicSoundUpdateCallback(ProjectileAudioTracker tracker, ActiveSound soundInstance)
        {
            // Update sound location according to projectile position
            // 根据弹丸位置更新声源位置
            soundInstance.Position = Projectile.position;
            // ProjectileAudioTracker is necessary to avoid rare situations where sounds can loop indefinitely. IsActiveAndInGame returns a value indicating if the sound should still be active.
            // ProjectileAudioTracker对于避免罕见情况下出现无限循环非常必要。IsActiveAndInGame返回一个值，
            return tracker.IsActiveAndInGame();
        }

        private bool AdvancedSoundUpdateCallback(ProjectileAudioTracker tracker, ActiveSound soundInstance)
        {
            soundInstance.Position = Projectile.position;

            // Dynamic pitch Analysis: Pitch rises each time the projectile bounces
            // 动态音高分析：每次弹丸反弹时，音高都会上升
            soundInstance.Pitch = (Projectile.maxPenetrate - Projectile.penetrate) * 0.15f;

            // Muffle the sound if the projectile is wet
            // 如果弹丸潮湿，则减小声音
            if (Projectile.wet)
            {
                soundInstance.Pitch -= 0.4f;
                soundInstance.Volume = MathHelper.Clamp(soundInstance.Style.Volume - 0.4f, 0f, 1f);
            }

            return tracker.IsActiveAndInGame();
        }

        public override void Kill(int timeLeft)
        {
            // For long sounds, the sound can be stopped when the projectile is killed.
            // This approach is not foolproof, so it should NOT be used, especially for looped sounds.
            // See SoundUpdateCallbackApproach for the better approach. This Analysis, however,
            // does show how an ActiveSound can be modified from another hook other than where the sound was played.

            // 对于长时间的声音，在清除弹丸时可以停止播放。
            // 这种方法不是万无一失的，因此不应使用它，特别是对于循环声音。
            // 请参见SoundUpdateCallbackApproach以获取更好的方法。但是，
            // 本分析显示了如何从与播放声音不同的另一个钩子修改ActiveSound。
            if (Style == ActiveSoundShowcaseStyle.SyncSoundToProjectilePosition)
            {
                if (SoundEngine.TryGetActiveSound(soundSlot, out var activeSound))
                {
                    activeSound.Stop();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If collide with tile, reduce the penetrate.
            // So the projectile can reflect at most 3 times

            // 如果与瓷砖碰撞，请减少穿透力。
            // 因此，弹丸最多可以反射3次
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                // If the projectile hits the left or right side of the tile, reverse the X velocity
                // 如果弹丸击中瓷砖的左侧或右侧，则将X速度反转
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                // 如果弹丸击中瓷砖顶部或底部，则将Y速度反转
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }

            return false;
        }
    }
}
