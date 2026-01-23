using Clockwork.Graphics;
using Clockwork.Tiles;

namespace Clockwork.LDTKImporter;

public class LDTKImporter
{
	private int tileSize;
	private Dictionary<string, ITexture> tilesetsByPath = new();
	private LDTKUtility ldtkUtility;
	public Action<LDTKGameArea> GameAreaImported;

	public LDTKImporter(string localPath, int tileSize)
	{
		this.tileSize = tileSize;
		ldtkUtility = new(localPath);
		ImportTilesets();
	}

	public LDTKGameLayer ImportWorld(string tileLayerName, string entityLayerName)
	{
		LDTKGameArea[] areas = ImportGameAreas(tileLayerName, entityLayerName);
		LDTKGameLayer world = new(areas, tileSize);
		return world;
	}

	private void ImportTilesets()
	{
		foreach (string tilesetPath in ldtkUtility.TilesetPaths) tilesetsByPath[tilesetPath] = Texture.Load(tilesetPath);
	}

	private Tile[] ImportTiles(LDTKLayer ldtkLayer)
	{
		List<Tile> tiles = new();

		foreach (LDTKTile ldtkTile in ldtkLayer.Tiles)
		{
			ITexture tilesetTexture = tilesetsByPath[ldtkLayer.TilesetPath];
			Tile tile = new(ldtkTile.LocalPosition, tilesetTexture, ldtkTile.TilesetX, ldtkTile.TilesetY, tileSize, ldtkTile.XFlipped, ldtkTile.YFlipped);
			tiles.Add(tile);
		}

		return tiles.ToArray();
	}

	private LDTKGameArea[] ImportGameAreas(string tileLayerName, string entityLayerName)
	{
		List<LDTKGameArea> gameAreas = new();
		foreach (LDTKLevel level in ldtkUtility.Levels)
		{
			// Get tiles layer
			LDTKLayer tileLayerData = level.LayersByName[tileLayerName];
			LDTKLayer entityLayerData = level.LayersByName[entityLayerName];

			// Create area
			LDTKGameArea area = new(level.Name, level.Fields, entityLayerData.Entities.ToArray(), level.Position, tileLayerData.WidthInTiles, tileLayerData.HeightInTiles, tileSize);
			area.Tiles = ImportTiles(tileLayerData);
			area.TilesByID = tileLayerData.TileIDs;
			GameAreaImported.Invoke(area);

			// Register area
			gameAreas.Add(area);
		}
		return gameAreas.ToArray();
	}

	public TiledLayer ImportDecorationLayer(string layerName, out Dictionary<string, TiledArea> areasByName)
	{
		TiledLayer layer = new(tileSize);
		areasByName = new();
		foreach (LDTKLevel level in ldtkUtility.Levels)
		{
			// Get tiles layer
			LDTKLayer tileLayerData = level.LayersByName[layerName];

			// Create area
			TiledArea area = new(level.Position, tileLayerData.WidthInTiles, tileLayerData.HeightInTiles, tileSize);
			area.Tiles = ImportTiles(tileLayerData);
			areasByName[level.Name] = area;
			layer.AddArea(area);
		}
		return layer;
	}
}
