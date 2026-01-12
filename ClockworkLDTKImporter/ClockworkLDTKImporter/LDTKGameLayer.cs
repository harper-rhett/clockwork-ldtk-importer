using Clockwork.Tiles;

namespace Clockwork.LDTKImporter;

public class LDTKGameLayer : TiledLayer<LDTKGameArea>
{
	private Dictionary<string, LDTKGameArea> areasByName = new();
	public IReadOnlyDictionary<string, LDTKGameArea> AreasByName => areasByName;

	public LDTKGameLayer(IEnumerable<LDTKGameArea> areas, int tileSize) : base(tileSize)
	{
		foreach (LDTKGameArea area in areas)
		{
			areasByName[area.Name] = area;
			AddArea(area);
		}
	}
}
