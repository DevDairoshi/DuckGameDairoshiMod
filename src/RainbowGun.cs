namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Funny")]
    public class RainbowGun : Gun
    {
        private SpriteMap map;
        public RainbowGun(float xval, float yval) : base(xval, yval)
        {
            map = new SpriteMap(GetPath("rainbowGun"), 23, 19);
            map.AddAnimation("*shine*", 0.3f, true, 0, 1, 2, 3, 4);
            map.SetAnimation("*shine*");

            this.ammo = 30;
            this._ammoType = (AmmoType)new ATRainbow();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = map;

            this.center = new Vec2(11.5f, 9.5f);
            this.collisionOffset = new Vec2(-11.5f, -2.5f);
            this.collisionSize = new Vec2(23f, 12f);
            this._barrelOffsetTL = new Vec2(23f, 11f);
            this._holdOffset = new Vec2(2f, -1f);

            this._fireWait = 3f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this.loseAccuracy = 6f;
            this._fireSound = GetPath("magic");
            this.editorTooltip = "I love unicorns! <3";
        }

        protected override void PlayFireSound()
        {
            SFX.Play(_fireSound, 1f, Rando.Float(-1f, 0.5f));
        }
    }

    public class ATRainbow : AmmoType
    {
        public ATRainbow()
        {
            this.accuracy = 1f;
            this.range = 400f;
            this.penetration = 0.35f;
            this.bulletSpeed = 10f;
            this.bulletThickness = 0.08f;
            this.bulletType = typeof(RainbowBullet);
            this.flawlessPipeTravel = true;
        }
    }

    public class RainbowBullet : Bullet
    {
        private Tex2D _beem;
        private float _thickness;

        public RainbowBullet(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            this._thickness = type.bulletThickness;
            this._beem = Content.Load<Tex2D>(GetPath("rainbowTex"));
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
            RainbowBullet b = new RainbowBullet(pos.x, pos.y, this.ammo, dir, (Thing)null, this.rebound, rng, false, false);
            Bullet.isRebound = false;
            b._teleporter = this._teleporter;
            b.firedFrom = this.firedFrom;
            Level.current.AddThing((Thing)b);
            Level.current.AddThing((Thing)new LaserRebound(pos.x, pos.y));
        }
    }
}
