namespace DuckGame.DairoshiMod
{
    public class ModifiedFluid : Fluid
    {
        public static FluidData Tar = new FluidData(0.0f, Color.Black.ToVector4(), 0.4f, Mod.GetPath<DairoshiMod>("tar"), 0f, 0.98f);
        public static FluidData Toxic = new FluidData(0.0f, new Color(200, 255, 15).ToVector4(), 0f, Mod.GetPath<DairoshiMod>("toxic"), 0.6f, 0.5f);
        public static FluidData Oil = new FluidData(0.0f, new Color(164, 139, 0).ToVector4(), 1f, Mod.GetPath<DairoshiMod>("oil"), 0f, 0.1f);

        public ModifiedFluid(float xpos, float ypos, Vec2 hitAngle, FluidData dat, Fluid stream = null, float thickMult = 1) : base(xpos, ypos, hitAngle, dat, stream, thickMult)
        {
        }
    }

    [EditorGroup("DairoshiMod|Environment")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class TarBarrel : YellowBarrel
    {
        public TarBarrel(float xpos, float ypos)
            : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("tarBarrel"));
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite(GetPath("tarBarrelMelting"));
            this._editorName = "Barrel (Tar)";
            this.editorTooltip = "Contains sticky tar.";
            this.flammable = 0.0f;
            this._fluid = ModifiedFluid.Tar;
            this._toreUp = new SpriteMap(GetPath("tarBarrelToreUp"), 14, 17);
            this._toreUp.frame = 1;
            this._toreUp.center = new Vec2(0.0f, -6f);
            this.flammable = 0.3f;
        }
    }

    [EditorGroup("DairoshiMod|Environment")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class ToxicWasteBarrel : YellowBarrel
    {
        public ToxicWasteBarrel(float xpos, float ypos)
            : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("toxicBarrel"));
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite(GetPath("toxicBarrelMelting"));
            this._editorName = "Barrel (Toxic waste)";
            this.editorTooltip = "Contains toxic waste, highly corrosive acid.";
            this.flammable = 0.0f;
            this._fluid = ModifiedFluid.Toxic;
            this._toreUp = new SpriteMap(GetPath("toxicBarrelToreUp"), 14, 17);
            this._toreUp.frame = 1;
            this._toreUp.center = new Vec2(0.0f, -6f);
        }
    }

    [EditorGroup("DairoshiMod|Environment")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class OilBarrel : YellowBarrel
    {
        public OilBarrel(float xpos, float ypos)
            : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("oilBarrel"));
            this.center = new Vec2(7f, 8f);
            this._melting = new Sprite(GetPath("oilBarrelMelting"));
            this._editorName = "Barrel (Oil)";
            this.editorTooltip = "Contains some kind of oil.";
            this.flammable = 0.0f;
            this._fluid = ModifiedFluid.Oil;
            this._toreUp = new SpriteMap(GetPath("oilBarrelToreUp"), 14, 17);
            this._toreUp.frame = 1;
            this._toreUp.center = new Vec2(0.0f, -6f);
            this.sequence = new SequenceItem((Thing)this);
            this.sequence.type = SequenceItemType.Goody;
            this.flammable = 0.3f;
        }
    }
}