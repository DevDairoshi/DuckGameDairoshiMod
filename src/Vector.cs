namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class Vector : Gun
    {
        public Vector(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("vector"));

            center = new Vec2(21f, 9f);
            collisionSize = new Vec2(42f, 11f);
            collisionOffset = new Vec2(-21f, -8f);
            _barrelOffsetTL = new Vec2(42f, 6f);
            _holdOffset = new Vec2(-1f, 4f);

            this.loseAccuracy = 0.6f;
            _fullAuto = true;
            this._fireWait = 0.6f;
            ammo = 60;
            _ammoType = (AmmoType)new ATVector();
            _type = "gun";
            _kickForce = 1f;
            _fireSound = GetPath("vectorShot");
            editorTooltip = "Fast shooting low guage riffle.";
        }
    }

    public class ATVector : AmmoType
    {
        public ATVector()
        {
            this.accuracy = 0.80f;
            this.range = 200f;
            this.penetration = 1f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }
}
