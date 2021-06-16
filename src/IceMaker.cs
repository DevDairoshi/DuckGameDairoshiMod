using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class IceMaker : Gun
    {
        private SpriteMap _barrelSteam;
        public IceMaker(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("icemaker"));

            center = new Vec2(23.5f, 16f);
            collisionSize = new Vec2(47f, 28f);
            collisionOffset = new Vec2(-23.5f, -16f);
            _barrelOffsetTL = new Vec2(38f, 2f);
            this._holdOffset = new Vec2(-1f, -11f);

            wideBarrel = true;
            _fireWait = 2f;
            ammo = 10;
            _weight = 8f;
            _type = "gun";
            _ammoType = (AmmoType)new AT9mm();
            _kickForce = 5f;
            _fireSound = "shotgunFire2";
            _bulletColor = Color.White;
            editorTooltip = "Chill bro";

            this._barrelSteam = new SpriteMap("steamPuff", 16, 16);
            this._barrelSteam.center = new Vec2(0.0f, 14f);
            this._barrelSteam.AddAnimation("puff", 0.4f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            this._barrelSteam.SetAnimation("puff");
            this._barrelSteam.speed = 0.0f;
        }

        public override void Update()
        {
            if ((double)this._barrelSteam.speed > 0.0 && this._barrelSteam.finished)
                this._barrelSteam.speed = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if ((double)this._barrelSteam.speed > 0.0)
            {
                this._barrelSteam.alpha = 0.6f;
                this.Draw((Sprite)this._barrelSteam, barrelOffset);
            }
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("campingThwoom", pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                IceBlock block = new IceBlock(0, 0);
                block.position = Offset(this.barrelOffset);
                Level.Add((Thing)block);
                this.Fondle((Thing)block);
                block.clip.Add(this.owner as MaterialThing);

                block.hSpeed = this.barrelVector.x * Rando.Float(4f) + this.barrelVector.x * 4f;
                block.vSpeed = this.barrelVector.y * 3f - 5f;
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // replaced with custom fire
        }
    }
}
