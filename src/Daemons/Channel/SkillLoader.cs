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
using System.Xml.Linq;
using System.IO;

#endregion

namespace Minerva
{
    public class SkillLoader
    {
        public Dictionary<int, SkillData> LoadSkills()
        {
            //var skills = new Dictionary<int, List<int>>();
            Log.Message("Loading SkillsData...", Log.DefaultFG);

            var skillxml = XDocument.Load("data/SkillData.xml");
            var skillnode = skillxml.Root;
            var skilldata = new Dictionary<int, SkillData>();

            foreach (var n in skillnode.Elements("skill_main"))
            {
                var id = int.Parse(n.Attribute("id").Value);
                var type = int.Parse(n.Attribute("type").Value);
                var samp = int.Parse(n.Attribute("samp").Value);
                var atk = int.Parse(n.Attribute("atk").Value);
                var critdmg = int.Parse(n.Attribute("critdmg").Value);

                var skills = new SkillData (id, type, samp, atk, critdmg);

                skilldata.Add(id, skills);
            }

            return skilldata;

        }

        public List<SkillData> LoadSkillData(int skillid)
        {
            var data = XDocument.Load("data/SkillData.xml");
            var node = data.Root;
            var skilldata = new List<SkillData>();

            foreach (var n in node.Elements("skill_main"))
            {
                var id = int.Parse(n.Attribute("id").Value);
                var type = int.Parse(n.Attribute("type").Value);
                var samp = int.Parse(n.Attribute("samp").Value);
                var atk = int.Parse(n.Attribute("atk").Value);
                var critdmg = int.Parse(n.Attribute("critdmg").Value);

                skilldata.Add(new SkillData(id, type, samp, atk, critdmg));
            }

            return skilldata;
        }
    }
}
