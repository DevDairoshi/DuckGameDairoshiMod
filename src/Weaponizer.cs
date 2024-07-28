using System;
using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class Weaponizer : Gun
    {
        private SpriteMap _barrelSteam;
        public Weaponizer(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("weaponizer"));

            center = new Vec2(22f, 11f);
            collisionOffset = new Vec2(-22f, -11f);
            collisionSize = new Vec2(44f, 22f);
            _barrelOffsetTL = new Vec2(44f, 9f);
            _holdOffset = new Vec2(-1f, -2f);

            wideBarrel = true;
            ammo = 5;
            _weight = 10f;
            _fireWait = 15f;
            _ammoType = (AmmoType)new ATGrenade();
            _type = "gun";
            _kickForce = 6f;
            _fireSound = "deepMachineGun";
            _bulletColor = Color.White;
            editorTooltip = "Manufactures a random weapon.";
            _dontCrush = false;

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
            if (justGuns == null)
            {
                justGuns = ItemBox.GetPhysicsObjects(Editor.Placeables).FindAll((Predicate<System.Type>)(
                    t => t.IsSubclassOf(typeof(Gun)) &&
                         t != typeof(Trumpet) &&
                         t != typeof(Banana) &&
                         t != typeof(BananaCluster) &&
                         t != typeof(Grenade) &&
                         t != typeof(FireExtinguisher) &&
                         t != typeof(FireCrackers) &&
                         t != typeof(RomanCandle) &&
                         t != typeof(Chainsaw) &&
                         t != typeof(EnergyScimitar) &&
                         t != typeof(SledgeHammer) &&
                         t != typeof(Sword) &&
                         t != typeof(Keytar) &&
                         t != typeof(GoodBook) &&
                         t != typeof(RCCar) &&
                         t != typeof(RCController) &&
                         t != typeof(Saxaphone) &&
                         t != typeof(Trombone) &&
                         t != typeof(Weaponizer) &&
                         t != typeof(BookOfTheFireGod) &&
                         t != typeof(DemonicBook) &&
                         t != typeof(BaseballBat) &&
                         t != typeof(Canister)
                ));
            }
            if (this.ammo > 0)
            {
                --this.ammo;
                _wait = _fireWait;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("campingThwoom", pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                SFX.Play("missile", pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                SFX.Play(GetPath("manufacture"));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                var contains = justGuns[Rando.Int(justGuns.Count - 1)];
                var gun = (MaterialThing)Editor.CreateThing(contains);
                gun.position = Offset(this.barrelOffset);

                Level.Add((Thing)gun);
                this.Fondle((Thing)gun);
                gun.clip.Add(this.owner as MaterialThing);

                gun.hSpeed = this.barrelVector.x * 5f;
                gun.vSpeed = this.barrelVector.y * 3f - 1f;

                int num = 0;
                for (int index = 0; index < 10; ++index)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(this.barrelPosition.x - 16f, this.barrelPosition.y);
                    musketSmoke.depth = (Depth)(float)(0.899999976158142 + (double)index * (1.0 / 1000.0));
                    if (num < 6)
                        musketSmoke.move -= this.barrelVector * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        musketSmoke.fly += this.barrelVector * (2f + Rando.Float(7.8f));
                    Level.Add((Thing)musketSmoke);
                    ++num;
                }
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here
        }

        public static List<System.Type> justGuns = null;
    }
}
