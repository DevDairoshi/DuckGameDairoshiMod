namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class MSG : Gun
    {
        public MSG(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("msg"));

            center = new Vec2(17.5f, 8f);
            collisionSize = new Vec2(35f, 10f);
            collisionOffset = new Vec2(-17.5f, -7f);
            _barrelOffsetTL = new Vec2(35f, 3f);
            _holdOffset = new Vec2(9f, 2f);

            wideBarrel = true;
            _numBulletsPerFire = 8;
            ammo = 15;
            _fullAuto = true;
            _fireWait = 2f;
            _type = "gun";
            _ammoType = (AmmoType)new ATMSG();
            _kickForce = 3f;
            _fireSound = GetPath("shotgunShot");
            _fireSoundPitch = -0.5f;
            editorTooltip = "Machine Shotgun!";
        }
    }

    public class ATMSG : AmmoType
    {
        public ATMSG()
        {
            this.accuracy = 0.5f;
            this.range = 150f;
            this.penetration = 2f;
            this.rangeVariation = 30f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y);
            shotgunShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)shotgunShell);
        }
    }
}
