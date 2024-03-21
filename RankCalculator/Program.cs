using NATS.Client;
using System.Text;
using StackExchange.Redis;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = connection.GetDatabase();

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);

                string textKey = "TEXT-" + id;
                string text = db.StringGet(textKey);

                string rankKey = "RANK-" + id;

                double rank = GetRank(text);

                db.StringSet(rankKey, rank);
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            // s.Unsubscribe();

            // c.Drain();
            // c.Close();
        }

        static double GetRank(string text)
        {
            if (String.IsNullOrEmpty(text))
                return 0;

            double numberOfNonAlphabeticCharacters = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (!Char.IsLetter(text[i]))
                    numberOfNonAlphabeticCharacters++;
            }
            return numberOfNonAlphabeticCharacters / text.Length;
        }
    }
}