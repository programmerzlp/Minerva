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
    public class Stats
    {
        public ushort curhp     { get; set; }
        public ushort maxhp     { get; set; }
        public ushort curmp     { get; set; }
        public ushort maxmp     { get; set; }
        public ushort cursp     { get; set; }
        public ushort maxsp     { get; set; }

        public ulong exp        { get; set; }


        public uint str_stat    { get; set; }
        public uint int_stat    { get; set; }
        public uint dex_stat    { get; set; }
        public uint pnt_stat    { get; set; }

        public uint honour       { get; set; }
        public uint rank        { get; set; }

        public byte swordrank   { get; set; }
        public ushort swordxp   { get; set; }
        public ushort swordpts  { get; set; }

        public byte magicrank   { get; set; }
        public ushort magicxp   { get; set; }
        public ushort magicpts  { get; set; }
        public ulong alz        { get; set; }
        public ulong wexp       { get; set; }
        public ulong honor      { get; set; }

        public ushort patk      { get; set; }
        public ushort matk      { get; set; }
        public ushort def       { get; set; }
    }
}
