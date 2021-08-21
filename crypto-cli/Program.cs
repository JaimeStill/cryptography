using System.CommandLine;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace CryptoCli
{
    [SupportedOSPlatform("windows")]
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var root = Commands.Initialize();
            return await root.InvokeAsync(args);
        }
    }
}
