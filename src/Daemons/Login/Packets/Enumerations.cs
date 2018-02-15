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

namespace Minerva
{
    // account status enum, sent to client
    public enum AccountStatus : byte
    {
        Normal = 0x20,      // login authentication is good
        Incorrect,          // wrong id or password
        Online,             // account is logged in already
        OutOfService,       // shows nothing, used to be "cannot connect at the moment" or so...
        AccountExpired,     // account expired
        IpBanned,           // ip is blocked, though not account related
        Banned,             // account id is blocked
        TestServerTrial,    // "cannot use test server during free trial period, bla bla"...
        PcCafe,             // "use pc cafe to login, bla bla"...
        Unverified,         // account is unverified
        AccountDeleted,     // inexistent or deleted account
        AccountLocked       // too many wrong passwd attempts
    }

    // authentication status, received from database
    // NOT RELATED WITH CLIENT ACCOUNT STATUS VALUES!
    public enum AuthStatus : byte
    {
        Unverified,
        Verified,
        Banned,
        IncorrectName,
        IncorrectPass
    }

    // system message enum, sent to client
    public enum SystemMsg : byte
    {
        None,           // no message
        DualLogin,      // when some one is trying to connect to same account
        Disconnected    // just disconnects client
    }

    // server status enum, sent to client
    public enum ServerStatus : byte
    {
        Online,         // servers are up and working, no message is shown
        Maintenance,    // shows "cannot connect at this moment"...
        AuthUnavailable // shows "auth method not available"...
    }
}