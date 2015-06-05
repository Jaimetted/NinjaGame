using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NinjaGame
{
    public partial class MainForm : Form
    {
        // TODO: Make a GameState class instead of holding onto state inside this Form
        private const int PLAYER_SPEED = 16;
        private const int PLAYER_SIZE = 32;
        private Player player;
        private bool canMove;

        public MainForm()
        {
            InitializeComponent();

            // Initialize game data
            canMove = true;
            player = new Player();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.DrawRectangle(Pens.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
            graphics.FillRectangle(Brushes.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
        }

        private void physicsTimer_Tick(object sender, EventArgs e)
        {
            canMove = true;
            this.Refresh(); // Redraw graphics
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (canMove)
            {
                if (e.KeyCode == Keys.Up)
                {
                    player.Move(0, -PLAYER_SPEED);
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.Move(0, PLAYER_SPEED);
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Left)
                {
                    player.Move(-PLAYER_SPEED, 0);
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.Move(PLAYER_SPEED, 0);
                    e.Handled = true;
                    canMove = false;
                }
            }
        }
    }
}
