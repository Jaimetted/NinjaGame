using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NinjaGame
{
    public partial class MainForm : Form
    {
        private readonly Vector UP_DIRECTION = new Vector(0, -1);
        private readonly Vector DOWN_DIRECTION = new Vector(0, 1);
        private readonly Vector LEFT_DIRECTION = new Vector(-1, 0);
        private readonly Vector RIGHT_DIRECTION = new Vector(1, 0);

        // TODO: Make a GameState class instead of holding onto state inside this Form
        private const float PLAYER_SPEED = 16;
        private const float PLAYER_SIZE = 32;
        private const float PROJECTILE_SPEED = 32;
        private const float PROJECTILE_SIZE = 10;
        private Player player;
        private List<Projectile> projectiles;
        private bool canMove;
        private bool canShoot;

        public MainForm()
        {
            InitializeComponent();

            // Initialize game data
            canMove = true;
            canShoot = true;
            player = new Player();
            projectiles = new List<Projectile>();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.DrawRectangle(Pens.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
            graphics.FillRectangle(Brushes.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);

            foreach (Project shot : projectiles)
            {
                graphics.DrawEllipse(Pens.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
                graphics.FillEllipse(Brushes.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
            }
        }

        private void physicsTimer_Tick(object sender, EventArgs e)
        {
            UpdatePhysics();
            this.Refresh(); // Redraw graphics
        }

        private void UpdatePhysics()
        {
            canMove = true;
            canShoot = true;
            for (int index = projectiles.Count - 1; index >= 0; index--)
            {
                Projectile shot = projectiles[index];
                shot.Move();
                if (!IsWithinBounds(shot))
                    projectiles.RemoveAt(index);
            }
        }

        private bool IsWithinBounds(Projectile shot)
        {
            return (shot.X < -50f || shot.Y < -50f || shot.X > 550f || shot.Y > 550f);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (canMove)
            {
                if (e.KeyCode == Keys.Up)
                {
                    player.Move(0, -PLAYER_SPEED);
                    player.Direction = UP_DIRECTION;
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Down)
                {
                    player.Move(0, PLAYER_SPEED);
                    player.Direction = DOWN_DIRECTION;
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Left)
                {
                    player.Move(-PLAYER_SPEED, 0);
                    player.Direction = LEFT_DIRECTION;
                    e.Handled = true;
                    canMove = false;
                }
                if (e.KeyCode == Keys.Right)
                {
                    player.Move(PLAYER_SPEED, 0);
                    player.Direction = RIGHT_DIRECTION;
                    e.Handled = true;
                    canMove = false;
                }
            }
            if (canShoot && e.KeyCode == Keys.Space)
            {
                // TODO: Haxed up the starting point for the projectile -- Fix this in the future
                projectiles.Add(new Projectile(player.X + PLAYER_SIZE / 2, player.Y + PLAYER_SIZE / 2, player.Direction.Multiply(PROJECTILE_SPEED)));
                e.Handled = true;
                canShoot = false;
            }
        }
    }
}
