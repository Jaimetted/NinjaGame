using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NinjaGame
{
    public partial class MainForm : Form
    {
        private readonly Vector UP_DIRECTION = new Vector(0, -1);
        private readonly Vector DOWN_DIRECTION = new Vector(0, 1);
        private readonly Vector LEFT_DIRECTION = new Vector(-1, 0);
        private readonly Vector RIGHT_DIRECTION = new Vector(1, 0);

        private const float PLAYER_SPEED = 16;
        private const float PLAYER_SIZE = 32;
        private const float ENEMY_SIZE = PLAYER_SIZE;
        private const float PROJECTILE_SPEED = 32;
        private const float PROJECTILE_SIZE = 10;
        private const float MIN_COORD_DIFFERENCE = 200;
        private const int MAX_ENEMY_COUNT = 3;
        private const int MAX_SPAWN_DELAY = 70;
        private const int MAX_SPAWN_ATTEMPTS = 10;

        // TODO: Make a GameState class instead of holding onto state inside this Form
        private Player player;
        private List<Enemy> enemies;
        private List<Projectile> projectiles;
        private bool canMove;
        private bool canShoot;
        private Random random;
        private int spawnDelay;

        public MainForm()
        {
            InitializeComponent();

            // Initialize game data
            random = new Random();
            canMove = true;
            canShoot = true;
            spawnDelay = MAX_SPAWN_DELAY;
            player = new Player();
            enemies = new List<Enemy>();
            projectiles = new List<Projectile>();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            // Draw player
            graphics.DrawRectangle(Pens.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
            graphics.FillRectangle(Brushes.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
            
            // Draw projectiles
            foreach (Projectile shot in projectiles)
            {
                graphics.DrawEllipse(Pens.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
                graphics.FillEllipse(Brushes.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
            }

            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                graphics.DrawRectangle(Pens.DarkRed, enemy.X, enemy.Y, ENEMY_SIZE, ENEMY_SIZE);
                graphics.FillRectangle(Brushes.DarkRed, enemy.X, enemy.Y, ENEMY_SIZE, ENEMY_SIZE);
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdatePhysics();
            TrySpawnEnemy();
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

                if (IsWithinBounds(shot))
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (CheckCollision(enemies[i], shot))
                        {
                            projectiles.RemoveAt(index);
                            enemies.RemoveAt(i);
                            break;
                        }
                    }
                }
                else
                {
                    projectiles.RemoveAt(index);
                }
            }
            foreach (Enemy enemy in enemies)
            {
                int direction = random.Next(4);
                if(direction==0)
                    enemy.Move(0, -PLAYER_SPEED);
                else if(direction==1)
                    enemy.Move(0, PLAYER_SPEED);
                else if(direction==2)
                    enemy.Move(-PLAYER_SPEED, 0);
                else if(direction==3)
                    enemy.Move(PLAYER_SPEED,0);



            }
        }

        private void TrySpawnEnemy()
        {
            float x;
            float y;
            int attempts;

            if (spawnDelay > 0)
                spawnDelay--;

            if (spawnDelay == 0 && enemies.Count < MAX_ENEMY_COUNT)
            {
                for (attempts = 0; attempts < MAX_SPAWN_ATTEMPTS; attempts++)
                {
                    x = (float)((this.Width - ENEMY_SIZE) * random.NextDouble());
                    // Enemy size is doubled here as a hax to compensate for the window's title bar
                    y = (float)((this.Height - 2 * ENEMY_SIZE) * random.NextDouble());

                    // Check that the enemy doesn't spawn too close to the player
                    if (Math.Abs(player.X - x) >= MIN_COORD_DIFFERENCE && Math.Abs(player.Y - y) >= MIN_COORD_DIFFERENCE)
                    {
                        enemies.Add(new Enemy(x, y));
                        spawnDelay = MAX_SPAWN_DELAY;
                        break;
                    }
                }
            }
        }

        private bool CheckCollision(Enemy enemy, Projectile shot)
        {
            float eXmin = enemy.X, eYmin = enemy.Y;
            float eXmax = eXmin + ENEMY_SIZE, eYmax = eYmin + ENEMY_SIZE;
            float pXmin = shot.X, pYmin = shot.Y;
            float pXmax = pXmin + PROJECTILE_SIZE, pYmax = pYmin + PROJECTILE_SIZE;

            return (eXmin <= pXmax && eXmax >= pXmin && eYmin <= pYmax && eYmax >= pYmin);
        }

        private bool IsWithinBounds(Projectile shot)
        {
            return (shot.X > -50f && shot.Y > -50f && shot.X < 550f && shot.Y < 550f);
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
                projectiles.Add(new Projectile(player.X + PLAYER_SIZE / 2 - PROJECTILE_SIZE / 2, player.Y + PLAYER_SIZE / 2 - PROJECTILE_SIZE / 2, player.Direction.Multiply(PROJECTILE_SPEED)));
                e.Handled = true;
                canShoot = false;
            }
        }
    }
}
