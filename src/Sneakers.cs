using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Equipment")]
    public class Sneakers : Boots
    {
        public Sneakers(float xpos, float ypos)
            : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite(GetPath("sneakers"));
            this._sprite = new SpriteMap(GetPath("sneaker"), 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 13f);
            this._equippedDepth = 3;
            this.editorTooltip = "Run, bitch, ruuuuuuuun!";
            this.charThreshold = 0.8f;
        }

        public override void Equip(Duck d)
        {
            _maxSpeedWithoutBoots = d.runMax;
            d.runMax *= 1.5f;
            base.Equip(d);
        }

        public override void UnEquip()
        {
            if (this._equippedDuck != null)
            {
                this.equippedDuck.runMax = _maxSpeedWithoutBoots;
            }
            base.UnEquip();
        }

        private float _maxSpeedWithoutBoots;
    }
}
