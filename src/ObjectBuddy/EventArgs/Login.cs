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
    public class LoginEventArgs : EventArgs
    {
        private string _username;
        private string _password;
        private int _account;
        private string _ip;
        private DateTime _time;
        private LoginResult _result;

        public string Username { get { return _username; } }
        public string Password { get { return _password; } }
        public int Account { get { return _account; } }
        public string IP { get { return _ip; } }
        public DateTime Time { get { return _time; } }
        public LoginResult Result { get { return _result; } }

        public LoginEventArgs(string user, string pass, string ip, LoginResult result = LoginResult.None, int account = 0)
        {
            _username = user;
            _password = pass;
            _account = account;
            _ip = ip;
            _time = DateTime.Now;
            _result = result;
        }
    }

    public enum LoginResult : byte
    {
        None,
        Success,
        UnknownUsername,
        WrongPassword,
        OutOfService
    }
}