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
        public bool FetchAccount(string user)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT id, password, auth, online FROM accounts WHERE username = '{0}' LIMIT 1", user);
            reader = cmd.ExecuteReader();
            cmd.Dispose();

            return reader.HasRows;
        }

        public void UpdateIPDate(int id, string ip, DateTime date)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE accounts SET lastip = '{0}', lastlogin = '{1:yyyy-MM-dd HH:mm:ss}' WHERE id = '{2}'", ip, date, id);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public void UpdateOnline(int id, bool online)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("UPDATE accounts SET online = {0} WHERE id = '{1}'", online, id);
            cmd.ExecuteScalar();
            cmd.Dispose();
        }

        public bool VerifyPassword(int id, string pass)
        {
            Cleanup();

            var cmd = sql.CreateCommand();
            cmd.CommandText = string.Format("SELECT id FROM accounts WHERE id = '{0}' AND password = '{1}' LIMIT 1", id, pass);
            var dbresult = cmd.ExecuteScalar();
            cmd.Dispose();

            if (dbresult != null)
                return true;

            return false;
        }
    }
}