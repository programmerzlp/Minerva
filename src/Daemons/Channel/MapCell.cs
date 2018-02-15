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
using System.Collections.Concurrent;

#endregion

namespace Minerva
{
    class MapCell
    {
        Map map;
        ConcurrentDictionary<int, ClientHandler> clients;
        bool _isactive;
        int timestamp;
        public int List;
        ushort itemIDs;
        ConcurrentDictionary<uint, MobEntity> mobs;
        ConcurrentDictionary<int, MobData> mobsdata;

        public int Row, Column;
        public ConcurrentDictionary<uint, ItemEntity> Items;

        public bool IsActive
            { get { return _isactive; } }

        public ICollection<ClientHandler> Clients
        {
            get { return clients.Values; }
        }

        public ICollection<MobEntity> Mobs
        {
            get { return mobs.Values; }
        }

        public ICollection<MobData> MobsData
        {
            get { return mobsdata.Values; }
        }

        public bool HasClients
            { get { return (clients.Count > 0); } }

        public MapCell(Map owner, int row, int column)
        {
            map = owner;
            clients = new ConcurrentDictionary<int, ClientHandler>();
            Items = new ConcurrentDictionary<uint, ItemEntity>();
            mobs = new ConcurrentDictionary<uint, MobEntity>();
            mobsdata = new ConcurrentDictionary<int, MobData>();
            Row = row;
            Column = column;
        }

        public void AddMob(MobEntity mob)
        {
            mobs.TryAdd(mob.Id, mob);
        }

        public void AddMobData(MobData mobdata)
        {
            mobsdata.TryAdd(mobdata.I, mobdata);
        }

        public void AddClient(ClientHandler client)
        {
            clients.TryAdd(client.AccountID, client);
            client.Metadata["cell"] = this;
        }

        public void RemoveClient(ClientHandler client)
        {
            ClientHandler temp;
            clients.TryRemove(client.AccountID, out temp);
            temp = null;
        }

        public void Activate()
        {
            if (!_isactive)
            {
                // Compare current tickcount with timestamp, then discard items, update mob HP, etc. accordingly.
                _isactive = true;
            }
        }

        public void Deactivate()
        {
            if (_isactive)
            {
                timestamp = Environment.TickCount;
                _isactive = false;
            }
        }

        public void Update()
        {
            foreach (var c in clients.Values)
            {
                map.Update(c);

                var hp = c.CreatePacket("UpdateStats", c, (byte)3, (ushort)0);    // HP update packet (client, subopcode, damaged_health)
                var mp = c.CreatePacket("UpdateStats", c, (byte)4, (ushort)0);    // MP update packet

                c.Send(hp, "UpdateStats_HP", true);
                c.Send(mp, "UpdateStats_MP", true);

                foreach (var i in Items)
                {
                    var expired = i.Value.UpdateOrDie();

                    if (expired)
                    {
                        var disposed = c.CreatePacket("ItemDisposed", i.Key);
                        RemoveItem(i.Key, disposed);
                    }
                }
            }
            /*
            foreach (var c in clients.Values)
            {
                foreach (var m in Mobs)
                {
                    int Tick = Environment.TickCount;

                    if (m.NextTime > Tick)
                    {
                        continue;
                    }

                    if (!m.IsMoving)
                    {
                        if (!FindNewPath(m))
                        {
                            SetPhaseFind(m);
                            continue;
                        }

                        m.IsMoving = true;
                        m.NextTime = Environment.TickCount + 1000;
                        foreach (var cl in clients.Values)
                        {
                            var beginMove = cl.CreatePacket("MobsMoveBgn", m);
                            cl.Send(beginMove, "MobsMoveBgn");
                        }
                    }

                    if (!UpdatePosition(m))
                    {
                        foreach (var cll in clients.Values)
                        {
                            var beginEnd = cll.CreatePacket("MobsMoveEnd", m);
                            cll.Send(beginEnd, "MobsMoveEnd");
                        }
                    }
                }
            }*/
        }
  

