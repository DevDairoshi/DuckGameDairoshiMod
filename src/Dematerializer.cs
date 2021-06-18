namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class Dematerializer : Gun
    {
        public SpriteMap map;
        public Dematerializer(float xval, float yval) : base(xval, yval)
        {
            map = new SpriteMap(GetPath("dematerializer"), 31, 12);
            map.AddAnimation("*zap*", 0.2f, true, 1, 2, 3, 4);
            map.SetAnimation("*zap*");
            graphic = map;

            center = new Vec2(15.5f, 6f);
            collisionSize = new Vec2(31f, 12f);
            collisionOffset = new Vec2(-15.5f, -6f);
            _barrelOffsetTL = new Vec2(24f, 8f);
            _holdOffset = new Vec2(7f, -1f);
            
            ammo = 100;
            _fullAuto = true;
            _fireWait = 0.5f;
            _kickForce = 0f;
            _fireSound = GetPath("energy");
            _ammoType = new ATDematerializer();
            editorTooltip = "Deadly electric device.";
        }

        protected override void PlayFireSound()
        {
            SFX.Play(this._fireSound, 0.5f, Rando.Float(-1f, 1f));
        }

        public override void Fire()
        {
            this._ammoType.barrelAngleDegrees = Rando.Float(-35f, 35f);
            this._ammoType.bulletThickness = Rando.Float(0.5f, 1.5f);
            base.Fire();
        }
    }

    public class ATDematerializer : AmmoType
    {
        public ATDematerializer()
        {
            this.accuracy = 1f;
            this.range = 120f;
            this.rangeVariation = 40f;
            this.penetration = 10f;
            this.bulletSpeed = 30f;
            this.speedVariation = 20f;
            this.bulletThickness = 1f;
            this.affectedByGravity = false;
            this.bulletType = typeof(EnergyBullet);
            this.flawlessPipeTravel = true;
        }
    }

    public class EnergyBullet : Bullet
    {
        private Tex2D _beem;
        private float _thickness;

        public EnergyBullet(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            this._thickness = type.bulletThickness;
            this._beem = Content.Load<Tex2D>(GetPath("energyBeam"));
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
            EnergyBullet bluelaserBullet = new EnergyBullet(pos.x, pos.y, this.ammo, dir, (Thing)null, this.rebound, rng, false, false);
            Bullet.isRebound = false;
            bluelaserBullet._teleporter = this._teleporter;
            bluelaserBullet.firedFrom = this.firedFrom;
            Level.current.AddThing((Thing)bluelaserBullet);
            Level.current.AddThing((Thing)new LaserRebound(pos.x, pos.y));
        }
    }
}
