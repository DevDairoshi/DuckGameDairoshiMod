namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class GangstaSMG : Gun
    {
        public GangstaSMG(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("smg"));

            center = new Vec2(9f, 7f);
            collisionSize = new Vec2(18f, 10f);
            collisionOffset = new Vec2(-9f, -6f);
            _barrelOffsetTL = new Vec2(18f, 2.5f);
            _holdOffset = new Vec2(-1f, 3f);

            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 1f;
            ammo = 30;
            _ammoType = (AmmoType)new ATGangsta();
            _type = "gun";
            _kickForce = 1f;
            _fireSound = GetPath("sgmShot");
            editorTooltip = "To protect Your homies.";
        }
    }

    public class ATGangsta : AmmoType
    {
        public ATGangsta()
        {
            this.accuracy = 0.7f;
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
