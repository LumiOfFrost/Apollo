using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (IsInRange(position))
            {

                tiles[(int)position.X, (int)position.Y] = new Tile(solid, tile, tileSet, this, new Vector2((int)position.X, (int)position.Y));

                int x = (int)position.X;
                int y = (int)position.Y;

                tiles[x, y].UpdateContacts();

            }

        }

        public void RemoveTile(Vector2 position)
        {

            if (IsInRange(position))
            {

                tiles[(int)Math.Floor(position.X), (int)Math.Floor(position.Y)] = null;

                int x = (int)position.X;
                int y = (int)position.Y;

                if (GetTileInRange(x, y - 1) != null) GetTileInRange(x, y - 1).UpdateContactExclusive();
                if (GetTileInRange(x - 1, y) != null) GetTileInRange(x - 1, y).UpdateContactExclusive();
                if (GetTileInRange(x + 1, y) != null) GetTileInRange(x + 1, y).UpdateContactExclusive();
                if (GetTileInRange(x, y + 1) != null) GetTileInRange(x, y + 1).UpdateContactExclusive();
                if (GetTileInRange(x - 1, y - 1) != null) GetTileInRange(x - 1, y - 1).UpdateContactExclusive();
                if (GetTileInRange(x + 1, y - 1) != null) GetTileInRange(x + 1, y - 1).UpdateContactExclusive();
                if (GetTileInRange(x - 1, y + 1) != null) GetTileInRange(x - 1, y + 1).UpdateContactExclusive();
                if (GetTileInRange(x + 1, y + 1) != null) GetTileInRange(x + 1, y + 1).UpdateContactExclusive();

            }

        }

        public Tile GetTileInRange(int x, int y)
        {

            if (y > -1 && y < tiles.GetLength(1) && x > -1 && x < tiles.GetLength(0) && tiles[x, y] != null)
            {

                return tiles[x, y];

            }
            else
            {

                return null;

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

            for (int i = 0; i < 8; i++)
            {

                nearbyContacts[i] = false;

            }

            //Cardinals
            if (GetTileInRange(x, y - 1) != null) nearbyContacts[1] = GetTileInRange(x, y - 1).tileSet == this.tileSet;
            if (GetTileInRange(x - 1, y) != null) nearbyContacts[3] = GetTileInRange(x - 1, y).tileSet == this.tileSet;
            if (GetTileInRange(x + 1, y) != null) nearbyContacts[4] = GetTileInRange(x + 1, y).tileSet == this.tileSet;
            if (GetTileInRange(x, y + 1) != null) nearbyContacts[6] = GetTileInRange(x, y + 1).tileSet == this.tileSet;
            //Diagonals (funky!!!)
            if (GetTileInRange(x - 1, y - 1) != null) nearbyContacts[0] = GetTileInRange(x - 1, y - 1).tileSet == this.tileSet && nearbyContacts[1] && nearbyContacts[3];
            if (GetTileInRange(x + 1, y - 1) != null) nearbyContacts[2] = GetTileInRange(x + 1, y - 1).tileSet == this.tileSet && nearbyContacts[1] && nearbyContacts[4];
            if (GetTileInRange(x - 1, y + 1) != null) nearbyContacts[5] = GetTileInRange(x - 1, y + 1).tileSet == this.tileSet && nearbyContacts[3] && nearbyContacts[6];
            if (GetTileInRange(x + 1, y + 1) != null) nearbyContacts[7] = GetTileInRange(x + 1, y + 1).tileSet == this.tileSet && nearbyContacts[4] && nearbyContacts[6];

            if (GetTileInRange(x, y - 1) != null) GetTileInRange(x, y - 1).UpdateContactExclusive();
            if (GetTileInRange(x - 1, y) != null) GetTileInRange(x - 1, y).UpdateContactExclusive();
            if (GetTileInRange(x + 1, y) != null) GetTileInRange(x + 1, y).UpdateContactExclusive();
            if (GetTileInRange(x, y + 1) != null) GetTileInRange(x, y + 1).UpdateContactExclusive();
            if (GetTileInRange(x - 1, y - 1) != null) GetTileInRange(x - 1, y - 1).UpdateContactExclusive();
            if (GetTileInRange(x + 1, y - 1) != null) GetTileInRange(x + 1, y - 1).UpdateContactExclusive();
            if (GetTileInRange(x - 1, y + 1) != null) GetTileInRange(x - 1, y + 1).UpdateContactExclusive();
            if (GetTileInRange(x + 1, y + 1) != null) GetTileInRange(x + 1, y + 1).UpdateContactExclusive();

        }

        public void UpdateContactExclusive()
        {

            for (int i = 0; i < 8; i++)
            {

                nearbyContacts[i] = false;

            }

            //Cardinals
            if (GetTileInRange(x, y - 1) != null) nearbyContacts[1] = GetTileInRange(x, y - 1).tileSet == this.tileSet;
            if (GetTileInRange(x - 1, y) != null) nearbyContacts[3] = GetTileInRange(x - 1, y).tileSet == this.tileSet;
            if (GetTileInRange(x + 1, y) != null) nearbyContacts[4] = GetTileInRange(x + 1, y).tileSet == this.tileSet;
            if (GetTileInRange(x, y + 1) != null) nearbyContacts[6] = GetTileInRange(x, y + 1).tileSet == this.tileSet;
            //Diagonals (funky!!!)
            if (GetTileInRange(x - 1, y - 1) != null) nearbyContacts[0] = GetTileInRange(x - 1, y - 1).tileSet == this.tileSet && nearbyContacts[1] && nearbyContacts[3];
            if (GetTileInRange(x + 1, y - 1) != null) nearbyContacts[2] = GetTileInRange(x + 1, y - 1).tileSet == this.tileSet && nearbyContacts[1] && nearbyContacts[4];
            if (GetTileInRange(x - 1, y + 1) != null) nearbyContacts[5] = GetTileInRange(x - 1, y + 1).tileSet == this.tileSet && nearbyContacts[3] && nearbyContacts[6];
            if (GetTileInRange(x + 1, y + 1) != null) nearbyContacts[7] = GetTileInRange(x + 1, y + 1).tileSet == this.tileSet && nearbyContacts[4] && nearbyContacts[6];

        }

        public Tile GetTileInRange(int x, int y)
        {

            if (y > -1 && y < tiles.GetLength(1) && x > -1 && x < tiles.GetLength(0) && tiles[x,y] != null)
            {

                return tiles[x, y];

            } else
            {

                return null;

            }

        }

        private byte UvValue(bool[] source)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            int[] ints = new int[8];

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << index);

                ints[index] = result;

                index++;
            }

            return result;
        }

        public Vector2 UvPos()
        {

            int[] values = new int[] {2, 8, 10, 11, 16, 18, 22, 24, 26, 27, 30, 31, 64, 66, 72, 74, 75, 80, 82, 86, 88, 90, 91, 94, 95, 104, 106, 107, 120, 122, 123, 126, 127, 208, 210, 214, 216, 218, 219, 222, 223, 248, 250, 251, 254, 255, 0};

            int value = Array.IndexOf(values, UvValue(nearbyContacts)) + 1;

            int x = 16 * (value % 8);
            int y = 16 * (value / 8);

            return new Vector2(x,y);

        }

    }

}
