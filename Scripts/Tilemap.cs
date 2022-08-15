using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo.Scripts
{
    class TileMap
    {

        public Tile[,] tiles;

        public int gridSize;

        public int width;
        public int height;

        public TileMap(int width, int height, int grid)
        {

            this.width = width;
            this.height = height;

            gridSize = grid;

            tiles = new Tile[width, height];

        }

        public Vector2 GridToGlobal(Vector2 input)
        {

            return input * gridSize;

        }

        public Vector2 GlobalToGrid(Vector2 input)
        {

            return new Vector2((float)Math.Round(input.X / gridSize), (float)Math.Round(input.Y / gridSize));

        }

        public void AddTile(Vector2 position, bool solid, Tiles tile, Texture2D tileSet)
        {

            if (this.IsInRange(position))
            {

                tiles[(int)position.X, (int)position.Y] = new Tile(solid, tile, tileSet, this, new Vector2((int)position.X, (int)position.Y));

                foreach (Tile t in tiles)
                {

                    if (t != null)
                    {
                        t.UpdateContacts();
                    }

                }

            }

        }

        public void RemoveTile(Vector2 position)
        {

            if (this.IsInRange(position))
            {

                tiles[(int)Math.Floor(position.Y), (int)Math.Floor(position.Y)] = null;

                foreach (Tile t in tiles)
                {

                    t.UpdateContacts();

                }

            }

        }
       public bool IsInRange(Vector2 position)
        {

            return (position.X >= 0 && position.X < width) && (position.Y >= 0 && position.Y < height);

        }

    }

    public enum Tiles
    {

        Ground, 
        Null

    }

    class Tile
    {

        public Tiles tile = new Tiles();
        public bool isSolid;

        public int x;
        public int y;

        private Tile[,] tiles;
        private TileMap tMap;

        public Tile(bool solid, Tiles til, Texture2D tileSet, TileMap tileMap, Vector2 position)
        {

            x = (int)position.X;
            y = (int)position.Y;

            tiles = tileMap.tiles;
            tMap = tileMap;

            isSolid = solid;
            tile = til;
            this.tileSet = tileSet;

        }

        public Texture2D tileSet;

        public bool[] nearbyContacts = new bool[8];

        public void UpdateContacts()
        {

            nearbyContacts[0] = !GetTileInRange(x - 1, y - 1).Equals(Tiles.Null);
            nearbyContacts[1] = !GetTileInRange(x, y - 1).Equals(Tiles.Null);
            nearbyContacts[2] = !GetTileInRange(x + 1, y - 1).Equals(Tiles.Null);
            nearbyContacts[3] = !GetTileInRange(x - 1, y).Equals(Tiles.Null);
            nearbyContacts[4] = !GetTileInRange(x + 1, y).Equals(Tiles.Null);
            nearbyContacts[5] = !GetTileInRange(x - 1, y + 1).Equals(Tiles.Null);
            nearbyContacts[6] = !GetTileInRange(x, y + 1).Equals(Tiles.Null);
            nearbyContacts[7] = !GetTileInRange(x + 1, y + 1).Equals(Tiles.Null);

        }

        public Tiles GetTileInRange(int x, int y)
        {

            if (y > -1 && y < tiles.GetLength(1) && x > -1 && x < tiles.GetLength(0) && tiles[x,y] != null)
            {

                return tiles[x, y].tile;

            } else
            {

                return Tiles.Null;

            }

        }

        public Vector2 UvPos()
        {

            if (
                !nearbyContacts[1] && !nearbyContacts[3] && nearbyContacts[4] && nearbyContacts[6] && nearbyContacts[7]
                )
            {

                return new Vector2(0, 0);

            } else if (
                !nearbyContacts[1] && nearbyContacts[3] && nearbyContacts[4] && nearbyContacts[5] && nearbyContacts[6] && nearbyContacts[7]
                )
            {

                return new Vector2(16, 0);

            }
            else if (
              !nearbyContacts[0] && !nearbyContacts[1] && !nearbyContacts[2] && nearbyContacts[3] && !nearbyContacts[4] && nearbyContacts[5] && nearbyContacts[6] && !nearbyContacts[7]
              )
            {

                return new Vector2(32, 0);

            }
            else
            {
                return new Vector2(0, 48);
            }

        }

    }

}
