﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#if WINDOWS
using System.Xml;
using System.IO.Compression;
using System.Reflection;
#endif

namespace Delta.Tiled
{

    public class TileLayer : Entity, ILayer
    {
        [ContentSerializer(FlattenContent=true, CollectionItemName="Tile")]
        List<Tile> _tiles = new List<Tile>();

        public float Parallax { get; set; }
        public int TileCount { get { return _tiles.Count; } }

#if DEBUG
        public int TilesDrawn { get; set; }
#endif

        public TileLayer()
            : base()
        {
        }

#if WINDOWS
        public TileLayer(string fileName, XmlNode node)
            : base()
        {
            this.ImportLayer(node);

            XmlNode dataNode = node["data"];
            uint[] tileLayerData = new uint[Map.Instance.Width * Map.Instance.Height];
            if (dataNode.Attributes["encoding"] == null)
                throw new NotSupportedException(string.Format("{0} does not support un-encoded Tiled layer data. Map: {1}", Assembly.GetExecutingAssembly().GetName().Name, fileName));
            switch (dataNode.Attributes["encoding"].Value.ToLower())
            {
                case "base64":
                    Stream stream = new MemoryStream(Convert.FromBase64String(dataNode.InnerText), false);
                    if (dataNode.Attributes["compression"] != null)
                    {
                        switch (dataNode.Attributes["compression"].Value.ToLower())
                        {
                            case "gzip":
                                stream = new GZipStream(stream, CompressionMode.Decompress, false);
                                break;
                            default:
                                throw new NotSupportedException(string.Format("{0} does not support the compression '{1}' for Tiled layer data. Map: {2}.", Assembly.GetExecutingAssembly().GetName().Name, dataNode.Attributes["compression"].Value, fileName));
                        }
                        using (stream)
                        {
                            using (BinaryReader reader = new BinaryReader(stream))
                            {
                                for (int i = 0; i < tileLayerData.Length; i++)
                                    tileLayerData[i] = reader.ReadUInt32();
                            }
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} does not support the encoding '{1}' for Tiled layer data. Map: {2}.", Assembly.GetExecutingAssembly().GetName().Name, dataNode.Attributes["encoding"].Value, fileName));
            }
            for (int x = 0; x < Map.Instance.Width; x++)
            {
                for (int y = 0; y < Map.Instance.Height; y++)
                {
                    Tile tile = new Tile(new Vector2(x * Map.Instance.TileWidth, y * Map.Instance.TileHeight), tileLayerData[y * Map.Instance.Width + x]);
                    if (tile._tilesetIndex >= 0)
                        _tiles.Add(tile);
                }
            }
        }
#endif

        protected override void LateInitialize()
        {
            base.LateInitialize();
            for (int i = 0; i < _tiles.Count; i++)
            {
                Tileset tileset = Map.Instance._tilesets[_tiles[i]._tilesetIndex];
                _tiles[i]._sourceRectangle = Map.Instance._spriteSheet.GetFrameSourceRectangle(Path.GetFileName(tileset.ExternalImagePath), _tiles[i]._imageFrameIndex);
            }
        }

        protected internal override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
#if DEBUG
            TilesDrawn = 0;
#endif
            Tile tile;
            Rectangle tileArea = Rectangle.Empty;
            Rectangle viewingArea = World.Instance.Camera.ViewingArea;
            viewingArea.Inflate(Map.Instance.TileWidth, Map.Instance.TileHeight); // pad the viewing area with a border of off-screen tiles. for smooth scrolling, otherwise tiles seem to 'pop' in.
            for (int i = 0; i < _tiles.Count; i++)
            {
                tile = _tiles[i];
                tileArea = new Rectangle((int)tile.Position.X, (int)tile.Position.Y, Map.Instance.TileWidth, Map.Instance.TileHeight);
                if (viewingArea.Contains(tileArea) || viewingArea.Intersects(tileArea))
                {
                    tile.Draw(spriteBatch);
#if DEBUG
                    TilesDrawn++;
#endif
                }
            }
        }

    }
}