using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class DroneLauncher : GrenadeLauncher
    {
        private SpriteMap _sprite;

        public DroneLauncher(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATDefault();
            this.wideBarrel = true;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("droneLauncher"), 41, 16);
            this._sprite._frame = 0;
            this.graphic = _sprite;

            this.center = new Vec2(20.5f, 8f);
            this.collisionOffset = new Vec2(-20.5f, -8f);
            this.collisionSize = new Vec2(41f, 16f);
            this._barrelOffsetTL = new Vec2(30f, 4f);
            this._holdOffset = new Vec2(3.5f, -2f);

            this._editorName = "Drone Launcher";
            this._wait = this._fireWait;
            this._fireSound = GetPath("droneLaunch");
            this._kickForce = 1f;
            this.weight = 5f;
            this.editorTooltip = "Pikachu, I choose You!";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Fire()
        {
            if (this.ammo > 0)
            {
                this.ammo--;
                this._wait = this._fireWait;
                this.PlayFireSound();

                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                Drone drone = new Drone(0, 0, this.duck);
                drone.position = Offset(this.barrelOffset);
                Level.Add((Thing)drone);
                this.Fondle((Thing)drone);
                drone.hSpeed = this.barrelVector.x * 5f;
                drone.vSpeed = this.barrelVector.y * 5f;
                this._sprite._frame = 1;
            }
            else
            {
                this.DoAmmoClick();
            }
        }
    }

    [BaggedProperty("canSpawn", false)]
    [EditorGroup("DairoshiMod|Lasers")]
    public class Drone : Gun
    {
        private Thing _ignore;

        public Drone(float xval, float yval, Thing ignore) : base(xval, yval)
        {
            this.ammo = int.MaxValue;
            this._ammoType = (AmmoType)new ATRedLaser();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("drone"));

            this.center = new Vec2(10f, 10f);
            this.collisionOffset = new Vec2(-10f, -10f);
            this.collisionSize = new Vec2(21f, 20f);
            this._barrelOffsetTL = new Vec2(21f, 10f);

            this._editorName = "Drone";
            this._fireWait = 15f;
            this._wait = this._fireWait * 2f;
            this._fireSound = GetPath("redShot");
            this._kickForce = 0f;
            this.weight = 1f;
            this.editorTooltip = "Modern device of destruction";
            this.physicsMaterial = PhysicsMaterial.Metal;

            this._skipPlatforms = true;
            this._skipAutoPlatforms = true;
            this.gravMultiplier = 0f;
            this._ignore = ignore;
        }

        public override void ApplyKick()
        {
            // empty
        }

        public override void Thrown()
        {
            this._wait = this._fireWait - 5f;
            base.Thrown();
        }

        public override void Update()
        {
            List<Thing> things = new List<Thing>();
            foreach (Thing t in Level.CheckCircleAll<Thing>(collisionCenter, 250f))
            {
                if (ReferenceEquals(t, _ignore)) continue;
                if (t is Duck) things.Add(t);
                if (t is Ragdoll && !(t as Ragdoll)._duck.dead) things.Add(t);
            }

            if (things.Count == 0)
            {
                base.Update();
                this.angleDegrees += 0.3f;
                if (this.angleDegrees > 360)
                {
                    this.angleDegrees = 0f;
                }
                if (this._vSpeed > 0.1f) this._vSpeed -= 0.8f;
                if (this._vSpeed < -0.1f) this._vSpeed += 0.06f;
                if (this._vSpeed < 0.1f && this._vSpeed > -0.1f) this._vSpeed = 0f;
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

            Vec2 target = things[index].position;
            if (this.duck == null)
            {
                this.angleDegrees = offDir > 0
                    ? -Maths.PointDirection(this.collisionCenter, new Vec2(target.x, target.y))
                    : -Maths.PointDirection(this.collisionCenter, new Vec2(target.x, target.y)) + 180f;
                this.Fire();
            }

            if (this._vSpeed > 0.1f) this._vSpeed -= 0.08f;
            if (this._vSpeed < -0.1f) this._vSpeed += 0.06f;
            if (this._vSpeed < 0.1f && this._vSpeed > -0.1f) this._vSpeed = 0f;

            base.Update();
        }
    }

    public class ATRedLaser : AmmoType
    {
        public ATRedLaser()
        {
            this.accuracy = 1f;
            this.range = 250f;
            this.penetration = 0.5f;
            this.bulletSpeed = 15f;
            this.bulletThickness = 0.1f;
            this.bulletType = typeof(RedLaser);
            this.flawlessPipeTravel = true;
        }
    }

    public class RedLaser : Bullet
    {
        private Tex2D _beem;
        private float _thickness;

        public RedLaser(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            this._thickness = type.bulletThickness;
            this._beem = Content.Load<Tex2D>(GetPath("redLaser"));
        }

        public override void Draw()
        {
            if (this._tracer || (double)this._bulletDistance <= 0.100000001490116)
                return;
            float length = (this.drawStart - this.drawEnd).length;
            float val = 0.0f;
            float num1 = (float)(1.0 / ((double)length / 8.0));
            float num2 = 0.0f;
            float num3 = 8f;
            while (true)
            {
                bool flag = false;
                if ((double)val + (double)num3 > (double)length)
                {
                    num3 = length - Maths.Clamp(val, 0.0f, 99f);
                    flag = true;
                }
                num2 += num1;
                DuckGame.Graphics.DrawTexturedLine((Tex2D)this._beem, this.drawStart + this.travelDirNormalized * val, this.drawStart + this.travelDirNormalized * (val + num3), Color.White * num2, this._thickness, (Depth)0.6f);
                if (!flag)
                    val += 8f;
                else
                    break;
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            Bullet.isRebound = true;
            RedLaser b = new RedLaser(pos.x, pos.y, this.ammo, dir, (Thing)null, this.rebound, rng, false, false);
            Bullet.isRebound = false;
            b._teleporter = this._teleporter;
            b.firedFrom = this.firedFrom;
            Level.current.AddThing((Thing)b);
            Level.current.AddThing((Thing)new LaserRebound(pos.x, pos.y));
        }
    }
}
