using Newtonsoft.Json.Linq;

namespace EventStreamProcessor
{

    internal static class EventHandlers
    {
        public static List<Token> StringToTokens(string _str)
        {
            bool isArray = _str.StartsWith("[");

            // convert to an array string if it's an object
            var formattedJson = isArray ? _str : "[" + _str + "]";

            var json = JToken.Parse(formattedJson);

            List<Token> tokens = json.Select(p => new Token
            {
                Id = p["TokenId"]!.ToString(),
                Owner = p["Address"]?.ToString() ?? p["To"]?.ToString() ?? ""
            }).ToList();

            return tokens;
        }
        public static void ReadInline(string _data)
        {
            var tokens = StringToTokens(_data);
            Database db = new Database();
            db.InsertBulk(tokens);
            Console.WriteLine($"Read {tokens.Count} transaction(s)");
        }

        public static void GetOwner(string _tokenId)
        {
            Database db = new Database();
            string response = db.GetTokenOwner(_tokenId);
            Console.WriteLine(response);
        }

        public static void GetOwnerTokens(string _owner)
        {
            Database db = new Database();
            List<Token> tokens = db.GetOwnerTokens(_owner);
            Console.WriteLine($"Wallet {_owner} holds {tokens.Count} tokens:");
            foreach (var item in tokens)
            {
                Console.WriteLine(item.Id);
            }
        }

        public static void ReadFile(string _fileName)
        {
            if (!File.Exists(_fileName))
                throw new Exception("JSON file supplied does not exist.");

            string content = File.ReadAllText(_fileName);

            var tokens = StringToTokens(content);
            Database db = new Database();
            db.InsertBulk(tokens);
            Console.WriteLine($"Read {tokens.Count} transaction(s)");
        }
        public static void ResetData()
        {
            // probably just empty the table here instead
            Database db = new();
            db.ExecQuery("DELETE From Tokens");
            Console.WriteLine("Program was reset");
        }
    }
}
