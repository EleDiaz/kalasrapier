using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

[JsonDerivedType(typeof(MaterialData), typeDiscriminator: "Material")]
public class MaterialData: ComponentData
{
    public string Name { get; set; } = "";
    public List<SlotData> Slots { get; set; } = new();

    public override Component BuildComponent(Actor actor)
    {
        return new Material(actor, this);
    }
}

public class SlotData
{
    public Vector3? DiffuseColor { get; set; }
    public string? BaseTexture { get; set; }
}

public class SlotMaterial {
    public Vector3 DiffuseColor = new(1, 1, 1);
    public Texture? BaseTexture;
}

// This isn't a PBR material is just a set of material which can have DiffuseColor and BaseTexture 
public class Material : Component
{
    public List<SlotMaterial> Slots = new();
    private MaterialManager _materialManager { get; }

    public Material(Actor actor, MaterialData materialData): base(actor)
    {
        _materialManager = actor.Director.MaterialManager;
        materialData.Slots.ForEach(slotData =>
        {
            var slotMaterial = new SlotMaterial();
            if (slotData.DiffuseColor is not null)
            {
                slotMaterial.DiffuseColor = slotData.DiffuseColor.Value;
            }

            if (slotData.BaseTexture is not null)
            {
                _materialManager.LoadTexture(slotData.BaseTexture);
                slotMaterial.BaseTexture = _materialManager.GetTexture(slotData.BaseTexture);
            }
            Slots.Add(slotMaterial);
        });
    }
    
    
}