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
            collisionSize = new Vec2(42f, 14f);
            collisionOffset = new Vec2(-21f, -5.5f);
            _barrelOffsetTL = new Vec2(21f, 9.5f);
            _holdOffset = new Vec2(1f, -2f);

            wideBarrel = true;
            _kickForce = 8f;
            ammo = 5;
            _fireWait = 1f;
            _type = "gun";
            _ammoType = (AmmoType)new ATDefault();
            _fireSound = "shotgunFire2";
            editorTooltip = "What the hell...";
        }

        public override void Update()
        {
            if (duck != null)
            {
                List<Thing> things = new List<Thing>();
                foreach (Thing t in Level.CheckCircleAll<Thing>(collisionCenter, 2000f))
                {
                    if (t is Duck && (t as Duck) != duck) things.Add(t);
                    if (t is Ragdoll && !(t as Ragdoll)._duck.dead) things.Add(t);
                }

                if (things.Count == 0)
                {
                    handAngle = 0;
                    _target = Vec2.Zero;
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

                if (offDir > 0)
                {
                    handAngle = Maths.DegToRad(-Maths.PointDirection(duck.collisionCenter, _target));
                }
                else
                {
                    handAngle = -Maths.DegToRad(Maths.PointDirection(duck.collisionCenter, _target)) + Maths.PI;
                }

            }

            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0 && duck != null)
            {
                --this.ammo;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play(GetPath("big"), pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                BigBullet bullet = new BigBullet(0, 0);
                this.Fondle(bullet);
                Level.Add(bullet);


                bullet.position = Offset(this.barrelOffset);
                bullet.clip.Add(this.owner as MaterialThing);
                
                bullet.hSpeed = this.barrelVector.x * 15f;
                bullet.vSpeed = this.barrelVector.y * 15f;
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here
        }

        private Vec2 _target;
    }

    public class BigBullet : PhysicsObject
    {
        public BigBullet(float x, float y) : base(x,y)
        {
            graphic = new Sprite(GetPath("bigBullet"));

            this.center = new Vec2(10f, 6f);
            this.collisionOffset = new Vec2(-10f, -6f);
            this.collisionSize = new Vec2(20f, 12f);
            this.collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Metal;

            weight = 1f;
            gravMultiplier = 0;
            airFrictionMult = 0;
            _skipPlatforms = true;
            _skipAutoPlatforms = true;
        }

        public override void Update()
        {
            this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with is Duck)
            {
                (with as Duck).Kill(new DTImpale(this));
                DestroyBullet();
            }

            if (with is RagdollPart && !(with as RagdollPart).doll._duck.dead)
            {
                (with as RagdollPart)._doll._duck.Kill(new DTImpale(this));
                DestroyBullet();
            }

            if (with is Block || with is Spikes)
            {
                DestroyBullet();
            }

            base.OnImpact(with, @from);
        }

        public void DestroyBullet()
        {
            ATMissile.DestroyRadius(this.position, 50f, this);
            Level.Add((Thing)new ExplosionPart(x, y));

            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 30; ++index)
            {
                float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                ATShrapnel atMissileShrapnel = new ATShrapnel();
                atMissileShrapnel.range = 40f + Rando.Float(20f);
                Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                Bullet bullet = new Bullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atMissileShrapnel, num);
                bullet.firedFrom = (Thing)this;
                varBullets.Add(bullet);
                Level.Add((Thing)bullet);
            }
            if (Network.isActive && this.isLocal)
            {
                Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }

            SFX.Play("explode");
            Level.Remove(this);
            _destroyed = true;
        }
    }
}
