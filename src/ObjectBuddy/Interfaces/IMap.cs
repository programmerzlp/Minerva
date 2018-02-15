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
using System.Collections.Generic;

#endregion

namespace Minerva
{
    public interface IMap
    {
        int ID { get; set; }
        void MoveClient(ClientHandler client, int gridRow, int gridColumn);
        void RemoveClient(ClientHandler client);
        List<ClientHandler> GetSurroundingClients(int gridRow, int gridColumn, int radius);
        List<ClientHandler> GetSurroundingClients(ClientHandler client, int radius);
        int[] GetDeathDestination();
        int[] GetWarpDestination(int npc, int order);
        void DropItem(int x, int y, int id, uint entity, uint owner, uint party = 0, int bonus = 0, int amount = 0, uint expiration = 0, int craft = 0, int craftBonus = 0, int upgrades = 0, int upgrade1 = 0, int upgrade2 = 0, int upgrade3 = 0, int upgrade4 = 0);
        byte[] LootItem(ClientHandler client, uint uid);
        List<MobEntity> GetSurroundingMobs(ClientHandler client, int radius);
        List<MobData> GetSurroundingMobsData(ClientHandler client, int radius);
        List<MobData> GetMobsData { get; }
        void GetTile(ClientHandler client, int x, int y);
        void UpdateCells(ClientHandler client);
    }
}