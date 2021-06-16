namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Environment")]
    public class Baseball : Holdable
    {
        public Baseball(float xpos, float ypos) : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("baseball"));
            this.center = new Vec2(7f, 7f);
            this.collisionOffset = new Vec2(-7f, -7f);
            this.collisionSize = new Vec2(14f, 14f);
            this.thickness = 1f;
            this.weight = 1f;
            this.flammable = 0.3f;
            this.physicsMaterial = PhysicsMaterial.Rubber;
            this._bouncy = 0.4f;
            this.friction = 0.03f;
            this._holdOffset = new Vec2(3f, 0.0f);
            this.handOffset = new Vec2(0.0f, -0.0f);
            _impactThreshold = 0.05f;
            collideSounds.Add("smallMetalCollide");
            this.editorTooltip = "Perfect for playing the world's greatest sport! Also basketball.";
        }
    }
}