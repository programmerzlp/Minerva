/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://github.com/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;

#endregion

namespace Minerva
{
    /* 
        Temporary disabled, since we need only map attributes(pixels);
        No need to store unnecessary data in ram.
    */

    class TileData
    {
        //public uint tile_count = 0;
        //public uint[] tile_id = new uint[9];
        //public uint[] unk = new uint[152];
    }

    class Tile
    {
        //public int iProcessIdx = 0;
        //public int iTileIdx = 0;
        //public int iPosX = 0;
        //public int iPosY = 0;
        //public uint bIsEdge = 0;
        //public uint unk = 0;

        //public TileData[] data = new TileData[9];

        /*  
            World Map Attributes(Pixels)

            Color (function) in BMP Map   - code in dec /  code in hex
            ----------------------------------------------------------
            WHITE (movable)               -         0   /   0x00000000
            BLACK (non-movable)           -  16777232   /   0x01000010
            DARK GREY (town, non-movable) - 117440560   /   0x07000030
            LIGHT GREY (town, movable)    - 100663328   /   0x06000020
        */

        // Map
        const int M_MOVABLE   = 0x00000000;
        const int M_UNMOVABLE = 0x01000010;

        // Town
        const int T_MOVABLE   = 0x06000020;
        const int T_UNMOVABLE = 0x07000030;

        public uint[] pixels = new uint[256];

        public bool IsMovable(int x, int y)
        {
            var iPosX = x - ((x / 16) * 16);
            var iPosY = y - ((y / 16) * 16);
            var attribute = 16 * iPosY + iPosX;

            if (pixels[attribute] == M_MOVABLE || pixels[attribute] == T_MOVABLE)
                return true;

            return false;
        }
    }
}
