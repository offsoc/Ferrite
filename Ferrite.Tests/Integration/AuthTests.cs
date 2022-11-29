// 
// Project Ferrite is an Implementation of the Telegram Server API
// Copyright 2022 Aykut Alparslan KOC <aykutalparslan@msn.com>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
// 

using System.Net;
using Ferrite.Core;
using TL;
using TL.Methods;
using WTelegram;
using Xunit;

namespace Ferrite.Tests.Integration;

public class AuthTests
{
    static AuthTests()
    {
        IFerriteServer ferriteServer = ServerBuilder.BuildServer("10.0.2.2", 52222);
        _ = ferriteServer.StartAsync(new IPEndPoint(IPAddress.Any, 52222), default);
        Client.LoadPublicKey(@"-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEAt1YElR7/5enRYr788g210K6QZzUAmaithnSzmQsKb+XL5KhQHrJw
VNMINO17SkB6i4fxG7ydDyFDbLA0Ls6jQZ0mX33Gl8vYWsczPbkbqzs9N6GkOo10
dDVoGObvSHRSwT9zkDixGiq/3b+WPhFcpdJ4OY+5ElD89+fXYgUIhskjYoI8P/PJ
LCf6GLfneJ00N+wDVxAiwOaD6dxvj8kiUDHSwdTXWQ56Nc/Il121fIGbEmho+ShA
fzwQPynnEsA0EyTsqtYHle+KowMhnQYpcvK/iv290NXwRjB4jWtH7tNT/PgB5tud
1LJ9Ta3FusvnDE35w97G6q+yXltErSpM/QIDAQAB
-----END RSA PUBLIC KEY-----
");
    }
    private static string Config(string what)
    {
        switch (what)
        {
            case "server_address": return "127.0.0.1:52222";
            case "api_id": return "11111";
            case "api_hash": return "11111111111111111111111111111111";
            case "phone_number": return "+15555555555";
            case "verification_code": return "12345";
            case "first_name": return "aaa";
            case "last_name": return "bbb";
            default: return null;
        }
    }
    [Fact]
    public async Task CreatesAuthKey()
    {
        using var client = new WTelegram.Client(Config, new MemoryStream());
        await client.ConnectAsync();
        Assert.NotNull(client.TLConfig);
    }
    [Fact]
    public async Task SendCode_Returns_SentCode()
    {
        using var client = new WTelegram.Client(Config, new MemoryStream());
        await client.ConnectAsync();
        var code = await client.Invoke(new Auth_SendCode()
        {
            phone_number = "+15555555555",
            api_id = 11111,
            api_hash = "11111111111111111111111111111111",
            settings = new CodeSettings()
        });
        Assert.IsType<Auth_SentCodeTypeSms>(code.type);
    }
    [Fact]
    public async Task ResendCode_Returns_SentCode()
    {
        using var client = new WTelegram.Client(Config, new MemoryStream());
        await client.ConnectAsync();
        var code = await client.Invoke(new Auth_SendCode()
        {
            phone_number = "+15555555555",
            api_id = 11111,
            api_hash = "11111111111111111111111111111111",
            settings = new CodeSettings()
        });
        code = await client.Invoke(new Auth_ResendCode()
        {
            phone_number = "+15555555555",
            phone_code_hash = code.phone_code_hash,
        });
        Assert.IsType<Auth_SentCodeTypeSms>(code.type);
    }
    [Fact]
    public async Task CancelCode_Returns_True()
    {
        using var client = new WTelegram.Client(Config, new MemoryStream());
        await client.ConnectAsync();
        var code = await client.Invoke(new Auth_SendCode()
        {
            phone_number = "+15555555555",
            api_id = 11111,
            api_hash = "11111111111111111111111111111111",
            settings = new CodeSettings()
        });
        var result = await client.Invoke(new Auth_CancelCode()
        {
            phone_number = "+15555555555",
            phone_code_hash = code.phone_code_hash,
        });
        Assert.True(result);
    }
}