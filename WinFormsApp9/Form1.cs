using System.Net.Sockets;
using System.Text;

namespace WinFormsApp9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using TcpClient client = new TcpClient();

                await client.ConnectAsync("127.0.0.1", 8888);

                using NetworkStream stream = client.GetStream();

                string[] currencies =
                {
                    "USD EURO",
                    "EURO USD"
                };

                textBox2.Clear();

                foreach (string currency in currencies)
                {
                    byte[] data = Encoding.UTF8.GetBytes(currency + "\n");
                    await stream.WriteAsync(data);

                    List<byte> response = new List<byte>();
                    int b;

                    while ((b = stream.ReadByte()) != '\n' && b != -1)
                    {
                        response.Add((byte)b);
                    }

                    string result = Encoding.UTF8.GetString(response.ToArray());

                    textBox2.AppendText(
                        $"{currency}: {result}{Environment.NewLine} "
                    );

                }


                await stream.WriteAsync(
                    Encoding.UTF8.GetBytes("END\n")
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
