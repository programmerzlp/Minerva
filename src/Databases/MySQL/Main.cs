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

using MySql.Data.MySqlClient;

#endregion

namespace Minerva
{
    public partial class DatabaseProtocol : IDatabaseProtocol
    {
        MySqlConnection sql;
        MySqlDataReader reader;

        public DatabaseProtocol(string ip, string db, string user, string pass)
        {
            sql = new MySqlConnection(string.Format("Server={0};Database={1};User={2};Password={3};convert zero datetime=true", ip, db, user, pass));
        }

        void Cleanup()
        {
            if (reader != null && !reader.IsClosed)
                reader.Close();
        }

        public void Connect()
        {
            sql.Open();
        }

        public void Disconnect()
        {
            reader.Close();
            reader.Dispose();
            reader = null;

            sql.Close();
            sql.Dispose();
            sql = null;
        }

        public object this[string key]
        {
            get
            {
                return reader[key];
            }
        }

        public bool ReadRow()
        {
            return reader.Read();
        }
    }
}