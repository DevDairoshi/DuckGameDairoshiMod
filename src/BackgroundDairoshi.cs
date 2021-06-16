namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Environment")]
    public class BackgroundDairoshi : BackgroundTile
    {
        public BackgroundDairoshi(float xpos, float ypos)
            : base(xpos, ypos)
        {
            this.graphic = (Sprite)new SpriteMap(GetPath("bg"), 16, 16);
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._editorName = "Background Tiles";
        }
    }
}
