using ClientApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApi
{
    public partial class LoginForm : Form
    {
        private static HttpClient client = new HttpClient();

        private const string PATH = "https://localhost:7272/";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri(PATH);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));          

        }
        static async Task<Player> GetPlayerAsync(string path)
        {
            Player player = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                player = await response.Content.ReadAsAsync<Player>();
            }
            return player;
        }


        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string name = NameTextBox.Text;
            string id = IdTextBox.Text;
            string LOGIN_PLAYER = "api/TblPlayers/" + id;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
            {
                Player player = await GetPlayerAsync(PATH + LOGIN_PLAYER);

                // ID doesn't exist in Database.
                if (player == null)
                {
                    MessageBox.Show("Please register first.", "WARNNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                // ID exists but name is incorrect.
                else if (player.Name.ToLower().Trim() != name.ToLower())
                {
                    MessageBox.Show("Incorrect data.", "WARNNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                // All data correct
                else
                {
                    GameForm form = new GameForm(player);
                    form.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Please enter both Name and ID.", "WARNNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
