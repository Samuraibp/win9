using System.Net;
using System.Net.Sockets;
using System.Text;

namespace server
{
    internal class Program
    {
        static async Task Main()
        {
            TcpListener server = new TcpListener(
                IPAddress.Any,
                8888
            );

            server.Start();

            Console.WriteLine("Сервер запущен.");
            Console.WriteLine("Ожидание подключений...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                Console.WriteLine("Подключился новый клиент.");

                _ = ProcessClientAsync(client);
            }
        }


        static async Task ProcessClientAsync(TcpClient tcpClient)
        {
            var rates = new Dictionary<string, string>()
        {
            { "USD EURO", "0.92" },
            { "EURO USD", "1.09" }
        };

            var stream = tcpClient.GetStream();

            while (true)
            {
                var response = new List<byte>();
                int bytesRead;

                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    if (bytesRead == -1)
                        return;

                    response.Add((byte)bytesRead);
                }

                string request = Encoding.UTF8.GetString(response.ToArray());

                if (request == "END")
                {
                    Console.WriteLine("Клиент завершил работу.");
                    break;
                }

                Console.WriteLine($"Запрошен курс: {request}");

                if (!rates.TryGetValue(request, out string? rate))
                {
                    rate = "Неизвестная валютная пара";
                }

                rate += '\n';

                byte[] data = Encoding.UTF8.GetBytes(rate);

                await stream.WriteAsync(data);
            }

            tcpClient.Close();
        }


    }
}
