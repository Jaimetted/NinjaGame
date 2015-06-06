using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NinjaGame
{
    public partial class MainForm : Form
    {
        private static readonly Vector UP_DIRECTION = new Vector(0, -1);
        private static readonly Vector DOWN_DIRECTION = new Vector(0, 1);
        private static readonly Vector LEFT_DIRECTION = new Vector(-1, 0);
        private static readonly Vector RIGHT_DIRECTION = new Vector(1, 0);
        private static readonly Vector[] DIRECTIONS = new Vector[] { UP_DIRECTION, DOWN_DIRECTION, LEFT_DIRECTION, RIGHT_DIRECTION };

        private const float PLAYER_SPEED = 16;
        private const float ENEMY_SPEED = 18;
        private const float PLAYER_SIZE = 32;
        private const float ENEMY_SIZE = PLAYER_SIZE;
        private const float PROJECTILE_SPEED = 32;
        private const float PROJECTILE_SIZE = 18;
        private const float MIN_COORD_DIFFERENCE = 200;
        private const int MAX_ENEMY_COUNT = 3;
        private const int MAX_SPAWN_DELAY = 70;
        private const int PLAYER_MOVE_DELAY = 5;
        private const int ENEMY_MOVE_DELAY = 4;
        private const int MAX_SPAWN_ATTEMPTS = 10;

        // TODO: Make a GameState class instead of holding onto state inside this Form
        private Random random;
        private Player player;
        private List<Enemy> enemies;
        private List<Projectile> projectiles;
        private bool canMove;
        private bool canShoot;
        private int spawnDelay;
        private int moveDelay;
        private Image playerImage;
        private Image enemyImage;
        private Image shurikenImage;

        public MainForm()
        {
            InitializeComponent();

            // Initialize game data
            random = new Random();
            canMove = true;
            canShoot = true;
            spawnDelay = MAX_SPAWN_DELAY;
            moveDelay = PLAYER_MOVE_DELAY;
            player = new Player();
            enemies = new List<Enemy>();
            projectiles = new List<Projectile>();

            // Load sprite sheet
            Bitmap ninjaSprites = new Bitmap(@"./resources/images/ninja_sprites.png");
            playerImage = ninjaSprites.Clone(new Rectangle(224, 0, 32, 32), ninjaSprites.PixelFormat);
            enemyImage = ninjaSprites.Clone(new Rectangle(128, 0, 32, 32), ninjaSprites.PixelFormat);
            shurikenImage = Image.FromFile(@"./resources/images/shuriken.png");
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            
            // Draw projectiles
            foreach (Projectile shot in projectiles)
            {
                graphics.DrawImage(shurikenImage, shot.X, shot.Y);
                // graphics.DrawEllipse(Pens.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
                // graphics.FillEllipse(Brushes.LightBlue, shot.X, shot.Y, PROJECTILE_SIZE, PROJECTILE_SIZE);
            }

            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                graphics.DrawImage(enemyImage, enemy.X, enemy.Y);
                // graphics.DrawRectangle(Pens.DarkRed, enemy.X, enemy.Y, ENEMY_SIZE, ENEMY_SIZE);
                // graphics.FillRectangle(Brushes.DarkRed, enemy.X, enemy.Y, ENEMY_SIZE, ENEMY_SIZE);
            }

            // Draw player
            graphics.DrawImage(playerImage, player.X, player.Y);
            // graphics.DrawRectangle(Pens.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
            // graphics.FillRectangle(Brushes.DarkBlue, player.X, player.Y, PLAYER_SIZE, PLAYER_SIZE);
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
            moveDelay--;
            if (moveDelay == 0)
            {

                foreach (Enemy enemy in enemies)
                {

                    if (enemy.DirectionDelay == 0)
                    {
                        enemy.DirectionDelay = random.Next(3, 5);
                        enemy.Direction = random.Next(4);
                    }

                    enemy.Move(DIRECTIONS[enemy.Direction].X*ENEMY_SPEED, DIRECTIONS[enemy.Direction].Y*ENEMY_SPEED);
                    enemy.DirectionDelay--;

                }
                    moveDelay = ENEMY_MOVE_DELAY;
                
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
                    // Enemy size is multiplied here as a hax to compensate for weird window bounds
                    x = (float)((this.Width - 2 * ENEMY_SIZE) * random.NextDouble());
                    y = (float)((this.Height - 3 * ENEMY_SIZE) * random.NextDouble());

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
