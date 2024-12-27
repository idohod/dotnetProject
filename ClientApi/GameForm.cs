using ClientApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApi
{
    public partial class GameForm : Form
    {
        private Player Player;
        public GameForm(Player player)
        {
            InitializeComponent();
            Player = player;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            PlayerDataLabel.Text = $"{Player.Name.Trim()} , {Player.Id} . ";
        }
    }
}
