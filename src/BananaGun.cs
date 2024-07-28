using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class BananaGun : Gun
    {
        private SpriteMap _barrelSteam;
        public BananaGun(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("banana"));

            center = new Vec2(15f, 9f);
            collisionSize = new Vec2(30f, 12f);
            collisionOffset = new Vec2(-15f, -3f);
            _barrelOffsetTL = new Vec2(30f, 10f);
            _holdOffset = new Vec2(3f, 0f);

            wideBarrel = true;
            _kickForce = 2f;
            ammo = 30;
            _type = "gun";
            _ammoType = (AmmoType) new ATDefault();
            _fireSound = "shotgunFire2";
            _bulletColor = Color.White;
            editorTooltip = "Slip slip slip...";

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

                Banana banana = new Banana(0, 0);
                banana._pin = false;
                banana.position = Offset(this.barrelOffset);
                Level.Add((Thing)banana);
                this.Fondle((Thing)banana);
                banana.clip.Add(this.owner as MaterialThing);
                
                banana.hSpeed = this.barrelVector.x * Rando.Float(2f, 4f) + Rando.Float(-2f, 2f);
                banana.vSpeed = this.barrelVector.y * Rando.Float(2f, 4f) + Rando.Float(-2f, 2f);
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here
            
        }
    }
}
