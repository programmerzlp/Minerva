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
using System.Linq;

using DItem = Minerva.Structures.Database.Item;
using CEquipment = Minerva.Structures.Client.Equipment;

#endregion

namespace Minerva
{
    public class Equipment
    {
        public byte[] head      { get; set; }
        public byte[] body      { get; set; }
        public byte[] hands     { get; set; }
        public byte[] feet      { get; set; }
        public byte[] righthand { get; set; }
        public byte[] lefthand  { get; set; }
        public byte[] back      { get; set; }
        public byte[] card { get; set; }
        public byte[] pet { get; set; }
        public byte[] neck { get; set; }
        public byte[] finger1 { get; set; }
        public byte[] finger2 { get; set; }
        public byte[] finger3 { get; set; }
        public byte[] finger4 { get; set; }
        public byte[] leftear { get; set; }
        public byte[] rightear { get; set; }
        public byte[] leftwrist { get; set; }
        public byte[] rightwrist { get; set; }

        public byte[] belt { get; set; }
        public byte[] charm { get; set; }
        public byte[] lefteffector { get; set; }
        public byte[] righteffector { get; set; }
        public byte[] cornalina { get; set; }
        public byte[] talisman { get; set; }
        public byte[] weapon { get; set; }
        public byte[] leftarcane { get; set; }
        public byte[] rightarcane { get; set; }
        List<CEquipment> cequipment;

        public List<CEquipment> GetEquipment()
        {
            cequipment = new List<CEquipment>();
            if (EquipmentExist(head)) AddEquipmentArray(head, 0);
            if (EquipmentExist(body)) AddEquipmentArray(body, 1);
            if (EquipmentExist(hands)) AddEquipmentArray(hands, 2);
            if (EquipmentExist(feet)) AddEquipmentArray(feet, 3);
            if (EquipmentExist(righthand)) AddEquipmentArray(righthand, 4);
            if (EquipmentExist(lefthand)) AddEquipmentArray(lefthand, 5);
            if (EquipmentExist(back)) AddEquipmentArray(back, 6);

            if (EquipmentExist(neck)) AddEquipmentArray(neck, 7);
            if (EquipmentExist(finger1)) AddEquipmentArray(finger1, 8);
            if (EquipmentExist(finger2)) AddEquipmentArray(finger2, 9);
            if (EquipmentExist(card)) AddEquipmentArray(card, 10);
            if (EquipmentExist(pet)) AddEquipmentArray(pet, 11);

            if (EquipmentExist(leftear)) AddEquipmentArray(leftear, 13);
            if (EquipmentExist(rightear)) AddEquipmentArray(rightear, 14);
            if (EquipmentExist(leftwrist)) AddEquipmentArray(leftwrist, 15);
            if (EquipmentExist(rightwrist)) AddEquipmentArray(rightwrist, 16);
            if (EquipmentExist(finger3)) AddEquipmentArray(finger3, 17);
            if (EquipmentExist(finger4)) AddEquipmentArray(finger4, 18);
            if (EquipmentExist(belt)) AddEquipmentArray(belt, 19);

            if (EquipmentExist(pet)) AddEquipmentArray(pet, 20);//pet2

            if (EquipmentExist(talisman)) AddEquipmentArray(talisman, 21);
            if (EquipmentExist(lefteffector)) AddEquipmentArray(lefteffector, 22);
            if (EquipmentExist(righteffector)) AddEquipmentArray(righteffector, 23);
            if (EquipmentExist(weapon)) AddEquipmentArray(weapon, 24);//29 dialog

            if (EquipmentExist(charm)) AddEquipmentArray(charm, 30);//31 mostrar
            if (EquipmentExist(cornalina)) AddEquipmentArray(cornalina, 32);

            if (EquipmentExist(leftarcane)) AddEquipmentArray(leftarcane, 33);
            if (EquipmentExist(rightarcane)) AddEquipmentArray(rightarcane, 34);

            return cequipment;
        }

        bool EquipmentExist(byte[] eq)
        {
            if (eq != null && eq.Length > 0 && eq.Any(b => b != 0))
                return true;

            return false;
        }
        
        void AddEquipmentArray(byte[] eq, int slot)
        {
            var de = (DItem)(eq.ToStructure<DItem>());
            var _slot = (EquipmentSlots)slot;
            cequipment.Add(de.ToClient(_slot));
        }
    }
}
