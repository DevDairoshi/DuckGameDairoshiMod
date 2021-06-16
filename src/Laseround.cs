namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class Laseround : Gun
    {
        public Laseround(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("laseround"));

            center = new Vec2(11.5f, 6f);
            collisionSize = new Vec2(23f, 12f);
            collisionOffset = new Vec2(-11.5f, -6f);
            _barrelOffsetTL = new Vec2(20f, 5.5f);
            _holdOffset = new Vec2(3f, 0f);

            ammo = 100;
            _fullAuto = true;
            _fireWait = 0.5f;
            _kickForce = 1f;
            _fireSound = "laserRifle";
            _ammoType = new ATLaseround();
            editorTooltip = "Shots lasers in angle of 180 degree.";
        }

        public override void Fire()
        {
            _ammoType.barrelAngleDegrees = Rando.Float(-90f, 90f);
            base.Fire();
        }
    }

    public class ATLaseround : ATLaser
    {
        public ATLaseround()
        {
            accuracy = 1f;
            range = 100f;
            barrelAngleDegrees = -90;
        }
    }
}