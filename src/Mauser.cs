namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class Mauser : Gun
    {
        public Mauser(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 10;
            this._ammoType = (AmmoType)new AT9mm();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("mauser"));

            this.center = new Vec2(11.5f, 7f);
            this.collisionOffset = new Vec2(-11.5f, -7f);
            this.collisionSize = new Vec2(23f, 14f);
            this._barrelOffsetTL = new Vec2(24f, 1f);
            this._holdOffset = new Vec2(3f, 1f);

            this._fireWait = 2f;
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Kick;
            this.loseAccuracy = 0.4f;
            this._fireSound = GetPath("mauserShot");
            this.editorTooltip = "For some kind of nazi ducks.";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }
    }
}
