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
    public partial class DatabaseProtocol : IDatabaseProtocol
    {
        public bool GetSubPass(int id)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT passwd, answer, question FROM subpass WHERE account = '{0}' LIMIT 1", id);
            reader = cmd.ExecuteReader();
            cmd.Dispose();

            return reader.HasRows;
        }

        public void SetSubPass(int id, string subpw, int question, string answer)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            if (subpw.Equals(string.Empty))
            {
                cmd.CommandText = string.Format("UPDATE subpass SET answer = '{0}', question = '{1}' WHERE account = '{2}'", answer, (byte)question, id);
            }
            else
            {
                if (question != 255)
                    cmd.CommandText = string.Format("INSERT INTO subpass (account, passwd, answer, question) VALUES ('{0}', '{1}', '{2}', '{3}')", id, subpw, answer, (byte)question);
                else
                    cmd.CommandText = string.Format("UPDATE subpass SET passwd = '{0}' WHERE account = '{1}'", subpw, id);
            }
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public int GetSubPassQuestion(int id)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT question FROM subpass WHERE account = '{0} LIMIT 1'", id);
            var dbresult = cmd.ExecuteScalar();

            if (dbresult != null)
                return Convert.ToInt32(dbresult);

            return 0;
        }

        public bool CheckSubPwAnswer(int id, string answer)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT id FROM subpass WHERE account = '{0}' AND answer = '{1}' LIMIT 1", id, answer);
            var dbresult = cmd.ExecuteScalar();

            if (dbresult != null)
                return true;

            cmd.Dispose();
            return false;
        }

        public bool CheckSubPw(int id, string pass)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT id FROM subpass WHERE account = '{0}' AND passwd = '{1}' LIMIT 1", id, pass);
            var dbresult = cmd.ExecuteScalar();

            if (dbresult != null)
                return true;

            cmd.Dispose();
            return false;
        }

        public void RemoveSubPass(int id)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("DELETE FROM subpass WHERE account = '{0}'", id);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public DateTime? GetSubPassTime(int id)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT expiration FROM subpass WHERE account = '{0}' LIMIT 1", id);
            var dbresult = cmd.ExecuteScalar();

            if (dbresult != null && string.Empty != dbresult.ToString())
                return Convert.ToDateTime(dbresult);

            return null;
        }

        public void SetSubPassTime(int id, int time)
        {
            Cleanup();

            DateTime expire = DateTime.Now.AddHours(time);

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE subpass SET expiration = '{0}' WHERE account = '{1}'", expire.ToString("yyyy-MM-dd HH:mm:ss"), id);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }
    }
}
