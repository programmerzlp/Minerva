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

using System.Collections.Generic;

using PacketInfo = System.Tuple<string, Minerva.PacketMethod>;

#endregion

namespace Minerva
{
    partial class PacketProtocol
    {
        public static void Initialise(Dictionary<ushort, PacketInfo> methods, Dictionary<string, PacketConstructor> constructors)
        {
            methods[0x065] = new PacketInfo("Connect2Svr", Connect);
            methods[0x066] = new PacketInfo("VerifyLinks", VerifyLinks);
            methods[0x067] = new PacketInfo("AuthAccount", AuthAccount);
            methods[0x07A] = new PacketInfo("CheckVersion", CheckVersion);
            methods[0x7D1] = new PacketInfo("PublicKey", PublicKey);
            methods[0x7D2] = new PacketInfo("PreServerEnvRequest", PreServerEnvRequest);
            methods[0x7D6] = new PacketInfo("Unknown_7D6", Unknown_7D6);
        }
    }
}