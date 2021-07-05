namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class Minigun : Gun
    {
        public SpriteMap map;
        public Minigun(float xval, float yval) : base(xval, yval)
        {
            map = new SpriteMap(GetPath("minigun"), 41, 25);
            map.AddAnimation("*spin*", 0.35f, true, 0,1);
            map.SetAnimation("*spin*");

            graphic = map;

            center = new Vec2(20.5f, 12.5f);
            collisionSize = new Vec2(41f, 25f);
            collisionOffset = new Vec2(-20.5f, -12.5f);
            _barrelOffsetTL = new Vec2(41f, 12f);
            _holdOffset = new Vec2(13f, 6f);

            _fullAuto = true;
            _fireWait = 0.5f;
            ammo = 100;
            _ammoType = (AmmoType)new AtMinigun();
            _type = "gun";
            _kickForce = 2f;
            _weight = 10f;
            _fireSound = GetPath("minigunShot");
            editorTooltip = "A little present from uncle Bob";
        }

        public override void Update()
        {
            base.Update();

            if (duck != null)
            {
                SFX.Play(GetPath("minigunNoise"), 0.01f);
            }
        }

        protected override void PlayFireSound()
        {
            SFX.Play(this._fireSound, 2f,(Rando.Float(0.2f) - 0.1f + this._fireSoundPitch) );
        }

        public override void Fire()
        {
            _barrelOffsetTL = new Vec2(41f, Rando.Float(7f, 16f));
            base.Fire();
        }
    }

    public class AtMinigun : AmmoType
    {
        public AtMinigun()
        {
            this.accuracy = 0.95f;
            this.range = 500f;
            this.penetration = 9f;
            this.combustable = true;
            this.bulletThickness = 2f;
        }
        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
