namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Equipment")]
    public class InfiniteJetpack : Jetpack
    {
        public InfiniteJetpack(float xpos, float ypos) : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("jetpack"));

            this.center = new Vec2(6f, 7.5f);
            this.collisionOffset = new Vec2(-6f, -7.5f);
            this.collisionSize = new Vec2(12f, 15f);
            this._offset = new Vec2(-3f, 3f);

            this.editorTooltip = "To infinity and beyond!";
        }

        public override void Update()
        {
            base.Update();
            _heat = 0f;
        }
    }
}
