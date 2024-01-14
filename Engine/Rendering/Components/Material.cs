using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class MaterialRef: ComponentData
{
    [JsonPropertyName("material_id")]
    public string Name { get; set; } = "";

    public override Component BuildComponent(Actor actor)
    {
        return new Material(actor, this);
    }
}

public record MaterialData
{
    public List<SlotMaterialData> Slots { get; set; } = new();
}

public class SlotMaterialData
{
    [JsonPropertyName("diffuse")]
    public float[]? DiffuseColor { get; set; }
    [JsonPropertyName("base_texture")]
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

    public Material(Actor actor, MaterialRef materialRef): base(actor)
    {
        _materialManager = actor.Director.MaterialManager;
        _materialManager.GetMaterial(materialRef.Name).Slots.ForEach(slotData =>
        {
            var slotMaterial = new SlotMaterial();
            if (slotData.DiffuseColor is not null)
            {
                var color = slotData.DiffuseColor;
                slotMaterial.DiffuseColor = new Vector3(color[0], color[1], color[2]);
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