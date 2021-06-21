using System;
using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Funny")]
    public class BigGun : Gun
    {
        public BigGun(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("bigGun"));

            center = new Vec2(21f, 8.5f);
            collisionSize = new Vec2(42f, 17f);
            collisionOffset = new Vec2(-21f, -5.5f);
            _barrelOffsetTL = new Vec2(42f, 9.5f);
            _holdOffset = new Vec2(1f, -2f);

            wideBarrel = true;
            _kickForce = 25f;
            ammo = 10;
            _fireWait = 5f;
            _type = "gun";
            _ammoType = (AmmoType)new ATBig();
            _fireSound = GetPath("big");
            editorTooltip = "What the hell...";

            _target = Vec2.Zero;
        }

        public override void Update()
        {
            if (duck != null)
            {
                List<Thing> things = new List<Thing>();
                foreach (Thing t in Level.CheckCircleAll<Thing>(duck.collisionCenter, 2000f))
                {
                    if (t is Duck && (t as Duck) != duck) things.Add(t);
                    if (t is Ragdoll && !(t as Ragdoll)._duck.dead) things.Add(t);
                }

                if (things.Count == 0)
                {
                    handAngle = 0;
                    base.Update();
                    return;
                }

                List<float> dist = new List<float>();
                foreach (Thing t in things)
                {
                    dist.Add((position - t.position).length);
                }

                float min = float.MaxValue;
                int index = -1;
                for (int i = 0; i < dist.Count; i++)
                {
                    if (dist[i] < min)
                    {
                        min = dist[i];
                        index = i;
                    }
                }

                _target = things[index].collisionCenter;
                var len = (duck.collisionCenter - _target).length;

                if (len < 100f)
                {
                    handAngle = 0;
                    base.Update();
                    return;
                }

                if (duck.offDir > 0)
                {
                    handAngle = Maths.DegToRad(-Maths.PointDirection(Offset(barrelOffset), _target));
                }
                else
                {
                    handAngle = -Maths.DegToRad(Maths.PointDirection(Offset(barrelOffset), _target)) + Maths.PI;
                }
            }

            base.Update();
        }

        public override void Fire()
        {
            base.Fire();
            if (duck != null)
            {
                duck.Swear();
                duck.Disarm(this);
            }
        }

        private Vec2 _target;
    }

    public class ATBig : ATMissile
    {
        public ATBig()
        {
            range = Single.MaxValue;
            bulletSpeed = 12f;
            bulletThickness = 10f;
            this.sprite = new Sprite(Mod.GetPath<DairoshiMod>("bigBullet"));
            this.sprite.CenterOrigin();
        }

        public override void PopShell(float x, float y, int dir)
        {
            BigShell Shell = new BigShell(x, y);
            Shell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)Shell);
        }
    }

    public class BigShell : EjectedShell
    {
        public BigShell(float xpos, float ypos)
            : base(xpos, ypos, Mod.GetPath<DairoshiMod>("bigShell"))
        {
        }
    }
}