        public void UpdateMonsterMovement(ClientHandler c)
        {
            foreach (var m in Mobs)
            {
                int Tick = Environment.TickCount;

                if (m.NextTime > Tick)
                {
                    continue;
                }

                if (!m.IsMoving)
                {
                    if (!FindNewPath(m))
                    {
                        SetPhaseFind(m);
                        continue;
                    }

                    m.IsMoving = true;
                    m.NextTime = Environment.TickCount + 1000;
                    foreach (var cl in clients.Values)
                    {
                        if (m.CurrentHP > 0)
                        {
                            var beginMove = cl.CreatePacket("MobsMoveBgn", m);
                            cl.Send(beginMove, "MobsMoveBgn", true);
                            /*
                            var NewMobList = cl.CreatePacket("NewMobsList", m);
                            cl.Send(NewMobList, "NewMobsList", true);*/
                        }

                    }

                   
                }

                if (!UpdatePosition(m))
                {
                    foreach (var cli in clients.Values)
                    {
                        if (m.CurrentHP > 0)
                        {
                            var beginEnd = cli.CreatePacket("MobsMoveEnd", m);
                            cli.Send(beginEnd, "MobsMoveEnd", true);
                        }
                    }
                }
            }
        }

        bool FindNewPath(MobEntity mob)
        {
            int iLength = 12;
            int iPosX = mob.rnd.Next(iLength) + mob.CurrentPosX - (iLength / 2);
            int iPosY = mob.rnd.Next(iLength) + mob.CurrentPosY - (iLength / 2);

            if (iPosX < 0 || iPosY < 0)
                return false;

            if (iPosX == mob.CurrentPosX && iPosY == mob.CurrentPosY)
                return false;

            if (!map.IsMovable(this, iPosX, iPosY))
                return false;

            if (iPosX / 16 != Row || iPosY / 16 != Column)
                return false;

            mob.EndPosX = (ushort)iPosX;
            mob.EndPosY = (ushort)iPosY;

            return true;
        }

        void SetPhaseFind(MobEntity mob)
        {
            mob.NextTime = Environment.TickCount + 1000;
           
        }

        bool UpdatePosition(MobEntity mob)
        {
            ushort oldX = mob.CurrentPosX;
            ushort oldY = mob.CurrentPosY;

            var Tick = Environment.TickCount;

            if (mob.CurrentPosX > mob.EndPosX )
            {
                mob.CurrentPosX --;
            }
            else if (mob.CurrentPosX < mob.EndPosX)
            {
                mob.CurrentPosX ++;
            }

            if (mob.CurrentPosY > mob.EndPosY)
            {
                mob.CurrentPosY --;
            }
            else if (mob.CurrentPosY < mob.EndPosY)
            {
                mob.CurrentPosY ++;
            }

            mob.NextTime = Environment.TickCount + 1001;

            if (!IsMovable(mob.CurrentPosX, mob.CurrentPosY))
            {
                mob.IsMoving = false;
                mob.NextTime = Environment.TickCount;// + new Random().Next(10) * 100 + 1000;
                mob.CurrentPosX = oldX;
                mob.CurrentPosY = oldY;
                mob.EndPosX = oldX;
                mob.EndPosY = oldY;
                return false;
            }

            if (mob.CurrentPosX == mob.EndPosX && mob.CurrentPosY == mob.EndPosY)
            {
                mob.IsMoving = false;
                mob.NextTime = Environment.TickCount + new Random().Next(10) * 100 + 1000;
                return false;
            }

            if (mob.Spawn <= Environment.TickCount && mob.CurrentHP == 0)
            {
                mob.CurrentHP = mob.MaxHP;

                return false;
            }

            return true;


            
        }

        public uint AddItem(ItemEntity item, int map)
        {
            uint uid = unchecked(itemIDs++);
            uid += (uint)(Row * 16 + Column) << 24;
            uid += (uint)map << 32;
            Items.TryAdd(uid, item);

            return uid;
        }

        public void RemoveItem(uint uid, PacketBuilder packet)
        {
            ItemEntity item = null;
            Items.TryRemove(uid, out item);
            item = null;

            var surrounding = map.GetSurroundingClients(this, 3);

            foreach (var s in surrounding)
                s.Send(packet, "ItemDisposed");
        }

        public bool IsMovable(int x, int y)
        {
            return map.IsMovable(this, x, y);
        }
    }
}