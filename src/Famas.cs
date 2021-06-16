using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class Famas : PewPewLaser
    {
        public Famas(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("famas"));

            center = new Vec2(21f, 8.5f);
            collisionSize = new Vec2(42f, 17f);
            collisionOffset = new Vec2(-21f, -8.5f);
            _barrelOffsetTL = new Vec2(42f, 7f);
            _holdOffset = new Vec2(-3f, 0f);

            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 0.7f;
            ammo = 30;
            _ammoType = (AmmoType)new ATFamas();
            _type = "gun";
            _kickForce = 2f;
            _fireSound = GetPath("famasShot");
            editorTooltip = "A french assault riffle.";
        }

        public override void Update()
        {
            base.Update();
            if (_burstWait > 0.75f)
                _burstWait = 0.75f;
        }
    }

    public class ATFamas : AmmoType
    {
        public ATFamas()
        {
            this.accuracy = 0.95f;
            this.range = 300f;
            this.penetration = 1.5f;
            this.combustable = true;
            this.bulletThickness = 1.5f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
