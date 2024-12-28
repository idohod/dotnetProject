using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
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

            NameTextBox.Text = "ido";
            IdTextBox.Text = "1";



        }
        static async Task<Uri> CreateProductAsync(Game product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(PATH + "api/TblGames", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(PATH + "api/TblGames/" + id);
            return response.StatusCode;
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
        static async Task<Player> UpdatePlayerAsync(Player player)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(PATH + "api/TblPlayers/" + player.Id);                      

                player = await response.Content.ReadAsAsync<Player>();                       
                player.NumOfGames++;
                response = await client.PutAsJsonAsync(PATH + "api/TblPlayers/" + player.Id, player);

                // If 204 No Content, assume success
                if (response.StatusCode == HttpStatusCode.NoContent)
                    return player;

                player = await response.Content.ReadAsAsync<Player>();
                            

                return player;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string name = NameTextBox.Text;
            string id = IdTextBox.Text;
            string LOGIN_PLAYER = "api/TblPlayers/" + id;

          
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
            {
                Player player = await GetPlayerAsync(PATH + LOGIN_PLAYER);

                if (player == null)
                {
                    MessageBox.Show("Player not found. Please register first.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (player.Name.ToLower().Trim() != name.ToLower())
                {
                    MessageBox.Show("Incorrect data.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                   
                    Player updatedPlayer = await UpdatePlayerAsync(player);

                    if (updatedPlayer == null)
                    {
                        MessageBox.Show("Failed to update player data.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    FormClientBoard form = new FormClientBoard(updatedPlayer);
                    this.Hide();
                    form.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Please enter both Name and ID.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


    }
}
