using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class BookOfTheFireGod : Gun
    {
        public SpriteMap _sprite;

        public BookOfTheFireGod(float xpos, float ypos) : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(GetPath("fireGod"), 19, 16);
            this._sprite.frame = 0;
            this.graphic = this._sprite;

            center = new Vec2(9.5f, 8f);
            collisionSize = new Vec2(19f, 16f);

            collisionOffset = new Vec2(-9.5f, -8f);
            this._holdOffset = new Vec2(3f, 4f);
            this.handOffset = new Vec2(1f, 1f);

            _hasTrigger = false;
            ammo = 1;
            _ammoType = (AmmoType)new ATLaser();
            _type = "gun";
            _kickForce = 0f;
            _fireSound = "deepMachineGun";
            editorTooltip = "A great sacriface to the Fire God!";
        }

        public override void Update()
        {
            base.Update();
            if (this.owner != null)
            {
                _sprite._frame = 1;
            }
            else
            {
                _sprite._frame = 0;
            }
        }

        public override void OnPressAction()
        {
            if (ammo > 0 && duck != null)
            {
                --this.ammo;
                SFX.Play(GetPath("fireSpell"), 1.5f);
                if (this.receivingPress)
                    return;

                List<Thing> things = new List<Thing>();
                foreach (Thing t in Level.CheckCircleAll<Thing>(collisionCenter, 2000f))
                {
                    if (t is Duck && t != duck)
                    {
                        (t as Duck).onFire = true;
                    }

                    if (t is RagdollPart)
                    {
                        (t as RagdollPart).onFire = true;
                    }
                }
                
                this.duck.Scream();
                this.duck.Disarm(this);
            }
        }
    }
}
